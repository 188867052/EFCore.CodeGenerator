using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class InfosMessageReply
    {
        public InfosMessageReply()
        {
            this.InverseParentReply = new HashSet<InfosMessageReply>();
        }

        public Guid Id { get; set; }

        public string Content { get; set; }

        public bool IsRead { get; set; }

        public Guid ParentMessageId { get; set; }

        public Guid ParentReplyId { get; set; }

        public bool IsLocked { get; set; }

        public DateTime? DeletedTime { get; set; }

        public DateTime CreatedTime { get; set; }

        public int UserId { get; set; }

        public Guid BelongMessageId { get; set; }

        public virtual InfosMessage BelongMessage { get; set; }

        public virtual InfosMessage ParentMessage { get; set; }

        public virtual InfosMessageReply ParentReply { get; set; }

        public virtual IdentityUser User { get; set; }

        public virtual ICollection<InfosMessageReply> InverseParentReply { get; set; }
    }
}
