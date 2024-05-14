namespace LearnHub.Models;

public partial class Assignments
{
    public int AssignmentId { get; set; }

    public string Task { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public int LessonId { get; set; }

    public virtual Lessons Lesson { get; set; } = null!;

    public virtual ICollection<Progress> Progress { get; set; } = new List<Progress>();
}