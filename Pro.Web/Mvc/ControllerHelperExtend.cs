using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using Pro.Web.Models;
using System.Text;

namespace System.Web.Mvc
{
    public static class ControllerHelperExtend
    {
        public static SelectList ToSelectList(this List<KeyValuePair<string, string>> value)
        {
            return ToSelectList(value, null);
        }
        public static Select ToSelectModel(this List<KeyValuePair<string, string>> value, string defaultValue = null)
        {
            var selectValue = new Select
            {
                DefaultValue = defaultValue,
                OptionValue = value.Select(f => new OptionValue
                {
                    Id = f.Key,
                    Text = f.Value
                }).ToList()
            };
            return selectValue;
        }
        /// <summary>
        /// 添加一个空元素
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SelectList ToSelectListWithEmptySelection(this List<KeyValuePair<string, string>> value, string emptyText = "")
        {
            value.Insert(0, new KeyValuePair<string, string>(null, emptyText));
            return ToSelectList(value, null);
        }

        public static SelectList ToSelectList(this List<KeyValuePair<string, string>> value, object selectedValue)
        {
            var firstValue = value.FirstOrDefault().Value;
            if (firstValue != null)
            {
                if (value.Count == 1)
                {
                    return new SelectList(value, "key", "value", value.FirstOrDefault().Value);
                }
                else if (selectedValue == null && firstValue.Contains("处理中心") && value.Count == 2)
                {
                    return new SelectList(value, "key", "value", value[1].Key);
                }
                else
                {
                    return new SelectList(value, "key", "value", selectedValue);
                }
            }
            else
            {
                return new SelectList(value, "key", "value", selectedValue);
            }

        }

        #region 通过指定实体的键值名称，然后直接转为SelectList

        /// <summary>
        /// 输入一个实体集合，生成一个SelectList
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="source">集合</param>
        /// <param name="keyColumn">键的lambda表达式</param>
        /// <param name="valueColumn">值的lambda表达式</param>
        /// <param name="emptyKey">空键</param>
        /// <param name="emptyValue">空值</param>
        /// <returns></returns>
        public static SelectList ToSelectList<T>(this IEnumerable<T> source, Expression<Func<T, string>> keyColumn, Expression<Func<T, string>> valueColumn, string emptyKey = null, string emptyValue = null)
        {
            return ToSelectList<T, string, string>(source, keyColumn, valueColumn, emptyKey, emptyValue);
        }

        /// <summary>
        /// 输入一个实体集合，生成一个SelectList
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <param name="source">集合</param>
        /// <param name="keyColumn">键的lambda表达式</param>
        /// <param name="valueColumn">值的lambda表达式</param>
        /// <param name="emptyKey">空键</param>
        /// <param name="emptyValue">空值</param>
        /// <returns></returns>
        public static SelectList ToSelectList<T, TKey, TValue>(this IEnumerable<T> source, Expression<Func<T, TKey>> keyColumn, Expression<Func<T, TValue>> valueColumn, string emptyKey = null, string emptyValue = null)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            if (emptyKey != null) result.Add(new KeyValuePair<string, string>(emptyKey, emptyValue));

            var keyName = (keyColumn.Body as MemberExpression).Member.Name;
            var valueName = (valueColumn.Body as MemberExpression).Member.Name;

            var keyProperty = typeof(T).GetProperty(keyName);
            var valueProperty = typeof(T).GetProperty(valueName);

            foreach (var item in source)
            {
                var key = keyProperty.GetValue(item, null).ToString();
                var value = valueProperty.GetValue(item, null).ToString();
                result.Add(new KeyValuePair<string, string>(key, value));
            }

            return result.ToSelectList();
        }

        /// <summary>
        /// 通过集合生成选择项
        /// </summary>
        /// <param name="sourceList">源数据</param>
        /// <param name="emptyKey">空键</param>
        /// <param name="emptyValue">空值</param>
        /// <returns></returns>
        public static SelectList ToSelectList(IList<string> sourceList, string emptyKey = null, string emptyValue = null)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            if (emptyKey != null) result.Add(new KeyValuePair<string, string>(emptyKey, emptyValue));

