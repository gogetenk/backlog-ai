using AgileMind.Application.Domain;
using AgileMind.Application.Interfaces;

namespace AgileMind.Infrastructure.Mongo
{
    internal class BacklogRepository : IBacklogRepository
    {
        public Task AddAsync(Backlog backlog)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Backlog>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Backlog> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Backlog backlog)
        {
            throw new NotImplementedException();
        }
    }
}
