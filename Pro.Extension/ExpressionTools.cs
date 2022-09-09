using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Pro.Model.dto;

namespace Pro.Extension
{
    //中间层 引用数据层
    public class ExpressionTools
    {


        #region 查询条件 lamada表达式(根据method而定)
        /// <summary>
        /// 查询条件 lamada表达式(method默认 Contains)
        /// </summary>
        /// <param name="IsLowLevel">是否包含显示下级</param>
        /// <param name="columnName">列名</param>
        public static void GetParsByCondition<T>(string columnName, List<Expression<Func<T, bool>>> parmList, string value = "", string methodInfo = "Contains")
        {
            try
            {
                if (!string.IsNullOrEmpty(columnName))
                {

                    ParameterExpression param = Expression.Parameter(typeof(T), "c");
                    MethodInfo method = typeof(string).GetMethod(methodInfo);
                    MethodInfo strings = typeof(object).GetMethod("ToString", new Type[] { });
                    MemberExpression left = Expression.Property(param, typeof(T).GetProperty(columnName));

                    //右边表达式
                    ConstantExpression right = null;

                    right = Expression.Constant(value);


                    MethodCallExpression filter = null;
                    Expression filters = null;
                    if (methodInfo == "Contains")
                    {
                        //int类型转为string的表达式
                        Expression left1 = Expression.Call(left, strings);
                        filter = Expression.Call(right, method, left1);
                    }
                    else if (methodInfo == "DateTime")
                    {
                        if (value.Contains(",") && value.Length >= 8)
                        {
                            string strValue = value.Split(',').ToString();
                            ConstantExpression right1 = null;

                            if (!string.IsNullOrEmpty(strValue[0].ToString()))
                            {
                                right = Expression.Constant(strValue[0]);
                            }
                            else if (!string.IsNullOrEmpty(strValue[1].ToString()))
                            {
                                right1 = Expression.Constant(strValue[1]);
                            }

                            filters = Expression.GreaterThanOrEqual(left, right);
                            filters = Expression.LessThanOrEqual(left, right1);
                        }
                    }
                    else if (methodInfo == "num")
                    {
                        filters = Expression.GreaterThanOrEqual(left, right);
                    }
                    else
                    {
                        filter = Expression.Call(left, method, right);
                    }

                    Expression<Func<T, bool>> pras = Expression.Lambda<Func<T, bool>>(filter, param);
                    parmList.Add(pras);
                }
            }
            catch (Exception e)
            {
                new Exception(e.Message);
            }
        }
        #endregion

