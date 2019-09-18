using System;

namespace Entities
{
    public partial class TeacherCourseMapping
    {
        /// <summary>
        /// 主键.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 课程ID.
        /// </summary>
        public int? CourseId { get; set; }

        /// <summary>
        /// 教师ID.
        /// </summary>
        public int? TeacherId { get; set; }

        /// <summary>
        /// 创建时间.
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间.
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        public Course Course { get; set; }

        public Teacher Teacher { get; set; }
    }
}
