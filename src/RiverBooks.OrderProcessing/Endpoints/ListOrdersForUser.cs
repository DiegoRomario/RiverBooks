using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;

namespace RiverBooks.OrderProcessing.Endpoints;


internal class ListOrdersForUser : EndpointWithoutRequest<ListOrdersForUserResponse>
{
  private readonly IMediator _mediator;

  public ListOrdersForUser(IMediator mediator)
  {
    _mediator = mediator;
  }
  public override void Configure()
  {
    Get("/orders");
    Claims("EmailAddress");
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var emailAddress = User.FindFirstValue("EmailAddress");

    var query = new ListOrdersForUserQuery(emailAddress!);

    var result = await _mediator.Send(query, ct);

    if (result.Status == ResultStatus.Unauthorized)
    {
      await SendUnauthorizedAsync(ct);
    }
    else
    {
      var response = new ListOrdersForUserResponse();
      response.Orders = result.Value.Select(o => new OrderSummary
      {
        DateCreated = o.DateCreated,
        DateShipped = o.DateShipped,
        Total = o.Total,
        UserId = o.UserId,
        OrderId = o.OrderId
      }).ToList();
      await SendAsync(response);
    }
  }
}