        #region 查询条件 lamada表达式( ==   or)
        /// <summary>
        /// 查询条件 lamada表达式( ==   or)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName"></param>
        /// <param name="parmList"></param>
        public static void GetEqualPars1<T>(string columnName, List<Expression<Func<T, bool>>> parmList, string value = "", string methodInfo = "")
        {
            try
            {

                if (!string.IsNullOrEmpty(value))
                {
                    ParameterExpression param = Expression.Parameter(typeof(T), "c");
                    MethodInfo method = null;
                    if (!string.IsNullOrEmpty(methodInfo))
                    {
                        method = typeof(string).GetMethod(methodInfo);
                    }
                    //转string类型
                    MethodInfo strings = typeof(object).GetMethod("ToString", new Type[] { });

                    //构造左表达式
                    MemberExpression left = null;

                    //构造右表达式  用ConstantExpression表达式表示具有常量值的表达式
                    ConstantExpression right2 = Expression.Constant(value);

                    Expression where2 = null;

                    if (columnName.Contains("/"))
                    {
                        string[] attrColumn = columnName.Split('/');
                        //获取字段(多字段)
                        for (int i = 0; i < attrColumn.Length; i++)
                        {
                            PropertyInfo property = typeof(T).GetProperty(attrColumn[i]);

                            left = Expression.Property(param, property);
                            Expression left1 = Expression.Call(left, strings);

                            if (methodInfo == "Contains")       //模糊查询
                            {
                                if (i == 0)
                                {
                                    //进行合并
                                    where2 = Expression.Call(left1, method, right2);
                                }
                                else
                                {
                                    //进行合并
                                    Expression filterTmp = Expression.Call(left1, method, right2);
                                    where2 = Expression.Or(filterTmp, where2);
                                }
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    //进行合并
                                    where2 = Expression.Equal(left1, right2);
                                }
                                else
                                {
                                    //进行合并
                                    Expression filterTmp = Expression.Equal(left1, right2);
                                    where2 = Expression.Or(filterTmp, where2);
                                }
                            }
                        }
                    }
                    else
                    {
                        left = Expression.Property(param, columnName);
                        if (methodInfo == "Contains")       //模糊查询
                        {
                            if (value.Contains(","))
                            {
                                string[] ids = value.Split(',');
                                Expression left1 = Expression.Call(left, strings);
                                for (int i = 0; i < ids.Length; i++)
                                {
                                    var id = ids[i];
                                    right2 = Expression.Constant(id);

                                    if (i == 0)
                                    {
                                        //进行合并
                                        where2 = Expression.Equal(left1, right2);
                                    }
                                    else
                                    {
                                        //进行合并
                                        Expression filterTmp = Expression.Equal(left1, right2);
                                        where2 = Expression.Or(filterTmp, where2);
                                    }
                                }
                            }
                            else
                            {
                                //Expression left1 = Expression.Call(left, strings);
                                where2 = Expression.Call(left, method, right2);
                            }
                        }
                        else if (methodInfo == "DateTime")
                        {
                            if (value.Contains(",") && value != ",")
                            {
                                string[] strValue = value.Split(',');
                                ConstantExpression right1 = null;
                                right2 = null;
                                //开始时间
                                if (!string.IsNullOrEmpty(strValue[0].ToString()))
                                {
                                    right1 = Expression.Constant(Convert.ToDateTime(strValue[0]), typeof(DateTime?));
                                }
                                //结束时间
                                if (!string.IsNullOrEmpty(strValue[1].ToString()))
                                {
                                    right2 = Expression.Constant(Convert.ToDateTime(strValue[1]), typeof(DateTime?));
                                }

                                if (right1 != null && right2 == null)       //只有开始时间
                                {
                                    where2 = Expression.GreaterThanOrEqual(left, right1);   //大于开始时间
                                }
                                else if (right1 == null && right2 != null)  //只有结束时间
                                {
                                    where2 = Expression.LessThanOrEqual(left, right2);      //小于结束时间
                                }
                                else if (right1 != null && right2 != null)  //有开始时间又有结束时间
                                {
                                    where2 = Expression.GreaterThanOrEqual(left, right1);
                                    Expression filterTmp = Expression.LessThanOrEqual(left, right2);
                                    where2 = Expression.And(filterTmp, where2);
                                }
                            }
                        }
                        else if (methodInfo == "num")
                        {
                            ConstantExpression right1 = Expression.Constant(Convert.ToInt32(value), typeof(int?));
                            where2 = Expression.Equal(left, right1);
                        }
                        else
                        {
                            //进行合并：例如 c.name.ToString()==value
                            //Expression left1 = Expression.Call(left, strings);
                            where2 = Expression.Equal(left, right2);

                        }
                    }

                    Expression<Func<T, bool>> pras = Expression.Lambda<Func<T, bool>>(where2, param);
                    parmList.Add(pras);
                }
            }
            catch (Exception e)
            {
                new Exception(e.Message);
            }

        }
        #endregion


