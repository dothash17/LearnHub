﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace LearnHub.Models;

public partial class Enrollments
{
    public int EnrollmentId { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public int UserId { get; set; }

    public int CourseId { get; set; }

    public virtual Courses Course { get; set; }

    public virtual Users User { get; set; }
}