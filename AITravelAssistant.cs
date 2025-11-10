using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Spectre.Console;

namespace Travel_Journal
{
    public class AITravelAssistant
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AITravelAssistant()
        {
            _apiKey = Environment.GetEnvironmentVariable("API_KEY")
                ?? throw new InvalidOperationException("❌ API_KEY not found. Set it with: setx API_KEY \"sk-...\"");

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openai.com/v1/")
            };
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        // === Skicka prompt till OpenAI och få svar ===
        public async Task<string> GetTravelSuggestionAsync(decimal budget, string tripType, int durationDays)
        {
            var prompt = $@"
You are a travel planner AI. 
Suggest one travel destination and a short itinerary based on:
- Budget: {budget} SEK
- Trip type: {tripType}
- Duration: {durationDays} days
Give me 3 parts: 
1️⃣ Destination  
2️⃣ Why it's perfect for this traveler  
3️⃣ Suggested activities.";

            // Spinner animation i terminalen
            return await AnsiConsole.Status()
                .StartAsync("🧠 Thinking of your next adventure...", async ctx =>
                {
                    var payload = new
                    {
                        model = "gpt-4o-mini", // Lätt, snabb modell – kan ändras till gpt-4o
                        messages = new[]
                        {
                            new { role = "system", content = "You are a friendly AI travel assistant." },
                            new { role = "user", content = prompt }
                        },
                        max_tokens = 300
                    };

                    var json = JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _httpClient.PostAsync("chat/completions", content);
                        response.EnsureSuccessStatusCode();

                        var result = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(result);

                        string message = doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString() ?? "No suggestion available.";

                        return message.Trim();
                    }
                    catch (Exception ex)
                    {
                        UI.Error($"AI request failed: {ex.Message}");
                        return "Could not generate a travel suggestion.";
                    }
                });
        }
    }
}
