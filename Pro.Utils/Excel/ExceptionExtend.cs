using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pro.Utils
{
    public static class ExceptionExtend
    {
        public static string ToMessage(this Exception e, bool withSimpleStackTrack = false)
        {
            if (withSimpleStackTrack)
            {
                var st = new System.Diagnostics.StackTrace(e, true);
                var num = 0;
                var lineNum = st.GetFrame(num).GetFileLineNumber();
                var colNum = st.GetFrame(num).GetFileColumnNumber();
                var fileName = st.GetFrame(num).GetFileName();
                var m = st.GetFrame(num).GetMethod().Name;

                return e.Message + string.Format("<br/>[程序信息]:文件名：{0}，方法：{1}，行号：{2}，列号：{3}", fileName, m, lineNum, colNum);
            }

            if (e.InnerException == null)
            {
                return e.Message;
            }
            if (e.InnerException.InnerException == null)
            {
                return e.InnerException.Message;
            }
            return e.InnerException.InnerException.Message;
        }
    }
}
