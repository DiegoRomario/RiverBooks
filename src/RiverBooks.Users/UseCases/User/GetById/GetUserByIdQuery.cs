using Ardalis.Result;
using MediatR;

namespace RiverBooks.Users.UseCases.User.GetById;
internal record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDTO>>;

