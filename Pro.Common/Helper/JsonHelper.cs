using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IMCOA.Service
{
    public class JsonHelper
    {
        public static T Deserialize<T>(string jsonStr)
        {
            try
            {
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                return serializer.Deserialize<T>(jsonStr);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static string Serialize<T>(T obj)
        {
            try
            {
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                return serializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 赋值对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopyByReflect<T>(T obj)
        {
            //如果是字符串或值类型则直接返回
            if (obj is string || obj.GetType().IsValueType) return obj;
            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopyByReflect(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }

    }
}
