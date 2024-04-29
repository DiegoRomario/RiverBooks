using FastEndpoints;
using MongoDB.Driver;

namespace RiverBooks.EmailSending.ListEmailsEndpoint;

public class ListEmailsResponse
{
  public int Count { get; set; }
  public List<EmailOutboxEntity> Emails { get; internal set; } = new();
}


internal class ListEmails : EndpointWithoutRequest<ListEmailsResponse>
{
  private readonly IMongoCollection<EmailOutboxEntity> _emailCollection;

  public ListEmails(IMongoCollection<EmailOutboxEntity> emailCollection)
  {
    _emailCollection = emailCollection;
  }

  public override void Configure()
  {
    Get("/emails");
    AllowAnonymous();
  }

  public override async Task HandleAsync(
  CancellationToken ct = default)
  {
    // TODO: Implement paging
    var filter = Builders<EmailOutboxEntity>.Filter.Empty;
    var emailEntities = await _emailCollection.Find(filter)
      .ToListAsync();

    var response = new ListEmailsResponse()
    {
      Count = emailEntities.Count,
      Emails = emailEntities // TODO: Use a separate DTO
    };

    Response = response;
  }

}
