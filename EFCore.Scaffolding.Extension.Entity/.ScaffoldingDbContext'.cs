using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public partial class ScaffoldingDbContext : DbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasOne(d => d.Grade)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.GradeId)
                    .HasConstraintName("FK_class_grade");
            });
        }
    }
}
