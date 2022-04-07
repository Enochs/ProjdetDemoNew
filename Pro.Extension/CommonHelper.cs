using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;
using System.Reflection;
using System.Globalization;

namespace Pro.Extension
{
    public static class CommonHelper
    {
        /// <summary>
        /// 写日志到记事本
        /// </summary>
        /// <param name="message">内容</param>
        /// <param name="fileName">文件名（如 aa.text）</param>
        /// <param name="dir">目录（如 C:\OA程序日志\）</param>
        /// <param name="isAddDate">是否为内容加上日期</param>
        public static void WriteToText(string message, string fileName, string dir, bool isAddDate = false)
        {
            //if (string.IsNullOrWhiteSpace(dir)) dir = @"C:\日志\"; //这里可以默认为当前目录

            //string path = dir + fileName;
            //FileStream fs = null;
            //StreamWriter sw = null;
            //try
            //{
            //    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            //    if (System.IO.File.Exists(path))
            //    {
            //        fs = new FileStream(path, FileMode.Append);
            //    }
            //    else
            //    {
            //        fs = new FileStream(path, FileMode.Create);
            //    }

            //    sw = new StreamWriter(fs, System.Text.Encoding.Default);
            //    sw.WriteLine("");
            //    if (isAddDate)
            //        sw.WriteLine(string.Format("{0} {1}", DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.fff"), message));
            //    else
            //        sw.WriteLine(message);
            //}
            //finally
            //{
            //    sw.Close();
            //    fs.Close();
            //}
        }

        /// <summary>
        /// 去除对象中所有字符串类型的属性前后空格
        /// pwk 2016-10-21 9:28:45
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object RemoveTrim(this object obj)
        {
            if (obj == null)
            {
                return obj;
            }
            var t = obj.GetType();
            var properties = t.GetProperties();
            foreach (var p in properties)
            {
                if (p.PropertyType.Name != "String") continue;

                var str = (string)p.GetValue(obj, null);

                p.SetValue(obj, str == null ? null : str.Trim(), null);
            }
            return obj;
        }
        /// <summary>
        /// 去除集合对象中所有字符串类型的属性前后空格
        /// wxy:2018-08-17
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> ListRemoveTrim<T>(List<T> obj)
        {
            if (obj == null)
            {
                return obj;
            }
            foreach (var item in obj)
            {
                var t = item.GetType();
                var properties = t.GetProperties();
                foreach (var p in properties)
                {
                    if (p.PropertyType.Name != "String") continue;

                    var str = (string)p.GetValue(item, null);

                    p.SetValue(item, str == null ? null : str.Trim(), null);
                }
            }

            return obj;
        }
        public static T RemoveTrim<T>(T obj)
        {
            if (obj == null)
            {
                return obj;
            }
            var t = obj.GetType();
            var properties = t.GetProperties();
            foreach (var p in properties)
            {
                if (p.PropertyType.Name != "String") continue;

                var str = (string)p.GetValue(obj, null);

                p.SetValue(obj, str == null ? null : str.Trim(), null);
            }
            return obj;
        }

        /// <summary>
        /// 截取字符串的最后一个逗号
        /// pwk 2016-12-9 9:51:42
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveLastComma(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? "" : str.Remove(str.Length - 1, 1);
        }

        /// <summary>
        /// 判断是否特殊符号
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <returns></returns>
        public static bool IsSpecialChar(this string str)
        {
            Regex regExp = new Regex("[^0-9a-zA-Z\u4e00-\u9fa5]");
            if (regExp.IsMatch(str))
            {
                return true;
            }
            return false;
        }

        /// <summary >
        /// 判断是否有中文
        /// 2017-3-6 14:57:43 pwk
        /// </summary >
        /// <param name="str" ></param >
        /// <returns ></returns >
        public static bool IsIncludeChinese(this string str)
        {
            Regex regex = new Regex("[\u4e00-\u9fa5]");
            Match m = regex.Match(str);
            return m.Success;
        }


