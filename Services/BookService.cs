using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using Repositories.EFCore.Extensions;
using Services.Contracts;

namespace Services
{
    public class BookService : IBookService
    {
        private readonly IRepositoryManager _manager;
        private readonly IMapper _mapper;
        private readonly IBookLinks _bookLinks;
        public BookService(IRepositoryManager manager, IMapper mapper, IBookLinks bookLinks)
        {
            _manager = manager;
            _mapper = mapper;
            _bookLinks = bookLinks;
        }

        public async Task<(LinkResponse linkResponse, MetaData metaData)> GetAllBooksWithFilterAsync(LinkParameters linkParameters, bool trackChanges)
        {
            if (!linkParameters.BookParameters.ValidPriceRange) throw new PriceOutOfRangeBadRequestException();

            IQueryable<Book> books = await _manager.Book.GetBooksEntityAsync(trackChanges);
            IQueryable<Book> filterBooks = books.FilterBooks(linkParameters.BookParameters).Search(linkParameters.BookParameters.SearchTerm).Sort(linkParameters.BookParameters.OrderBy);

            PagedList<Book> booksWithMetaData = await PagedList<Book>.ToPagedListAsync(filterBooks, linkParameters.BookParameters.PageNumber, linkParameters.BookParameters.PageSize);

            var booksDto = _mapper.Map<IEnumerable<BookDto>>(booksWithMetaData);
            LinkResponse links = _bookLinks.TryGenerateLinks(booksDto, linkParameters.BookParameters.Fields, linkParameters.HttpContext);
            return (linkResponse: links, metaData: booksWithMetaData.MetaData);
        }

        public async Task<BookDto> GetOneBookByIdAsync(int id, bool trackChanges)
        {
            Book book = await GetOneBookByIdAndCheckExists(id, trackChanges);
            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> CreateOneBookAsync(BookDtoForInsertion bookDto)
        {
            Book book = _mapper.Map<Book>(bookDto);
            _manager.Book.CreateOneBook(book);
            await _manager.SaveAsync();
            return _mapper.Map<BookDto>(book);
        }
        public async Task UpdateOneBookAsync(int id, BookDtoForUpdate bookDto, bool trackChanges)
        {
            //check entity
            Book book = await GetOneBookByIdAndCheckExists(id, trackChanges);

            // Auto Mapper
            Book bookDtoForUpdate = _mapper.Map<Book>(book);

            _manager.Book.Update(bookDtoForUpdate);
            await _manager.SaveAsync();
        }
        public async Task DeleteOneBookAsync(int id, bool trackChanges)
        {
            Book entity = await GetOneBookByIdAndCheckExists(id, trackChanges);

            _manager.Book.DeleteOneBook(entity);
            await _manager.SaveAsync();

        }

        public async Task<(BookDtoForUpdate bookDtoForUpdate, Book book)> GetOneBookForPatchAsync(int id, bool trackChanges)
        {
            Book book = await GetOneBookByIdAndCheckExists(id, trackChanges);

            var bookDtoForUpdate = _mapper.Map<BookDtoForUpdate>(book);
            return (bookDtoForUpdate, book);
        }

        public async Task SaveChangesForPatchAsync(BookDtoForUpdate bookDtoForUpdate, Book book)
        {
            _mapper.Map(bookDtoForUpdate, book);
            await _manager.SaveAsync();
        }
        public async Task<Book> GetOneBookByIdAndCheckExists(int id, bool trackChanges)
        {
            //check entity
            Book? entity = await _manager.Book.GetOneBookByIdAsync(id, trackChanges);
            if (entity is null) throw new BookNotFoundException(id);
            return entity;
        }
    }
}
