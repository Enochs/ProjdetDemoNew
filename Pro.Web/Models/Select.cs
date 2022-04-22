using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pro.Web.Models
{
    public class Select
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        ///下拉选项集合 
        /// </summary>
        public List<OptionValue> OptionValue { get; set; }

    }

    /// <summary>
    /// 下拉框键值对模型
    /// </summary>
    public class OptionValue
    {

        public string Id { get; set; }

        public string Text { get; set; }
    }
}