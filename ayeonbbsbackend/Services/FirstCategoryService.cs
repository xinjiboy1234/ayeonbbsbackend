using ayeonbbsbackend.ModelDbContext;
using ayeonbbsbackend.Models;
using ayeonbbsbackend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ayeonbbsbackend.Services
{
    public class FirstCategoryService
    {
        private readonly DataContext _dataContext;

        public FirstCategoryService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// 获取全部使用中的大分类
        /// </summary>
        /// <returns></returns>
        public List<FirstCategory> GetFirstCategories()
        {
            try
            {
                return _dataContext.FirstCategories.Where(fc => fc.Status == 1).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取大分类
        /// </summary>
        /// <param name="firstCategory"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object GetFirstCategories(FirstCategory firstCategory, int currentPage = 1, int pageSize = 10)
        {
            try
            {
                var query = (from fc in _dataContext.FirstCategories
                              select new FirstCategoryViewModel
                              {
                                  FirstCategoryId= fc.FirstCategoryId,
                                  FirstCategoryName=fc.FirstCategoryName,
                                  Status=fc.Status,
                                  CreateDate=fc.CreateDate,
                                  UpdateDate=fc.UpdateDate,
                                  SecondCategoryCount = _dataContext.SecondCategories.Count(x =>
                                      x.FirstCategory.FirstCategoryId == fc.FirstCategoryId),
                                  PostCount = _dataContext.PostInfos.Count(p => p.SecondCategory.FirstCategory.FirstCategoryId == fc.FirstCategoryId)
                              }).Skip((currentPage - 1) * pageSize).Take(pageSize);
                if (firstCategory != null)
                {
                    if (firstCategory.Status > 0)
                    {
                        query = query.Where(fc => fc.Status == firstCategory.Status);
                    }
                    if (!string.IsNullOrEmpty(firstCategory.FirstCategoryName))
                    {
                        query = query.Where(fc => fc.FirstCategoryName.Contains(firstCategory.FirstCategoryName));
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
        /// 根据ID获取大分类
        /// </summary>
        /// <param name="firstCategory"></param>
        /// <returns></returns>
        public FirstCategory GetFirstCategory(FirstCategory firstCategory)
        {
            try
            {
                return _dataContext.FirstCategories
                    .FirstOrDefault(fc => fc.FirstCategoryId == firstCategory.FirstCategoryId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 添加大分类
        /// </summary>
        /// <param name="firstCategory"></param>
        /// <returns></returns>
        public int AddFirstCategory(FirstCategory firstCategory)
        {
            try
            {
                _dataContext.Add(firstCategory);
                return _dataContext.SaveChanges() > 0 ? firstCategory.FirstCategoryId : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新大分类
        /// </summary>
        /// <param name="firstCategory"></param>
        /// <returns></returns>
        public bool ChangeFirstCategory(FirstCategory firstCategory)
        {
            try
            {
                if (firstCategory == null || firstCategory.FirstCategoryId <= 0)
                    return false;

                var fc = _dataContext.FirstCategories.FirstOrDefault(f => f.FirstCategoryId == firstCategory.FirstCategoryId);
                if (fc == null)
                    return false;
                fc.FirstCategoryName = firstCategory.FirstCategoryName; // 更新名称
                fc.UpdateDate = DateTime.Now; // 更新时间
                _dataContext.Update(fc);
                return _dataContext.SaveChanges() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取大分类总数
        /// </summary>
        /// <returns></returns>
        public int GetTotalCountOfFristCategories(FirstCategory firstCategory)
        {
            try
            {
                if (firstCategory == null)
                    return 0;
                var query = _dataContext.FirstCategories.Where(x => 1 == 1);
                if (firstCategory.Status > 0)
                {
                    query = query.Where(fc => fc.Status == firstCategory.Status);
                }
                if (!string.IsNullOrEmpty(firstCategory.FirstCategoryName))
                {
                    query = query.Where(fc => fc.FirstCategoryName.Contains(firstCategory.FirstCategoryName));
                }
                return query.Count();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改大分类状态
        /// </summary>
        /// <returns></returns>
        public bool ChangeFirstCategoryStatus(FirstCategory firstCategory)
        {
            try
            {
                if (firstCategory == null || firstCategory.FirstCategoryId <= 0)
                    return false;
                var fc = _dataContext.FirstCategories.FirstOrDefault(f => f.FirstCategoryId == firstCategory.FirstCategoryId);
                if (fc == null)
                    return false;
                fc.Status = firstCategory.Status;
                fc.UpdateDate = DateTime.Now;
                _dataContext.Update(fc);
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