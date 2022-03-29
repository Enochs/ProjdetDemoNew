using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Pro.Web.Controllers
{
    public class VueController : Controller
    {
        // GET: Vue
        public ActionResult Index()
        {
            return View();
        }



        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFileData(string data, HttpPostedFileBase file)
        {
            var json = Json(new { success = false, });
            try
            {
                var m_formData = JsonConvert.DeserializeObject<FormData>(data);
                if (m_formData != null)
                {
                    var fileData = m_formData.fileData;
                    var m_fileData = JsonConvert.DeserializeObject<List<FileData>>(fileData);

                    var files = Request.Files;

                    json = Json(new { success = true, msg = "获取数据成功" });
                }
            }
            catch (Exception ex)
            {
                json = Json(new { success = false, errMsg = ex.Message });
            }

            return json;
        }

        /// <summary>
        /// 自动上传
        /// </summary>
        /// <param name="fileData">文件</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadImg(HttpPostedFileBase fileData, string path)
        {
            fileData = Request.Files[0];
            string filePath = ""; // 附件保存路径
            if (null != fileData)
            {
                try
                {
                    string ext = Path.GetExtension(fileData.FileName); //获得文件扩展名
                    string saveName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext; //实际保存文件名
                    string folder = "/Uploadfile/" + path; // 保存路径
                    saveFile(fileData, folder, saveName);
                    filePath = folder + "/" + saveName; //保存文件
                }
                catch (Exception)
                {
                    filePath = "";
                }
            }
            return Json(new { success = true, filePath = filePath, message = "导入成功！" });
        }

        [NonAction]
        private string saveFile(HttpPostedFileBase postedFile, string filepath, string saveName)
        {
            string phyPath = Request.MapPath("~" + filepath + "/");
            if (!Directory.Exists(phyPath))
            {
                Directory.CreateDirectory(phyPath);
            }
            try
            {
                postedFile.SaveAs(phyPath + saveName);
                return phyPath;
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

    }


    /// <summary>
    /// 表单实体
    /// </summary>
    public class FormData
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string loginName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public string date { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string dept { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string job { get; set; }

        /// <summary>
        /// 上传文件
        /// </summary>
        public string fileData { get; set; }
    }

    /// <summary>
    /// 上传文件实体
    /// </summary>
    public class FileData
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 文件上传地址
        /// </summary>
        public string url { get; set; }

    }
}