using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class Class
    {
        /// <summary>
        /// 主键.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 班主任ID.
        /// </summary>
        public int? HeadTeacherId { get; set; }

        /// <summary>
        /// 创建时间.
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间.
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        public Teacher HeadTeacher { get; set; }
    }
}
