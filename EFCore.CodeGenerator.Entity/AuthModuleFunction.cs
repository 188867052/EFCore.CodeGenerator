using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class AuthModuleFunction
    {
        public Guid Id { get; set; }

        public int ModuleId { get; set; }

        public Guid FunctionId { get; set; }

        public virtual AuthFunction Function { get; set; }

        public virtual AuthModule Module { get; set; }
    }
}
