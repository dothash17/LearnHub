namespace LearnHub.Models;

public partial class Enrollments
{
    public int EnrollmentId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public virtual Courses Course { get; set; } = null!;
    public virtual Users User { get; set; } = null!;
}