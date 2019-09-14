using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class Log
    {
        public Guid Identifier { get; set; }

        public string Message { get; set; }
    }
}
