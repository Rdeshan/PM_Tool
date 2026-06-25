using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;

namespace PMTool.Application.Services.Ai;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    private const string Endpoint = "https://api.groq.com/openai/v1/chat/completions";
    private const string Model = "llama-3.3-70b-versatile";
    private const int MaxDocChars = 40_000;

    public GeminiService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["gorq:ApiKey"] ?? throw new InvalidOperationException(
            "gorq:ApiKey is not configured in appsettings.json");
    }

    public async Task<List<AiBacklogDraftItem>> GenerateBacklogItemsAsync(string documentText)
    {
        if (documentText.Length > MaxDocChars)
            documentText = documentText[..MaxDocChars];

        var prompt = $$"""
            You are a Senior Business Analyst and Agile Product Owner.

            Analyze the requirements document below and generate a complete, structured backlog following the STRICT hierarchy:

                BRD → Epic → User Story → Use Case

            ── TYPE VALUES ──────────────────────────────────────────────────
            1 = BRD       Major business domain (e.g. "User Management", "Project Management")
            4 = Epic      Feature area within a BRD (e.g. "Authentication & Authorization")
            2 = UserStory Agile-format user story
            3 = UseCase   Single system action that fulfils a User Story step

            ── STRICT RULES ─────────────────────────────────────────────────
            1. Every Epic MUST have a parent BRD.
            2. Every UserStory MUST have a parent Epic. NEVER create standalone UserStories.
            3. Every UseCase MUST have a parent UserStory.
            4. Every UserStory MUST have at least one UseCase.
            5. Do NOT create any other type. Use only types 1, 4, 2, 3.

            ── OUTPUT ORDER (critical) ───────────────────────────────────────
            Emit items in DEPTH-FIRST PRE-ORDER. The array must look like:
              BRD
                Epic
                  UserStory
                    UseCase
                    UseCase
                  UserStory
                    UseCase
                Epic
                  UserStory
                    UseCase
              BRD
                ...
            The sequence position IS the parent relationship. Do not skip levels.

            ── DESCRIPTION FORMAT ───────────────────────────────────────────
            BRD        : One sentence describing the business domain.
            Epic       : One sentence describing the feature area.
            UserStory  : MUST follow this exact format —
                           As a <role>, I want <goal>, so that <business value>.

                           Acceptance Criteria:
                           - <criterion>
                           - <criterion>

                           Business Priority: High | Medium | Low
            UseCase    : Short imperative phrase for the system action (e.g. "Validate Credentials").

            ── GROUPING RULES ───────────────────────────────────────────────
            - Group related requirements under shared Epics; avoid single-story Epics where possible.
            - Merge duplicate or overlapping requirements.
            - Focus on business functionality, not technical implementation.
            - If a requirement does not fit any existing BRD, create a new BRD.

            ── JSON SCHEMA ──────────────────────────────────────────────────
            Return ONLY a valid JSON array. No explanation, no markdown, no extra text.
            Each element must have exactly these fields:
            {
              "title": "short phrase, max 15 words",
              "description": "see description format above",
              "type": <1|4|2|3>,
              "priority": <1|2|3|4>,
              "storyPoints": <1|2|3|5|8|13>
            }

            priority: 1=Low  2=Medium  3=High  4=Critical
            storyPoints: applies to UserStory and UseCase; use 1 for BRD/Epic.

            ── PRE-OUTPUT VALIDATION ────────────────────────────────────────
            Before writing the JSON verify:
            ✓ Every UserStory is preceded in the array by its parent Epic (type 4)
            ✓ Every Epic is preceded by its parent BRD (type 1)
            ✓ Every UseCase is preceded by its parent UserStory (type 2)
            ✓ Every UserStory has at least one UseCase immediately following it
            ✓ No orphan items exist
            ✓ No duplicate titles

            Document:
            {{documentText}}
            """;

        var requestBody = new
        {
            model = Model,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            temperature = 0.1
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        request.Content = JsonContent.Create(requestBody);

        var response = await _http.SendAsync(request);
        var responseText = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Groq API error {response.StatusCode}: {responseText}");

        using var doc = JsonDocument.Parse(responseText);
        var text = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "[]";

        var start = text.IndexOf('[');
        var end = text.LastIndexOf(']');
        if (start < 0 || end < 0) return new List<AiBacklogDraftItem>();
        var json = text[start..(end + 1)];

        var raw = JsonSerializer.Deserialize<List<RawItem>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new List<RawItem>();

        return raw.Select((r, i) => new AiBacklogDraftItem
        {
            Id = Guid.NewGuid(),
            Title = r.Title ?? "Untitled",
            Description = r.Description ?? string.Empty,
            Type = Clamp(r.Type, 1, 9),
            Priority = Clamp(r.Priority, 1, 4),
            StoryPoints = r.StoryPoints is 1 or 2 or 3 or 5 or 8 or 13 ? r.StoryPoints : 3,
            SortOrder = i
        }).ToList();
    }

    private static int Clamp(int value, int min, int max) =>
        value < min ? min : value > max ? max : value;

    private class RawItem
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Type { get; set; } = 2;
        public int Priority { get; set; } = 2;
        public int StoryPoints { get; set; } = 3;
    }
}
