using System.Net.Http.Json;
using System.Text.Json;

namespace AgileMind.Web.Components.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _options;

    public ApiService(HttpClient http)
    {
        _http = http;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<TResponse> PostPromptAsync<TResponse>(string prompt)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/backlog", new { userPrompt = prompt });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(_options);
        }
        catch (HttpRequestException httpEx)
        {
            // Logique pour gérer les erreurs HTTP
            throw new ApplicationException("Erreur lors de la communication avec l'API.", httpEx);
        }
        catch (Exception ex)
        {
            // Logique pour gérer les autres exceptions
            throw new ApplicationException("Une erreur inattendue est survenue.", ex);
        }
    }

    public async Task<List<TResponse>> GetBacklogItemsAsync<TResponse>()
    {
        return null;
        //try
        //{
        //    // Remplacez 'backlogItemsEndpoint' par l'URL de l'endpoint de votre API pour récupérer les éléments du backlog
        //    var response = await _http.GetAsync("backlogItemsEndpoint");
        //    response.EnsureSuccessStatusCode();

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var backlogItems = await response.Content.ReadFromJsonAsync<List<BacklogItem>>();
        //        return backlogItems ?? new List<BacklogItem>();
        //    }
        //    else
        //    {
        //        throw new ApplicationException($"Erreur API : {response.ReasonPhrase}");
        //    }
        //}
        //catch (HttpRequestException httpEx)
        //{
        //    // Logique pour gérer les erreurs HTTP
        //    throw new ApplicationException("Erreur lors de la communication avec l'API.", httpEx);
        //}
        //catch (Exception ex)
        //{
        //    // Logique pour gérer les autres exceptions
        //    throw new ApplicationException("Une erreur inattendue est survenue.", ex);
        //}
    }
}
