using System;
using System.Collections.Generic;

namespace Entities
{
    /// <summary>
    /// 日志.
    /// </summary>
    public partial class Log
    {
        /// <summary>
        /// 主键.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// 日志内容.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 创建时间.
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建时间.
        /// </summary>
        public DateTime? UpdateTimeTicks { get; set; }

        /// <summary>
        /// 链接.
        /// </summary>
        public Uri Url { get; set; }
    }
}
