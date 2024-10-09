using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Entities.RequestFeatures;

namespace Services.Contracts;

public interface IBookService
{
    Task<List<Book>> GetAllBooksAsync(bool trackChanges);
    Task<(LinkResponse linkResponse, MetaData metaData)> GetAllBooksWithFilterAsync(LinkParameters linkParameters, bool trackChanges);
    Task<BookDto> GetOneBookByIdAsync(int id, bool trackChanges);
    Task<BookDto> CreateOneBookAsync(BookDtoForInsertion book);
    Task UpdateOneBookAsync(int id, BookDtoForUpdate book, bool trackChanges);
    Task DeleteOneBookAsync(int id, bool trackChanges);
    Task<(BookDtoForUpdate bookDtoForUpdate, Book book)> GetOneBookForPatchAsync(int id, bool trackChanges);
    Task SaveChangesForPatchAsync(BookDtoForUpdate bookDtoForUpdate, Book book);
}
