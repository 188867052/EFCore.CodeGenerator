using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class SystemsAuditProperty
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public string FieldName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }

        public string DataType { get; set; }

        public Guid AuditEntityId { get; set; }

        public virtual SystemsAuditEntity AuditEntity { get; set; }
    }
}
