using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Entities
{
    public partial class TestDbContext : DbContext
    {
        public TestDbContext()
        {
        }

        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AuthEntityInfo> AuthEntityInfo { get; set; }

        public virtual DbSet<AuthEntityRole> AuthEntityRole { get; set; }

        public virtual DbSet<AuthEntityUser> AuthEntityUser { get; set; }

        public virtual DbSet<AuthFunction> AuthFunction { get; set; }

        public virtual DbSet<AuthModule> AuthModule { get; set; }

        public virtual DbSet<AuthModuleFunction> AuthModuleFunction { get; set; }

        public virtual DbSet<AuthModuleRole> AuthModuleRole { get; set; }

        public virtual DbSet<AuthModuleUser> AuthModuleUser { get; set; }

        public virtual DbSet<ConverterTest> ConverterTest { get; set; }

        public virtual DbSet<IdentityLoginLog> IdentityLoginLog { get; set; }

        public virtual DbSet<IdentityOrganization> IdentityOrganization { get; set; }

        public virtual DbSet<IdentityRole> IdentityRole { get; set; }

        public virtual DbSet<IdentityRoleClaim> IdentityRoleClaim { get; set; }

        public virtual DbSet<IdentityUser> IdentityUser { get; set; }

        public virtual DbSet<IdentityUserClaim> IdentityUserClaim { get; set; }

        public virtual DbSet<IdentityUserDetail> IdentityUserDetail { get; set; }

        public virtual DbSet<IdentityUserLogin> IdentityUserLogin { get; set; }

        public virtual DbSet<IdentityUserRole> IdentityUserRole { get; set; }

        public virtual DbSet<IdentityUserToken> IdentityUserToken { get; set; }

        public virtual DbSet<InfosMessage> InfosMessage { get; set; }

        public virtual DbSet<InfosMessageReceive> InfosMessageReceive { get; set; }

        public virtual DbSet<InfosMessageReply> InfosMessageReply { get; set; }

        public virtual DbSet<SystemsAuditEntity> SystemsAuditEntity { get; set; }

        public virtual DbSet<SystemsAuditOperation> SystemsAuditOperation { get; set; }

        public virtual DbSet<SystemsAuditProperty> SystemsAuditProperty { get; set; }

        public virtual DbSet<SystemsKeyValue> SystemsKeyValue { get; set; }

        public virtual DbSet<VConverter> VConverter { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(EFCore.CodeGenerator.Connection.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthEntityInfo>(entity =>
            {
                entity.ToTable("Auth_EntityInfo");

                entity.HasIndex(e => e.TypeName)
                    .HasName("ClassFullNameIndex")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.PropertyJson).IsRequired();

                entity.Property(e => e.TypeName).IsRequired();
            });

            modelBuilder.Entity<AuthEntityRole>(entity =>
            {
                entity.ToTable("Auth_EntityRole");

                entity.HasIndex(e => e.RoleId);

                entity.HasIndex(e => new { e.EntityId, e.RoleId, e.Operation })
                    .HasName("EntityRoleIndex")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.AuthEntityRole)
                    .HasForeignKey(d => d.EntityId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AuthEntityRole)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AuthEntityUser>(entity =>
            {
                entity.ToTable("Auth_EntityUser");

                entity.HasIndex(e => e.UserId);

                entity.HasIndex(e => new { e.EntityId, e.UserId })
                    .HasName("EntityUserIndex");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.AuthEntityUser)
                    .HasForeignKey(d => d.EntityId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AuthEntityUser)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AuthFunction>(entity =>
            {
                entity.ToTable("Auth_Function");

                entity.HasIndex(e => new { e.Area, e.Controller, e.Action })
                    .HasName("AreaControllerActionIndex")
                    .IsUnique()
                    .HasFilter("([Area] IS NOT NULL AND [Controller] IS NOT NULL AND [Action] IS NOT NULL)");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<AuthModule>(entity =>
            {
                entity.ToTable("Auth_Module");

                entity.HasIndex(e => e.ParentId);

                entity.Property(e => e.Code).IsRequired();

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId);
            });

            modelBuilder.Entity<AuthModuleFunction>(entity =>
            {
                entity.ToTable("Auth_ModuleFunction");

                entity.HasIndex(e => e.FunctionId);

                entity.HasIndex(e => new { e.ModuleId, e.FunctionId })
                    .HasName("ModuleFunctionIndex")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Function)
                    .WithMany(p => p.AuthModuleFunction)
                    .HasForeignKey(d => d.FunctionId);

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.AuthModuleFunction)
                    .HasForeignKey(d => d.ModuleId);
            });

            modelBuilder.Entity<AuthModuleRole>(entity =>
            {
                entity.ToTable("Auth_ModuleRole");

                entity.HasIndex(e => e.RoleId);

                entity.HasIndex(e => new { e.ModuleId, e.RoleId })
                    .HasName("ModuleRoleIndex")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.AuthModuleRole)
                    .HasForeignKey(d => d.ModuleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AuthModuleRole)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AuthModuleUser>(entity =>
            {
                entity.ToTable("Auth_ModuleUser");

                entity.HasIndex(e => e.UserId);

                entity.HasIndex(e => new { e.ModuleId, e.UserId })
                    .HasName("ModuleUserIndex")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.AuthModuleUser)
                    .HasForeignKey(d => d.ModuleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AuthModuleUser)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<ConverterTest>(entity =>
            {
                entity.HasKey(e => e.Identifier)
                    .HasName("PK_log");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Message).HasMaxLength(50);

                entity.Property(e => e.UpdateTimeTicks).HasConversion(new DateTimeToTicksConverter());

                entity.Property(e => e.Url)
                    .HasConversion(new UriToStringConverter())
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<IdentityLoginLog>(entity =>
            {
                entity.ToTable("Identity_LoginLog");

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.IdentityLoginLog)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<IdentityOrganization>(entity =>
            {
                entity.ToTable("Identity_Organization");

                entity.HasIndex(e => e.ParentId);

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId);
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("Identity_Role");

                entity.HasIndex(e => e.MessageId);

                entity.HasIndex(e => new { e.NormalizedName, e.DeletedTime })
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([DeletedTime] IS NOT NULL)");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.NormalizedName).IsRequired();

                entity.Property(e => e.Remark).HasMaxLength(512);

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.IdentityRole)
                    .HasForeignKey(d => d.MessageId);
            });

            modelBuilder.Entity<IdentityRoleClaim>(entity =>
            {
                entity.ToTable("Identity_RoleClaim");

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.IdentityRoleClaim)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<IdentityUser>(entity =>
            {
                entity.ToTable("Identity_User");

                entity.HasIndex(e => e.MessageId);

                entity.HasIndex(e => new { e.NormalizeEmail, e.DeletedTime })
                    .HasName("EmailIndex");

                entity.HasIndex(e => new { e.NormalizedUserName, e.DeletedTime })
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([DeletedTime] IS NOT NULL)");

                entity.Property(e => e.NormalizedUserName).IsRequired();

                entity.Property(e => e.UserName).IsRequired();

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.IdentityUser)
                    .HasForeignKey(d => d.MessageId);
            });

            modelBuilder.Entity<IdentityUserClaim>(entity =>
            {
                entity.ToTable("Identity_UserClaim");

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.ClaimType).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.IdentityUserClaim)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<IdentityUserDetail>(entity =>
            {
                entity.ToTable("Identity_UserDetail");

                entity.HasIndex(e => e.UserId)
                    .IsUnique();

                entity.HasOne(d => d.User)
                    .WithOne(p => p.IdentityUserDetail)
                    .HasForeignKey<IdentityUserDetail>(d => d.UserId);
            });

            modelBuilder.Entity<IdentityUserLogin>(entity =>
            {
                entity.ToTable("Identity_UserLogin");

                entity.HasIndex(e => e.UserId);

                entity.HasIndex(e => new { e.LoginProvider, e.ProviderKey })
                    .HasName("UserLoginIndex")
                    .IsUnique()
                    .HasFilter("([LoginProvider] IS NOT NULL AND [ProviderKey] IS NOT NULL)");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.IdentityUserLogin)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<IdentityUserRole>(entity =>
            {
                entity.ToTable("Identity_UserRole");

                entity.HasIndex(e => e.RoleId);

                entity.HasIndex(e => new { e.UserId, e.RoleId, e.DeletedTime })
                    .HasName("UserRoleIndex")
                    .IsUnique()
                    .HasFilter("([DeletedTime] IS NOT NULL)");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.IdentityUserRole)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.IdentityUserRole)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<IdentityUserToken>(entity =>
            {
                entity.ToTable("Identity_UserToken");

                entity.HasIndex(e => new { e.UserId, e.LoginProvider, e.Name })
                    .HasName("UserTokenIndex")
                    .IsUnique()
                    .HasFilter("([LoginProvider] IS NOT NULL AND [Name] IS NOT NULL)");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.IdentityUserToken)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<InfosMessage>(entity =>
            {
                entity.ToTable("Infos_Message");

                entity.HasIndex(e => e.SenderId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.InfosMessage)
                    .HasForeignKey(d => d.SenderId);
            });

            modelBuilder.Entity<InfosMessageReceive>(entity =>
            {
                entity.ToTable("Infos_MessageReceive");

                entity.HasIndex(e => e.MessageId);

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.InfosMessageReceive)
                    .HasForeignKey(d => d.MessageId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.InfosMessageReceive)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<InfosMessageReply>(entity =>
            {
                entity.ToTable("Infos_MessageReply");

                entity.HasIndex(e => e.BelongMessageId);

                entity.HasIndex(e => e.ParentMessageId);

                entity.HasIndex(e => e.ParentReplyId);

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Content).IsRequired();

                entity.HasOne(d => d.BelongMessage)
                    .WithMany(p => p.InfosMessageReplyBelongMessage)
                    .HasForeignKey(d => d.BelongMessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ParentMessage)
                    .WithMany(p => p.InfosMessageReplyParentMessage)
                    .HasForeignKey(d => d.ParentMessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ParentReply)
                    .WithMany(p => p.InverseParentReply)
                    .HasForeignKey(d => d.ParentReplyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.InfosMessageReply)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SystemsAuditEntity>(entity =>
            {
                entity.ToTable("Systems_AuditEntity");

                entity.HasIndex(e => e.OperationId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.SystemsAuditEntity)
                    .HasForeignKey(d => d.OperationId);
            });

            modelBuilder.Entity<SystemsAuditOperation>(entity =>
            {
                entity.ToTable("Systems_AuditOperation");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SystemsAuditProperty>(entity =>
            {
                entity.ToTable("Systems_AuditProperty");

                entity.HasIndex(e => e.AuditEntityId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.AuditEntity)
                    .WithMany(p => p.SystemsAuditProperty)
                    .HasForeignKey(d => d.AuditEntityId);
            });

            modelBuilder.Entity<SystemsKeyValue>(entity =>
            {
                entity.ToTable("Systems_KeyValue");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Key).IsRequired();
            });

            modelBuilder.Entity<VConverter>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_Converter");

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
