using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class AuthModule
    {
        public AuthModule()
        {
            this.AuthModuleFunction = new HashSet<AuthModuleFunction>();
            this.AuthModuleRole = new HashSet<AuthModuleRole>();
            this.AuthModuleUser = new HashSet<AuthModuleUser>();
            this.InverseParent = new HashSet<AuthModule>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public string Code { get; set; }

        public double OrderCode { get; set; }

        public string TreePathString { get; set; }

        public int? ParentId { get; set; }

        public virtual AuthModule Parent { get; set; }

        public virtual ICollection<AuthModuleFunction> AuthModuleFunction { get; set; }

        public virtual ICollection<AuthModuleRole> AuthModuleRole { get; set; }

        public virtual ICollection<AuthModuleUser> AuthModuleUser { get; set; }

        public virtual ICollection<AuthModule> InverseParent { get; set; }
    }
}
