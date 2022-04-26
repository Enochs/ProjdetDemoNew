using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pro.Utils
{
    /// <summary>
    /// 配置
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class LqImExportAttribute : Attribute
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 页面名称
        /// 如果为空，则不显示
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// 表格名称
        /// 如果为空，则不导出
        /// </summary>
        public string ExcelName { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 是否只读
        /// true：则不做修改，默认为false
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// 是否是多属性
        /// </summary>
        public bool IsVariation { get; set; }

        public LqImExportAttribute()
        {

        }

        public LqImExportAttribute(string pageName, string groupName, string excelName, bool readOnly = false, bool isVariation = false, int order = 99)
        {
            this.PageName = pageName;
            this.GroupName = groupName;
            this.ExcelName = excelName;
            this.ReadOnly = readOnly;
            this.IsVariation = isVariation;
            this.Order = order;
        }
    }
}
