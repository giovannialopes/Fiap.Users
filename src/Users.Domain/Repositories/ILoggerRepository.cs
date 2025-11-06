using Users.Domain.Entity;

namespace Users.Domain.Repositories;

public interface ILoggerRepository : ICommit
{
    Task AddILogger(ILoggerEnt loggerEnt);
}
