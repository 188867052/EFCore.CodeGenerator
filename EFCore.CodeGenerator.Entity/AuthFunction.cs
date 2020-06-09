using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class AuthFunction
    {
        public AuthFunction()
        {
            this.AuthModuleFunction = new HashSet<AuthModuleFunction>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Area { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public bool IsController { get; set; }

        public bool IsAjax { get; set; }

        public int AccessType { get; set; }

        public bool IsAccessTypeChanged { get; set; }

        public bool AuditOperationEnabled { get; set; }

        public bool AuditEntityEnabled { get; set; }

        public int CacheExpirationSeconds { get; set; }

        public bool IsCacheSliding { get; set; }

        public bool IsLocked { get; set; }

        public virtual ICollection<AuthModuleFunction> AuthModuleFunction { get; set; }
    }
}