        /// <summary>
        /// 判断是否包含中文字符集（如中文逗号、中文符号、汉字、等相关中文字符）
        /// XN 2017-8-9 18:00:00
        /// </summary>
        /// <returns></returns>
        public static bool IsIncludeChineseCharacter(this string str)
        {
            Regex cjkCharacterRegex = new Regex("[\u3400-\u4DB5\u4E00-\u9FA5\u9FA6-\u9FBB\uF900-\uFA2D\uFA30-\uFA6A\uFA70-\uFAD9\uFF00-\uFFEF\u2E80-\u2EFF\u3000-\u303F\u31C0-\u31EF\u2F00-\u2FDF\u2FF0-\u2FFF\u3100-\u312F\u31A0-\u31BF\u3040-\u309F\u30A0-\u30FF\u31F0-\u31FF\uAC00-\uD7AF\u1100-\u11FF\u3130-\u318F\u4DC0-\u4DFF\uA000-\uA48F\uA490-\uA4CF\u2800-\u28FF\u3200-\u32FF\u3300-\u33FF\u2700-\u27BF\u2600-\u26FF\uFE10-\uFE1F\uFE30-\uFE4F]", RegexOptions.Compiled);
            return cjkCharacterRegex.IsMatch(str);
        }
        /// <summary>
        /// 去除特殊字符 换行 空格 table结尾的字符
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string TrimSpecialChar(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return data;
            }
            data = data.TrimEnd().TrimEnd('\t', '\n');
            var removeKey = "\t\n";
            if (data.EndsWith(removeKey))
            {
                data = data.Substring(0, data.Length - removeKey.Length);
            }
            return data;
        }

