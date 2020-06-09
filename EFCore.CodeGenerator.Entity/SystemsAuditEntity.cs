using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class SystemsAuditEntity
    {
        public SystemsAuditEntity()
        {
            this.SystemsAuditProperty = new HashSet<SystemsAuditProperty>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string TypeName { get; set; }

        public string EntityKey { get; set; }

        public int OperateType { get; set; }

        public Guid OperationId { get; set; }

        public virtual SystemsAuditOperation Operation { get; set; }

        public virtual ICollection<SystemsAuditProperty> SystemsAuditProperty { get; set; }
    }
}
