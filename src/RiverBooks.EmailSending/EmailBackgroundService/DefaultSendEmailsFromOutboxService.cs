using Ardalis.Result;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace RiverBooks.EmailSending.EmailBackgroundService;

internal interface IGetEmailsFromOutboxService
{
  Task<Result<EmailOutboxEntity>> GetUnprocessedEmailEntity();
}

internal class MongoDbGetEmailsFromOutboxService : IGetEmailsFromOutboxService
{
  private readonly IMongoCollection<EmailOutboxEntity> _emailCollection;

  public MongoDbGetEmailsFromOutboxService(
    IMongoCollection<EmailOutboxEntity> emailCollection)
  {
    _emailCollection = emailCollection;
  }

  public async Task<Result<EmailOutboxEntity>> GetUnprocessedEmailEntity()
  {
    var filter = Builders<EmailOutboxEntity>.Filter.Eq(entity =>
            entity.DateTimeUtcProcessed, null);
    var unsentEmailEntity = await _emailCollection
                          .Find(filter)
                          .FirstOrDefaultAsync();

    if (unsentEmailEntity == null) return Result.NotFound();

    return unsentEmailEntity;
  }
}

internal class DefaultSendEmailsFromOutboxService : ISendEmailsFromOutboxService
{
  private readonly IGetEmailsFromOutboxService _outboxService;
  private readonly ISendEmail _emailSender;
  private readonly IMongoCollection<EmailOutboxEntity> _emailCollection;
  private readonly ILogger<DefaultSendEmailsFromOutboxService> _logger;

  public DefaultSendEmailsFromOutboxService(IGetEmailsFromOutboxService outboxService,
    ISendEmail emailSender,
    IMongoCollection<EmailOutboxEntity> emailCollection,
    ILogger<DefaultSendEmailsFromOutboxService> logger)
  {
    _outboxService = outboxService;
    _emailSender = emailSender;
    _emailCollection = emailCollection;
    _logger = logger;
  }

  public async Task CheckForAndSendEmails()
  {
    try
    {

      var result = await _outboxService.GetUnprocessedEmailEntity();

      if (result.Status == ResultStatus.NotFound) return;

      var emailEntity = result.Value;

      await _emailSender.SendEmailAsync(emailEntity.To,
        emailEntity.From,
        emailEntity.Subject,
        emailEntity.Body);

      var updateFilter = Builders<EmailOutboxEntity>
        .Filter.Eq(x => x.Id, emailEntity.Id);
      var update = Builders<EmailOutboxEntity>
        .Update.Set("DateTimeUtcProcessed", DateTime.UtcNow);
      var updateResult = await _emailCollection
        .UpdateOneAsync(updateFilter, update);

      _logger.LogInformation("Processed {result} email records.",
        updateResult.ModifiedCount);
    }
    finally
    {
      _logger.LogInformation("Sleeping...");
    }

  }
}
