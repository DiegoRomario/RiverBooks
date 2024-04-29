using System.Dynamic;
using System.Reflection;
using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RiverBooks.Books.Contracts;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

public class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, 
  Result>
{
  private readonly IApplicationUserRepository _userRepository;
  private readonly IMediator _mediator;
  private readonly IServiceProvider _serviceProvider;

  public AddItemToCartHandler(IApplicationUserRepository userRepository,
    IMediator mediator,
    IServiceProvider serviceProvider)
  {
    _userRepository = userRepository;
    _mediator = mediator;
    _serviceProvider = serviceProvider;
  }

  /// <summary>
  /// Do NOT use this method in production code.
  /// This is a demonstration of how to use reflection to call a method 
  /// on a dynamically loaded assembly.
  /// </summary>
  /// <param name="bookId"></param>
  /// <returns></returns>
  public async Task<dynamic> GetBookByIdAsync(Guid bookId)
  {
#nullable disable
    // Load the assembly containing the BookService and IBookRepository types
    var booksAssembly = Assembly.Load("RiverBooks.Books");

    // Dynamically get the IBookRepository type
    var bookRepositoryType = booksAssembly.GetType("RiverBooks.Books.IBookRepository");

    // Use the ServiceProvider to dynamically get an instance of IBookRepository
    var bookRepository = _serviceProvider.GetRequiredService(bookRepositoryType);

    // Dynamically get the BookService type
    var bookServiceType = booksAssembly.GetType("RiverBooks.Books.BookService");

    // Create an instance of BookService using reflection
    var bookServiceConstructor = bookServiceType
      .GetConstructor(new[] { bookRepositoryType });
    var bookServiceInstance = bookServiceConstructor
      .Invoke(new object[] { bookRepository });

    // Get the MethodInfo for GetBookByIdAsync
    var getBookByIdAsyncMethod = bookServiceType.GetMethod("GetBookByIdAsync", BindingFlags.Public | BindingFlags.Instance);

    // Invoke GetBookByIdAsync without specifying the return type
    var task = (Task)getBookByIdAsyncMethod.Invoke(bookServiceInstance, new object[] { bookId });

    // Await the task to complete
    await task.ConfigureAwait(false);

    // Access the result of the task once completed
    var resultProperty = task.GetType().GetProperty("Result");
    var bookDto = resultProperty.GetValue(task);

    // Map the result to a dynamic object
    dynamic result = new ExpandoObject();
    result.Id = bookDto.GetType().GetProperty("Id").GetValue(bookDto);
    result.Title = bookDto.GetType().GetProperty("Title").GetValue(bookDto);
    result.Author = bookDto.GetType().GetProperty("Author").GetValue(bookDto);
    result.Price = bookDto.GetType().GetProperty("Price").GetValue(bookDto);

    return result;
#nullable restore
  }

  public async Task<Result> Handle(AddItemToCartCommand request, 
    CancellationToken ct)
  {
    var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

    if (user is null)
    {
      return Result.Unauthorized();
    }

    // Use reflection to call a method on a dynamically loaded assembly
    //var dynamicResult = await GetBookByIdAsync(request.BookId);

    //string description = $"{dynamicResult.Title} by {dynamicResult.Author}";

    //var newCartItem = new CartItem(request.BookId,
    //  description,
    //  request.Quantity,
    //  dynamicResult.Price);

    var query = new BookDetailsQuery(request.BookId);

    var result = await _mediator.Send(query);

    if (result.Status == ResultStatus.NotFound) return Result.NotFound();

    var bookDetails = result.Value;


    var description = $"{bookDetails.Title} by {bookDetails.Author}";
    var newCartItem = new CartItem(request.BookId,
      description,
      request.Quantity,
      bookDetails.Price);

    user.AddItemToCart(newCartItem);

    await _userRepository.SaveChangesAsync();

    return Result.Success();
  }
}
