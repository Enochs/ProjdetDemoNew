using Pro.Core.DAL.Model;
using Pro.Core.DAL;
using System;
using System.Collections.Generic;

namespace Pro.Core.BLL
{
    public class StudentBLL
    {
        StudentService stuSvc = new StudentService();
        public List<Student> Getlist()
        {
            var list=stuSvc.Getlist();
            return list;
            //var dbConn = new DbBase().CreateDbConnection();
            //var data = dbConn.Queryable<Student>().ToList();
            //return data;
        }
    }
}
