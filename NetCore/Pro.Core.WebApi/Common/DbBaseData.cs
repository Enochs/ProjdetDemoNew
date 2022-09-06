using Microsoft.Extensions.Configuration;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pro.Core.WebApi.Common
{
    public class DbBaseData
    {
        #region 数据库连接配置
        public SqlSugarClient CreateDbConnection()
        {
            //获取链接字符串
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var config = builder.Build();
            string connString = config.GetConnectionString("DefaultConnection");
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connString,//连接符字串
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true //不设成true要手动close
            });

            return db;
        }
        #endregion


    }

}
