using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace RiverBooks.Users.UserEndpoints;
internal class Create : Endpoint<CreateUserRequest>
{
  private readonly UserManager<ApplicationUser> _userManager;

  public Create(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public override void Configure()
  {
    Post("/users");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateUserRequest request,
    CancellationToken cancellationToken)
  {
    var newUser = new ApplicationUser { 
      Email = request.Email,
      UserName = request.Email
    };

    await _userManager.CreateAsync(newUser, request.Password);

    await SendOkAsync();
  }
}
