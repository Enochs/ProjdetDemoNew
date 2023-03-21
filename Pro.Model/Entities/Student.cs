using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pro.Model
{
    /// <summary>
    /// -
    /// </summary>
    [Table("Student")]
    public partial class Student
    {

        [Display(Name = "编号")]
        [Key]
        public Guid s_id { get; set; }

        [Display(Name = "姓名")]
        [StringLength(50)]
        public string s_name { get; set; }

        [Display(Name = "登录名")]
        [StringLength(50)]
        public string s_loginName { get; set; }

        [Display(Name = "密码")]
        [StringLength(50)]
        public string s_passWord { get; set; }

        [Display(Name = "地址")]
        [StringLength(200)]
        public string s_address { get; set; }

        [Display(Name = "性别")]
        public byte? s_sex { get; set; }

        [Display(Name = "年龄")]
        public int? s_age { get; set; }

        [Display(Name = "联系电话")]
        [StringLength(20)]
        public string s_phone { get; set; }

        [Display(Name = "状态")]
        public byte? s_status { get; set; }

        [Display(Name = "备注")]
        public string s_remark { get; set; }

        [Display(Name = "创建日期")]
        public DateTime? s_createDate { get; set; }

        [Display(Name = "年级")]
        public Guid? s_Grade_ID { get; set; }

    }
}
