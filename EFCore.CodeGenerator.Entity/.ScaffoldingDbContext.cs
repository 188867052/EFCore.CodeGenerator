using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using EFCore.CodeGenerator;

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
                optionsBuilder.UseSqlServer(Connection.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>(entity =>
            {
                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Location)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Grade)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.GradeId);

                entity.HasOne(d => d.HeadTeacher)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.HeadTeacherId);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.IsDeleted).HasConversion(new BoolToZeroOneConverter<int>());

                entity.Property(e => e.Name)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.TeacherId)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CourseScore>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

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

                entity.Property(e => e.Identifier).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Message).HasMaxLength(50);

                entity.Property(e => e.UpdateTimeTicks).HasConversion(new DateTimeToTicksConverter());

                entity.Property(e => e.Url)
                    .HasConversion(new UriToStringConverter())
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(e => e.Address)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.IsDeleted)
                    .HasConversion(new BoolToStringConverter(bool.FalseString, bool.TrueString))
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Mobile)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Sex)
                    .HasConversion(new EnumToStringConverter<SexEnum>())
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("FK_student_class");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Sex)
                    .HasConversion(new EnumToStringConverter<SexEnum>())
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TeacherCourseMapping>(entity =>
            {
                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

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

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Message).HasMaxLength(50);

                entity.Property(e => e.NewId).HasColumnName("new_id");

                entity.Property(e => e.UpdateTimeTicks).HasConversion(new DateTimeToTicksConverter());

                entity.Property(e => e.Url)
                    .HasConversion(new UriToStringConverter())
                    .HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
