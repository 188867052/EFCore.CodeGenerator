using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class IdentityUser
    {
        public IdentityUser()
        {
            this.AuthEntityUser = new HashSet<AuthEntityUser>();
            this.AuthModuleUser = new HashSet<AuthModuleUser>();
            this.IdentityLoginLog = new HashSet<IdentityLoginLog>();
            this.IdentityUserClaim = new HashSet<IdentityUserClaim>();
            this.IdentityUserLogin = new HashSet<IdentityUserLogin>();
            this.IdentityUserRole = new HashSet<IdentityUserRole>();
            this.IdentityUserToken = new HashSet<IdentityUserToken>();
            this.InfosMessage = new HashSet<InfosMessage>();
            this.InfosMessageReceive = new HashSet<InfosMessageReceive>();
            this.InfosMessageReply = new HashSet<InfosMessageReply>();
        }

        public int Id { get; set; }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string NickName { get; set; }

        public string Email { get; set; }

        public string NormalizeEmail { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string HeadImg { get; set; }

        public string SecurityStamp { get; set; }

        public string ConcurrencyStamp { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public bool IsSystem { get; set; }

        public bool IsLocked { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? DeletedTime { get; set; }

        public string Remark { get; set; }

        public Guid? MessageId { get; set; }

        public virtual InfosMessage Message { get; set; }

        public virtual IdentityUserDetail IdentityUserDetail { get; set; }

        public virtual ICollection<AuthEntityUser> AuthEntityUser { get; set; }

        public virtual ICollection<AuthModuleUser> AuthModuleUser { get; set; }

        public virtual ICollection<IdentityLoginLog> IdentityLoginLog { get; set; }

        public virtual ICollection<IdentityUserClaim> IdentityUserClaim { get; set; }

        public virtual ICollection<IdentityUserLogin> IdentityUserLogin { get; set; }

        public virtual ICollection<IdentityUserRole> IdentityUserRole { get; set; }

        public virtual ICollection<IdentityUserToken> IdentityUserToken { get; set; }

        public virtual ICollection<InfosMessage> InfosMessage { get; set; }

        public virtual ICollection<InfosMessageReceive> InfosMessageReceive { get; set; }

        public virtual ICollection<InfosMessageReply> InfosMessageReply { get; set; }
    }
}
