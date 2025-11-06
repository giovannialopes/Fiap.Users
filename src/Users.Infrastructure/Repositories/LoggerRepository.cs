using Users.Domain.Entity;
using Users.Domain.Repositories;
using Users.Infrastructure.Data;

namespace Users.Infrastructure.Repositories;

public class LoggerRepository : ILoggerRepository
{
    private readonly DbUser _dbUser;

    public LoggerRepository(DbUser dbUser) {
        _dbUser = dbUser;
    }

    public async Task Commit() => await _dbUser.SaveChangesAsync();

    public async Task AddILogger(ILoggerEnt loggerEnt) => await _dbUser.LOGS.AddAsync(loggerEnt);
}

