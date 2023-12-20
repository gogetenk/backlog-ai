using static AgileMind.Web.Components.Services.ApiService;

namespace AgileMind.Web.Components.Services;

public interface IApiService
{
    Task<TResponse> PostPromptAsync<TResponse>(string prompt);
    Task<List<BacklogItem>> GetBacklogItemsAsync();
}