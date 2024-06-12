namespace LearnHub.Models;

public partial class Messages
{
    public int MessageId { get; set; }
    public string MessageText { get; set; } = null!;
    public DateTime SentDate { get; set; }
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
    public int ChatId { get; set; }
    public virtual Chats Chat { get; set; } = null!;
    public virtual Users Recipient { get; set; } = null!;
    public virtual Users Sender { get; set; } = null!;
}