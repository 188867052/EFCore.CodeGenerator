using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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

        public virtual DbSet<Student> Student { get; set; }

        public virtual DbSet<Teacher> Teacher { get; set; }

        public virtual DbSet<TeacherCourseMapping> TeacherCourseMapping { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=47.105.214.235;Initial Catalog=Scaffolding;Persist Security Info=True;User ID=sa;Password=931592457czA");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("class");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.HeadTeacherId).HasColumnName("head_teacher_id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
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

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(10);

                entity.Property(e => e.TeacherId)
                    .HasColumnName("teacher_id")
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
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime");

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

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("student");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(10);

                entity.Property(e => e.ClassId).HasColumnName("class_id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(10);

                entity.Property(e => e.Sex)
                    .HasColumnName("sex")
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

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Sex)
                    .HasColumnName("sex")
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<TeacherCourseMapping>(entity =>
            {
                entity.ToTable("teacher_course_mapping");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("datetime");

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
        }
    }
}
