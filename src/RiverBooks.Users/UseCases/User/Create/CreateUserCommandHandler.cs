using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RiverBooks.EmailSending.Contracts;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.UseCases.User.Create;

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result>
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IMediator _mediator;

  public CreateUserCommandHandler(UserManager<ApplicationUser> userManager,
    IMediator mediator)
  {
    _userManager = userManager;
    _mediator = mediator;
  }

  public async Task<Result> Handle(CreateUserCommand command, CancellationToken cancellationToken)
  {
    var newUser = new ApplicationUser
    {
      Email = command.Email,
      UserName = command.Email
    };

    var result = await _userManager.CreateAsync(newUser, command.Password);

    if (!result.Succeeded)
    {
      return Result.Error(result.Errors.Select(e => e.Description).ToArray());
    }

    // send welcome email
    var sendEmailCommand = new SendEmailCommand
    {
      To = command.Email,
      From = "donotreply@test.com",
      Subject = "Welcome to RiverBooks!",
      Body = "Thank you for registering."
    };

    _ = await _mediator.Send(sendEmailCommand);

    return Result.Success();
  }
}
