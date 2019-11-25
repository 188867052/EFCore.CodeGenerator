using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using EFCore.Scaffolding.Extension.Entity.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Entities
{
    public partial class ScaffoldingDbContext : DbContext
    {
        public ScaffoldingDbContext()
        {
        }

        public ScaffoldingDbContext(DbContextOptions<ScaffoldingDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Class> Class { get; set; }

        public virtual DbSet<Course> Course { get; set; }

        public virtual DbSet<CourseScore> CourseScore { get; set; }

        public virtual DbSet<Grade> Grade { get; set; }

        public virtual DbSet<Log> Log { get; set; }

        public virtual DbSet<Student> Student { get; set; }

        public virtual DbSet<Teacher> Teacher { get; set; }

        public virtual DbSet<TeacherCourseMapping> TeacherCourseMapping { get; set; }

        public virtual DbSet<VLog> VLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(EFCore.Scaffolding.Extension.Connection.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.GradeId).HasColumnName("grade_id");

                entity.Property(e => e.HeadTeacherId).HasColumnName("head_teacher_id");

                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.HeadTeacher)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.HeadTeacherId);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.IsDeleted)
                    .HasConversion(new BoolToZeroOneConverter<int>())
                    .HasColumnName("is_deleted");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.TeacherId)
                    .HasColumnName("teacher_id")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<CourseScore>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.CourseScore)
                    .HasForeignKey<CourseScore>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_course_score_course");
            });

            modelBuilder.Entity<Grade>(entity =>
            {
                entity.Property(e => e.Id).HasComment("主键");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasComment("名称");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasKey(e => e.Identifier)
                    .HasName("PK_log");

                entity.Property(e => e.Identifier)
                    .HasColumnName("identifier")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateTimeTicks)
                    .HasConversion(new DateTimeToTicksConverter())
                    .HasColumnName("update_time_ticks");

                entity.Property(e => e.Url)
                    .HasConversion(new UriToStringConverter())
                    .HasColumnName("url")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.ClassId).HasColumnName("class_id");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.IsDeleted)
                    .HasConversion(new BoolToStringConverter(bool.FalseString, bool.TrueString))
                    .IsRequired()
                    .HasColumnName("is_deleted")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Sex)
                    .HasConversion(new EnumToStringConverter<SexEnum>())
                    .HasColumnName("sex")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("FK_student_class");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Sex)
                    .HasConversion(new EnumToStringConverter<SexEnum>())
                    .HasColumnName("sex")
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<TeacherCourseMapping>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.TeacherCourseMapping)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("FK_course_id");

                entity.HasOne(d => d.Teacher)
                    .WithMany(p => p.TeacherCourseMapping)
                    .HasForeignKey(d => d.TeacherId)
                    .HasConstraintName("FK_teacher_id");
            });

            modelBuilder.Entity<VLog>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_log");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.Identifier).HasColumnName("identifier");

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasMaxLength(50);

                entity.Property(e => e.NewId).HasColumnName("new_id");

                entity.Property(e => e.UpdateTimeTicks)
                    .HasConversion(new DateTimeToTicksConverter())
                    .HasColumnName("update_time_ticks");

                entity.Property(e => e.Url)
                    .HasConversion(new UriToStringConverter())
                    .HasColumnName("url")
                    .HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
