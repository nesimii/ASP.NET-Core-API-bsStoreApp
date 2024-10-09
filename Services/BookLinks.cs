using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using Services.Contracts;

namespace Services;

public class BookLinks : IBookLinks
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IDataShaper<BookDto> _dataShaper;

    public BookLinks(LinkGenerator linkGenerator, IDataShaper<BookDto> dataShaper)
    {
        _linkGenerator = linkGenerator;
        _dataShaper = dataShaper;
    }

    public LinkResponse TryGenerateLinks(IEnumerable<BookDto> booksDto, string fields, HttpContext httpContext)
    {
        List<Entity> shapedBooks = ShapeData(booksDto, fields);
        if (ShouldGenerateLinks(httpContext)) return ReturnLinkedBooks(booksDto, fields, httpContext, shapedBooks);
        return ReturnShapedBooks(shapedBooks);
    }

    private LinkResponse ReturnLinkedBooks(IEnumerable<BookDto> booksDto, string fields, HttpContext httpContext, List<Entity> shapedBooks)
    {
        List<BookDto> bookDtoList = booksDto.ToList();
        for (int index = 0; index < bookDtoList.Count; index++)
        {
            List<Link> bookLinks = CreateForBook(httpContext, bookDtoList[index], fields);
            shapedBooks[index].Add("Links", bookLinks);
        }
        LinkCollectionWrapper<Entity> bookCollection = new LinkCollectionWrapper<Entity>(shapedBooks);
        CreateForBooks(httpContext, bookCollection);
        return new LinkResponse { HasLinks = true, LinkedEntities = bookCollection };
    }

    private void CreateForBooks(HttpContext httpContext, LinkCollectionWrapper<Entity> bookCollectionWrapper)
    {
        bookCollectionWrapper.Links.Add(new Link()
        {
            Href = $"/api/{httpContext.GetRouteData().Values["controller"].ToString().ToLower()}",
            Rel = "self",
            Method = "GET",
        });
    }

    private List<Link> CreateForBook(HttpContext httpContext, BookDto bookDto, string fields)
    {
        List<Link> links = new List<Link>()
        {
            new Link("a1", "b1", "c1")
            {
                Href=$"/api/{httpContext.GetRouteData().Values["controller"].ToString().ToLower()}/{bookDto.Id}",
                Rel="self",
                Method="GET"
            },
            new Link("a2", "b2", "c2")
            {
                Href=$"/api/{httpContext.GetRouteData().Values["controller"].ToString().ToLower()}",
                Rel="create",
                Method="POST"
            },
        };
        return links;
    }

    private LinkResponse ReturnShapedBooks(List<Entity> shapedBooks)
    {
        return new LinkResponse() { ShapedEntities = shapedBooks };
    }

    private bool ShouldGenerateLinks(HttpContext httpContext)
    {
        MediaTypeHeaderValue mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];
        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
    }

    private List<Entity> ShapeData(IEnumerable<BookDto> booksDto, string fields)
    {
        return _dataShaper.ShapeListData(booksDto, fields).Select(b => b.Entity).ToList();
    }
}
