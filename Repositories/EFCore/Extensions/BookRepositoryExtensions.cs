namespace Repositories.EFCore.Extensions;

using Entities.Models;
using Entities.RequestFeatures;

public static class BookRepositoryExtensions
{
    public static IQueryable<Book> FilterBooks(this IQueryable<Book> books, BookParameters bookParameters)
    {
        return books.OrderBy(x => x.Id)
            .Where(book => book.Price >= bookParameters.MinPrice && book.Price <= bookParameters.MaxPrice);
    }

    public static IQueryable<Book> Search(this IQueryable<Book> books, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return books;

        var lowerCaseTerm = searchTerm.Trim().ToLower();
        return books.Where(b => b.Title.ToLower().Contains(lowerCaseTerm));
    }
}
