using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ExamPortal.Domain.Entities;

namespace ExamPortal.Infrastructure.Data;

public partial class ExamPortalDbContext : DbContext
{
    public ExamPortalDbContext()
    {
    }

    public ExamPortalDbContext(DbContextOptions<ExamPortalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnswerOption> AnswerOptions { get; set; }

    public virtual DbSet<ExamAttempt> ExamAttempts { get; set; }

    public virtual DbSet<ExamSet> ExamSets { get; set; }

    public virtual DbSet<InstructionSet> InstructionSets { get; set; }

    public virtual DbSet<QuestionSet> QuestionSets { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAnswer> UserAnswers { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        // => optionsBuilder.UseSqlServer("Data Source=LAPTOP-THEAYUSH\\SQLEXPRESS;Database=ExamPortalDb;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Command Timeout=0");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnswerOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AnswerOp__3214EC07202807D0");

            entity.HasIndex(e => e.QuestionSetId, "IX_AnswerOptions_QuestionSetId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.OptionText).HasMaxLength(500);

            entity.HasOne(d => d.QuestionSet).WithMany(p => p.AnswerOptions)
                .HasForeignKey(d => d.QuestionSetId)
                .HasConstraintName("FK_AnswerOptions_QuestionSets");
        });

        modelBuilder.Entity<ExamAttempt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExamAtte__3214EC07E6731D39");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.TotalScore).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.ExamSet).WithMany(p => p.ExamAttempts)
                .HasForeignKey(d => d.ExamSetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExamAttempts_ExamSets");

            entity.HasOne(d => d.User).WithMany(p => p.ExamAttempts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExamAttempts_Users");
        });

        modelBuilder.Entity<ExamSet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExamSets__3214EC07C42C9423");

            entity.HasIndex(e => e.TenantId, "IX_ExamSets_TenantId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PassingScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Tenant).WithMany(p => p.ExamSets)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_ExamSets_Tenants");
        });

        modelBuilder.Entity<InstructionSet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Instruct__3214EC075309B5E7");

            entity.HasIndex(e => e.ExamSetId, "UQ__Instruct__03D28BA435D00C5E").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AgreementRequired).HasDefaultValue(true);
            entity.Property(e => e.Content).HasMaxLength(1000);

            entity.HasOne(d => d.ExamSet).WithOne(p => p.InstructionSet)
                .HasForeignKey<InstructionSet>(d => d.ExamSetId)
                .HasConstraintName("FK_InstructionSets_ExamSets");
        });

        modelBuilder.Entity<QuestionSet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC07B537FBB9");

            entity.HasIndex(e => e.ExamSetId, "IX_QuestionSets_ExamSetId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Points)
                .HasDefaultValue(1.0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.QuestionText).HasMaxLength(800);

            entity.HasOne(d => d.ExamSet).WithMany(p => p.QuestionSets)
                .HasForeignKey(d => d.ExamSetId)
                .HasConstraintName("FK_QuestionSets_ExamSets");
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tenants__3214EC071CFE9784");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Domain).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0790B1D544");

            entity.HasIndex(e => new { e.Email, e.TenantId }, "IX_Users_Email_Tenant").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Role).HasMaxLength(20);

            entity.HasOne(d => d.Tenant).WithMany(p => p.Users)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_Users_Tenants");
        });

        modelBuilder.Entity<UserAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAnsw__3214EC070F84E405");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Attempt).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.AttemptId)
                .HasConstraintName("FK_UserAnswers_ExamAttempts");

            entity.HasOne(d => d.Question).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAnswers_QuestionSets");

            entity.HasOne(d => d.SelectedOption).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.SelectedOptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAnswers_AnswerOptions");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
