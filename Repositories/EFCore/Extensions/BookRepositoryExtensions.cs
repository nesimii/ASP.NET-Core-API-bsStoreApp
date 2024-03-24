namespace Repositories.EFCore.Extensions;

using Entities.Models;
using Entities.RequestFeatures;
using System.Linq.Dynamic.Core;

public static class BookRepositoryExtensions
{
    public static IQueryable<Book> FilterBooks(this IQueryable<Book> books, BookParameters bookParameters)
    {
        return books.Where(book => book.Price >= bookParameters.MinPrice && book.Price <= bookParameters.MaxPrice);
    }

    public static IQueryable<Book> Search(this IQueryable<Book> books, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return books;

        var lowerCaseTerm = searchTerm.Trim().ToLower();
        return books.Where(b => b.Title.ToLower().Contains(lowerCaseTerm));
    }
    public static IQueryable<Book> Sort(this IQueryable<Book> books, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString)) return books.OrderBy(b => b.Id);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Book>(orderByQueryString);

        if (orderQuery is null) return books.OrderBy(b => b.Id);
        return books.OrderBy(orderQuery);
    }
}
