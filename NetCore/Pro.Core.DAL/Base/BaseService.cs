using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Pro.Core.DAL
{
    public class BaseService<TEntity> where TEntity : class, new()
    {
        public SqlSugarClient dbContext;

        public BaseService()
        {
            dbContext = DbScoped.Sugar;
        }

        #region 获取所有数据  (增、删、改、查)

        #region 查询
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public List<TEntity> GetAllList()
        {
            return dbContext.Queryable<TEntity>().ToList();
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return dbContext.Queryable<TEntity>().Where(predicate).ToList();
        }

        /// <summary>
        /// 根据条件分页查询
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<TEntity> GetAllList(int pageIndex, int pageSize, ref int Total, Expression<Func<TEntity, bool>> predicate)
        {
            return dbContext.Queryable<TEntity>().Where(predicate).ToPageList(pageIndex, pageSize, ref Total);
        }
        #endregion

        #region 添加
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="T"></param>
        public TEntity Insert(TEntity T)
        {
            int i = dbContext.Insertable(T).ExecuteCommand();
            if (i > 0)
            {
                return T;
            }
            return null;
        }


        public bool Insert(string strSql, SugarParameter[] parameters = null)
        {
            return dbContext.Ado.ExecuteCommand(strSql, parameters) > 0;
        }
        #endregion

        #region 更新
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="columns">更新列</param>
        /// <param name="predicate">条件</param>
        public void Update(Expression<Func<TEntity, bool>> columns, Expression<Func<TEntity, bool>> predicate)
        {
            dbContext.Updateable<TEntity>().SetColumns(columns).Where(predicate).ExecuteCommand();
        }

        public void Update(TEntity T)
        {
            dbContext.Updateable(T).ExecuteCommand();
        }

        public void Update(List<TEntity> list)
        {
            dbContext.Updateable(list).ExecuteCommand();
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除数据 根据主键ID
        /// </summary>
        /// <param name="ID"></param>
        public void Delete(object ID)
        {
            dbContext.Deleteable<TEntity>().In(ID).ExecuteCommand();
        }

        /// <summary>
        /// 删除数据 根据条件
        /// </summary>
        /// <param name="ID"></param>
        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            dbContext.Deleteable<TEntity>().Where(predicate).ExecuteCommand();
        }

        /// <summary>
        /// 删除数据 根据实体
        /// </summary>
        /// <param name="ID"></param>
        public void Delete(List<TEntity> list)
        {
            dbContext.Deleteable<TEntity>().Where(list).ExecuteCommand();
        }

        /// <summary>
        /// 删除数据 根据list集合
        /// </summary>
        /// <param name="ID"></param>
        public void Delete(TEntity T)
        {
            dbContext.Deleteable<TEntity>().Where(T).ExecuteCommand();
        }
        #endregion

        #endregion


    }
}
