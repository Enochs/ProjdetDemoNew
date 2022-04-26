using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Pro.Utils
{
    /// <summary>
    /// 常用方法
    /// </summary>
    public static class Common
    {
        #region 产品图片路径常量

        /// <summary>
        /// UploadfilePath.ProductPath
        /// </summary>
        public const string UPLOADFILE_PRODUCT = "/UPLOADFILE/PRODUCT/";

        /// <summary>
        /// UploadfilePath.OldCommonUploadFileAttachment
        /// 
        /// </summary>
        public const string UPLOADFILE_1 = "/UPLOADFILE/1/";

        /// <summary>
        /// /Uploadfile/ProductImg
        /// </summary>
        public const string UPLOADFILE_PRODUCTIMG = "/UPLOADFILE/PRODUCTIMG/";

        /// <summary>
        /// /Product/
        /// </summary>
        public const string PRODUCT_TOPPATH = "/PRODUCT/";

        /// <summary>
        /// /Uploadfile/temp/Product/
        /// </summary>
        public const string PRODUCT_TEMPPATH = "/UPLOADFILE/TEMP/PRODUCT/";

        #endregion

        public static bool YorNToBool(string value)
        {
            return Defaults.YesorNo_True.Equals(value, StringComparison.CurrentCultureIgnoreCase);
        }
        public static string BoolToYorN(bool value)
        {
            return (value ? Defaults.YesorNo_True : Defaults.YesorNo_False);
        }
        /// <summary>
        /// 字符串true/false 转化成 Y/N
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TrueorFalseToYorN(string value)
        {
            return Defaults.TrueorFalse_True.Equals(value, StringComparison.CurrentCultureIgnoreCase) ? Defaults.YesorNo_True : Defaults.YesorNo_False;
        }

        public static string YesOrNoString(string value)
        {
            return Defaults.YesorNo_True.Equals(value, StringComparison.CurrentCultureIgnoreCase) ? "是" : "否";
        }

        public static string YesOrNoStringExtra(string value)
        {
            return Defaults.YesorNo_True.Equals(value, StringComparison.CurrentCultureIgnoreCase) ? "有" : "无";
        }

        public static bool IsTrueOrFalse(this string value, Func<string, bool> func)
        {
            value = value ?? "";
            return func.Invoke(value);
        }

        public static T IsYesOrNo<T>(this bool value, T yesValue, T noValue)
        {
            return value ? yesValue : noValue;
        }

        public static void ToPagingProcess<T>(this IEnumerable<T> item, int pageSize, Action<IEnumerable<T>> singlePageList)
        {
            if (item != null)
            {
                var cnt = item.Count();
                if (cnt > 0)
                {
                    var totalPages = cnt / pageSize;
                    if (cnt % pageSize > 0) totalPages += 1;
                    for (int pageIndex = 1; pageIndex <= totalPages; pageIndex++)
                    {
                        var currentPageItems = item.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                        singlePageList(currentPageItems);
                    }
                }
            }
        }

        /// <summary>
        /// 数据分页处理
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="item">数据源</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="dataCount">数据总行数</param>
        /// <param name="singlePageList">分页调用处理函数</param>
        public static void ToPagingProcess<T>(this IEnumerable<T> item, int pageSize, int dataCount, Action<IEnumerable<T>, int> singlePageList)
        {
            if (item != null)
            {
                var cnt = dataCount;
                if (cnt > 0)
                {
                    var totalPages = cnt / pageSize;
                    if (cnt % pageSize > 0) totalPages += 1;
                    for (int pageIndex = 1; pageIndex <= totalPages; pageIndex++)
                    {
                        var currentPageItems = item.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                        singlePageList(currentPageItems, pageIndex);
                    }
                }
            }
        }

        /// <summary>
        /// 把输入的字符串转化为数据库存储的Y或N。
        /// </summary>
        /// <param name="isValidityCheck">如果不为Y时是否都为N</param>
        public static string ConvertToYorN(string value, bool isValidityCheck = false)
        {
            if (value != null) value = value.Trim();
            if ("Y".Equals(value, StringComparison.CurrentCultureIgnoreCase) || "True".Equals(value, StringComparison.CurrentCultureIgnoreCase) || "T".Equals(value, StringComparison.CurrentCultureIgnoreCase) || "是" == value || "1" == value)
                return Defaults.YesorNo_True;
            else if (!isValidityCheck || "N".Equals(value, StringComparison.CurrentCultureIgnoreCase) || "False".Equals(value, StringComparison.CurrentCultureIgnoreCase) || "F".Equals(value, StringComparison.CurrentCultureIgnoreCase) || "否" == value || "0" == value)
                return Defaults.YesorNo_False;
            else throw new Exception("不能转换为Y或N");
        }

        /// <summary>
        /// 判断 非负浮点数（整数、浮点）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsFloat(string value)
        {
            string pattern = @"^\d+(\.\d+)?$";
            bool IsSuccess = true;
            if (!string.IsNullOrEmpty(value))
            {
                Match m = Regex.Match(value, pattern);
                if (!m.Success)
                {
                    IsSuccess = false;
                }
            }
            return IsSuccess;
        }

        /// <summary>
        /// 判断是否为整数
        /// </summary>
        /// <param name="number"></param>
        /// <param name="isIncludeZero">是否包括零 默认true:包括</param>
        /// <returns></returns>
        public static bool IsNumeric(string number, bool isIncludeZero = true)
        {
            string patternNO = @"^[0-9]\d*$";
            if (!isIncludeZero)
                patternNO = @"^[1-9]\d*$";
            bool IsSuccess = true;
            if (!string.IsNullOrEmpty(number))
            {
                Match m = Regex.Match(number, patternNO);
                if (!m.Success)
                {
                    IsSuccess = false;
                }
            }
            return IsSuccess;
        }

        /// <summary>
        /// 判断 是否纯英文和空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEnglishChar(string value)
        {
            bool IsSuccess = true;
            if (!string.IsNullOrEmpty(value))
            {
                bool result = Regex.IsMatch(value, @"^[a-zA-Z\\s]+");
                if (!result)
                {
                    IsSuccess = false;
                }
            }
            return IsSuccess;
        }

        /// <summary>
        /// 获得日期指定格式
        /// </summary>
        /// <param name="sourceDate">目标日期</param>
        /// <param name="dateType">转换格式</param>
        /// <returns></returns>
        public static string GetDateTimeStringByType(DateTime? sourceDate, string dateType)
        {
            return sourceDate.HasValue ? sourceDate.Value.ToString(dateType) : string.Empty;
        }

        /// <summary>
        /// 获得原日期的最小值  [小时:分钟:秒 为  00:00:00]
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static DateTime GetCurrentMinDate(DateTime sourceDate)
        {
            return sourceDate.Date;
        }
        /// <summary>
        /// 获得原日期的最大值  [小时:分钟:秒 为  23:59:59]
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static DateTime GetCurrentMaxDate(DateTime sourceDate)
        {
            return sourceDate.Date.AddDays(1).AddSeconds(-1);
        }

        /// <summary>
        /// 获得原日期 这个月的第一天
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static DateTime GetCurrentMonthFirstDay(DateTime sourceDate)
        {
            //sourceDate = sourceDate.AddMonths(-1);
            return new DateTime(sourceDate.Year, sourceDate.Month, 1);
        }
        /// <summary>
        /// 获得原日期 这个月的最后一天
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static DateTime GetCurrentMonthLastDay(DateTime sourceDate)
        {
            sourceDate = new DateTime(sourceDate.Year, sourceDate.Month, 1);
            return sourceDate.AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 获得原日期 上一个月的第一天
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static DateTime GetLastMonthFirstDay(DateTime sourceDate)
        {
            sourceDate = sourceDate.AddMonths(-1);
            return new DateTime(sourceDate.Year, sourceDate.Month, 1);
        }

        public static string GetYearMonthDay(DateTime sourceDate)
        {
            sourceDate = sourceDate.AddMonths(-1);
            return sourceDate.ToString(Defaults.DateMonthFormat);
        }
        /// <summary>
        /// 获得原日期 上一个月的最后一天
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static DateTime GetLastMonthLastDay(DateTime sourceDate)
        {
            return new DateTime(sourceDate.Year, sourceDate.Month, 1).AddDays(-1);
        }

        /// <summary>
        /// 获得当前日期格式[yyyy-MM-dd]
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static string GetShortDateTimeString(DateTime? sourceDate)
        {
            return sourceDate.HasValue ? sourceDate.Value.ToString(Defaults.DateFormat) : string.Empty;
        }

        /// <summary>
        /// 获得当前日期格式[yyyy/MM/dd]
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static string GetShortDateTimeYmdString(DateTime? sourceDate)
        {
            return sourceDate.HasValue ? sourceDate.Value.ToString(Defaults.DateFormatymd) : string.Empty;
        }

        /// <summary>
        /// 获得当前年 的 第一天日期
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static DateTime GetYearsFirstDayTimeString()
        {
            return new DateTime(DateTime.Now.Year, 1, 1);
        }

        /// <summary>
        /// 获得当前日期格式[yyyy-MM-dd hh mi]
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static string GetShortMinDateTimeString(DateTime? sourceDate)
        {
            return sourceDate.HasValue ? sourceDate.Value.ToString(Defaults.DateTimeFormatmin) : string.Empty;
        }
        /// <summary>
        /// 获取最近一年的日期格式【yyyy-MM-dd hh mi】
        /// </summary>
        /// <param name="sourceDate"></param>
        public static string GetLastYearShortMinDateTimeString(DateTime? sourceDate)
        {
            return sourceDate.HasValue ? sourceDate.Value.AddYears(-1).ToString(Defaults.DateTimeFormatmin) : string.Empty;
        }
        /// <summary>
        /// 获取最近一天的日期格式【yyyy-MM-dd hh mi】
        /// </summary>
        /// <param name="sourceDate"></param>
        /// <returns></returns>
        public static string GetLastDayShortMaxDateTimeString(DateTime? sourceDate)
        {
            return sourceDate.HasValue ? sourceDate.Value.AddDays(-1).ToString(Defaults.DateTimeFormatmax) : string.Empty;
        }
        /// <summary>
        /// 获取当太难的日期格式【yyyy-MM-dd hh mi】
        /// </summary>
        /// <param name="sourceDate"></param>
        /// <returns></returns>
        public static string GetDayShortMaxDateTimeString(DateTime? sourceDate)
        {
            return sourceDate.HasValue ? sourceDate.Value.ToString(Defaults.DateTimeFormatmaxs) : string.Empty;
        }
        /// <summary>
        /// 获得当前日期格式[yyyy-MM-dd hh mi]
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static string GetShortMaxDateTimeString(DateTime? sourceDate)
        {
            return sourceDate.HasValue ? sourceDate.Value.ToString(Defaults.DateTimeFormatmax) : string.Empty;
        }

        /// <summary>根据日期，获得星期几</summary>
        /// <param name="y">年</param> 
        /// <param name="m">月</param> 
        /// <param name="d">日</param> 
        /// <returns>星期几，1代表星期一；7代表星期日</returns>
        public static string getWeekDay(DateTime date)
        {
            string week = "星期一";
            string dt = date.DayOfWeek.ToString();
            switch (dt)
            {
                case "Monday":
                    week = "星期一";
                    break;
                case "Tuesday":
                    week = "星期二";
                    break;
                case "Wednesday":
                    week = "星期三";
                    break;
                case "Thursday":
                    week = "星期四";
                    break;
                case "Friday":
                    week = "星期五";
                    break;
                case "Saturday":
                    week = "星期六";
                    break;
                case "Sunday":
                    week = "星期日";
                    break;
            }
            return week;
        }
        /// <summary>
        /// 获得当前日期格式[yyyy-MM-dd HH:mm:ss]
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static string GetLongDateTimeString(DateTime? sourceDate)
        {
            return sourceDate.HasValue ? sourceDate.Value.ToString(Defaults.DateTimeFormat) : string.Empty;
        }

        /// <summary>
        /// 获得当前日期格式[HH:mm:ss]
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static string GetTimeDateTimeString(DateTime? sourceDate)
        {
            return sourceDate.HasValue ? sourceDate.Value.ToString(Defaults.DateFormatTime) : string.Empty;
        }

        /// <summary>
        /// 获得当前日期格式[HH:mm:ss]
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static string GetTimeDateHHmm(DateTime? sourceDate)
        {
            return sourceDate.HasValue ? sourceDate.Value.ToString(Defaults.DateHHmm) : string.Empty;
        }

        /// <summary>
        /// 获得当前日期格式的月份[yyyy-MM]
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static string GetDateMonthString(DateTime? sourceDate)
        {
            return sourceDate.HasValue ? sourceDate.Value.ToString(Defaults.DateMonthFormat) : string.Empty;
        }

        /// <summary>
        /// 获得当前日期格式的月份[yyyyMMdd]
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static string GetDateyyyyMMddString(DateTime? sourceDate)
        {
            return sourceDate.HasValue ? sourceDate.Value.ToString(Defaults.DateyyyyMMdd) : string.Empty;
        }

        /// <summary>
        /// 获得当前日期格式的[yyyyMMddHHmmssfff]
        /// </summary>
        /// <param name="sourceDate">原日期</param>
        /// <returns></returns>
        public static string GetDateyyyyMMddHHmmssfffString(DateTime? sourceDate)
        {
            return sourceDate.HasValue ? sourceDate.Value.ToString(Defaults.DATE_YYYY_MM_DD_HH_MM_SS_FFF) : string.Empty;
        }

        /// <summary>
        /// 获得数据的百分比
        /// </summary>
        /// <param name="molecular">分子</param>
        /// <param name="denominator">分母</param>
        /// <returns></returns>
        public static string GetDataPercentage(decimal molecular, decimal denominator)
        {
            if (molecular == 0M && denominator == 0M)   //分子与分母都为0
            {
                return "0%";
            }
            else if (molecular == 0M && denominator != 0M)      //分子为0 分母不为0
            {
                return "0%";
            }
            else if (molecular != 0M && denominator == 0M)      //分子不为0 分母为0
            {
                return "100%";
            }
            else
            {
                return Convert.ToDecimal((molecular / denominator) * 100).ToString("0.00") + "%";
            }
        }

        /// <summary>
        /// 小数转换成百分比
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ConvertDecimalToPercentage(decimal? d)
        {
            string result = string.Empty;
            if (d.HasValue)
            {
                result = Math.Round(d.Value * 100, 2) + "%";
            }
            return result;
        }

        /// <summary>
        /// 将百分比转换成小数
        /// </summary>
        /// <param name="perc">百分比值，可纯为数值，或都加上%号的表示，
        /// 如：65|65%</param>
        /// <returns></returns>
        public static decimal PerctangleToDecimal(string perc)
        {
            try
            {
                // string patt = @"/^(?<num>[\d]{1,})(%?)$/";
                // decimal percNum = Decimal.Parse(System.Text.RegularExpressions.Regex.Match(perc, patt).Groups["num"].Value);
                var sdf = perc.Substring(0, perc.Length - 1);
                var d = Decimal.Parse(sdf);
                decimal percNum = Decimal.Parse(perc.Substring(0, perc.Length - 1));
                return percNum / (decimal)100;
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// 获取指定时间属于哪个季度
        /// </summary>
        /// <param name="dt">时间</param>
        /// <returns></returns>
        public static int GetCurQuarter(DateTime dt)
        {
            return ((int)(dt.Month - 1) / 3) + 1;
        }

        /// <summary>
        /// 获得数据类型字符串
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static string GetDataTypeString(string dataType)
        {
            switch (dataType)
            {
                case "1":
                    return "字符串";
                case "2":
                    return "整数";
                case "3":
                    return "小数";
                case "4":
                    return "布尔值";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 将重量转换和盎司
        /// 1克=0.035274盎司
        /// </summary>
        /// <param name="weight">重量（克）</param>
        /// <param name="roundDecimals">四舍五入保留的小数位数，如果传null则不四舍五入</param>
        /// <param name="zeroToValue">如果算出的结果为0，则用此值代替</param>
        /// <returns></returns>
        public static decimal ConvertToOZ(object weight, int? roundDecimals = null, decimal? zeroToValue = null)
        {
            var oz = Convert.ToDecimal(weight) * (decimal)0.035274;
            if (roundDecimals.HasValue) oz = Math.Round(oz, roundDecimals.Value);
            if (zeroToValue.HasValue && oz == 0) oz = zeroToValue.Value;
            return oz;
        }

        /// <summary>
        /// 将值转化为decimal类型，报错时返回0
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>value</returns>
        public static decimal ConvertToDecimal(object value)
        {
            try
            {
                return Convert.ToDecimal(value);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// 系统最小时间
        /// </summary>
        public static DateTime MinDateTime
        {
            get { return Convert.ToDateTime("1900-1-1 0:00:00"); }
        }
        /// <summary>
        /// 是否为null或无效
        /// </summary>
        public static bool IsNullDateTime(DateTime? value)
        {
            if (value == null || value == DateTime.MinValue || value <= Common.MinDateTime)
                return true;
            return false;
        }
        public static DateTime? ConvertToNullableDateTime(DateTime? value)
        {
            if (IsNullDateTime(value)) return null;
            return value;
        }
        public static DateTime? ConvertToNullableDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return ConvertToNullableDateTime(Convert.ToDateTime(value.Trim()));
        }

        /// <summary>
        /// 获得站点下币种
        /// </summary>
        /// <param name="saleSite">站点</param>
        /// <returns></returns>
        public static string GetSaleSiteCurrency(string saleSite)
        {
            switch (saleSite)
            {
                case "US":
                    return "USD";       //美元
                case "UK":
                    return "GBP";       //英磅
                case "CA":
                    return "CAD";       //加元
                case "JP":
                    return "JPY";       //日元
                case "DE":
                case "FR":
                case "ES":
                case "IT":
                case "UZ":
                case "KZ":
                case "AF":
                case "IQ":
                case "LB":
                case "AZ":
                case "NL":
                    return "EUR";       //欧元
                case "AU":
                    return "AUD";       //澳元
                case "MX":
                    return "MXN";       //比索  
                case "SGAMZ":
                    return "SGD";       //新加坡
                case "TH":
                    return "THB";       //泰国
                case "ID":
                    return "IDR";       //印尼
                case "MY":
                    return "MYR";       //马来西亚
                case "PH":
                    return "PHP";       //菲律宾
                case "IN":
                    return "INR";       //印度
                case "HK":
                    return "HKD";       //香港
                case "VN":
                    return "VND";       //越南
                case "TW":
                    return "TWD";       //台湾
                case "SG":
                    return "SGD";       //新加坡
                case "KE":
                    return "KES";       //肯尼亚
                case "NG":
                    return "NGN";       //尼日利亚
                case "EG":
                    return "EGP";       //埃及镑
                case "MA":
                    return "MAD";       //摩洛哥道拉姆
                case "GH":
                    return "GHS";       //加纳
                case "CI":
                    return "XOF";       //科特迪瓦西部法郎
                case "RU":
                    return "RUB";       //俄罗斯卢布
                case "CN":
                    return "RMB";       //人民币
                case "PK": //巴基斯坦
                    return "PKR";
                case "BD": //孟加拉
                    return "BDT";
                case "MM": //缅甸
                    return "MMK";
                case "NP": //尼泊尔
                    return "NPR";
                case "LK": //斯里兰卡
                    return "LKR";
                case "BR": //巴西
                    return "BRL";
                case "AE": //阿联酋
                    return "AED";
                case "SE": //瑞典
                    return "SEK";//瑞典克朗
                case "SA": //沙特
                    return "SAR";//沙特里亚尔
                case "PL": //波兰
                    return "PLN";//兹罗提
                case "KR":
                    return "KRW";//韩国
                case "CL":
                    return "CLP";//智利
                case "CO":
                    return "COP";//哥伦比亚
                case "TR":
                    return "YTL";//土耳其（新土耳其里拉）
                default:
                    return "";
            }
        }

        /// <summary>
        /// //获取相差时间
        /// </summary>
        /// <param name="Interval">相差的时间类别</param>
        /// <param name="StartDate">开始时间</param>
        /// <param name="EndDate">截止时间</param>
        /// <returns></returns>
        public static long GetDateDiff(DateInterval Interval, DateTime StartDate, DateTime EndDate)
        {
            long lngDateDiffValue = 0;
            System.TimeSpan TS = new System.TimeSpan(EndDate.Ticks - StartDate.Ticks);
            switch (Interval)
            {
                case DateInterval.Second:
                    lngDateDiffValue = (long)TS.TotalSeconds;
                    break;
                case DateInterval.Minute:
                    lngDateDiffValue = (long)TS.TotalMinutes;
                    break;
                case DateInterval.Hour:
                    lngDateDiffValue = (long)TS.TotalHours;
                    break;
                case DateInterval.Day:
                    lngDateDiffValue = (long)TS.Days;
                    break;
                case DateInterval.Week:
                    lngDateDiffValue = (long)(TS.Days / 7);
                    break;
                case DateInterval.Month:
                    lngDateDiffValue = (long)(TS.Days / 30);
                    break;
                case DateInterval.Quarter:
                    lngDateDiffValue = (long)((TS.Days / 30) / 3);
                    break;
                case DateInterval.Year:
                    lngDateDiffValue = (long)(TS.Days / 365);
                    break;
            }
            return (lngDateDiffValue);
        }

        /// <summary>
        /// 计算日期间隔，获取两个时间相差多少年多少月
        /// </summary>
        /// <param name="beginDate">要参与计算的其中一个日期</param>
        /// <param name="endDate">要参与计算的另一个日期</param>
        /// <param name="dateFormat">决定返回值日期形式【dd：天数；mm：月数；yy：年数；默认yymm：相差年月】</param>
        /// <returns>一个代表年月日的int数组，具体数组长度与枚举参数drf有关</returns>
        public static int[] GetDiffDateYearMonthDay(DateTime beginDate, DateTime endDate, string dateFormat)
        {
            #region 数据初始化

            DateTime max;
            DateTime min;
            int year;
            int month;
            int tempYear, tempMonth;

            if (beginDate > endDate)
            {
                max = beginDate;
                min = endDate;
            }
            else
            {
                max = endDate;
                min = beginDate;
            }

            tempYear = max.Year;
            tempMonth = max.Month;

            if (max.Month < min.Month)
            {
                tempYear--;
                tempMonth = tempMonth + 12;
            }

            year = tempYear - min.Year;
            month = tempMonth - min.Month;

            #endregion

            #region 按条件计算
            //天数
            if (dateFormat == "dd")
            {
                TimeSpan ts = max - min;
                return new int[] { ts.Days };
            }
            //月数
            else if (dateFormat == "mm")
            {
                return new int[] { month + year * 12 };
            }
            //年数
            else if (dateFormat == "yy")
            {
                return new int[] { year };
            }
            //年数和月数
            return new int[] { year, month };

            #endregion
        }

        ///   <summary>
        ///   将指定字符串按指定长度进行剪切，
        ///   </summary>
        ///   <param   name= "oldStr "> 需要截断的字符串 </param>
        ///   <param   name= "maxLength "> 字符串的最大长度 </param>
        ///   <param   name= "endWith "> 超过长度的后缀 </param>
        ///   <returns> 如果超过长度，返回截断后的新字符串加上后缀，否则，返回原字符串 </returns>
        public static string SubString(string oldStr, int maxLength, string endWith)
        {
            if (string.IsNullOrEmpty(oldStr))
                return oldStr + endWith;
            if (maxLength < 1)
                throw new Exception("返回的字符串长度必须大于[0] ");
            if (oldStr.Length > maxLength)
            {
                string strTmp = oldStr.Substring(0, maxLength);
                if (string.IsNullOrEmpty(endWith))
                    return strTmp;
                else
                    return strTmp + endWith;
            }
            return oldStr;
        }

        #region 返回当前日期的星期名称
        /// <summary>返回当前日期的星期名称</summary>  
        /// <param name="dt">日期</param>  
        /// <returns>星期名称</returns>  
        public static string GetWeekNameOfDay(DateTime idt)
        {
            string dt, week = "";

            dt = idt.DayOfWeek.ToString();
            switch (dt)
            {
                case "Monday":
                    week = "星期一";
                    break;
                case "Tuesday":
                    week = "星期二";
                    break;
                case "Wednesday":
                    week = "星期三";
                    break;
                case "Thursday":
                    week = "星期四";
                    break;
                case "Friday":
                    week = "星期五";
                    break;
                case "Saturday":
                    week = "星期六";
                    break;
                case "Sunday":
                    week = "星期日";
                    break;

            }
            return week;
        }
        #endregion

        /// <summary>
        /// 保留2为小数
        /// </summary>
        /// <param name="dec">数字</param>
        /// <param name="n">小数的位置</param>
        /// <returns></returns>
        public static decimal ToRound(this Decimal dec)
        {
            return decimal.Parse(string.Format("{0:F2}", dec));
        }

        public static decimal ToRound(this Decimal? dec)
        {
            if (dec.HasValue)
            {
                return decimal.Parse(string.Format("{0:F2}", dec.Value));
            }

            return 0.00M;
        }

        /// <summary>
        /// 保留指定位数的小数，不四舍五入
        /// </summary>
        /// <param name="d"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static decimal CutDecimalWithN(this decimal d, int n)
        {
            string strDecimal = d.ToString();
            int index = strDecimal.IndexOf(".");
            if (index == -1 || strDecimal.Length < index + n + 1)
            {
                strDecimal = string.Format("{0:F" + n + "}", d);
            }
            else
            {
                int length = index;
                if (n != 0)
                {
                    length = index + n + 1;
                }
                strDecimal = strDecimal.Substring(0, length);
            }
            return Decimal.Parse(strDecimal);
        }

        /// <summary>
        /// 将字符串转为 Int32 的扩展方法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt32(this string str)
        {
            return int.Parse(str);
        }
        /// <summary>
        /// 将字符串转为 Int64 的扩展方法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long ToInt64(this string str)
        {
            return long.Parse(str);
        }

        /// <summary>
        /// 获得产品标签信息
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static string GetProductIconInfo(string icon)
        {
            return icon.Replace("CC", "循环").Replace("TC", "垃圾桶").Replace("VC", "电压电流")
                .Replace("CM", "杯叉").Replace("HS", "3岁窒息锋利边").Replace("HP", "3岁窒息小物件").Replace("HE", "8岁窒息小物件");
        }

        public static string[] SplitDefault(this string str, string chars = ",")
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.Split(new string[] { chars }, StringSplitOptions.RemoveEmptyEntries);
            }
            return null;
        }

        public static List<string> SplitDefaultToList(this string str, string chars = ",")
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.Split(new string[] { chars }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            return new List<string>();
        }

        /// <summary>
        /// 根据规则处理参数值返回List
        /// </summary>
        /// <param name="str">参数</param>
        /// <param name="chars">规则</param>
        /// <returns></returns>
        public static List<string> SplitDefaultToListByEnter(this string str, string chars = "\r\n")
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.Split(new string[] { chars }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            return new List<string>();
        }

        /// <summary>
        /// 根据规则处理参数值返回List
        /// </summary>
        /// <param name="str">参数</param>
        /// <param name="chars">规则</param>
        /// <returns></returns>
        public static List<string> SplitDefaultToListBySpace(this string str, string chars = " ")
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.Split(new string[] { chars }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            return new List<string>();
        }

        public static List<string> SplitDefaultToList2(this string str, string chars = "|")
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.Split(new string[] { chars }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            return new List<string>();
        }

        public static List<int> SplitDefaultToIntList(this string str, string chars = ",")
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.Split(new string[] { chars }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();
            }
            return new List<int>();
        }

        /// <summary>
        /// 根据符号“-”拆分
        /// </summary>
        /// <param name="str">数据源</param>
        /// <param name="chars">规定字符</param>
        /// <returns></returns>
        public static List<int> SplitToIntList(this string str, string chars = "-")
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.Split(new string[] { chars }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();
            }
            return new List<int>();
        }

        /// <summary>
        /// 日期空处理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NullDateProcessing(DateTime? value)
        {
            string pram = null;
            if (value == null)
            {
                pram = "null";
            }
            else
            {
                pram = string.Format("TO_DATE('{0}','yyyy-MM-dd HH24:mi:ss')", value);
            }
            return pram;
        }

        /// <summary>
        /// 数字空值处理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NullNumProcessing(decimal? value)
        {
            string pram = null;
            if (value == null)
            {
                pram = "null";
            }
            else
            {
                pram = "" + value + "";
            }
            return pram;
        }
        /// <summary>
        /// 字符串空值处理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NullStrProcessing(string value)
        {
            string pram = null;
            if (value == null)
            {
                pram = "null";
            }
            else
            {
                pram = "'" + value + "'";
            }
            return pram;
        }

        /// <summary>  
        /// 通过url获取整个页面 
        /// </summary>  
        /// <param name="url">url</param>  
        public static string GetSourceByUrl(string url)
        {
            string strBuff = "";
            Uri httpURL = new Uri(url);
            ///HttpWebRequest类继承于WebRequest，并没有自己的构造函数，需通过WebRequest的Creat方法 建立，并进行强制的类型转换   
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(httpURL);
            ///通过HttpWebRequest的GetResponse()方法建立HttpWebResponse,强制类型转换   
            HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();
            ///GetResponseStream()方法获取HTTP响应的数据流,并尝试取得URL中所指定的网页内容   
            ///若成功取得网页的内容，则以System.IO.Stream形式返回，若失败则产生ProtoclViolationException错 误。在此正确的做法应将以下的代码放到一个try块中处理。这里简单处理   
            Stream respStream = httpResp.GetResponseStream();
            ///返回的内容是Stream形式的，所以可以利用StreamReader类获取GetResponseStream的内容，并以   
            //StreamReader类的Read方法依次读取网页源程序代码每一行的内容，直至行尾（读取的编码格式：UTF8）   
            StreamReader respStreamReader = new StreamReader(respStream, Encoding.UTF8);
            strBuff = respStreamReader.ReadToEnd().Replace("\n", "").Replace("\t", "").Replace("\r", "");
            return strBuff;
        }

        /// <summary>
        /// Http post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static string GetHttpRequest(string url, string param, string contentType = null, string method = null, string hearder = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Timeout = 600000;
                request.Method = method ?? "POST";
                request.ContentType = contentType ?? "application/x-www-form-urlencoded";
                request.Accept = "application/json";

                if (!string.IsNullOrEmpty(hearder))
                {
                    request.Headers.Add("Authorization", hearder);
                }

                if (!string.IsNullOrEmpty(param))
                {
                    byte[] bs = Encoding.ASCII.GetBytes(param);
                    request.ContentLength = bs.Length;
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                        reqStream.Close();
                    }
                }
                else
                {
                    request.ContentLength = 0;
                }

                string responseData = String.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseData = reader.ReadToEnd();
                    }
                }
                return responseData;
            }
            catch (System.Net.WebException webException)
            {
                HttpWebResponse response = webException.Response as HttpWebResponse;
                StreamReader responseStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string errorMessage = responseStream.ReadToEnd();
                response.Close();
                responseStream.Close();
                responseStream.Dispose();
                throw new Exception(errorMessage, webException);
            }
        }

        /// <summary>
        /// 清楚Html标签
        /// </summary>
        /// <param name="html">HTML代码</param>
        /// <param name="removeScripts">是否删除脚本，包括样式</param>
        /// <returns></returns>
        public static string ClearHtml(string html, bool removeScripts = false)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            if (removeScripts)
            {
                html = Regex.Replace(html, "(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", "", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            }

            string removeHtmlReg = @"</?[^>]*>";

            if (!string.IsNullOrWhiteSpace(html))
            {
                html = Regex.Replace(html, removeHtmlReg, "", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline).Replace("&nbsp", " ");
            }

            return html;
        }


        /// <summary>
        /// 转半角的函数(DBC case)
        /// 全角空格为12288，半角空格为32
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248 
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns></returns>
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32; continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);

        }

        /// <summary>
        /// 中文符号转换成英文符号
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertToEn(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                const string s1 = "。；，？！【】：–（）×～—％　、“”";
                const string s2 = @".;,?![]:-()x~-% ,""""";
                char[] c = text.ToCharArray();
                for (int i = 0; i < c.Length; i++)
                {
                    int n = s1.IndexOf(c[i]);
                    if (n != -1) c[i] = s2[n];
                }
                return new string(c);
            }
            return text;
        }

        /// <summary>
        /// 将dateTime转换为long时间戳
        /// </summary>
        /// <param name="time">时间   </param>
        /// <returns></returns>
        public static long ConvertDateTimeToLong(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;            //除10000调整为13位
            return t;
        }

        /// <summary>
        /// 将时间戳转换为datetime
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="timestampLength">时间戳长度，10位或13位</param>
        /// <returns></returns>
        public static DateTime? GetDateTimeByTimestamp(long? timestamp, int timestampLength = 10)
        {
            if (!timestamp.HasValue) return null;

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));

            if (timestampLength == 10)
            {
                startTime = startTime.AddSeconds(timestamp.Value);
            }
            else if (timestampLength == 13)
            {
                startTime = startTime.AddMilliseconds(timestamp.Value);
            }

            return startTime;
        }

        /// <summary>
        /// 速卖通系统时间转换为数据库时间格式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime? GetDateTimeByString(string input)
        {
            try
            {
                if (string.IsNullOrEmpty(input) && input.Length < 14)
                {
                    return null;
                }
                return DateTime.ParseExact(input.Substring(0, 14), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture); ;
            }
            catch (Exception ex)
            {
                string r = ex.Message;
                return null;
            }
        }
        public static DateTime TimeSpanToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            //long lTime = long.Parse(timeStamp + "0000000");  

            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        public static DateTime ConvertIntDateTime(double d)
        {
            DateTime time = System.DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            time = startTime.AddMilliseconds(d);
            return time;
        }


        /// <summary>  
        /// 过滤多个敏感词 不区分大小写
        ///</summary>
        /// <param name="str">需要替换的字符串</param>  
        /// <param name="replaceStr">需要替换的字符</param>  
        /// <param name="replaceWord">将替换为</param>  
        /// <param name="isWord">是否为单词，true表示只替换对应的单词，false替换所有匹配的字符</param>
        /// <param name="unReplaceStr">不需要过滤的字符串</param>
        /// <returns></returns>  
        public static string ToReplaceIgnoreCase(this string str, string replaceStr, string replaceWord = "", bool isWord = false, string unReplaceStr = null)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(replaceStr))
            {
                return str;
            }
            //将不需要替换的字符串替换为特殊字符，替换关键字之后再还原
            string tempStr = "☃";
            if (!string.IsNullOrEmpty(unReplaceStr))
            {
                str = str.Replace(unReplaceStr, tempStr);
            }
            var StrList = replaceStr.Split(',').ToList();
            foreach (var item in StrList)
            {
                var changrItem = Regex.Escape(item);
                if (isWord)
                {
                    str = Regex.Replace(str, string.Format(@"\b{0}\b", changrItem), replaceWord, RegexOptions.IgnoreCase);
                }
                else
                {
                    str = Regex.Replace(str, changrItem, replaceWord, RegexOptions.IgnoreCase);
                }
            }
            //将不需要替换的字符串进行还原
            if (!string.IsNullOrEmpty(unReplaceStr))
            {
                str = str.Replace(tempStr, unReplaceStr);
            }
            return str;
        }
        /// <summary>
        /// 过滤多个敏感词 不区分大小写
        /// </summary>
        /// <param name="str"></param>
        /// <param name="replaceStr"></param>
        /// <param name="replaceWord"></param>
        /// <param name="isWord"></param>
        /// <param name="unReplaceStr"></param>
        /// <returns></returns>
        public static string ToReplaceIgnoreCase(this string str, List<string> replaceStr, string replaceWord = "", bool isWord = false, string unReplaceStr = null)
        {
            if (string.IsNullOrEmpty(str) || replaceStr == null || !replaceStr.Any())
            {
                return str;
            }
            //将不需要替换的字符串替换为特殊字符，替换关键字之后再还原
            string tempStr = "☃";
            if (!string.IsNullOrEmpty(unReplaceStr))
            {
                str = str.Replace(unReplaceStr, tempStr);
            }
            foreach (var item in replaceStr)
            {
                var changrItem = Regex.Escape(item);
                if (isWord)
                {
                    str = Regex.Replace(str, string.Format(@"\b{0}\b", changrItem), replaceWord, RegexOptions.IgnoreCase);
                }
                else
                {
                    str = Regex.Replace(str, changrItem, replaceWord, RegexOptions.IgnoreCase);
                }
            }
            //将不需要替换的字符串进行还原
            if (!string.IsNullOrEmpty(unReplaceStr))
            {
                str = str.Replace(tempStr, unReplaceStr);
            }
            return str;
        }
        /// <summary>
        /// 获取传入的字符串中，第一个数字
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string RegResult(string str)
        {
            string regex = @"(\d+(\.\d+)?)";
            System.Text.RegularExpressions.Match mstr = Regex.Match(str, regex);
            return mstr.Groups[1].Value.ToString();
        }


        #region 得到一周的周一和周日的日期
        /// <summary> 
        /// 计算本周的周一日期 
        /// </summary> 
        /// <returns></returns> 
        public static DateTime GetMondayDate()
        {
            return GetMondayDate(DateTime.Now);
        }
        /// <summary> 
        /// 计算本周周日的日期 
        /// </summary> 
        /// <returns></returns> 
        public static DateTime GetSundayDate()
        {
            return GetSundayDate(DateTime.Now);
        }
        /// <summary> 
        /// 计算某日起始日期（礼拜一的日期） 
        /// </summary> 
        /// <param name="someDate">该周中任意一天</param> 
        /// <returns>返回礼拜一日期，后面的具体时、分、秒和传入值相等</returns> 
        public static DateTime GetMondayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Monday;
            if (i == -1) i = 6;// i值 > = 0 ，因为枚举原因，Sunday排在最前，此时Sunday-Monday=-1，必须+7=6。 
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Subtract(ts);
        }
        /// <summary> 
        /// 计算某日结束日期（礼拜日的日期） 
        /// </summary> 
        /// <param name="someDate">该周中任意一天</param> 
        /// <returns>返回礼拜日日期，后面的具体时、分、秒和传入值相等</returns> 
        public static DateTime GetSundayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Sunday;
            if (i != 0) i = 7 - i;// 因为枚举原因，Sunday排在最前，相减间隔要被7减。 
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Add(ts);
        }

        /// <summary>
        /// 获取两个时间的时间差并去除周末
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static int GetDayRemoveWeek(DateTime startDate, DateTime endDate)
        {
            TimeSpan span = endDate - startDate;
            int AllDays = Convert.ToInt32(span.TotalDays) + 1;//差距的所有天数
            int totleWeek = AllDays / 7;//差别多少周
            int yuDay = AllDays % 7; //除了整个星期的天数
            int lastDay = 0;
            if (yuDay == 0) //正好整个周
            {
                lastDay = AllDays - (totleWeek * 2);
            }
            else
            {
                int weekDay = 0;
                int endWeekDay = 0;  //多余的天数有几天是周六或者周日
                switch (startDate.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        weekDay = 1;
                        break;
                    case DayOfWeek.Tuesday:
                        weekDay = 2;
                        break;
                    case DayOfWeek.Wednesday:
                        weekDay = 3;
                        break;
                    case DayOfWeek.Thursday:
                        weekDay = 4;
                        break;
                    case DayOfWeek.Friday:
                        weekDay = 5;
                        break;
                    case DayOfWeek.Saturday:
                        weekDay = 6;
                        break;
                    case DayOfWeek.Sunday:
                        weekDay = 7;
                        break;
                }
                if ((weekDay == 6 && yuDay >= 2) || (weekDay == 7 && yuDay >= 1) || (weekDay == 5 && yuDay >= 3) || (weekDay == 4 && yuDay >= 4) || (weekDay == 3 && yuDay >= 5) || (weekDay == 2 && yuDay >= 6) || (weekDay == 1 && yuDay >= 7))
                {
                    endWeekDay = 2;
                }
                if ((weekDay == 6 && yuDay < 1) || (weekDay == 7 && yuDay < 5) || (weekDay == 5 && yuDay < 2) || (weekDay == 4 && yuDay < 3) || (weekDay == 3 && yuDay < 4) || (weekDay == 2 && yuDay < 5) || (weekDay == 1 && yuDay < 6))
                {
                    endWeekDay = 1;
                }
                lastDay = AllDays - (totleWeek * 2) - endWeekDay;
            }
            return lastDay;
        }

        public static int GetWeekDayToInt(DateTime date)
        {
            int result = 1;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    result = 1;
                    break;
                case DayOfWeek.Tuesday:
                    result = 2;
                    break;
                case DayOfWeek.Wednesday:
                    result = 3;
                    break;
                case DayOfWeek.Thursday:
                    result = 4;
                    break;
                case DayOfWeek.Friday:
                    result = 5;
                    break;
                case DayOfWeek.Saturday:
                    result = 6;
                    break;
                case DayOfWeek.Sunday:
                    result = 7;
                    break;
            }
            return result;
        }

        #endregion


        /// <summary>
        /// 获得日期形式:yyyy-mm
        /// </summary>
        /// <param name="sourceDate"></param>
        /// <returns></returns>
        public static string GetDateyyyymm()
        {
            int NowYear = DateTime.Now.Year;
            int NowMouth = DateTime.Now.Month;
            string NowTime = NowYear + "-" + (NowMouth < 10 ? "0" + NowMouth : NowMouth.ToString());
            return NowTime;
        }

        /// <summary>
        /// 获得当年的第一天
        /// </summary>
        /// <returns>:yyyy-mm-dd</returns>
        public static string GetDateFirstDay()
        {
            DateTime dtFirstDay = new DateTime(DateTime.Now.Year, 1, 1);
            return dtFirstDay.ToString(Defaults.DateFormat);
        }

        /// <summary>
        /// 获得当年的最后一天
        /// </summary>
        /// <returns>:yyyy-mm-dd</returns>
        public static string GetDateLastDay()
        {
            DateTime dtFirstDay = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime dtLastDay = dtFirstDay.AddYears(1).AddDays(-1);
            return dtLastDay.ToString(Defaults.DateFormat);
        }


        /// <summary>
        /// 去除字符串首尾处的空格、回车、换行符、制表符
        /// </summary>
        public static string SpeciaStringTrim(string str)
        {
            if (!string.IsNullOrEmpty(str))
                return str.Trim().Trim('\r').Trim('\n').Trim('\t');
            return string.Empty;
        }
        /// <summary>
        /// 打印文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void PrintFile(string filePath)
        {
            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
            Process processInstance = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.Verb = "Print";
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = @"/p /h \" + filePath + "\" \"" + pd.PrinterSettings.PrinterName + " \"";
            startInfo.FileName = filePath;
            processInstance.StartInfo = startInfo;
            processInstance.Start();
            processInstance.Dispose();
            processInstance.Close();
        }

        /// <summary>
        /// 检查字符串的编码中是否存在中文乱码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckStringGarbled(string str)
        {
            foreach (var item in str)
            {
                int ss = (int)item;
                if (((ss >= 0) && (ss <= 8)) || ((ss >= 11) && (ss <= 12)) || ((ss >= 14) && (ss <= 32)))
                {
                    return true;
                }
            }
            return false;
        }



        #region 把特殊字符替换成英文逗号
        /// <summary>
        /// 把特殊字符替换成英文逗号  
        /// </summary>
        /// <param name="str">把特殊字符替换成英文逗号字符串</param>
        /// <returns>返回字符串</returns>
        public static string SpecialReplaceEnglish(string strValue)
        {
            return Regex.Replace(strValue, "(、|，|；|;)", ",", RegexOptions.IgnoreCase);
        }
        #endregion

        #region 把特殊字符替换成SQL查询字符串
        /// <summary>
        ///  把特殊字符替换成SQL查询字符串  
        /// </summary>
        /// <param name="str">特殊字符串</param>
        /// <returns>返回字符串</returns>
        public static string SpecialReplaceSqlStr(string strValue)
        {
            //string pattern = "[\\[ \\] \\^ \\-_*×――(^)$%~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,;\"‘’“”......-..\\\r\n]";
            //return Regex.Replace(strValue, pattern, "','");
            return Regex.Replace(strValue, "(、|，|,|；|;|\r\n| )", "','", RegexOptions.IgnoreCase);
        }
        #endregion
        #region 移除字符串前后英文逗号
        /// <summary>
        /// 移除字符串前后英文逗号 
        /// </summary>
        /// <param name="str">移除字符串前后英文逗号的字符串</param>
        /// <returns>返回字符串</returns>
        public static string TrimStartTrimEndComma(string strValue)
        {
            return strValue.TrimStart(',').TrimEnd(',');
        }
        #endregion

        public static string MaxLength(this string str, int maxLen, bool byByteLens = false)
        {
            //if (str.IsNullOrEmpty()) return str;

            if (byByteLens)
            {
                string tmp = string.Empty;
                if (System.Text.Encoding.Default.GetByteCount(str) <= maxLen)//如果长度比需要的长度n小,返回原字符串
                {
                    return str;
                }

                int t = 0;
                char[] chars = str.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    // 有些字符同样占两个byte 但是又不在汉字范围内：Беларусь
                    var tempchar = chars[i].ToString();
                    var unitLength = Encoding.Default.GetByteCount(tempchar);
                    tmp += chars[i];
                    t += unitLength;

                    if (t >= maxLen)
                    {
                        break;
                    }
                }
                return tmp;
            }

            if (!string.IsNullOrEmpty(str))
            {
                if (str.Length > maxLen)
                {
                    return str.Substring(0, maxLen);
                }
            }

            return str;
        }

        /// <summary>
        /// 按长度拆分字符串
        /// </summary>
        /// <param name="str">字符</param>
        /// <param name="subNum">拆分长度</param>
        /// <param name="resStrList">返回结果集合</param>
        /// <param name="byByteLens">是否按byte拆分</param>
        public static void SplitString(this string str, int subNum, ref List<string> resStrList, bool byByteLens = true)
        {
            //if (subNum < 0) throw new ArgumentException(nameof(subNum), "拆分长度不能小于0");
            //if (str.IsNullOrEmpty()) return;

            //if (resStrList == null) resStrList = new List<string>();

            //var tempString = str.MaxLength(subNum, byByteLens);
            //var remainString = string.Empty;
            //if (!tempString.IsNullOrEmpty())
            //{
            //    resStrList.Add(tempString);
            //    remainString = str.Remove(0, tempString.Length);
            //}

            //remainString.SplitString(subNum, ref resStrList, byByteLens);
        }

        /// <summary>
        /// 按长度拆分字符串
        /// </summary>
        /// <param name="str">字符</param>
        /// <param name="subNum">拆分长度</param> 
        /// <param name="byByteLens">是否按byte拆分</param>
        public static List<string> SplitString(this string str, int subNum, bool byByteLens = true)
        {
            var splitStringList = new List<string>();
            str.SplitString(subNum, ref splitStringList, byByteLens);
            return splitStringList;
        }

        /// <summary>
        /// 随机打乱一个list的排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> RandomSort<T>(List<T> list)
        {
            Random random = new Random();
            var newList = new List<T>();
            int sourceCount = list.Count;
            for (int i = 0; i < sourceCount; i++)
            {
                int index = random.Next(0, list.Count - 1);
                newList.Add(list[index]);
                list.Remove(list[index]);
            }
            return newList;
        }

        public static String formatDuring(long mss)
        {
            long days = mss / (1000 * 60 * 60 * 24);
            long hours = (mss % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60);
            long minutes = (mss % (1000 * 60 * 60)) / (1000 * 60);
            long seconds = (mss % (1000 * 60)) / 1000;
            return days + " 天 " + hours + " 小时 " + minutes + " 分 "
                    + seconds + " 秒 ";
        }

        /// <summary>
        /// 根据分钟数获取 ??天??小时??分钟
        /// </summary>
        /// <param name="totalMinutes">分钟</param>
        /// <returns>xx天xx小时xx分钟</returns>
        public static string GetHoursMinutes(int totalMinutes)
        {
            if (totalMinutes < 0)
            {

                return "";
            }
            else if (totalMinutes == 0)
            {

                return "0分钟";
            }
            else
            {
                int hours = (totalMinutes / 60) % 24;//小时
                int minutes = totalMinutes % 60;//分钟
                int day = (totalMinutes / 60) / 24;//天

                return (day > 0 ? day + "天" : "") + (hours > 0 ? hours + "小时" : "") + minutes + "分钟";
            }
        }

        /// <summary>
        /// 获取年份下拉框 
        /// </summary>
        /// <param name="emptyKey"></param>
        /// <param name="emptyValue"></param>
        /// <param name="start">开始年份</param>
        /// <param name="End">结束年份， 默认当前年份</param>
        public static List<KeyValuePair<string, string>> YearsDateSelection(string emptyKey, string emptyValue, int startYear, int? endYear = null)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            if (emptyKey != null)
            {
                result.Add(new KeyValuePair<string, string>(emptyKey, emptyValue));
            }
            //没有传入 默认当前年份
            if (!endYear.HasValue)
            {
                endYear = int.Parse(DateTime.Now.ToString("yyyy"));
            }

            for (int i = endYear.Value; i > startYear; i--)
            {
                result.Add(new KeyValuePair<string, string>(i.ToString(), i.ToString()));
            }
            return result;
        }

        /// <summary>
        /// 获取月份下拉框
        /// </summary>
        /// <param name="emptyKey"></param>
        /// <param name="emptyValue"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> MonthDateSelection(string emptyKey, string emptyValue)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            if (emptyKey != null)
            {
                result.Add(new KeyValuePair<string, string>(emptyKey, emptyValue));
            }

            for (int i = 1; i <= 12; i++)
            {
                result.Add(new KeyValuePair<string, string>(i.ToString(), i.ToString()));
            }
            return result;
        }

        /// <summary>
        /// 获取两个时间的相差 向上取整
        /// </summary>
        /// <param name="minDate">开始时间</param>
        /// <param name="maxDate">结束时间</param>
        /// <param name="company">时间格式保留 [yyyy-MM-dd] [yyyy-MM-dd HH24] ... </param>
        /// <returns></returns>

        public static int GetDifferDay(DateTime? minDate, DateTime? maxDate, string company = "yyyy-MM-dd")
        {
            if (!minDate.HasValue || !maxDate.HasValue)
            {
                return 0;
            }

            if (minDate > maxDate)
            {
                return 0;
            }

            if (!string.IsNullOrWhiteSpace(company))
            {
                minDate = DateTime.Parse(minDate.Value.ToString(company));
                //如果开始时间和结束时间是同一天 则至少为一天
                maxDate = DateTime.Parse(maxDate.Value.ToString(company));

                switch (company)
                {
                    //天单位
                    case Defaults.DateFormat:
                        maxDate = maxDate.Value.AddDays(1);
                        return (int)Math.Ceiling(Convert.ToDecimal((maxDate.Value - minDate.Value).TotalDays));
                    //小时单位
                    case Defaults.DateFormatHours:
                        maxDate = maxDate.Value.AddHours(1);
                        return (int)Math.Ceiling(Convert.ToDecimal((maxDate.Value - minDate.Value).TotalHours));

                    //分钟单位
                    case Defaults.DateFormatMinutes:
                        maxDate = maxDate.Value.AddMinutes(1);
                        return (int)Math.Ceiling(Convert.ToDecimal((maxDate.Value - minDate.Value).TotalMinutes));
                    //秒单位
                    case Defaults.DateTimeFormat:
                        maxDate = maxDate.Value.AddSeconds(1);
                        return (int)Math.Ceiling(Convert.ToDecimal((maxDate.Value - minDate.Value).TotalSeconds));
                    default:
                        return 0;
                }
            }
            return 0;
        }
        // <summary>
        /// 是否为订单号  ,字母开头及12位为订单号
        /// </summary>
        /// <param name="Code">搜索的单号</param>
        /// <returns>是否为订单号格式</returns>
        public static bool isOrderId(string Code)
        {
            if (!string.IsNullOrEmpty(Code) && Code.StartsWith("A", StringComparison.CurrentCultureIgnoreCase) && Code.Trim().Length == 13 && Regex.IsMatch(Code.Trim(), @"^[A]{1}\d{12}$"))// 第一个字母,后面为12或者12以上 数字
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// html代码闭包
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CloseHTML(string str)
        {
            string[] arrTags = new string[] { "span", "font", "b", "u", "i", "h1", "h2", "h3", "h4", "h5", "h6", "p", "li", "ul", "table", "div" };
            for (var i = 0; i < arrTags.Length; i++)
            {
                var intOpen = 0;
                var intClose = 0;
                var re = new Regex("\\<" + arrTags[i] + "( [^\\<\\>]+|)\\>", RegexOptions.None);
                var arrMatch = re.Match(str);
                if (arrMatch != null) intOpen = arrMatch.Length;
                re = new Regex("\\<\\/" + arrTags[i] + "\\>", RegexOptions.None);
                arrMatch = re.Match(str);
                if (arrMatch != null) intClose = arrMatch.Length;
                for (var j = 0; j < intOpen - intClose; j++)
                {
                    str += "</" + arrTags[i] + ">";
                }
                /*for(var j=(intOpen-intClose-1);j>=0;j--){
                str+="</"+arrTags[i]+">";
                }*/
            }
            return str;
        }

        /// <summary>
        /// 获取函数的执行时间
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TimeSpan GetExecutionTimeSpan(Action action)
        {
            var sw = new Stopwatch();
            sw.Start();
            action.Invoke();
            sw.Stop();
            return sw.Elapsed;
        }

        /// <summary>
        /// 获取TotalSeconds并重置Stopwatch
        /// </summary>
        /// <param name="stopwatch"></param>
        /// <returns></returns>
        public static double TotalSecondsAndRestart(this Stopwatch stopwatch)
        {
            if (stopwatch == null) throw new ArgumentNullException(nameof(stopwatch));

            stopwatch.Stop();
            var totalSeconds = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Restart();

            return totalSeconds;
        }

        /// <summary>
        /// 相差时间
        /// </summary>
        /// <param name="dateStart">开始</param>
        /// <param name="dateEnd">结束</param>
        /// <returns>返回相差的</returns>
        public static int DateDiff(DateTime dateStart, DateTime dateEnd)
        {
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString());
            DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());
            TimeSpan sp = end.Subtract(start);
            return sp.Days;
        }

        /// <summary>
        /// 判断字符是否为大写字母
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>       
        public static bool IsCapitalLetter(this char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        /// <summary>
        /// SQL 关键字过滤 有些搜索拼接SQL会直接把内容拼接进去了
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SqlFilters(string source)
        {

            //半角括号替换为全角括号
            source = source.Replace("'", "'''");
            //去除执行SQL语句的命令关键字
            source = Regex.Replace(source, "select", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "insert", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "update", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "delete", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "drop", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "truncate", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "declare", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "xp_cmdshell", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "/add", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "net user", "", RegexOptions.IgnoreCase);
            //去除执行存储过程的命令关键字 
            source = Regex.Replace(source, "exec", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "execute", "", RegexOptions.IgnoreCase);
            //去除系统存储过程或扩展存储过程关键字
            source = Regex.Replace(source, "xp_", "x p_", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "sp_", "s p_", RegexOptions.IgnoreCase);
            //防止16进制注入
            source = Regex.Replace(source, "0x", "0 x", RegexOptions.IgnoreCase);
            return source;
        }

        /// <summary>
        /// 获取西邮寄key
        /// </summary>
        /// <returns>返回西邮寄Key</returns>
        public static string GetWestPoatKey()
        {
            string pwd = Defaults.IMcWpApiPwd;  //密码
            string userId = Defaults.IMcWpApiUserId;// "c88vJb";  //apiKey
            string md5String = GetMD5ToLower(GetMD5ToLower(pwd) + userId);
            return md5String;
        }
        /// <summary>
        ///  md 加密
        /// </summary>
        /// <param name="myString">加密的字符串</param>
        /// <returns>返回加密后MD5</returns>
        public static string GetMD5ToLower(string myString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(myString);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = "";
            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String = byte2String + targetData[i].ToString("x2");
            }
            return byte2String;
        }

        /// <summary>
        /// 判断两值是否相等
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="value1">值1</param>
        /// <returns></returns>
        public static bool ValueToBool(int? value, int? value1)
        {
            if (!value.HasValue || !value1.HasValue)
            {
                return false;
            }
            return value.Value == value1.Value;
        }

        /// <summary>
        /// Http post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static string HttpRequest(string url, string param, string contentType = null, string method = null, Dictionary<string, string> header = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Timeout = 20000;
                request.Method = method ?? "POST";
                request.ContentType = contentType ?? "application/x-www-form-urlencoded";
                request.Accept = "application/json";
                if (header != null)
                {
                    foreach (string key in header.Keys)
                    {
                        if (!request.Headers.AllKeys.Contains(key))
                        {
                            request.Headers.Add(key, header[key]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(param))
                {
                    byte[] bs = Encoding.ASCII.GetBytes(param);
                    request.ContentLength = bs.Length;
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                        reqStream.Close();
                    }
                }
                else
                {
                    request.ContentLength = 0;
                }

                string responseData = String.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseData = reader.ReadToEnd();
                    }
                }
                return responseData;
            }
            catch (System.Net.WebException webException)
            {
                HttpWebResponse response = webException.Response as HttpWebResponse;
                StreamReader responseStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string errorMessage = responseStream.ReadToEnd();
                response.Close();
                responseStream.Close();
                responseStream.Dispose();
                throw new Exception(errorMessage, webException);
            }
        }

        #region 下载图片--返回Stream流

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="url">图片URL网络路径</param>
        /// <returns>返回文件流</returns>
        public static Stream DownloadImage(string url)
        {
            System.IO.Stream stream = null;
            HttpWebResponse rsp = null;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.ServicePoint.Expect100Continue = false;
                req.Method = "GET";
                req.KeepAlive = true;
                rsp = (HttpWebResponse)req.GetResponse();
                stream = rsp.GetResponseStream();

                MemoryStream ms = null;
                Byte[] buffer = new Byte[rsp.ContentLength];
                int offset = 0, actuallyRead = 0;
                do
                {
                    actuallyRead = stream.Read(buffer, offset, buffer.Length - offset);
                    offset += actuallyRead;
                }
                while (actuallyRead > 0);
                ms = new MemoryStream(buffer);

                byte[] buffurPic = ms.ToArray();
                return new MemoryStream(buffurPic);
            }
            catch (Exception ex)
            {
                // 释放资源
                if (stream != null)
                {
                    stream.Close();
                }
                if (rsp != null)
                {
                    rsp.Close();
                }
            }
            return stream;
        }

        #endregion

        #region Stream保存为文件

        /// <summary>
        /// Stream保存为文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="savePath">保存路径(含文件完整路径)</param>
        /// <param name="folderfPath">文件夹路径</param>
        /// <returns>返回文件流</returns>
        public static void StreamToFile(Stream stream, string savePath, string folderfPath)
        {
            if (!Directory.Exists(folderfPath))
            {
                Directory.CreateDirectory(folderfPath);
            }
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            FileStream fs = new FileStream(savePath, FileMode.Create);
            stream.Read(bytes, 0, bytes.Length);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
        }

        /// <summary>
        /// Bytes保存为文件
        /// </summary>
        /// <param name="bytes">字节流</param>
        /// <param name="savePath">保存路径(含文件完整路径)</param>
        /// <param name="folderPath">文件夹路径</param>
        /// <returns></returns>
        public static void BytesToFile(byte[] bytes, string savePath, string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            FileStream fs = new FileStream(savePath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
        }

        #endregion

        #region 深度复制 - 克隆

        /// <summary>
        /// 深度复制
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static T Clone<T>(T obj)
        {
            using (Stream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, obj);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)serializer.Deserialize(stream);
            }
        }

        #endregion

        #region 特殊字符替换

        /// <summary>
        /// 特殊字符替换
        /// </summary>
        /// <returns></returns>
        public static string SpecialCharactersReplace(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                //被替换前的空格,看似空格，但是会引起前端的转义,会被转为&nbsp;,所以替换
                str = Regex.Replace(str, " ", " ", RegexOptions.IgnoreCase);
                //替换制表符
                str = Regex.Replace(str, "\t", "", RegexOptions.IgnoreCase);
            }
            return str;
        }

        #endregion

        #region byte[] 转成 Stream

        /// 将 byte[] 转成 Stream
        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        #endregion

        #region Stream 转成 byte[]

        public static byte[] StreamToBytes(Stream stream)
        {
            stream.Position = 0;
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        #endregion


        #region 文件转bytes-FileStream

        /// <summary>
        /// 将文件转换成byte[] 数组
        /// </summary>
        /// <param name="fileUrl">文件路径文件名称</param>
        /// <returns>byte[]</returns>
        public static byte[] GetFileData(string fileUrl)
        {
            using (FileStream fs = new FileStream(fileUrl, FileMode.Open, FileAccess.Read))
            {
                byte[] buffur = new byte[fs.Length];
                fs.Read(buffur, 0, (int)fs.Length);
                return buffur;
            }
        }

        #endregion

        /// <summary>
        /// 根据文件路径判断 是否产品文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static bool CheckProductS3Path(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            filePath = filePath.ToUpper();
            //判断这三个路径 UploadfilePath.ProductPath，UploadfilePath.OldCommonUploadFileAttachment，/Uploadfile/ProductImg/,/Product/,/Uploadfile/temp/Product/
            if (filePath.Contains(UPLOADFILE_PRODUCT) || filePath.Contains(UPLOADFILE_1) || filePath.Contains(UPLOADFILE_PRODUCTIMG)
                || filePath.Contains(PRODUCT_TOPPATH) || filePath.Contains(PRODUCT_TEMPPATH))
                return true;
            return false;
        }


		/// <summary>
		/// 替换Unicode 空白字符
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string ReplaceUnicodeBlankCharacter(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				var regex = new Regex(@"[\p{C}-[\r\n]]+");
				return regex.Replace(text, "");
			}

			return text;
		}

        /// <summary>
        /// 列出2个日期中的每一天 
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>yyyy-MM-dd格式的字符串数组</returns>
        public static string[] getDayArray(string startTime, string endTime, string format = "yyyy-MM-dd")
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            try
            {
                startDate = Convert.ToDateTime(startTime);
                endDate = Convert.ToDateTime(endTime);
            }
            catch
            {
            }
            TimeSpan ts = endDate - startDate;
            int differenceInDays = ts.Days;//相隔的天数
            string[] strArray = new string[differenceInDays + 1];//每一天数组

            DateTime tempTime = startDate;
            string tempStr = "";
            for (int i = 0; i <= differenceInDays; i++)
            {
                if (i < strArray.Length)
                {
                    tempStr = tempTime.ToString(format);
                    strArray[i] = tempStr;
                    tempTime = tempTime.AddDays(1);
                }
            }

            return strArray;
        }
    }

	public enum DateInterval
    {
        Second, Minute, Hour, Day, Week, Month, Quarter, Year
    }
    /// <summary>
    /// html代码闭包
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>

}

