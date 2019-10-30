using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class Grade
    {
        public Grade()
        {
            this.Class = new HashSet<Class>();
        }

        public virtual ICollection<Class> Class { get; set; }
    }
}
