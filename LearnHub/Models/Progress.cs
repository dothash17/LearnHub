namespace LearnHub.Models;

public partial class Progress
{
    public int ProgressId { get; set; }
    public DateTime CompletedAssignment { get; set; }
    public int UserId { get; set; }
    public int AssignmentId { get; set; }
    public virtual Assignments Assignment { get; set; } = null!;
    public virtual Users User { get; set; } = null!;
}