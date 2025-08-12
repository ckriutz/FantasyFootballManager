using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Collections.Generic;


namespace FantasyFootballManager.Api.Services
{
    public class AiRecommendation
    {
        public string PlayerId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string MatchupStrength { get; set; } = string.Empty;
        public string RiskLevel { get; set; } = string.Empty;
    }

    public class ProcessResult
    {
        public bool Success { get; set; }
        public List<AiRecommendation> Recommendations { get; set; } = new List<AiRecommendation>();
        public string ErrorMessage { get; set; } = string.Empty;
        public string RawResponse { get; set; } = string.Empty;
    }

    public interface IAiInferenceService
    {
        Task<ProcessResult> GetResponseAsync(string prompt);
    }

    public class AiInferenceService : IAiInferenceService
    {
        readonly static string aiEndpoint = Environment.GetEnvironmentVariable("endpoint");
        readonly static string apiKey = Environment.GetEnvironmentVariable("api-key");
        readonly System.Uri endpoint = new System.Uri(aiEndpoint);
        readonly AzureKeyCredential credential = new AzureKeyCredential(apiKey);
        readonly ChatCompletionsClient _client;
        readonly ILogger<AiInferenceService> _logger;

        public AiInferenceService(ILogger<AiInferenceService> logger)
        {
            _client = new ChatCompletionsClient(endpoint, credential, new AzureAIInferenceClientOptions());
            _logger = logger;
        }

        public async Task<ProcessResult> GetResponseAsync(string prompt)
        {
            _logger.LogInformation("Sending request to AI model");
            var requestOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatRequestUserMessage(prompt)
                },
            };

            Response<ChatCompletions> response = await _client.CompleteAsync(requestOptions);
            _logger.LogInformation($"AI model response: {response.Value.Content}");
            ProcessResult processResult = ProcessResponse(response.Value.Content);
            return processResult;
        }

        private ProcessResult ProcessResponse(string response)
        {
            _logger.LogInformation("Processing AI response");
            
            var result = new ProcessResult
            {
                RawResponse = response
            };

            try
            {
                // Trim whitespace from response
                var cleanResponse = response.Trim();
                
                if (string.IsNullOrEmpty(cleanResponse))
                {
                    _logger.LogWarning("Empty response from AI");
                    result.Success = false;
                    result.ErrorMessage = "Empty response from AI";
                    return result;
                }

                // Handle markdown code block formatting
                // Extract JSON content between markdown delimiters
                if (cleanResponse.StartsWith("```json") || cleanResponse.StartsWith("```"))
                {
                    _logger.LogInformation("Detected markdown code block - extracting JSON content");
                    
                    // Find the start of JSON content
                    int startIndex = 0;
                    if (cleanResponse.StartsWith("```json"))
                    {
                        startIndex = cleanResponse.IndexOf('\n', 7) + 1; // Find newline after ```json
                    }
                    else if (cleanResponse.StartsWith("```"))
                    {
                        startIndex = cleanResponse.IndexOf('\n', 3) + 1; // Find newline after ```
                    }
                    
                    // Find the end delimiter (closing ```)
                    int endIndex = cleanResponse.IndexOf("\n```", startIndex);
                    if (endIndex == -1)
                    {
                        // Try without newline before closing ```
                        endIndex = cleanResponse.IndexOf("```", startIndex);
                    }
                    
                    if (startIndex > 0 && endIndex > startIndex)
                    {
                        cleanResponse = cleanResponse.Substring(startIndex, endIndex - startIndex).Trim();
                        _logger.LogInformation("Successfully extracted JSON from markdown code block");
                    }
                    else
                    {
                        // Fallback: try the old method
                        if (cleanResponse.StartsWith("```json"))
                        {
                            cleanResponse = cleanResponse.Substring(7);
                        }
                        else if (cleanResponse.StartsWith("```"))
                        {
                            cleanResponse = cleanResponse.Substring(3);
                        }
                        
                        if (cleanResponse.EndsWith("```"))
                        {
                            cleanResponse = cleanResponse.Substring(0, cleanResponse.Length - 3);
                        }
                        cleanResponse = cleanResponse.Trim();
                        _logger.LogInformation("Used fallback method for markdown removal");
                    }
                }
                else
                {
                    // No markdown formatting detected, proceed with original content
                    _logger.LogInformation("No markdown formatting detected");
                }

                // Trim again after removing markdown
                cleanResponse = cleanResponse.Trim();

                if (string.IsNullOrEmpty(cleanResponse))
                {
                    _logger.LogWarning("Response was empty after removing markdown formatting");
                    result.Success = false;
                    result.ErrorMessage = "Response was empty after removing markdown formatting";
                    return result;
                }

                _logger.LogInformation($"Processing JSON response: {cleanResponse}");

                // Configure JSON serializer options for case-insensitive matching
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                // Try to parse as array first (expected format)
                if (cleanResponse.StartsWith('['))
                {
                    var recommendations = JsonSerializer.Deserialize<List<AiRecommendation>>(cleanResponse, jsonOptions);
                    if (recommendations != null)
                    {
                        result.Recommendations = recommendations;
                        result.Success = true;
                        _logger.LogInformation($"Successfully parsed {recommendations.Count} recommendations");
                        return result;
                    }
                }
                // Try to parse as single object if it's not an array
                else if (cleanResponse.StartsWith('{'))
                {
                    var recommendation = JsonSerializer.Deserialize<AiRecommendation>(cleanResponse, jsonOptions);
                    if (recommendation != null)
                    {
                        result.Recommendations = new List<AiRecommendation> { recommendation };
                        result.Success = true;
                        _logger.LogInformation("Successfully parsed single recommendation");
                        return result;
                    }
                }

                result.Success = false;
                result.ErrorMessage = "Response does not appear to be valid JSON format";
                return result;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error");
                result.Success = false;
                result.ErrorMessage = $"JSON parsing error: {ex.Message}";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing AI response");
                result.Success = false;
                result.ErrorMessage = $"Unexpected error: {ex.Message}";
                return result;
            }
        }
    }
}