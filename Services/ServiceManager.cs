using AutoMapper;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Repositories.Contracts;
using Services.Contracts;

namespace Services;
public class ServiceManager : IServiceManager
{
    private readonly Lazy<IBookService> _bookService;
    private readonly Lazy<IAuthenticationService> _authenticationService;
    public ServiceManager(IRepositoryManager repositoryManager,
        ILoggerService loggerService,
        IMapper mapper, IConfiguration configuration,
        UserManager<User> userManager,
        IBookLinks bookLinks)
    {
        _bookService = new Lazy<IBookService>(() => new BookService(repositoryManager, mapper, bookLinks));
        _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(loggerService, mapper, userManager, configuration));
    }

    public IBookService BookService => _bookService.Value;

    public IAuthenticationService AuthenticationService => _authenticationService.Value;
}