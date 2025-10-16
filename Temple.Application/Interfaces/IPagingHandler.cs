using Temple.Application.Core;

namespace Temple.Application.Interfaces;

public interface IPagingHandler<T>
{
    Task<PagedList<T>> CreateAsync(
         IQueryable<T> source,
         int pageNumber,
         int pageSize);

    PagedList<T> Create(
        IEnumerable<T> source,
        int pageNumber,
        int pageSize);
}

