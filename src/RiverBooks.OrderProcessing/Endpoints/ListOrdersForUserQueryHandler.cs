using Ardalis.Result;
using MediatR;
using RiverBooks.OrderProcessing.Interfaces;

namespace RiverBooks.OrderProcessing.Endpoints;

internal class ListOrdersForUserQueryHandler :
  IRequestHandler<ListOrdersForUserQuery,
  Result<List<OrderSummary>>>
{
  private readonly IOrderRepository _orderRepository;

  public ListOrdersForUserQueryHandler(IOrderRepository orderRepository)
  {
    _orderRepository = orderRepository;
  }
  public async Task<Result<List<OrderSummary>>> Handle(ListOrdersForUserQuery request, CancellationToken cancellationToken)
  {
    // look up UserId for EmailAddress

    // TODO: Filter by User
    var orders = await _orderRepository.ListAsync();

    var summaries = orders.Select(o => new OrderSummary
      {
        DateCreated = o.DateCreated,
        OrderId = o.Id,
        UserId = o.UserId,
        Total = o.OrderItems.Sum(oi => oi.UnitPrice) // need to .Include OrderItems
      })
      .ToList();

    return summaries;
  }
}