        #region 查询条件 lamada表达式( ==   or)
        /// <summary>
        /// 查询条件 lamada表达式( ==   or)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName"></param>
        /// <param name="parmList"></param>
        public static void GetEqualPars<T>(string columnName, List<Expression<Func<T, bool>>> parmList, string value = "", string methodInfo = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    //expression表达式树主体构造开始
                    ParameterExpression param = Expression.Parameter(typeof(T), "c");   //声明Lambda表达式中的参数表达式c(c=>c.columnaName == value)
                    MethodInfo method = null;
                    if (!string.IsNullOrEmpty(methodInfo))
                    {
                        method = typeof(string).GetMethod(methodInfo);
                    }
                    //转string类型
                    MethodInfo strings = typeof(object).GetMethod("ToString", new Type[] { });

                    string[] attrColumn = null;
                    MemberExpression left = null;

                    //构造右表达式  用ConstantExpression表达式表示具有常量值的表达式
                    ConstantExpression right1 = null;
                    ConstantExpression right2 = null;

                    Expression where2 = null;
                    PropertyInfo propertys = typeof(T).GetProperty(columnName);

                    ConstanRights<T>(columnName, value, methodInfo, ref right1, ref right2);

                    if (columnName.Contains("/"))
                    {
                        attrColumn = columnName.Split('/');
                        //获取字段(多字段)
                        for (int i = 0; i < attrColumn.Length; i++)
                        {
                            PropertyInfo property = typeof(T).GetProperty(attrColumn[i]);

                            left = Expression.Property(param, property);
                            Expression left1 = Expression.Call(left, strings);

                            //构造右表达式  用ConstantExpression表达式表示具有常量值的表达式
                            right2 = Expression.Constant(value);
                            if (methodInfo == "Contains")       //模糊查询
                            {
                                if (i == 0)
                                {
                                    where2 = Expression.Call(left1, method, right2);
                                }
                                else
                                {
                                    Expression filterTmp = Expression.Call(left1, method, right2);
                                    where2 = Expression.Or(filterTmp, where2);
                                }
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    //进行合并：例如:employeeid==登录员工ID
                                    where2 = Expression.Equal(left1, right2);
                                }
                                else
                                {
                                    //进行合并：例如:employeeid==登录员工ID
                                    Expression filterTmp = Expression.Equal(left1, right2);
                                    where2 = Expression.Or(filterTmp, where2);
                                }
                            }
                        }
                    }
                    else
                    {
                        left = Expression.Property(param, columnName);
                        if (methodInfo == "Contains")       //模糊查询
                        {
                            Expression left1 = Expression.Call(left, strings);
                            if (value.Contains(","))            //多选
                            {
                                where2 = Expression.Call(right2, method, left1);
                            }
                            else                        //单个模糊查询
                            {
                                where2 = Expression.Call(left1, method, right2);
                            }
                        }
                        else if (methodInfo == "equals")       //多个数据查询
                        {
                            Expression left1 = Expression.Call(left, strings);
                            if (value.Contains(","))            //多选
                            {
                                method = typeof(string).GetMethod("Contains");
                                where2 = Expression.Call(right2, method, left1);

                            }
                            else                        //单个查询
                            {
                                where2 = Expression.Equal(left, right2);
                            }
                        }
                        else if (methodInfo == "DateTime")
                        {
                            if (right2 == null && right1 != null)
                            {
                                where2 = Expression.GreaterThanOrEqual(left, right1);
                            }
                            else if (right2 != null && right1 == null)
                            {
                                where2 = Expression.LessThanOrEqual(left, right2);
                            }
                            else if (right2 != null && right1 != null)
                            {
                                where2 = Expression.GreaterThanOrEqual(left, right1);
                                Expression filterTmp = Expression.LessThanOrEqual(left, right2);
                                where2 = Expression.And(filterTmp, where2);
                            }
                        }
                        else
                        {
                            where2 = Expression.Equal(left, right2);

                        }
                    }

