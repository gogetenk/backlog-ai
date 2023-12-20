
using AgileMind.Application.Domain;

namespace AgileMind.Application.Interfaces;

public interface IAiClient
{
    Task<Backlog> GenerateBacklogFromPrompt(string userPrompt);
}
