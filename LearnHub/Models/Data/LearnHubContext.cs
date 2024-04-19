﻿using Microsoft.EntityFrameworkCore;

namespace LearnHub.Models.Data;

public partial class LearnHubContext : DbContext
{
    public LearnHubContext()
    {
    }

    public LearnHubContext(DbContextOptions<LearnHubContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignments> Assignments { get; set; }

    public virtual DbSet<Chats> Chats { get; set; }

    public virtual DbSet<Courses> Courses { get; set; }

    public virtual DbSet<Enrollments> Enrollments { get; set; }

    public virtual DbSet<Grades> Grades { get; set; }

    public virtual DbSet<Lessons> Lessons { get; set; }

    public virtual DbSet<Materials> Materials { get; set; }

    public virtual DbSet<Messages> Messages { get; set; }

    public virtual DbSet<Progress> Progress { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-ID583BI\\SQLEXPRESS;Initial Catalog=LearnHub;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignments>(entity =>
        {
            entity.HasKey(e => e.AssignmentId);

            entity.HasIndex(e => e.LessonId, "IX_Assignments_LessonID");

            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.Deadline).HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Lesson).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assignments_Lessons");
        });

        modelBuilder.Entity<Chats>(entity =>
        {
            entity.HasKey(e => e.ChatId);

            entity.HasIndex(e => e.FirstParticipant, "IX_Chats_FirstParticipant");

            entity.HasIndex(e => e.SecondParticipant, "IX_Chats_SecondParticipant");

            entity.Property(e => e.ChatId).HasColumnName("ChatID");

            entity.HasOne(d => d.FirstParticipantNavigation).WithMany(p => p.ChatsFirstParticipantNavigation)
                .HasForeignKey(d => d.FirstParticipant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chats_Users");

            entity.HasOne(d => d.SecondParticipantNavigation).WithMany(p => p.ChatsSecondParticipantNavigation)
                .HasForeignKey(d => d.SecondParticipant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chats_Users2");
        });

        modelBuilder.Entity<Courses>(entity =>
        {
            entity.HasKey(e => e.CourseId);

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Courses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Courses_Users");
        });

        modelBuilder.Entity<Enrollments>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId);

            entity.HasIndex(e => e.CourseId, "IX_Enrollments_CourseID");

            entity.HasIndex(e => e.UserId, "IX_Enrollments_UserID");

            entity.Property(e => e.EnrollmentId).HasColumnName("EnrollmentID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.EnrollmentDate).HasColumnType("date");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollments_Courses");

            entity.HasOne(d => d.User).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollments_Users");
        });

        modelBuilder.Entity<Grades>(entity =>
        {
            entity.HasKey(e => e.GradeId);

            entity.HasIndex(e => e.CourseId, "IX_Grades_CourseID");

            entity.HasIndex(e => e.UserId, "IX_Grades_UserID");

            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.Comment).HasMaxLength(300);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Course).WithMany(p => p.Grades)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grades_Courses");

            entity.HasOne(d => d.User).WithMany(p => p.Grades)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grades_Users");
        });

        modelBuilder.Entity<Lessons>(entity =>
        {
            entity.HasKey(e => e.LessonId);

            entity.HasIndex(e => e.CourseId, "IX_Lessons_CourseID");

            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Text).HasColumnType("text");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Course).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lessons_Courses");
        });

        modelBuilder.Entity<Materials>(entity =>
        {
            entity.HasKey(e => e.MaterialId);

            entity.HasIndex(e => e.LessonId, "IX_Materials_LessonID");

            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.Link).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Lesson).WithMany(p => p.Materials)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Materials_Lessons");
        });

        modelBuilder.Entity<Messages>(entity =>
        {
            entity.HasKey(e => e.MessageId);

            entity.HasIndex(e => e.ChatId, "IX_Messages_ChatID");

            entity.HasIndex(e => e.RecipientId, "IX_Messages_RecipientID");

            entity.HasIndex(e => e.SenderId, "IX_Messages_SenderID");

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.ChatId).HasColumnName("ChatID");
            entity.Property(e => e.MessageText).HasMaxLength(200);
            entity.Property(e => e.RecipientId).HasColumnName("RecipientID");
            entity.Property(e => e.SenderId).HasColumnName("SenderID");
            entity.Property(e => e.SentDate).HasColumnType("datetime");

            entity.HasOne(d => d.Chat).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Chats");

            entity.HasOne(d => d.Recipient).WithMany(p => p.MessagesRecipient)
                .HasForeignKey(d => d.RecipientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Users3");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessagesSender)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Users2");
        });

        modelBuilder.Entity<Progress>(entity =>
        {
            entity.HasIndex(e => e.CourseId, "IX_Progress_CourseID");

            entity.HasIndex(e => e.UserId, "IX_Progress_UserID");

            entity.Property(e => e.ProgressId).HasColumnName("ProgressID");
            entity.Property(e => e.CompletedMaterial).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Course).WithMany(p => p.Progress)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Progress_Courses");

            entity.HasOne(d => d.User).WithMany(p => p.Progress)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Progress_Users");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(30);
            entity.Property(e => e.LastName).HasMaxLength(30);
            entity.Property(e => e.Password)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationDate).HasColumnType("date");
            entity.Property(e => e.Username).HasMaxLength(30);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}