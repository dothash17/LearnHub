namespace LearnHub.Models;

public partial class Assignments
{
    public int AssignmentId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime? Deadline { get; set; }

    public int LessonId { get; set; }

    public virtual Lessons Lesson { get; set; } = null!;
}