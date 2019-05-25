using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ayeonbbsbackend.ModelDbContext;
using ayeonbbsbackend.Models;

namespace ayeonbbsbackend.Services
{
    public class SecondCategoryService
    {
        private readonly DataContext _dataContext;

        public SecondCategoryService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// 获取小分类
        /// </summary>
        /// <returns></returns>
        public object GetSecondCategories(SecondCategory secondCategory, int currentPage = 1, int pageSize = 10)
        {
            try
            {
                var query = (from sc in _dataContext.SecondCategories
                              select new
                              {
                                  sc.SecondCategoryId,
                                  sc.SecondCategoryName,
                                  sc.Status,
                                  sc.CreateDate,
                                  FirstCategory = _dataContext.FirstCategories.FirstOrDefault(x =>
                                      x.FirstCategoryId == sc.FirstCategory.FirstCategoryId),
                                  PostCount = _dataContext.PostInfos.Count(x =>
                                      x.SecondCategory.SecondCategoryId == sc.SecondCategoryId)
                              })
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize);
                if (secondCategory != null)
                {
                    if (secondCategory.Status > 0)
                    {
                        query = query.Where(sc => sc.Status == secondCategory.Status);
                    }
                    if (!string.IsNullOrEmpty(secondCategory.SecondCategoryName))
                    {
                        query = query.Where(sc => sc.SecondCategoryName.Contains(secondCategory.SecondCategoryName));
                    }
                }
                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取小分类
        /// </summary>
        /// <param name="secondCategory"></param>
        /// <returns></returns>
        public IEnumerable<SecondCategory> GetSecondCategories(SecondCategory secondCategory)
        {
            try
            {
                var result = from sc in _dataContext.SecondCategories
                             join fc in _dataContext.FirstCategories on sc.FirstCategory.FirstCategoryId equals fc.FirstCategoryId
                             select new SecondCategory
                             {
                                 SecondCategoryId = sc.SecondCategoryId,
                                 SecondCategoryName = sc.SecondCategoryName,
                                 FirstCategory = fc
                             };
                if (secondCategory?.FirstCategory != null && secondCategory.FirstCategory.FirstCategoryId > 0)
                {
                    result = result.Where(sc => sc.FirstCategory.FirstCategoryId == secondCategory.FirstCategory.FirstCategoryId);
                }

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<SecondCategory> GetSecondCategories()
        {
            try
            {
                return _dataContext.SecondCategories
                    .Where(sc => sc.Status == 1).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 添加二级分类
        /// </summary>
        /// <param name="secondCategory"></param>
        /// <returns></returns>
        public int AddSecondCategory(SecondCategory secondCategory)
        {
            try
            {
                _dataContext.Entry(secondCategory.FirstCategory).State = EntityState.Unchanged; //状态变更为 不更新
                _dataContext.SecondCategories.Update(secondCategory);
                return _dataContext.SaveChanges() > 0 ? secondCategory.SecondCategoryId : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 二级分类变更
        /// </summary>
        /// <param name="secondCategory"></param>
        /// <returns></returns>
        public bool ChangeSecondCategory(SecondCategory secondCategory)
        {
            try
            {
                if (secondCategory == null || secondCategory.SecondCategoryId <= 0 
                    || secondCategory.FirstCategory == null || secondCategory.FirstCategory.FirstCategoryId <=0)
                    return false;

                var sc = _dataContext.SecondCategories.FirstOrDefault(f => f.SecondCategoryId == secondCategory.SecondCategoryId);
                if (sc == null)
                    return false;
                sc.SecondCategoryName = secondCategory.SecondCategoryName; // 更新名称
                sc.FirstCategory = new FirstCategory
                {
                    FirstCategoryId = secondCategory.FirstCategory.FirstCategoryId
                };
                sc.UpdateDate = DateTime.Now; // 更新时间
                _dataContext.Entry(sc.FirstCategory).State = EntityState.Unchanged; // 大分类不修改
                _dataContext.Update(sc);
                return _dataContext.SaveChanges() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取二级分类总数
        /// </summary>
        /// <param name="secondCategory"></param>
        /// <returns></returns>
        public int GetTotalCountOfSecondCategories(SecondCategory secondCategory)
        {
            try
            {
                var query = _dataContext.SecondCategories.Where(sc => 1 == 1);
                if (secondCategory != null)
                {
                    if (secondCategory.Status > 0)
                    {
                        query = query.Where(sc => sc.Status == secondCategory.Status);
                    }
                    if (!string.IsNullOrEmpty(secondCategory.SecondCategoryName))
                    {
                        query = query.Where(sc => sc.SecondCategoryName.Contains(secondCategory.SecondCategoryName));
                    }
                }
                return query.Count();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改二级分类状态
        /// </summary>
        /// <returns></returns>
        public bool ChangeSecondCategoryStatus(SecondCategory secondCategory)
        {
            try
            {
                if (secondCategory == null || secondCategory.SecondCategoryId <= 0)
                    return false;
                var sc = _dataContext.SecondCategories.FirstOrDefault(s => s.SecondCategoryId == secondCategory.SecondCategoryId);
                if (sc == null)
                    return false;
                sc.Status = secondCategory.Status;
                sc.UpdateDate = DateTime.Now;
                _dataContext.Update(sc);
                return _dataContext.SaveChanges() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
    }
}