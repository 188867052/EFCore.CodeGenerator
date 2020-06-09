using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class InfosMessage
    {
        public InfosMessage()
        {
            this.IdentityRole = new HashSet<IdentityRole>();
            this.IdentityUser = new HashSet<IdentityUser>();
            this.InfosMessageReceive = new HashSet<InfosMessageReceive>();
            this.InfosMessageReplyBelongMessage = new HashSet<InfosMessageReply>();
            this.InfosMessageReplyParentMessage = new HashSet<InfosMessageReply>();
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int MessageType { get; set; }

        public int NewReplyCount { get; set; }

        public bool IsSended { get; set; }

        public bool CanReply { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsLocked { get; set; }

        public DateTime? DeletedTime { get; set; }

        public DateTime CreatedTime { get; set; }

        public int SenderId { get; set; }

        public virtual IdentityUser Sender { get; set; }

        public virtual ICollection<IdentityRole> IdentityRole { get; set; }

        public virtual ICollection<IdentityUser> IdentityUser { get; set; }

        public virtual ICollection<InfosMessageReceive> InfosMessageReceive { get; set; }

        public virtual ICollection<InfosMessageReply> InfosMessageReplyBelongMessage { get; set; }

        public virtual ICollection<InfosMessageReply> InfosMessageReplyParentMessage { get; set; }
    }
}
