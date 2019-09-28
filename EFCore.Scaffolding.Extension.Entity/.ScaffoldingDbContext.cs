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

        public virtual DbSet<Log> Log { get; set; }

        public virtual DbSet<Student> Student { get; set; }

        public virtual DbSet<Teacher> Teacher { get; set; }

        public virtual DbSet<TeacherCourseMapping> TeacherCourseMapping { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(EFCore.Scaffolding.Extension.Connection.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("class");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.Grade)
                    .HasColumnName("grade")
                    .HasColumnType("nchar(10)")
                    .HasMaxLength(10);

                entity.Property(e => e.HeadTeacherId)
                    .HasColumnName("head_teacher_id")
                    .HasColumnType("int");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasColumnType("bit");

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasColumnType("nchar(10)")
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("nvarchar(50)")
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.HeadTeacher)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.HeadTeacherId)
                    .HasConstraintName("FK_class_teacher");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("course");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsDeleted)
                    .HasConversion(new BoolToZeroOneConverter<int>())
                    .HasColumnName("is_deleted")
                    .HasColumnType("int");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("nchar(10)")
                    .HasMaxLength(10);

                entity.Property(e => e.TeacherId)
                    .HasColumnName("teacher_id")
                    .HasColumnType("nchar(10)")
                    .HasMaxLength(10);

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<CourseScore>(entity =>
            {
                entity.ToTable("course_score");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CourseId)
                    .HasColumnName("course_id")
                    .HasColumnType("int");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.Score)
                    .HasColumnName("score")
                    .HasColumnType("int");

                entity.Property(e => e.StudentId)
                    .HasColumnName("student_id")
                    .HasColumnType("int");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.CourseScore)
                    .HasForeignKey<CourseScore>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_course_score_course");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasKey(e => e.Identifier);

                entity.ToTable("log");

                entity.Property(e => e.Identifier)
                    .HasColumnName("identifier")
                    .HasColumnType("uniqueidentifier")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasColumnType("nvarchar(50)")
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateTimeTicks)
                    .HasConversion(new DateTimeToTicksConverter())
                    .HasColumnName("update_time_ticks")
                    .HasColumnType("bigint");

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasColumnType("nvarchar(100)")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("student");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasColumnType("nchar(10)")
                    .HasMaxLength(10);

                entity.Property(e => e.ClassId)
                    .HasColumnName("class_id")
                    .HasColumnType("int");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsDeleted)
                    .HasConversion(new BoolToStringConverter(bool.FalseString, bool.TrueString))
                    .IsRequired()
                    .HasColumnName("is_deleted")
                    .HasColumnType("nchar(10)")
                    .HasMaxLength(10);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasColumnType("nchar(10)")
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("nchar(10)")
                    .HasMaxLength(10);

                entity.Property(e => e.Sex)
                    .HasConversion(new EnumToStringConverter<SexEnum>())
                    .HasColumnName("sex")
                    .HasColumnType("nchar(10)")
                    .HasMaxLength(10);

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
                entity.ToTable("teacher");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("nvarchar(50)")
                    .HasMaxLength(50);

                entity.Property(e => e.Sex)
                    .HasConversion(new EnumToStringConverter<SexEnum>())
                    .HasColumnName("sex")
                    .HasColumnType("nvarchar(50)")
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<TeacherCourseMapping>(entity =>
            {
                entity.ToTable("teacher_course_mapping");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int");

                entity.Property(e => e.CourseId)
                    .HasColumnName("course_id")
                    .HasColumnType("int");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.TeacherId)
                    .HasColumnName("teacher_id")
                    .HasColumnType("int");

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
        }
    }
}
