namespace LearnHub.Models;

public partial class Users
{
    public int UserId { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime RegistrationDate { get; set; }

    public string? Avatar { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<Chats> ChatsFirstParticipantNavigation { get; set; } = new List<Chats>();

    public virtual ICollection<Chats> ChatsSecondParticipantNavigation { get; set; } = new List<Chats>();

    public virtual ICollection<Courses> Courses { get; set; } = new List<Courses>();

    public virtual ICollection<Enrollments> Enrollments { get; set; } = new List<Enrollments>();

    public virtual ICollection<Grades> Grades { get; set; } = new List<Grades>();

    public virtual ICollection<Messages> MessagesRecipient { get; set; } = new List<Messages>();

    public virtual ICollection<Messages> MessagesSender { get; set; } = new List<Messages>();

    public virtual ICollection<Progress> Progress { get; set; } = new List<Progress>();
}