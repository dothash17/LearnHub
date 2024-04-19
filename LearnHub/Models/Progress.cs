namespace LearnHub.Models;

public partial class Progress
{
    public int ProgressId { get; set; }

    public decimal CompletedMaterial { get; set; }

    public int UserId { get; set; }

    public int CourseId { get; set; }

    public virtual Courses Course { get; set; } = null!;

    public virtual Users User { get; set; } = null!;
}