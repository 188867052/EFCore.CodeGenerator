using System;

namespace Entities
{
    public partial class CourseScore
    {
        /// <summary>
        /// 主键.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 分数.
        /// </summary>
        public int? Score { get; set; }

        /// <summary>
        /// 学生ID.
        /// </summary>
        public int? StudentId { get; set; }

        /// <summary>
        /// 课程ID.
        /// </summary>
        public int? CourseId { get; set; }

        /// <summary>
        /// 创建时间.
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间.
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        public Course IdNavigation { get; set; }
    }
}
