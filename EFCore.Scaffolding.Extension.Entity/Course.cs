using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class Course
    {
        public Course()
        {
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
        /// 教师ID.
        /// </summary>
        public string TeacherId { get; set; }

        /// <summary>
        /// 创建时间.
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间.
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 是否已删除.
        /// </summary>
        public bool IsDeleted { get; set; }

        public virtual CourseScore CourseScore { get; set; }

        public virtual ICollection<TeacherCourseMapping> TeacherCourseMapping { get; set; }
    }
}
