using System.Collections;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Infrastructure.Repositories;

namespace Ticketing.Query.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private Hashtable _repositories = new();
    private readonly DatabaseContextFactory _contextFactory;
    private readonly TicketDbContext _context;
    private IEmployeeRepository? _employeeRepository;

    public IEmployeeRepository EmployeeRepository => 
    _employeeRepository ??= new EmployeeRepository(_context);

    public UnitOfWork(DatabaseContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
        _context = _contextFactory.CreateDbContext();
    }

    public async Task<int> Complete()
    {
        return await _context.SaveChangesAsync();
    }

    public IGenericRepository<TEntity> RepositoyGeneric<TEntity>() where TEntity : class
    {
        if(_repositories is null)
        {
            _repositories = new Hashtable();
        }

        var type = typeof(TEntity).Name;

        if(!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>);
            var repositoryInstance = Activator
            .CreateInstance(
                repositoryType.MakeGenericType(
                    typeof(TEntity)
                ), 
                _context
            );

            _repositories.Add(type, repositoryInstance);
        }

        return (IGenericRepository<TEntity>)_repositories[type]!;
    }
}