        /// <summary>
        /// 获取时间间隔描述字符串。如 6小时
        /// </summary>
        /// <param name="startDatetime">开始时间</param>
        /// <param name="endDatetime">结束时间</param>      
        /// <param name="ignoreTimeSpan">忽略多少差异以下的（分钟为单位，取差异的绝对值，不包含本值）</param>
        /// <param name="isOnlyGreaterThanZero">仅显示大于0的时间差</param>
        /// <returns></returns>
        public static string GetTimeSpanString(DateTime startDatetime, DateTime? endDatetime = null, double? ignoreTimeSpan = null, bool isOnlyGreaterThanZero = false)
        {
            string result = string.Empty;
            if (!endDatetime.HasValue) endDatetime = DateTime.Now; //结束时间为空默认为当前时间
            TimeSpan ts = endDatetime.Value - startDatetime;
            //忽略指定分钟后的差
            if (ignoreTimeSpan.HasValue && Math.Abs(ts.TotalMinutes) < ignoreTimeSpan) return result;
            //是否仅显示大于0的时间差
            if (isOnlyGreaterThanZero && ts.TotalSeconds < 0) return result;

            if (Math.Abs(ts.Days) >= 1)
                result = string.Format("{0}天", Math.Round(ts.TotalDays, 2));
            else if (Math.Abs(ts.Hours) >= 1)
                result = string.Format("{0}小时", Math.Round(ts.TotalHours, 2));
            else if (Math.Abs(ts.Minutes) >= 1)
                result = string.Format("{0}分钟", Math.Round(ts.TotalMinutes, 2));

            return result;
        }
        /// <summary>
        /// 当in中的条数大于1000时返回用or分割的sql语句
        /// </summary>
        /// <param name="columns">比如"p.ID"</param>
        /// <param name="ids">条件</param>
        /// <returns></returns>
        public static string SqlAddMoreIn(String columns, int[] ids)
        {
            if (columns == null || ids == null || ids.Length == 0)
                return "";
            if (ids.Length > 0)
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < ids.Length; i++)
                {
                    if ((i % 1000) == 0 && i > 0)
                    {
                        sql.Remove(sql.Length - 1, 1);
                        sql.Append(") or " + columns + " in ( '" + ids[i] + "',");
                    }
                    else
                    {
                        sql.Append("'" + ids[i] + "',");
                    }
                }
                sql.Remove(sql.Length - 1, 1);
                return "" + columns + " in (" + sql.ToString() + ") ";
            }
            return "";
        }

        /// <summary>
        /// 当in中的条数大于1000时返回用or分割的sql语句
        /// </summary>
        /// <param name="columns">比如"p.ID"</param>
        /// <param name="ids">条件</param>
        /// <returns></returns>
        public static string StrSqlAddMoreIn(String columns, string[] ids)
        {
            if (columns == null || ids == null || ids.Length == 0)
                return "";
            if (ids.Length > 0)
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < ids.Length; i++)
                {
                    if ((i % 1000) == 0 && i > 0)
                    {
                        sql.Remove(sql.Length - 1, 1);
                        sql.Append(") or " + columns + " in ( '" + ids[i] + "',");
                    }
                    else
                    {
                        sql.Append("'" + ids[i] + "',");
                    }
                }
                sql.Remove(sql.Length - 1, 1);
                return "" + columns + " in (" + sql.ToString() + ") ";
            }
            return "";
        }

        /// <summary>
        /// 当in中的条数大于1000时返回用or分割的sql语句
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static string ArrAySqlAddMoreIn(string columns, List<string> ids)
        {
            if (columns == null || ids == null || ids.Count() == 0)
                return "";
            if (ids.Count() > 0)
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < ids.Count(); i++)
                {
                    if ((i % 1000) == 0 && i > 0)
                    {
                        sql.Remove(sql.Length - 1, 1);
                        sql.Append(") or " + columns + " in ( '" + ids[i] + "',");
                    }
                    else
                    {
                        sql.Append("'" + ids[i] + "',");
                    }
                }
                sql.Remove(sql.Length - 1, 1);
                return "" + columns + " in (" + sql.ToString() + ") ";
            }
            return "";
        }


        public static decimal ConvertYuanToFen(decimal? fee)
        {
            return fee.HasValue ? fee.Value * 100 : 0;
        }
        public static decimal? ConvertYuanToFenOrNull(decimal? fee)
        {
            if (fee.HasValue)
            {
                return fee.Value * 100;
            }
            return null;
        }
        public static string ConvertLongOrNullToStringOrNull(long? val)
        {
            return val.HasValue ? val.Value.ToString() : null;
        }
        public static DateTime? ConvertAliDTToDTOrNull(string dateTimeStr)
        {
            if (string.IsNullOrWhiteSpace(dateTimeStr))
            {
                return null;
            }
            else
            {
                var time = dateTimeStr.Substring(0, 14);//2017 04 17 18 09 53 000+0800
                var year = time.Substring(0, 4);
                var month = time.Substring(4, 2);
                var date = time.Substring(6, 2);
                var hour = time.Substring(8, 2);
                var minute = time.Substring(10, 2);
                var second = time.Substring(12, 2);
                //"2012/2/2 5:25:10"
                time = year + "/" + month + "/" + date + " " + hour + ":" + minute + ":" + second;
                var result = new DateTime();
                var succ = DateTime.TryParse(time, out result);
                if (succ)
                    return result;
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ConvertAliDTToDTOrNow(string dateTimeStr)
        {
            ///20180420115235000+0800
            if (string.IsNullOrWhiteSpace(dateTimeStr))
            {
                return DateTime.Now;
            }
            else
            {
                var time = dateTimeStr.Substring(0, 14);//2017 04 17 18 09 53 000+0800
                var year = time.Substring(0, 4);
                var month = time.Substring(4, 2);
                var date = time.Substring(6, 2);
                var hour = time.Substring(8, 2);
                var minute = time.Substring(10, 2);
                var second = time.Substring(12, 2);
                //"2012/2/2 5:25:10"
                time = year + "/" + month + "/" + date + " " + hour + ":" + minute + ":" + second;
                var result = new DateTime();
                var succ = DateTime.TryParse(time, out result);
                if (succ)
                    return result;
                return DateTime.Now;
            }
        }
        /// <summary>
        /// 富文本去掉标签
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlToText(string html)
        {
            Regex regex = new Regex("<.+?>", RegexOptions.IgnoreCase);
            string strOutput = regex.Replace(html, "");//替换掉"<"和">"之间的内容
            strOutput = strOutput.Replace("<", "");
            strOutput = strOutput.Replace(">", "");
            strOutput = strOutput.Replace("&nbsp;", "");
            return strOutput;
        }

        public static List<KeyValuePair<string, string>> GetYearList(string emptyKey = null, string emptyValue = null, int from = 2018, int to = 2050)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            if (emptyKey != null) result.Add(new KeyValuePair<string, string>(emptyKey, emptyValue));

            for (var i = from; i <= to; i++)
            {
                result.Add(new KeyValuePair<string, string>(i.ToString(), i.ToString()));
            }
            return result;
        }
        /// <summary>
        /// 获取计算超期时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetOutDatetime()
        {
            var dtNow = DateTime.Now;
            var outDatetime=dtNow;//计算超期时间
            var offWorkDatetime = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 18, 10, 00);
            if (Convert.ToInt32(dtNow.DayOfWeek) == 0)//如果是星期天，那么前一天下班之前的数据
            {
                outDatetime = dtNow.AddDays(-1);
                outDatetime = new DateTime(outDatetime.Year, outDatetime.Month, outDatetime.Day, 18, 10, 00);
            }
            else if (Convert.ToInt32(dtNow.DayOfWeek)==1&&dtNow.Hour<14)//如果是星期一的上午就算周六下班之前的数据
            {
                outDatetime = dtNow.AddDays(-2);
                outDatetime = new DateTime(outDatetime.Year, outDatetime.Month, outDatetime.Day, 18, 10, 00);
            }
            else if (dtNow.Hour < 14)//其他时间的上午算前一天下班前的数据
            {
                outDatetime = dtNow.AddDays(-1);
                outDatetime = new DateTime(outDatetime.Year, outDatetime.Month, outDatetime.Day, 18, 10, 00);
            }
            else if (dtNow.Hour >=14 && dtNow < offWorkDatetime)//下午2点到下班前算上午12以前未处理的数据
            {
                outDatetime = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 12, 00, 00);
            }
            else//下班之后的算下班之前未处理的数据
            {
                outDatetime = offWorkDatetime;
            }
            return outDatetime;
        }
        public static string ClearHTMLTags1(string HTML)

        {
            string[] Regexs ={
                        @"<script[^>]*?>.*?</script>",
                        @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
                        @"([\r\n])[\s]+",
                        @"&(quot|#34);",
                        @"&(amp|#38);",
                        @"&(lt|#60);",
                        @"&(gt|#62);",
                        @"&(nbsp|#160);",
                        @"&(iexcl|#161);",
                        @"&(cent|#162);",
                        @"&(pound|#163);",
                        @"&(copy|#169);",
                        @"&#(\d+);",
                        @"-->",
                        @"<!--.*\n",
            };
            string[] Replaces ={
                            "",
                            "",
                            "",
                            "\"",
                            "&",
                            "<",
                            ">",
                            " ",
                            "\xa1", //chr(161),
                            "\xa2", //chr(162),
                            "\xa3", //chr(163),
                            "\xa9", //chr(169),
                            "",
                            "\r\n",
                            "",
                            ""
            };
            string s = HTML;
            for (int i = 0; i < Regexs.Length; i++)
            {
                s = new Regex(Regexs[i], RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(s, Replaces[i]);
            }
            s.Replace("<", "");
            s.Replace(">", "");
            s.Replace("\r\n", "");
            return s;
        }

        /// <summary>
        /// 阿拉伯数字转中文
        /// </summary>
        public static string NumToChinese(int num)
        {
            string sb = "";
            string[] unit = { "", "十", "百", "千" };
            char[] chineseNum = { '零', '一', '二', '三', '四', '五', '六', '七', '八', '九' };
            string chars = num.ToString();
            int a = chars.Length;
            int count = 0;

            if (a == 2 && chars[0] == '1')//如12，13
            {
                sb += unit[1];
                if (chars[1] != '0')//10，20
                {
                    // 将字符转为对应的中文 例如： '1' -> 一 ;
                    sb += chineseNum[chars[1] - 48];
                }
            }
            else if (a <= 4)//三到四位
            {
                for (int i = 0; i < chars.Length; i++)//1011，1010
                {
                    if (chars[i] == '0')
                    {
                        count++;
                        continue;
                    }
                    if (count >= 1)//有零
                    {
                        sb += chineseNum[0];
                        count = -4;//防止多个零叠在一起
                    }
                    sb += chineseNum[chars[i] - 48];
                    sb += unit[a - 1 - i];//权重
                }
            }
            else if (a <= 8)
            {
                //截取
                int index = chars.Length - 4;//万的部分
                string wan = chars.Substring(0, chars.Length - 4);
                string qita = chars.Substring(chars.Length - 4, 4);
                if (chars[a - 4] == '0')
                {
                    sb = NumToChinese(int.Parse(wan)) + "万" + "零" + NumToChinese(int.Parse(qita));
                }
                else
                {
                    sb = NumToChinese(int.Parse(wan)) + "万" + NumToChinese(int.Parse(qita));
                }
            }
            else
            {
                sb = "超出范围！";
            }
            return sb;
        }
    }
}
