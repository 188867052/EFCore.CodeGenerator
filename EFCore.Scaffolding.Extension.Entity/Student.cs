using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class Student
    {
        /// <summary>
        /// 名称.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 性别.
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 主键.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 班级ID.
        /// </summary>
        public int? ClassId { get; set; }

        /// <summary>
        /// 创建时间.
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间.
        /// </summary>
        public DateTime? UpdateTime { get; set; }
    }
}