            foreach (var item in sourceList)
            {
                result.Add(new KeyValuePair<string, string>(item, item));
            }
            return result.ToSelectList();
        }

        #endregion

        public static ContentResult Ajax_RedirectToAction(this Controller cntrl, string actionName, string controller = null, string id = null)
        {
            string url = cntrl.Url.Action(actionName, controller);
            return new ContentResult() { Content = "<script>self.parent.openPage('" + url + "?ids=" + id + "');</script>" };
            // return new ContentResult() { Content = "<script>self.parent.tb_remove();self.parent.refreshData('#list_grid');</script>" };
        }

        public static ContentResult Ajax_FBClose(this Controller cntrl, string jScript = "")
        {
            if (string.IsNullOrWhiteSpace(jScript))
                return new ContentResult() { Content = "<script>parent.$.fancybox.close();</script>" };
            else
                return new ContentResult() { Content = string.Format("<script>parent.$.fancybox.close();{0}</script>", jScript) };
        }

        /// <summary>
        /// 关闭FancyBox脚本扩展方法
        /// </summary>
        /// <param name="cntrl">所在的控制器</param>
        /// <param name="objId">要赋值的对象ID，用于ID选择器</param>
        /// <param name="val">要赋的值</param>
        /// <returns></returns>
        public static ContentResult Ajax_FBClose(this Controller cntrl, string objId, string val)
        {
            return new ContentResult() { Content = string.Format("<script>parent.$.fancybox.close();parent.$('#{0}').val('{1}');</script>", objId, val) };
        }

        public static ContentResult Ajax_FBCloseToCode(this Controller cntrl, string jsCode)
        {
            return new ContentResult() { Content = string.Format("<script>parent.$.fancybox.close();{0}</script>", jsCode) };
        }

        /// <summary>
        /// 关闭FancyBox脚本扩展方法
        /// </summary>
        /// <param name="cntrl">所在的控制器</param>
        /// <param name="objId">要赋值的对象ID，用于ID选择器</param>
        /// <param name="val">要赋的值</param>
        /// <returns></returns>
        public static ContentResult Ajax_CloseAndReload(this Controller cntrl)
        {
            return new ContentResult() { Content = "<script>parent.$.fancybox.close();self.parent.location.reload();</script>" };
        }

        /* begin jqGrid */
        public static ContentResult Ajax_JGClose(this Controller cntrl, string gridID = "gridList", string extParam = "")
        {
            string result = "<script>";
            result += "if(!parent || !parent.$ || !parent.$.fancybox){ window.close(); window.opener.reloadJqGrid('" + gridID + "'); }";
            result += "else{ parent.$.fancybox.close();";
            result += "self.parent.reloadJqGrid('" + gridID + "');";
            result += extParam;
            result += "}</script>";
            return new ContentResult() { Content = result };
        }

        public static ContentResult Ajax_JGCloseAndOpen(this Controller cntrl, string url, string id, string gridID = "gridList", string extParam = "")
        {
            string result = "<script>";
            result += string.Format("self.parent.window.open('{0}?ids=" + id + "', '', 'width=1000,height=649, top=0, left=0, toolbar=no, menubar=no, resizable=yes,location=no, status=no');", url);
            result += "parent.$.fancybox.close();";
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridID);
            result += extParam;
            result += "</script>";
            return new ContentResult() { Content = result };
        }

        public static ContentResult Ajax_JGClosePur(this Controller cntrl, string gridID = "gridList", string gridChildID = "gridList2", string extParam = "")
        {
            string result = "<script>";
            result += "parent.$.fancybox.close();";
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridID);
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridChildID);
            result += extParam;
            result += "</script>";
            return new ContentResult() { Content = result };
        }

        public static ContentResult Ajax_JGCloseMultiple(this Controller cntrl, string gridID1 = "gridList", string gridID2 = "gridList1", string gridID3 = "gridList2", string extParam = "")
        {
            string result = "<script>";
            result += "parent.$.fancybox.close();";
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridID1);
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridID2);
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridID3);
            result += extParam;
            result += "</script>";
            return new ContentResult() { Content = result };
        }



        //零售订单计数
        public static ContentResult Ajax_OrderMenuClose(this Controller cntrl, string gridID = "gridList", string extParam = "")
        {
            string result = "<script>";
            result += "parent.$.fancybox.close();";
            //result += "parent.parent.displayMenuData();";
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridID);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }
        //退款计数
        public static ContentResult Ajax_JGRefundMenuClose(this Controller cntrl, string gridID = "gridList", string extParam = "")
        {
            string result = "<script>";
            result += "parent.$.fancybox.close();";
            //result += "parent.parent.displayMenuRefundData();";
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridID);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }
        //批发计数
        public static ContentResult Ajax_JGWholesaleMenuClose(this Controller cntrl, string gridID = "gridList", string extParam = "")
        {
            string result = "<script>";
            result += "parent.$.fancybox.close();";
            //result += "parent.parent.displayMenuWholesaleData();";
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridID);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }
        public static ContentResult Ajax_JGCloseAndMessage(this Controller cntrl, string message, string gridID = "gridList", string extParam = "")
        {
            if (string.IsNullOrWhiteSpace(message)) return Ajax_JGClose(cntrl, gridID, extParam);

            string result = "<script>";
            result += "parent.$.fancybox.close();";
            result += string.Format("parent.showSucceedMsg('{0}');", message);
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridID);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }
        //弹出提示信息，部分不成功信息
        public static ContentResult Ajax_JGCloseAndAlertMessage(this Controller cntrl, string message, string gridID = "gridList", string extParam = "")
        {
            if (string.IsNullOrWhiteSpace(message)) return Ajax_JGClose(cntrl, gridID, extParam);

            string result = "<script>";
            result += "parent.$.fancybox.close();";
            result += string.Format("parent.showAlertMsg('{0}');", message);
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridID);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }
        public static ContentResult Ajax_JGCloseErrorMessage(this Controller cntrl, string message, string gridID = "gridList", string extParam = "")
        {
            if (string.IsNullOrWhiteSpace(message)) return Ajax_JGClose(cntrl, gridID, extParam);

            string result = "<script>";
            result += "parent.$.fancybox.close();";
            result += string.Format("parent.showErrorMsg(\"{0}\");", message);
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridID);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }

        /// <summary>
        /// 关闭窗口不刷新页面数据
        /// </summary>
        public static ContentResult Ajax_JGCloseNoRefresh(this Controller cntrl, string message, string extParam = "")
        {
            if (string.IsNullOrWhiteSpace(message)) return Ajax_JGClose(cntrl, extParam);

            string result = "<script>";
            result += "parent.$.fancybox.close();";
            result += string.Format("parent.showSucceedMsg(\"{0}\");", message);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }

        /// <summary>
        /// 显示错误信息关闭窗口,因为之前写的先关闭窗口的话显示信息的话后面那一部分好像会执行不到造成无法提示
        /// </summary>
        /// <param name="cntrl"></param>
        /// <param name="message">消息内容</param>
        /// <param name="gridId">jqGrid列表Id，为空时不刷新</param>
        /// <param name="isSuccessMessage">提示的消息类型</param>
        /// <param name="extParam"></param>
        /// <returns></returns>
        public static ContentResult Ajax_JGCloseShowMessage(this Controller cntrl, string message, string gridId = "gridList", bool isSuccessMessage = true, string extParam = "")
        {
            if (string.IsNullOrWhiteSpace(message)) return Ajax_JGClose(cntrl, extParam);

            string result = "<script>";

            if (!string.IsNullOrWhiteSpace(message))
            {
                if (isSuccessMessage)
                {
                    result += string.Format("parent.showSucceedMsg(\"{0}\");", message);
                }
                else
                {
                    result += string.Format("parent.showErrorMsg(\"{0}\");", message);
                }
            }

            result += extParam;

            if (!string.IsNullOrEmpty(gridId))
            {
                result += string.Format("self.parent.reloadJqGrid('{0}');", gridId);
            }

            result += "parent.$.fancybox.close();";
            result += "</script>";

            return new ContentResult() { Content = result };
        }

        /// <summary>
        /// 显示错误信息关闭窗口,因为之前写的先关闭窗口的话显示信息的话后面那一部分好像会执行不到造成无法提示
        /// </summary>
        /// <param name="cntrl"></param>
        /// <param name="message"></param>
        /// <param name="gridID"></param>
        /// <param name="extParam"></param>
        /// <returns></returns>
        public static ContentResult Ajax_JGShowErrorMessageCloseWin(this Controller cntrl, string message, string gridID = "gridList", string extParam = "")
        {
            if (string.IsNullOrWhiteSpace(message)) return Ajax_JGClose(cntrl, gridID, extParam);

            string result = "<script>";
            result += string.Format("parent.showErrorMsg(\"{0}\");", message);
            result += "parent.$.fancybox.close();";
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridID);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }


        public static ContentResult Ajax_JGCloseAndWithiConMessage(this Controller cntrl, string message, string gridID = "gridList", string extParam = "")
        {
            if (string.IsNullOrWhiteSpace(message)) return Ajax_JGClose(cntrl, gridID, extParam);

            string result = "<script>";
            result += "parent.$.fancybox.close();";
            result += string.Format("parent.showAlertMsgWithiConMessage('{0}');", message);
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridID);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }

        /* end jqGrid */

        /* begin Json */
        /// <summary>
        /// 用于操作成功后返回。
        /// 返回json数据包含success、message,link
        /// </summary>
        public static JsonResult Json_SuccessResult(this Controller cntrl, string messageText = null, string toUrl = null, string toLink = null)
        {
            return Json_Result(cntrl, messageText, true, toUrl, toLink);
        }

        /// <summary>
        /// 用于操作成功后返回JSON
        /// </summary>
        /// <param name="cntrl">当前控制器</param>
        /// <param name="successData">成功返回数据</param>
        /// <param name="messageText">消息内容</param>
        /// <param name="toUrl"></param>
        /// <param name="toLink"></param>
        /// <returns></returns>
        public static JsonResult Json_SuccessResult(this Controller cntrl, dynamic successData, string messageText = null, string toUrl = null, string toLink = null)
        {

            var result = new JsonResult();
            result.Data = new { success = true, message = messageText, url = toUrl, link = toLink, Data = successData };
            return result;
        }

        /// <summary>
        /// 用于操作失败后返回。
        /// 返回json数据包含success、message
        /// </summary>
        public static JsonResult Json_ErrorResult(this Controller cntrl, string errorMessage = null, string toUrl = null)
        {
            return Json_Result(cntrl, errorMessage, false, toUrl);
        }
        /// <summary>
        /// 用于操作失败后返回。
        /// 返回json数据包含success、message
        /// </summary>
        public static JsonResult Json_ErrorResult(this Controller cntrl, Exception e)
        {
            return Json_ErrorResult(cntrl, (e.InnerException == null ? e.Message : (e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message)));
        }
        /// <summary>
        /// 返回json数据包含success、message
        /// </summary>
        public static JsonResult Json_Result(this Controller cntrl, string messageText = null, bool isSuccess = true, string toUrl = null, string toLink = null)
        {
            var result = new JsonResult();
            result.Data = new { success = isSuccess, message = messageText, url = toUrl, link = toLink };
            return result;
        }
        /* end Json */

        /// <summary>
        /// write a client javascript to the server side
        /// </summary>
        /// <param name="cntrl"></param>
        /// <param name="script">javascript string</param>
        /// <param name="scriptInclude">if the script tag is included</param>
        /// <returns></returns>
        public static ContentResult Ajax_ClientScript(this Controller cntrl, string script, bool scriptInclude = false)
        {
            string scriptString = String.Empty;
            if (!scriptInclude)
            {
                scriptString = "<script>" + script + "</script>";
            }
            else
            {
                scriptString = script;
            }
            return new ContentResult() { Content = scriptString };
        }

        public static ContentResult Ajax_WindowClose(this Controller cntrl, string alertInfo = "", string gridID = "gridList", string extParam = "")
        {
            //window.close(); window.opener.reloadJqGrid('gridList');
            string result = "<script>";
            result += alertInfo;
            result += "window.close();";
            result += string.Format("window.opener.reloadJqGrid('{0}');", gridID);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }
        public static ContentResult Ajax_WindowCloseAndMessage(this Controller cntrl, string message, string alertInfo = "", string gridID = "gridList", string extParam = "")
        {
            //window.close(); window.opener.reloadJqGrid('gridList');
            string result = "<script>";
            result += alertInfo;
            result += "window.close();";
            result += string.Format("window.opener.showSucceedMsg('{0}');", message);
            result += string.Format("window.opener.reloadJqGrid('{0}');", gridID);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }

        public static ContentResult Ajax_WindowCloseAndErrorMessage(this Controller cntrl, string message, string alertInfo = "", string gridID = "gridList", string extParam = "")
        {
            //window.close(); window.opener.reloadJqGrid('gridList');
            string result = "<script>";
            result += alertInfo;
            result += "window.close();";
            result += string.Format("window.opener.showErrorMsg('{0}');", message);
            result += string.Format("window.opener.reloadJqGrid('{0}');", gridID);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }

        //父JG和子JG同时刷新
        public static ContentResult Ajax_TwoJGClose(this Controller cntrl, string gridIDPar, string gridIDChild, string extParam = "")
        {
            string result = "<script>";
            result += "parent.$.fancybox.close();";
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridIDPar);
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridIDChild);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }

        public static ContentResult Ajax_TwoWindowClose(this Controller cntrl, string alertInfo = "", string gridIDPar = "gridList", string gridID = "gridList", string extParam = "")
        {
            string result = "<script>";
            result += alertInfo;
            result += "window.close();";
            result += string.Format("window.opener.reloadJqGrid('{0}');", gridIDPar);
            result += string.Format("window.opener.reloadJqGrid('{0}');", gridID);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }
       
        public static ActionResult AjaxErrorBoxMsg(this Controller controller, string msg)
        {
            var script = string.Format("self.parent.showAlertMsg('{0}');", msg);
            return controller.Ajax_ClientScript(script);
        }
        /// <summary>
        /// 返回错误信息
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="msg"></param>
        /// <param name="gridID"></param>
        /// <param name="extParam"></param>
        /// <returns></returns>
        public static ActionResult AjaxJGCloseErrorBoxMsg(this Controller controller, string msg, string gridID = "gridList", string extParam = "")
        {
            if (string.IsNullOrWhiteSpace(msg)) return Ajax_JGClose(controller, gridID, extParam);

            string result = "<script>";
            result += "parent.$.fancybox.close();";
            result += string.Format("parent.showAlertMsg(\"{0}\");", msg);
            result += string.Format("self.parent.reloadJqGrid('{0}');", gridID);
            result += extParam;
            result += "</script>";

            return new ContentResult() { Content = result };
        }

      
    }
}