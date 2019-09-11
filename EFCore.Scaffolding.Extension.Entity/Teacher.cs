using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class Teacher
    {
        public Teacher()
        {
            this.Class = new HashSet<Class>();
            this.TeacherCourseMapping = new HashSet<TeacherCourseMapping>();
        }

        /// <summary>
        /// 主键.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 性别.
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 创建时间.
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间.
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        public ICollection<Class> Class { get; set; }

        public ICollection<TeacherCourseMapping> TeacherCourseMapping { get; set; }
    }
}
