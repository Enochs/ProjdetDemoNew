using Pro.Core.DAL.Model;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pro.Core.DAL
{
    public class StudentService
    {
        public List<Student> Getlist()
        {
            using (var db = DbScoped.Sugar)
            {
                var list = db.Queryable<Student>().ToList();
                return list;
            }
        }
    }
}
