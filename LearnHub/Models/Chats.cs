namespace LearnHub.Models;

public partial class Chats
{
    public int ChatId { get; set; }
    public int FirstParticipant { get; set; }
    public int SecondParticipant { get; set; }
    public virtual Users FirstParticipantNavigation { get; set; } = null!;
    public virtual ICollection<Messages> Messages { get; set; } = new List<Messages>();
    public virtual Users SecondParticipantNavigation { get; set; } = null!;
}