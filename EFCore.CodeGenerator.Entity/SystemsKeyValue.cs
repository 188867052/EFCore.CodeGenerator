using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class SystemsKeyValue
    {
        public Guid Id { get; set; }

        public string ValueJson { get; set; }

        public string ValueType { get; set; }

        public string Key { get; set; }

        public bool IsLocked { get; set; }
    }
}
