using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Principal;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;
using System.Threading;

namespace IMCOA.Service
{
    public static class ServiceHelper
    {

        //获取美国官方时间和日期
        public static void GetNistTimeUS(out string time, out string Data)
        {
            try
            {
                //nist.time.gov的url
                string strNistUrl = "http://nist.time.gov/timezone.cgi?Pacific/d/-8/";

                //构造并实例化一个WebRequest
                System.Net.WebRequest myHttpWebRequest = System.Net.HttpWebRequest.Create(strNistUrl);
                //设置连接超时时间
                myHttpWebRequest.Timeout = 8000;
                //设置WebResponse，接收返回信息
                System.Net.WebResponse myHttpWebResponse = myHttpWebRequest.GetResponse();
                //获取返回信息流信息
                Stream sr = myHttpWebResponse.GetResponseStream();
                //设置流阅读器
                StreamReader reader = new StreamReader(sr, System.Text.Encoding.ASCII);
                //流输出为字符串
                String srdata = reader.ReadToEnd();
                //将返回html文本中的双引号变为单引号
                srdata = srdata.Replace("\"", "\'");

                //用模式匹配加分割的方式定位和获取时间信息
                string strOut = Regex.Split(srdata, "color='white'><b>", RegexOptions.IgnoreCase)[1];
                string strTime = Regex.Split(strOut, "<br>", RegexOptions.IgnoreCase)[0];
                time = DateTime.Parse(strTime).ToLongTimeString();

                //用模式匹配加分割的方式定位和获取日期信息
                string strOut1 = Regex.Split(srdata, "'5' color='white'>", RegexOptions.IgnoreCase)[1];
                string strData = Regex.Split(strOut1, "<br>", RegexOptions.IgnoreCase)[0];
                Data = DateTime.Parse(strData).ToShortDateString();
            }
            catch (System.Exception ex)
            {
                time = string.Empty;
                Data = string.Empty;
                throw new Exception("获取时间出错：" + ex.Message);
            }
        }

        /// <summary>
        /// DataTable分页处理数据
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="PageSize">数据大小</param>
        /// <returns>DataTable</returns>
        public static DataTable GetPagedTable(DataTable dt, int PageIndex, int PageSize)
        {
            if (PageIndex == 0)
                return dt;
            DataTable newdt = dt.Clone();
            //newdt.Clear();  
            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;
            if (rowbegin >= dt.Rows.Count)
                return newdt;
            if (rowend > dt.Rows.Count)
                rowend = dt.Rows.Count;
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }
            return newdt;
        }

