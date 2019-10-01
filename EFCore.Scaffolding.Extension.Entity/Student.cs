using System;
using System.Collections.Generic;
using EFCore.Scaffolding.Extension.Entity.Enums;

namespace Entities
{
    /// <summary>
    /// 学生.
    /// </summary>
    public partial class Student
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
        /// 性别.
        /// </summary>
        public SexEnum Sex { get; set; }

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

        /// <summary>
        /// 地址.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系电话.
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 是否已删除.
        /// </summary>
        public bool IsDeleted { get; set; }

        public virtual Class Class { get; set; }
    }
}
