using FastEndpoints;

namespace RiverBooks.Books.Endpoints;

internal class UpdatePrice(IBookService bookService) :
  Endpoint<UpdateBookPriceRequest, BookDto>
{
  private readonly IBookService _bookService = bookService;

  public override void Configure()
  {
    Post("/books/{Id}/pricehistory");
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateBookPriceRequest request,
    CancellationToken ct)
  {
    // TODO: Handle not found

    await _bookService.UpdateBookPriceAsync(request.Id, request.NewPrice);

    var updatedBook = await _bookService.GetBookByIdAsync(request.Id);

    await SendAsync(updatedBook);
  }
}

