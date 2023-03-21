using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Pro.Core.Extension
{
    public static class ImgHelper
    {
        /// <summary>
        /// 字节数组转为整形数组
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <returns>整形数组</returns>
        public static byte[] ToBase64Bytes(this string b)
        {
            return string.IsNullOrEmpty(b) ? null : Convert.FromBase64String(b);
        }

        #region Image图片转为Base64
        /// <summary>
        /// 图片转为base64
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string ImageToBase64(Image image)
        {
            return ImageToBase64(image, ImageFormat.Jpeg);
        }

        public static string ImageToBase64(Image image, ImageFormat format)
        {
            string result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                new Bitmap(image).Save(memoryStream, format);
                result = Convert.ToBase64String(memoryStream.ToArray());
            }
            return result;
        }
        #endregion

        #region 保存图片到本地
        ///ImageData：图片的byte数组数据
        ///imageName：图片保存的路径
        public static void SaveImage(byte[] ImageData, string imageName, string dirName, ref string path)
        {
            try
            {
                //保存图片到本地文件夹
                System.IO.MemoryStream ms = new System.IO.MemoryStream(ImageData);
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                //保存到磁盘文件
                var dicPath = System.Environment.CurrentDirectory;
                string imagePath = System.IO.Path.Combine(dicPath, dirName, string.Empty);
                if (!System.IO.Directory.Exists(imagePath))
                    System.IO.Directory.CreateDirectory(imagePath);
                path = System.IO.Path.Combine(imagePath, imageName);
                img.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                ms.Dispose();
                //MessageBox.Show("图片已保存至：" + m_ImagrRootDir);
            }
            catch (Exception exception)
            {
            }
        }
        #endregion

        #region 读取路径中的图片
        /// <summary>
        /// 通过FileStream 来打开文件，这样就可以实现不锁定Image文件，到时可以让多用户同时访问Image文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static Image ReadImageFile(string path)
        {
            try
            {
                FileStream fs = File.OpenRead(path); //OpenRead
                int filelength = 0;
                filelength = (int)fs.Length; //获得文件长度 
                Byte[] image = new Byte[filelength]; //建立一个字节数组 
                fs.Read(image, 0, filelength); //按字节流读取 
                System.Drawing.Image result = System.Drawing.Image.FromStream(fs);
                fs.Close();
                return result;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region pdf转换成图片
        /// <summary>
        /// pdf转换成图片
        /// </summary>
        /// <param name="filePath">pdf路径</param>
        /// <param name="imageName">图片名称</param>
        /// <returns></returns>
        public static string PdfToJpg(string filePath, string imageName)
        {
            var dicPath = System.Environment.CurrentDirectory;
            string imagePath = System.IO.Path.Combine(dicPath, "电测听", string.Empty);
            if (!System.IO.Directory.Exists(imagePath))
            {
                System.IO.Directory.CreateDirectory(imagePath);
            }
            var fileJpg = Path.Combine(imagePath, imageName);
            try
            {
                //jpg文件不存在
                if (!File.Exists(fileJpg))
                {
                    Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                    doc.LoadFromFile(filePath);
                    System.Drawing.Image image = doc.SaveAsImage(0);
                    image.Save(fileJpg);
                    doc.Close();
                }
            }
            catch
            {
                return string.Empty;
            }
            return fileJpg;
        }
        #endregion
    }
}