        /// <summary>
        /// 获取当前站点对应的服务器名
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentSiteServer()
        {
            //增加当前服务器标识 lz  2019.5.17
            string serverSign = "";
            try
            {
                serverSign = System.Net.Dns.GetHostName();//本地计算机的 DNS 主机名的字符串  
                System.Net.IPHostEntry hostInfo = System.Net.Dns.GetHostEntry(serverSign);
                foreach (var item in hostInfo.AddressList) //多IP
                {
                    if (item.ToString().Contains("."))
                    {
                        serverSign = "A" + item.ToString().Substring(item.ToString().LastIndexOf('.') + 1);
                        break;
                    }
                }

            }
            catch (global::System.Exception) { }
            return serverSign;
        }

    }

    public static class ArrayHelper
    {
        /// <summary>
        /// 用指定符号串连数组列表（默认为逗号）
        /// </summary>
        /// <param name="separator">分隔符号</param>
        /// <param name="addSingleQuotes">是否添加单引号</param>
        public static string ToSeparateString(this IEnumerable<string> list, bool isAddSingleQuotes, char separator = ',')
        {
            if (list == null || list.Count() == 0) return string.Empty;
            string result = "";
            if (isAddSingleQuotes)
            {
                foreach (var item in list)
                {
                    result += string.Format("'{0}'{1}", item, separator);
                }
            }
            else
            {
                foreach (var item in list)
                {
                    result += string.Format("{0}{1}", item, separator);
                }
            }
            return result.TrimEnd(separator);
        }

        /// <summary>
        /// 用指定符号串连数组列表（默认为逗号）
        /// </summary>
        /// <param name="separator">分隔字符串</param>
        /// <param name="addSingleQuotes">是否添加单引号</param>
        public static string ToSeparateJoinString(this IEnumerable<string> list, bool isAddSingleQuotes, string separatorStr = ",")
        {
            if (list == null || list.Count() == 0) return string.Empty;
            string result = "";
            if (isAddSingleQuotes)
            {
                foreach (var item in list)
                {
                    result += string.Format("'{0}'{1}", item, separatorStr);
                }
            }
            else
            {
                foreach (var item in list)
                {
                    result += string.Format("{0}{1}", item, separatorStr);
                }
            }
            return TrimEnd(result,separatorStr);
        }

        /// <summary>
        /// 去除原字符串结尾处的所有替换字符串
        /// 如：原字符串"sdlfjdcdcd",替换字符串"cd" 返回"sdlfjd"
        /// </summary>
        /// <param name="strSrc"></param>
        /// <param name="strTrim"></param>
        /// <returns></returns>
        public static string TrimEnd(string strSrc, string strTrim)
        {
            if (strSrc.EndsWith(strTrim))
            {
                string strDes = strSrc.Substring(0, strSrc.Length - strTrim.Length);
                return TrimEnd(strDes, strTrim);
            }
            return strSrc;
        }


        /// <summary>
        /// 拼接字符串并去除重复
        /// </summary>
        /// <param name="stringArr"></param>
        /// <returns>string</returns>
        public static string Merge(string[] stringArr)
        {
            string returnString = string.Empty;

            foreach (string s in stringArr)
            {
                returnString += s + ",";
            }

            returnString = returnString.Substring(0, returnString.Length - 1);

            returnString = returnString.Replace(",,", ",");

            string[] strArr = returnString.Split(',');

            ArrayList arrList = new ArrayList();

            arrList.Add(strArr[0]);

            if (strArr.Length > 1)
            {
                for (int i = 1; i < strArr.Length - 1; i++)
                {
                    if (!arrList.Contains(strArr[i]))
                    {
                        arrList.Add(strArr[i]);
                    }
                }
            }

            returnString = string.Empty;

            for (int i = 0; i < arrList.Count; i++)
            {
                returnString += arrList[i] + ",";
            }

            returnString = returnString.Substring(0, returnString.Length - 1);

            return returnString;
        }

        /// <summary>
        /// 用指定符号串连数组列表（默认为逗号）
        /// </summary>
        /// <param name="separator">分隔符号</param>
        /// <param name="addSingleQuotes">是否添加单引号</param>
        public static string ToSeparateString(this IEnumerable<int> list, bool isAddSingleQuotes, char separator = ',')
        {
            return ToSeparateString(list.Select(p => p.ToString()), isAddSingleQuotes, separator);
        }

        /// <summary>
        /// 用指定符号串连数组列表（默认为逗号）
        /// </summary>
        /// <param name="separator">分隔符号</param>
        public static string ToSeparateString(this IEnumerable<string> list, char separator = ',')
        {
            return ToSeparateString(list, false, separator);
        }
        /// <summary>
        /// 用指定符号串连数组列表（默认为逗号）
        /// </summary>
        /// <param name="separator">分隔符号</param>
        public static string ToSeparateString(this IEnumerable<int> list, char separator = ',')
        {
            return ToSeparateString(list.Select(p => p.ToString()), separator);
        }
        /// <summary>
        /// 用指定符号串连数组列表（默认为逗号）
        /// </summary>
        /// <param name="separator">分隔符号</param>
        public static string ToSeparateString(this IEnumerable<decimal> list, char separator = ',')
        {
            return ToSeparateString(list.Select(p => p.ToString()), separator);
        }
        /// <summary>
        /// 用指定符号串连数组列表（默认为逗号）
        /// </summary>
        /// <param name="separator">分隔符号</param>
        public static string ToSeparateString(this IEnumerable<float> list, char separator = ',')
        {
            return ToSeparateString(list.Select(p => p.ToString()), separator);
        }
        /// <summary>
        /// 用指定符号串连数组列表（默认为逗号）
        /// </summary>
        /// <param name="separator">分隔符号</param>
        public static string ToSeparateString(this IEnumerable<double> list, char separator = ',')
        {
            return ToSeparateString(list.Select(p => p.ToString()), separator);
        }

        public static bool Compare(this IEnumerable<string> list, IEnumerable<string> listCompare)
        {
            bool IsSame = false;
            int count = 0;
            if (list.Count() == listCompare.Count())
            {
                foreach (string str in list)
                {
                    if (listCompare.Contains(str))
                        count += 1;
                }
            }
            if (count == list.Count())
                IsSame = true;
            return IsSame;
        }
        /// <summary>
        /// 字符串转化成整型数组
        /// </summary>
        /// <param name="separator"></param>
        public static int[] ToIntArray(this string str, char separator = ',')
        {
            string[] strArray = str.Split(separator);
            return Array.ConvertAll<string, int>(strArray, delegate (string s) { return int.Parse(s); });
        }

        /// <summary>
        /// 去除字符串字符串数组中 重复的字符串
        /// 输入："qw,de,we,qw"
        /// 输出："qw,de,we"
        /// </summary>
        /// <param name="str">字符串数组</param>
        /// <returns></returns>
        public static string ToRemoveRepeat(this string str)
        {
            return string.Join(",", str.Split(',').Distinct().ToArray());
        }
        /// <summary>  
        /// 字符串替换方法  
        /// </summary>  
        /// <param name="myStr">需要替换的字符串</param>  
        /// <param name="replaceStr">需要替换的字符</param>  
        /// <param name="replaceWord">将替换为</param>  
        /// <returns></returns>  
        public static string ToReplace(this string str, string replaceStr, string replaceWord = "")
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(replaceStr))
            {
                return str;
            }
            var StrList = replaceStr.Split(',').ToList();
            foreach (var item in StrList)
            {
                str.Replace(item, replaceWord);
            }
            return str;
        }
        /// <summary>
        /// 查找字符串中重复出现最多的字符
        /// </summary>
        /// <param name="str">字符串 例："AS,SD,DE,DE,AS"</param>
        /// <returns>返回出现最多的字符串</returns>
        public static string MaxNumString(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            var StrList = str.Split(',').ToList();
            Dictionary<string, int> strDictionary = new Dictionary<string, int>();
            string max = StrList[0];
            foreach (var item in StrList)
            {
                if (strDictionary.ContainsKey(item))
                {
                    strDictionary[item]++;
                }
                else
                {
                    strDictionary.Add(item, 1);
                }
                if (strDictionary[max] < strDictionary[item]) max = item;
            }
            return max;

        }


    }
}
