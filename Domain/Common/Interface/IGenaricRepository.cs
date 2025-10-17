using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Common.Interface
{
    public interface IGenaricRepository<T> where T : class
    {
        // Basic CRUD operations
        Task<T> GetByIdAsync(object id , CancellationToken ct);
        Task<T> GetByUserIdAsnc(Guid id , CancellationToken ct);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct);
     
        Task AddAsync(T entity , CancellationToken ct);
        Task UpdateAsync(T entity, CancellationToken ct);
        Task DeleteAsync(T id , CancellationToken ct);

        // Queryable operations
        IQueryable<T> GetQueryable(bool tracking = false);
        IQueryable<T> GetAsNoTracking();
        IQueryable<T> GetAsTracking();

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // Pagination methods
        //Task<PaginatedResult<T>> GetPagedAsync(
        //    int pageNumber,
        //    int pageSize,
        //    Expression<Func<T, bool>> filter = null,
        //    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        //    string includeProperties = "");

        //Task<PaginatedResult<TResult>> GetPagedProjectionAsync<TResult>(
        //    int pageNumber,
        //    int pageSize,
        //    Expression<Func<T, TResult>> selector,
        //    Expression<Func<T, bool>> filter = null,
        //    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        //    string includeProperties = "");
    }
}
