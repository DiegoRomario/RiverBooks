namespace RiverBooks.EmailSending.EmailBackgroundService;

internal interface ISendEmailsFromOutboxService
{
  Task CheckForAndSendEmails();
}
