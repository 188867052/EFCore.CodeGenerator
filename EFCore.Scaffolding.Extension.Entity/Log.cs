using System;

namespace Entities
{
    public partial class Log
    {
        public Guid Identifier { get; set; }

        public string Message { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
