namespace Repositories.EFCore;

using Entities.Models;
using Entities.RequestFeatures;

public static class BookRepositoryExtensions
{
    public static IQueryable<Book> FilterBooks(this IQueryable<Book> books, BookParameters bookParameters)
    {
        return books.OrderBy(x => x.Id)
            .Where(book => book.Price >= bookParameters.MinPrice && book.Price <= bookParameters.MaxPrice);
    }
}
