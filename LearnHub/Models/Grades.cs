namespace LearnHub.Models;

public partial class Grades
{
    public int GradeId { get; set; }
    public byte Grade { get; set; }
    public string? Comment { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public virtual Courses Course { get; set; } = null!;
    public virtual Users User { get; set; } = null!;
}