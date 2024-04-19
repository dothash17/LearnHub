namespace LearnHub.Models;

public partial class Lessons
{
    public int LessonId { get; set; }

    public string Title { get; set; } = null!;

    public string Text { get; set; } = null!;

    public int CourseId { get; set; }

    public virtual ICollection<Assignments> Assignments { get; set; } = new List<Assignments>();

    public virtual Courses Course { get; set; } = null!;

    public virtual ICollection<Materials> Materials { get; set; } = new List<Materials>();
}