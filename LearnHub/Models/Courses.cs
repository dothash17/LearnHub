namespace LearnHub.Models;

public partial class Courses
{
    public int CourseId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public virtual ICollection<Enrollments> Enrollments { get; set; } = new List<Enrollments>();

    public virtual ICollection<Grades> Grades { get; set; } = new List<Grades>();

    public virtual ICollection<Lessons> Lessons { get; set; } = new List<Lessons>();

    public virtual ICollection<Progress> Progress { get; set; } = new List<Progress>();

    public virtual ICollection<UserCourse> UserCourse { get; set; } = new List<UserCourse>();
}