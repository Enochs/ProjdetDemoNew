using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pro.Core.BLL;
using Pro.Core.DAL;
using Pro.Core.DAL.Model;
using Pro.Core.WebApi.Common;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pro.Core.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        //    StudentBLL stuBll = null;
        //    public StudentController(StudentBLL _stuBll)
        //    {
        //        this.stuBll = _stuBll;
        //    }


        [HttpGet("GetList")]
        public List<Student> GetList()
        {
            StudentBLL stuBll = new StudentBLL();
            var list = stuBll.Getlist();
            return list;
            // var dbConn = new DbBase().CreateDbConnection();
            ////var data= dbConn.Queryable<Student>().ToList();
            // using (var db = DbScoped.Sugar)
            // {
            //     var list = db.Queryable<Student>().ToList();
            //     return list;
            // }
        }
    }
}
