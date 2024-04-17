using FluentValidation;
using FastEndpoints;

namespace RiverBooks.Books.Endpoints;

public class CreateBookRequestValidator : Validator<CreateBookRequest>
{
  public CreateBookRequestValidator()
  {
    RuleFor(x => x.Title)
    .NotNull()
    .NotEmpty()
    .WithMessage("A book title is required.");

    RuleFor(x => x.Author)
    .NotNull()
    .NotEmpty()
    .WithMessage("A book author is required.");

    RuleFor(x => x.Price)
      .GreaterThanOrEqualTo(0m)
      .WithMessage("Book prices must be positive values.");
  }
}
