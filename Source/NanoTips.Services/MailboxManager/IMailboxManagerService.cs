using NanoTips.Services.Models;

namespace NanoTips.Services.MailboxManager;

public interface IMailboxManagerService
{
    Task<MailboxViewModel> RegisterMailbox(MailboxEditorModel model);
}