using AgileMind.Application.Domain;

namespace AgileMind.Application.Interfaces;

public interface IBacklogRepository
{
    Task<Backlog> GetByIdAsync(string id);
    Task<IEnumerable<Backlog>> GetAllAsync();
    Task AddAsync(Backlog backlog);
    Task UpdateAsync(Backlog backlog);
    Task DeleteAsync(string id);
}
