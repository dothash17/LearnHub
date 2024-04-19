﻿namespace LearnHub.Models;

public partial class Materials
{
    public int MaterialId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string Link { get; set; } = null!;

    public int LessonId { get; set; }

    public virtual Lessons Lesson { get; set; } = null!;
}