﻿using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class VConverter
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

        /// <summary>
        /// New Id.
        /// </summary>
        public Guid? NewId { get; set; }
    }
}