                    Expression<Func<T, bool>> pras = Expression.Lambda<Func<T, bool>>(where2, param);
                    parmList.Add(pras);
                }
            }
            catch (Exception e)
            {
                new Exception(e.Message);
            }

        }
        #endregion


        #region 组装右边的值 常量表达式
        /// <summary>
        /// 右边的值 并转成相应的数据类型
        /// </summary>
        public static void ConstanRights<T>(string columnName, string value, string methodInfo, ref ConstantExpression right1, ref ConstantExpression right2)
        {
            bool isGenericType = false;
            PropertyInfo propertys = typeof(T).GetProperty(columnName);

            if (propertys != null)
            {
                //搜索PropertyType是否可为空
                isGenericType = propertys.PropertyType.IsGenericType && propertys.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

                if (columnName.ToUpper() == propertys.Name.ToUpper())
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (isGenericType)
                        {
                            if (value.Contains(","))
                            {
                                if (methodInfo == "Contains" || methodInfo == "equals")
                                {
                                    right2 = Expression.Constant(Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(string))), typeof(string));  //如果可为空类型，则将value的类型设置为可为空类型
                                }
                                else
                                {
                                    string[] strValue = value.Split(',');
                                    if (!string.IsNullOrEmpty(strValue[0]))
                                    {
                                        right1 = Expression.Constant(Convert.ChangeType(strValue[0], Nullable.GetUnderlyingType(propertys.PropertyType)), propertys.PropertyType);  //如果可为空类型，则将value的类型设置为可为空类型
                                    }
                                    if (!string.IsNullOrEmpty(strValue[1]))
                                    {
                                        right2 = Expression.Constant(Convert.ChangeType(strValue[1], Nullable.GetUnderlyingType(propertys.PropertyType)), propertys.PropertyType);  //如果可为空类型，则将value的类型设置为可为空类型
                                    }
                                }
                            }
                            else
                            {
                                right2 = Expression.Constant(Convert.ChangeType(value, Nullable.GetUnderlyingType(propertys.PropertyType)), propertys.PropertyType);  //如果可为空类型，则将value的类型设置为可为空类型
                            }
                        }
                        else
                        {
                            if (value.Contains(","))
                            {
                                if (methodInfo == "Contains" || methodInfo == "equals")
                                {
                                    right2 = Expression.Constant(Convert.ChangeType(value, typeof(string)));
                                }
                                else
                                {
                                    string[] strValue = value.Split(',');
                                    if (!string.IsNullOrEmpty(strValue[0]))
                                    {
                                        right1 = Expression.Constant(Convert.ChangeType(strValue[0], propertys.PropertyType));
                                    }
                                    if (!string.IsNullOrEmpty(strValue[1]))
                                    {
                                        right2 = Expression.Constant(Convert.ChangeType(strValue[1], propertys.PropertyType));
                                    }
                                }

                            }
                            else
                            {
                                right2 = Expression.Constant(Convert.ChangeType(value, propertys.PropertyType));
                            }
                        }
                    }

                    //多字段查询
                    //if (columnName.Contains("/") && isGenericType)
                    //{
                    //    attrColumn = columnName.Split('/');
                    //    for (int i = 0; i < attrColumn.Length; i++)
                    //    {
                    //        var singleColumn = attrColumn[i];

                    //    }
                    //    if (isGenericType)
                    //    {

                    //    }
                    //    else
                    //    {
                    //    }
                    //}
                }
            }
            else
            {
                if (columnName.Contains("/"))
                {
                    int i = 0;
                    List<string> columnList = columnName.Split('/').ToList();
                    foreach (var column in columnList)
                    {
                        PropertyInfo property = typeof(T).GetProperty(column);  //字段属性
                        //MemberExpression left = Expression.Property(param, property);       //主题表达式
                        //Expression left1 = Expression.Call(left, strings);
                        //if (i == 0)
                        //{
                        //    where2 = Expression.Call(left1, method, right2);
                        //}
                        //else
                        //{
                        //    Expression where = Expression.Call(left1, method, right2);
                        //    where2 = Expression.Or(where2, where);
                        //}
                        i++;
                    }
                }
            }
        }
        #endregion




    }

    /// <summary>
    /// 封装条件
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ExpressionSearch<TEntity> where TEntity : class
    {
        /// <summary>
        /// 封装条件
        /// </summary>
        /// <param name="searchs"></param>
        /// <param name="parmList"></param>
        public void GetSearch(List<PropModel> searchs, List<Expression<Func<TEntity, bool>>> parmList)
        {
            if (searchs.Count() > 0)
            {
                //排除可能出生的异常数据
                searchs = searchs.Where(c => !string.IsNullOrEmpty(c.value) && c.value != "," && c.value != "-1").ToList();

                foreach (PropModel item in searchs)
                {
                    ExpressionTools.GetEqualPars(item.property, parmList, item.value, item.method);
                }
            }
        }
    }

    public partial class PropModel
    {
        /// <summary>
        /// 字段名称属性
        /// </summary>
        public string property { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// lamada符号(运算符)
        /// </summary>
        public string method { get; set; }
    }
}
