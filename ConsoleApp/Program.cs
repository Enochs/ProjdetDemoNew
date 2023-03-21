using Newtonsoft.Json;
using Pro.Model;
using Pro.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //IsRunYear(1896);
            //new_MaoPao();
            //Console.WriteLine(Foo(30));

            //ExpressionTest();
            //ExpressionTest1();
            //Test();
            //ExpressionTest3();
            //ExpressionTest4();

            decimal price = 3.14000m;
            Console.WriteLine(price.ToString("0.#####"));
            // output: 3.14

            var newPrice = Convert.ToDecimal(price.ToString("0.#####"));
            Console.WriteLine(newPrice);
           Console.Read();

            Console.ReadKey();
        }



        #region 冒泡排序法
        /// <summary>
        /// @author:wp
        /// @datetime:2020-07-22
        /// @desc:冒泡排序法
        /// </summary>
        public static void MaoPao()
        {
            var nums = new int[] { 25, 18, 98, 75, 189, 36, 9, 166, 24 };
            List<int> new_num = new List<int>();
            var o_length = nums.Length;
            var temp = 0;
            for (int i = 1; i < o_length; i++)
            {
                for (int j = 0; j < o_length - i; j++)
                {
                    if (nums[j] > nums[j + 1])
                    {
                        temp = nums[j + 1];
                        nums[j + 1] = nums[j];
                        nums[j] = temp;
                    }
                }
            }
            Console.WriteLine("开始");
            foreach (var item in nums)
            {
                Console.Write(item + ",");
            }
            Console.WriteLine("结束");
        }
        #endregion

        #region 计算第三十位数
        /// <summary>
        /// 计算第三十位数
        /// </summary>
        public static int Foo(int i)
        {
            var num = 0;
            if (i == 0)
            {
                num = 0;
            }
            else if (i <= 2)
            {
                num = 1;
            }
            else
            {
                num = Foo(i - 1) + Foo(i - 2);
            }
            return num;
        }
        #endregion

        #region 计算最大数
        /// <summary>
        /// 计算最大数
        /// </summary>
        /// <returns></returns>
        public static int MaxNum()
        {
            int max = 0;
            int x = 12, y = 10, z = 15;
            if (x > y)
            {
                max = x;
            }
            else
            {
                max = y;
            }
            if (z > max)
            {
                max = z;
            }
            return max;
        }

        #endregion

        #region 猜数字
        /// <summary>
        /// 猜数字 1-10
        /// </summary>
        public static void IsNum()
        {
            var randon = new Random();
            var text = randon.Next(1, 10);
            var num = Convert.ToInt32(Console.ReadLine());

            while (num != text)
            {
                Console.WriteLine("不好意思 你猜错了 请继续猜");
                num = Convert.ToInt32(Console.ReadLine());
                continue;
            }

            if (num == text)
            {
                Console.WriteLine("恭喜你,猜对了");
            }
        }
        #endregion

        #region 冒泡排序
        /// <summary>
        /// 冒泡排序
        /// </summary>
        public static void new_MaoPao()
        {
            int[] nums = { 12, 15, 8, 165, 78, 95, 31 };

            var n_length = nums.Length;

            for (int i = 0; i < n_length; i++)
            {
                for (int j = 0; j < n_length - i - 1; j++)
                {
                    if (nums[j] > nums[j + 1])
                    {
                        int temp = nums[j + 1];
                        nums[j + 1] = nums[j];
                        nums[j] = temp;
                    }
                }
            }
            foreach (var num in nums)
            {
                Console.Write(num + ",");
            }
        }
        #endregion

        #region 是否是闰年
        /// <summary>
        /// 是否是闰年
        /// </summary>
        /// <param name="year"></param>
        public static void IsRunYear(int year = 1900)
        {
            var isRun = false;
            if (year % 4 == 0 && year % 100 != 0)
            {
                isRun = true;
            }
            else if (year % 400 == 0)
            {
                isRun = true;
            }
            if (isRun)
            {
                Console.WriteLine($"{year}是润年");
            }
            else
            {
                Console.WriteLine($"{year}不是闰年");
            }
        }
        #endregion

        #region 奇偶数加减
        /// <summary>
        /// 1-2+3-4+5
        /// </summary>
        /// <param name="num"></param>
        public static void Plus(int num)
        {
            int result = 0;
            for (int i = 0; i < num + 1; i++)
            {
                if (i % 2 == 0)
                {
                    result = result - i;
                }
                else
                {
                    result += i;
                }
            }
            Console.Write(result);
        }
        #endregion

        #region 树表达式  模糊查询
        /// <summary>
        /// 树表达式
        /// </summary>
        public static void ExpressionTest()
        {
            var stuQuery = InitData().AsQueryable();

            PropModel prop = new PropModel()
            {
                Name = "s_name",
                Method = "Contains",
                Value = "张"
            };

            //expression表达式树主体构造开始
            ParameterExpression param = Expression.Parameter(typeof(Student), "c");     //变量, 参数表达式 c=>c
            PropertyInfo property = typeof(Student).GetProperty(prop.Name);     //字段
            MethodInfo method = null;
            if (!string.IsNullOrEmpty(prop.Method))
            {
                method = typeof(string).GetMethod(prop.Method);
            }

            //转string类型
            MethodInfo strings = typeof(object).GetMethod("ToString", new Type[] { });
            //左表达式  主体表达式
            MemberExpression left = Expression.Property(param, property);
            Expression left1 = null;
            if (property != null && property.PropertyType.Name == "String")
            {
                left1 = Expression.Call(left, strings);
            }

            //构造右表达式  用ConstantExpression表达式表示具有常量值的表达式
            ConstantExpression right2 = Expression.Constant(prop.Value);

            var where2 = Expression.Call(left1 ?? left, method, right2);
            Expression<Func<Student, bool>> pras = Expression.Lambda<Func<Student, bool>>(where2, param);

            List<Student> list = new List<Student>();

            list = stuQuery.Where(pras).ToList(); ;

            //list = list.Where(c => c.s_name.Contains("张")).ToList();

            foreach (var item in list)
            {
                Console.WriteLine($"模糊查询 姓名：{item.s_name},地址：{item.s_address}");
            }

        }
        #endregion

        #region 树表达式  等于
        /// <summary>
        /// 树表达式
        /// </summary>
        public static void ExpressionTest1()
        {
            var stuQuery = InitData().AsQueryable();

            PropModel prop = new PropModel()
            {
                Name = "s_name",
                Method = "Equal",
                Value = "李四"
            };

            ParameterExpression param = Expression.Parameter(typeof(Student), "c");
            PropertyInfo property = typeof(Student).GetProperty(prop.Name);     //字段

            //转string类型
            MethodInfo strings = typeof(object).GetMethod("ToString", new Type[] { });

            //左表达式  主体表达式
            MemberExpression left = Expression.Property(param, property);
            Expression left1 = null;
            if (property != null && property.PropertyType.Name == "String")
            {
                left1 = Expression.Call(left, strings);
            }

            //构造右表达式  用ConstantExpression表达式表示具有常量值的表达式
            var right2 = Expression.Constant(prop.Value);

            var where2 = Expression.Equal(left1 ?? left, right2);
            Expression<Func<Student, bool>> pras = Expression.Lambda<Func<Student, bool>>(where2, param);

            List<Student> list = new List<Student>();
            list = stuQuery.Where(pras).ToList(); ;

            //list = list.Where(c => c.s_name == "张三")).ToList();

            foreach (var item in list)
            {
                Console.WriteLine($"精确查找 姓名：{item.s_name},地址：{item.s_address}");
            }

        }
        #endregion

        #region 树表达式  模糊查询 多个值
        /// <summary>
        /// 树表达式
        /// </summary>
        public static void ExpressionTest3()
        {
            var stuQuery = InitData().AsQueryable();

            PropModel prop = new PropModel()
            {
                Name = "s_name",
                Method = "Contains",
                Value = "张三,李四"
            };

            ParameterExpression param = Expression.Parameter(typeof(Student), "c");
            PropertyInfo property = typeof(Student).GetProperty(prop.Name);
            MethodInfo method = null;
            if (!string.IsNullOrEmpty(prop.Method))
            {
                method = typeof(string).GetMethod(prop.Method);
            }

            //转string类型
            MethodInfo strings = typeof(object).GetMethod("ToString", new Type[] { });

            //左表达式  主体表达式
            MemberExpression left = Expression.Property(param, property);
            Expression left1 = null;
            if (property != null && property.PropertyType.Name == "String")
            {
                left1 = Expression.Call(left, strings);
            }

            //构造右表达式  用ConstantExpression表达式表示具有常量值的表达式
            string value = prop.Value;
            var right2 = Expression.Constant(value);

            var where2 = Expression.Call(right2, method, left1 ?? left);

            Expression<Func<Student, bool>> pras = Expression.Lambda<Func<Student, bool>>(where2, param); //使用Expression动态创建lambda表达式

            List<Student> list = stuQuery.Where(pras).ToList(); ;

            foreach (var item in list)
            {
                Console.WriteLine($"多个值 姓名：{item.s_name},地址：{item.s_address}");
            }

        }
        #endregion

        #region 树表达式  模糊查询 多列
        /// <summary>
        /// 树表达式
        /// </summary>
        public static void ExpressionTest4()
        {
            var stuQuery = InitData().AsQueryable();

            PropModel prop = new PropModel()
            {
                Name = "s_name/s_loginName/s_address",
                Method = "Contains",
                Value = "重"
            };

            ParameterExpression param = Expression.Parameter(typeof(Student), "c");
            PropertyInfo property = typeof(Student).GetProperty(prop.Name);
            MethodInfo method = null;
            if (!string.IsNullOrEmpty(prop.Method))
            {
                method = typeof(string).GetMethod(prop.Method);
            }

            //转string类型
            MethodInfo strings = typeof(object).GetMethod("ToString", new Type[] { });

            //构造右表达式  用ConstantExpression表达式表示具有常量值的表达式
            string value = prop.Value;
            var right2 = Expression.Constant(value);
            Expression where2 = null;

            if (property == null && prop.Name.Contains("/"))
            {
                int i = 0;
                List<string> columnList = prop.Name.Split('/').ToList();
                foreach (var column in columnList)
                {
                    property = typeof(Student).GetProperty(column);  //字段属性
                    MemberExpression left = Expression.Property(param, property);       //主题表达式
                    Expression left1 = Expression.Call(left, strings);
                    if (i == 0)
                    {
                        where2 = Expression.Call(left1, method, right2);
                    }
                    else
                    {
                        Expression where = Expression.Call(left1, method, right2);
                        where2 = Expression.Or(where2, where);
                    }
                    i++;
                }
            }



            Expression<Func<Student, bool>> pras = Expression.Lambda<Func<Student, bool>>(where2, param); //使用Expression动态创建lambda表达式

            List<Student> list = stuQuery.Where(pras).ToList(); ;

            foreach (var item in list)
            {
                Console.WriteLine($"多列查询 姓名：{item.s_name},地址：{item.s_address}");
            }

        }
        #endregion

        #region 树表达式  范围
        /// <summary>
        /// 树表达式
        /// </summary>
        public static void ExpressionTest2()
        {
            var stuQuery = InitData().AsQueryable();

            PropModel prop = new PropModel()
            {
                Name = "s_age",
                Method = "Range",
                Value = "19,"
            };

            ParameterExpression param = Expression.Parameter(typeof(Student), "c");
            PropertyInfo property = typeof(Student).GetProperty(prop.Name);     //字段
            MethodInfo method = null;
            if (!string.IsNullOrEmpty(prop.Method))
            {
                method = typeof(string).GetMethod(prop.Method);     //符号
            }

            //转string类型
            MethodInfo strings = typeof(object).GetMethod("ToString", new Type[] { });

            //左表达式  主体表达式
            MemberExpression left = Expression.Property(param, property);
            Expression left1 = null;
            if (property != null && property.PropertyType.Name == "String")
            {
                left1 = Expression.Call(left, strings);
            }

            //构造右表达式  用ConstantExpression表达式表示具有常量值的表达式
            ConstantExpression right1 = null;
            ConstantExpression right2 = null;

            string[] strValue = prop.Value.Split(',');
            if (!string.IsNullOrEmpty(strValue[0]))
            {
                right1 = Expression.Constant(Convert.ChangeType(strValue[0], Nullable.GetUnderlyingType(property.PropertyType)), property.PropertyType);  //如果可为空类型，则将value的类型设置为可为空类型
            }
            if (!string.IsNullOrEmpty(strValue[1]))
            {
                right2 = Expression.Constant(Convert.ChangeType(strValue[1], Nullable.GetUnderlyingType(property.PropertyType)), property.PropertyType);  //如果可为空类型，则将value的类型设置为可为空类型
            }

            Expression where2 = null;
            if (right1 != null && right2 == null)
            {
                where2 = Expression.GreaterThanOrEqual(left1 ?? left, right1);
            }
            else if (right1 == null && right2 != null)
            {
                where2 = Expression.GreaterThanOrEqual(left1 ?? left, right2);
            }
            else if (right1 != null && right2 != null)
            {
                where2 = Expression.GreaterThanOrEqual(left1 ?? left, right1);
                Expression filterTmp = Expression.LessThanOrEqual(left1 ?? left, right2);
                where2 = Expression.And(where2, filterTmp);
            }

            Expression<Func<Student, bool>> pras = Expression.Lambda<Func<Student, bool>>(where2, param);

            var list = stuQuery.Where(pras).ToList(); ;

            foreach (var item in list)
            {
                Console.WriteLine($"范围查找 姓名：{item.s_name},地址：{item.s_address},年龄：{item.s_age}");
            }

        }
        #endregion

        public static List<Student> InitData()
        {
            var stuList = new List<Student>()
            {
                new Student() { s_id = Guid.NewGuid(), s_name = "张三",s_loginName="zhangsan", s_address = "重庆" ,s_age=18 },
                new Student() { s_id = Guid.NewGuid(), s_name = "李四",s_loginName="lisi", s_address = "天津" ,s_age=18 },
                new Student() { s_id = Guid.NewGuid(), s_name = "赵五",s_loginName="wangwu", s_address = "北京" ,s_age=22 },
                new Student() { s_id = Guid.NewGuid(), s_name = "王六",s_loginName="zhaoliu", s_address = "上海" ,s_age=19 },
                new Student() { s_id = Guid.NewGuid(), s_name = "李宁",s_loginName="lining", s_address = "重庆" ,s_age=18 },
                new Student() { s_id = Guid.NewGuid(), s_name = "萧峰",s_loginName="xiaofeng", s_address = "南京" ,s_age=26 },
                new Student() { s_id = Guid.NewGuid(), s_name = "姚明",s_loginName="yaoming", s_address = "重庆" ,s_age=33 },
                new Student() { s_id = Guid.NewGuid(), s_name = "小强",s_loginName="xiaoqiang", s_address = "天津" ,s_age=27 },
            };
            return stuList;
        }
    }


    #region 表达式
    /// <summary>
    /// 构建表达式所需模型
    /// </summary>
    public class PropModel
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 符号(等于 大于 小于)
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
    #endregion
}
