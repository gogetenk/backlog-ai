namespace AgileMind.Web.Components.Services;

public interface IApiService
{
    Task<TResponse> PostPromptAsync<TResponse>(string prompt);
    Task<List<TResponse>> GetBacklogItemsAsync<TResponse>();
}