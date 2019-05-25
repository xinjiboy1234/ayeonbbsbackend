using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ayeonbbsbackend.ModelDbContext;
using ayeonbbsbackend.Models;
using ayeonbbsbackend.ViewModels;

namespace ayeonbbsbackend.Services
{
    public class PostService
    {
        private readonly DataContext _dataContext;

        public PostService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public int AddPostInfo(PostInfo postInfo)
        {
            try
            {
                if (postInfo?.Author == null || string.IsNullOrEmpty(postInfo.PostTitle) ||
                    string.IsNullOrEmpty(postInfo.PostContent))
                    return 0;
                _dataContext.Entry(postInfo.Author).State = EntityState.Unchanged; // 不修改用户
                if (postInfo.SecondCategory != null)
                    _dataContext.Entry(postInfo.SecondCategory).State = EntityState.Unchanged; //不修改分类
                _dataContext.PostInfos.Update(postInfo);
                return _dataContext.SaveChanges() > 0 ? postInfo.PostId : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 发布帖子
        /// </summary>
        /// <param name="postInfo"></param>
        /// <returns></returns>
        public int SubmitPostInfo(PostInfo postInfo)
        {
            try
            {
                if (postInfo?.Author == null || string.IsNullOrEmpty(postInfo.PostTitle) ||
                    string.IsNullOrEmpty(postInfo.PostContent) || postInfo.PostId < 0)
                    return 0;
                _dataContext.Entry(postInfo.Author).State = EntityState.Unchanged; // 不修改用户
                if (postInfo.SecondCategory != null)
                    _dataContext.Entry(postInfo.SecondCategory).State = EntityState.Unchanged; //不修改分类
                _dataContext.PostInfos.Update(postInfo);
                return _dataContext.SaveChanges() > 0 ? postInfo.PostId : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取帖子
        /// </summary>
        /// <param name="postInfo"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<PostInfoViewModel> GetPostInfos(PostInfo postInfo, UserInfo userInfo, bool needOrderBy, int currentPage = 1, int pageSize = 10)
        {
            try
            {
                var postInfos = from p in _dataContext.PostInfos
                                join u in _dataContext.UserInfos
                                   on p.Author.UserId equals u.UserId
                                join sc in _dataContext.SecondCategories
                                   on p.SecondCategory.SecondCategoryId equals sc.SecondCategoryId
                                join fc in _dataContext.FirstCategories
                                   on sc.FirstCategory.FirstCategoryId equals fc.FirstCategoryId
                                select new PostInfoViewModel
                                {
                                    PostId = p.PostId,
                                    PostTitle = p.PostTitle,
                                    SecondCategory = new SecondCategory
                                    {
                                        SecondCategoryId = sc.SecondCategoryId,
                                        SecondCategoryName = sc.SecondCategoryName,
                                        Status = sc.Status,
                                        FirstCategory = new FirstCategory
                                        {
                                            FirstCategoryId = fc.FirstCategoryId,
                                            FirstCategoryName = fc.FirstCategoryName,
                                            Status = fc.Status
                                        }
                                    },
                                    Description = p.Description,
                                    CreateDate = p.CreateDate,
                                    UpdateDate = p.UpdateDate,
                                    Status = p.Status,
                                    Author = new UserInfo
                                    {
                                        UserId = u.UserId,
                                        UserName = u.UserName,
                                        NickName = u.NickName
                                    },
                                    ReplyCount = _dataContext.ReplyInfos.Count(rp => rp.PostInfo.PostId == p.PostId),
                                    PostGoodCount = _dataContext.PostGoods.Count(pg => pg.PostInfo.PostId == p.PostId),
                                    Watch = 0,
                                    IsTop = p.IsTop,
                                    PostGood = _dataContext.PostGoods
                                    .Where(x => x.PostInfo.PostId == p.PostId && x.GoodsUser.UserId == userInfo.UserId)
                                    .FirstOrDefault()
                                };
                // 排序
                if (needOrderBy)
                {
                    postInfos = postInfos.OrderBy(p => p.IsTop).ThenByDescending(p => p.PostId).Skip((currentPage - 1) * pageSize).Take(pageSize);
                }
                else
                {
                    postInfos = postInfos.OrderByDescending(p => p.PostId).Skip((currentPage - 1) * pageSize).Take(pageSize);
                }

                if (postInfo != null && postInfo.Status > 0)
                {
                    postInfos = postInfos.Where(p => p.Status == postInfo.Status);
                }

                if (postInfo != null && !string.IsNullOrEmpty(postInfo.PostTitle))
                {
                    postInfos = postInfos.Where(p => p.PostTitle.Contains(postInfo.PostTitle));
                }

                if (postInfo != null && !string.IsNullOrEmpty(postInfo.Description))
                {
                    postInfos = postInfos.Where(p => p.Description.Contains(postInfo.Description));
                }
                //根据二级分类获取
                if (postInfo.SecondCategory?.SecondCategoryId > 0)
                {
                    postInfos = postInfos.Where(p => p.SecondCategory.SecondCategoryId == postInfo.SecondCategory.SecondCategoryId);
                }
                // 根据大分类获取
                if (postInfo.SecondCategory?.FirstCategory?.FirstCategoryId > 0)
                {
                    postInfos = postInfos.Where(p =>
                        p.SecondCategory.FirstCategory.FirstCategoryId ==
                        postInfo.SecondCategory.FirstCategory.FirstCategoryId);
                }

                return postInfos.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取帖子
        /// </summary>
        /// <param name="postInfo"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<PostInfoViewModel> GetPostInfos(PostInfoViewModel postInfoViewModel, UserInfo userInfo, int currentPage = 1, int pageSize = 10)
        {
            try
            {
                var query = (from p in _dataContext.PostInfos
                             join u in _dataContext.UserInfos
                                on p.Author.UserId equals u.UserId
                             join sc in _dataContext.SecondCategories
                                on p.SecondCategory.SecondCategoryId equals sc.SecondCategoryId
                             join fc in _dataContext.FirstCategories
                                on sc.FirstCategory.FirstCategoryId equals fc.FirstCategoryId
                             join pm in _dataContext.PostManagers
                                on sc.SecondCategoryId equals pm.SecondCategory.SecondCategoryId
                             where pm.UserInfo.UserId == userInfo.UserId
                             select new PostInfoViewModel
                             {
                                 PostId = p.PostId,
                                 PostTitle = p.PostTitle,
                                 SecondCategory = new SecondCategory
                                 {
                                     SecondCategoryId = sc.SecondCategoryId,
                                     SecondCategoryName = sc.SecondCategoryName,
                                     Status = sc.Status,
                                     FirstCategory = new FirstCategory
                                     {
                                         FirstCategoryId = fc.FirstCategoryId,
                                         FirstCategoryName = fc.FirstCategoryName,
                                         Status = fc.Status
                                     }
                                 },
                                 Description = p.Description,
                                 CreateDate = p.CreateDate,
                                 UpdateDate = p.UpdateDate,
                                 Status = p.Status,
                                 Author = new UserInfo
                                 {
                                     UserId = u.UserId,
                                     UserName = u.UserName,
                                     NickName = u.NickName
                                 },
                                 ReplyCount = _dataContext.ReplyInfos.Count(rp => rp.PostInfo.PostId == p.PostId),
                                 PostGoodCount = _dataContext.PostGoods.Count(pg => pg.PostInfo.PostId == p.PostId),
                                 Watch = 0,
                                 IsTop = p.IsTop,
                                 PostGood = _dataContext.PostGoods
                                 .Where(x => x.PostInfo.PostId == p.PostId && x.GoodsUser.UserId == userInfo.UserId)
                                 .FirstOrDefault()
                             }).OrderBy(p => p.PostId).Skip((currentPage - 1) * pageSize).Take(pageSize);

                if (postInfoViewModel != null && postInfoViewModel.Status > 0)
                {
                    query = query.Where(p => p.Status == postInfoViewModel.Status);
                }

                if (postInfoViewModel != null && !string.IsNullOrEmpty(postInfoViewModel.PostTitle))
                {
                    query = query.Where(p => p.PostTitle.Contains(postInfoViewModel.PostTitle));
                }

                if (postInfoViewModel != null && !string.IsNullOrEmpty(postInfoViewModel.PostContent))
                {
                    query = query.Where(p => p.PostContent.Contains(postInfoViewModel.PostContent));
                }
                //根据二级分类获取
                if (postInfoViewModel.SecondCategoryIds?.Count > 0)
                {
                    query = query.Where(p => postInfoViewModel.SecondCategoryIds.Contains(p.SecondCategory.SecondCategoryId));
                }

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取帖子总数
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetPostInfosTotalCount(PostInfo postInfo)
        {
            try
            {
                if (postInfo == null)
                {
                    return 0;
                }

                var result = _dataContext.PostInfos.Where(p => 1 == 1);
                //根据二级分类获取
                if (postInfo.SecondCategory?.SecondCategoryId > 0)
                {
                    result = result.Where(p => p.SecondCategory.SecondCategoryId == postInfo.SecondCategory.SecondCategoryId);
                }
                // 根据大分类获取
                if (postInfo.SecondCategory?.FirstCategory?.FirstCategoryId > 0)
                {
                    result = result.Where(p =>
                        p.SecondCategory.FirstCategory.FirstCategoryId ==
                        postInfo.SecondCategory.FirstCategory.FirstCategoryId);
                }
                if (postInfo.Status > 0)
                {
                    result = result.Where(p => p.Status == postInfo.Status);
                }
                return result.Count();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PostInfoViewModel> GetHotPostInfos(UserInfo userInfo, int currentPage = 1, int pageSize = 10)
        {
            try
            {
                var postInfos = (from p in _dataContext.PostInfos
                                 join u in _dataContext.UserInfos
                                    on p.Author.UserId equals u.UserId
                                 join sc in _dataContext.SecondCategories
                                    on p.SecondCategory.SecondCategoryId equals sc.SecondCategoryId
                                 select new PostInfoViewModel
                                 {
                                     PostId = p.PostId,
                                     PostTitle = p.PostTitle.Length > 10 ? p.PostTitle.Substring(0, 10) + " ..." : p.PostTitle,
                                     SecondCategory = new SecondCategory
                                     {
                                         SecondCategoryId = sc.SecondCategoryId,
                                         SecondCategoryName = sc.SecondCategoryName,
                                         Status = sc.Status
                                     },
                                     Description = p.Description,
                                     CreateDate = p.CreateDate,
                                     UpdateDate = p.UpdateDate,
                                     Status = p.Status,
                                     Author = new UserInfo
                                     {
                                         UserId = u.UserId,
                                         UserName = u.UserName,
                                         NickName = u.NickName
                                     },
                                     ReplyCount = _dataContext.ReplyInfos.Count(rp => rp.PostInfo.PostId == p.PostId),
                                     PostGoodCount = _dataContext.PostGoods.Count(pg => pg.PostInfo.PostId == p.PostId),
                                     Watch = p.Watch,
                                     IsTop = p.IsTop,
                                     PostGood = _dataContext.PostGoods
                                    .Where(x => x.PostInfo.PostId == p.PostId && x.GoodsUser.UserId == userInfo.UserId)
                                    .FirstOrDefault()
                                 }).Where(p => p.Status == 1)
                                 .OrderByDescending(p => p.PostGoodCount)
                                 .ThenByDescending(p => p.ReplyCount)
                                 .Skip((currentPage - 1) * pageSize).Take(pageSize);

                return postInfos.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PostInfoViewModel GetPostInfo(PostInfo postInfo)
        {
            try
            {
                if (postInfo.PostId <= 0)
                    return null;

                var result = from p in _dataContext.PostInfos
                             join u in _dataContext.UserInfos
                                on p.Author.UserId equals u.UserId
                             join sc in _dataContext.SecondCategories
                                on p.SecondCategory.SecondCategoryId equals sc.SecondCategoryId
                             where p.PostId == postInfo.PostId && p.Status == postInfo.Status
                             select new PostInfoViewModel
                             {
                                 PostId = p.PostId,
                                 PostTitle = p.PostTitle,
                                 SecondCategory = new SecondCategory
                                 {
                                     SecondCategoryId = sc.SecondCategoryId,
                                     SecondCategoryName = sc.SecondCategoryName,
                                     Status = sc.Status
                                 },
                                 PostContent = p.PostContent,
                                 CreateDate = p.CreateDate,
                                 Author = new UserInfo
                                 {
                                     UserId = u.UserId,
                                     UserName = u.UserName,
                                     NickName = u.NickName
                                 },
                                 ReplyCount = _dataContext.ReplyInfos.Count(rp => rp.PostInfo.PostId == p.PostId),
                                 PostGoodCount = _dataContext.PostGoods.Count(pg => pg.PostInfo.PostId == p.PostId),
                                 Watch = p.Watch
                             };
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        public PostInfoViewModel GetPostInfo(PostInfo postInfo, UserInfo userInfo)
        {
            try
            {
                if (postInfo.PostId <= 0)
                    return null;

                var result = from p in _dataContext.PostInfos
                             join u in _dataContext.UserInfos
                                on p.Author.UserId equals u.UserId
                             join sc in _dataContext.SecondCategories
                                on p.SecondCategory.SecondCategoryId equals sc.SecondCategoryId
                             where p.PostId == postInfo.PostId && p.Status == postInfo.Status
                             && p.Author.UserId == userInfo.UserId
                             select new PostInfoViewModel
                             {
                                 PostId = p.PostId,
                                 PostTitle = p.PostTitle,
                                 SecondCategory = new SecondCategory
                                 {
                                     SecondCategoryId = sc.SecondCategoryId,
                                     SecondCategoryName = sc.SecondCategoryName
                                 },
                                 PostContent = p.PostContent,
                                 CreateDate = p.CreateDate
                             };
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 个人中心查询帖子方法
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<PostInfoViewModel> GetPostsByUserInfo(int userId, bool loginFlag, int currentPage = 1, int pageSize = 10)
        {
            try
            {
                var postInfos = (from p in _dataContext.PostInfos
                                 join u in _dataContext.UserInfos
                                    on p.Author.UserId equals u.UserId
                                 join sc in _dataContext.SecondCategories
                                    on p.SecondCategory.SecondCategoryId equals sc.SecondCategoryId
                                 join fc in _dataContext.FirstCategories
                                    on sc.FirstCategory.FirstCategoryId equals fc.FirstCategoryId
                                 select new PostInfoViewModel
                                 {
                                     PostId = p.PostId,
                                     PostTitle = p.PostTitle,
                                     SecondCategory = new SecondCategory
                                     {
                                         SecondCategoryId = sc.SecondCategoryId,
                                         SecondCategoryName = sc.SecondCategoryName,
                                         Status = sc.Status,
                                         FirstCategory = new FirstCategory
                                         {
                                             FirstCategoryId = fc.FirstCategoryId,
                                             FirstCategoryName = fc.FirstCategoryName,
                                             Status = fc.Status
                                         }
                                     },
                                     Description = p.Description,
                                     CreateDate = p.CreateDate,
                                     UpdateDate = p.UpdateDate,
                                     Status = p.Status,
                                     Author = new UserInfo
                                     {
                                         UserId = u.UserId,
                                         UserName = u.UserName,
                                         NickName = u.NickName
                                     },
                                     ReplyCount = _dataContext.ReplyInfos.Count(rp => rp.PostInfo.PostId == p.PostId),
                                     PostGoodCount = _dataContext.PostGoods.Count(pg => pg.PostInfo.PostId == p.PostId),
                                     Watch = 0,
                                     IsTop = p.IsTop,
                                     PostGood = _dataContext.PostGoods
                                     .Where(x => x.PostInfo.PostId == p.PostId && x.GoodsUser.UserId == userId)
                                     .FirstOrDefault()
                                 });
                if (loginFlag)
                {
                    postInfos = postInfos.Where(p => (p.Status == 1 || p.Status == 2));
                }
                else
                {
                    postInfos = postInfos.Where(p => (p.Status == 1));
                }
                postInfos = postInfos.Where(p => p.Author.UserId == userId)
                .OrderByDescending(p => p.CreateDate)
                .ThenByDescending(p => p.PostGoodCount)
                .ThenByDescending(p => p.ReplyCount)
                .Skip((currentPage - 1) * pageSize).Take(pageSize);
                return postInfos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取帖子总数
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetPostInfosTotalCount(PostInfoViewModel postInfoViewModel, UserInfo userInfo)
        {
            try
            {
                if (userInfo == null || userInfo.UserId <= 0)
                {
                    return 0;
                }
                var query = from p in _dataContext.PostInfos
                            join pm in _dataContext.PostManagers
                            on p.SecondCategory.SecondCategoryId equals pm.SecondCategory.SecondCategoryId
                            where pm.UserInfo.UserId == userInfo.UserId
                            select p;
                //根据二级分类获取
                if (postInfoViewModel.SecondCategoryIds != null && postInfoViewModel.SecondCategoryIds.Count() > 0)
                {
                    query = query.Where(p => postInfoViewModel.SecondCategoryIds.Contains(p.SecondCategory.SecondCategoryId));
                }
                if (postInfoViewModel.Status > 0)
                {
                    query = query.Where(p => p.Status == postInfoViewModel.Status);
                }
                return query.Count();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取帖子总数
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetPostInfosTotalCount(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return 0;
                }
                var result = _dataContext.PostInfos.Where(p => p.Status == 1 && p.Author.UserId == userId);
                return result.Count();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeletePostInfo(PostInfo postInfo)
        {
            try
            {
                _dataContext.Entry(postInfo.SecondCategory).State = EntityState.Unchanged;
                _dataContext.Entry(postInfo.Author).State = EntityState.Unchanged;
                _dataContext.Entry(postInfo.SecondCategory.FirstCategory).State = EntityState.Unchanged;
                _dataContext.Update(postInfo);
                return _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取置顶帖子数量
        /// </summary>
        /// <returns></returns>
        public int GetTopPostCount()
        {
            try
            {
                return _dataContext.PostInfos.Count(p => p.IsTop == 1 && p.Status == 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="postInfo"></param>
        /// <returns></returns>
        public int SetTop(PostInfo postInfo)
        {
            if (postInfo == null || postInfo.PostId <= 0)
                return 0;
            var post = _dataContext.PostInfos.Where(p => p.PostId == postInfo.PostId).FirstOrDefault();
            post.IsTop = 1;
            _dataContext.PostInfos.Update(post);
            return _dataContext.SaveChanges();
        }

        /// <summary>
        /// 置顶下架
        /// </summary>
        /// <param name="postInfo"></param>
        /// <returns></returns>
        public int DropTop(PostInfo postInfo)
        {
            if (postInfo == null || postInfo.PostId <= 0)
                return 0;
            var post = _dataContext.PostInfos.Where(p => p.PostId == postInfo.PostId).FirstOrDefault();
            post.IsTop = 999;
            _dataContext.PostInfos.Update(post);
            return _dataContext.SaveChanges();
        }

        public bool IsManager(PostInfo postInfo, UserInfo userInfo)
        {
            try
            {
                return _dataContext.PostManagers
                    .Count(pm => pm.UserInfo.UserId == userInfo.UserId
                    && pm.SecondCategory.SecondCategoryId == postInfo.SecondCategory.SecondCategoryId) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        public int UpdatePostInfo(PostInfo postInfo)
        {
            try
            {
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        public int RecoveryPostStatus(PostInfo postInfo)
        {
            try
            {
                var pst = _dataContext.PostInfos.FirstOrDefault(p => p.PostId == postInfo.PostId);
                pst.Status = 1;
                _dataContext.Update(pst);
                return _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
        /// <summary>
        /// 批量删除帖子
        /// </summary>
        /// <param name="postIds"></param>
        /// <returns></returns>
        public int PostMultyDelete(List<int> postIds)
        {
            try
            {
                if (postIds == null || postIds.Count <= 0)
                    return 0;

                var posts = _dataContext.PostInfos.Where(p => postIds.Contains(p.PostId));
                foreach (var p in posts)
                    p.Status = 3;
                _dataContext.UpdateRange(posts);
                return _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 搜索方法，
        /// </summary>
        /// <param name="postInfo"></param>
        /// <param name="userInfo"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<PostInfoViewModel> SearchPost(string searchText, UserInfo userInfo, int currentPage = 1, int pageSize = 10)
        {
            try
            {
                var postInfos = (from p in _dataContext.PostInfos
                                 join u in _dataContext.UserInfos
                                    on p.Author.UserId equals u.UserId
                                 join sc in _dataContext.SecondCategories
                                    on p.SecondCategory.SecondCategoryId equals sc.SecondCategoryId
                                 join fc in _dataContext.FirstCategories
                                    on sc.FirstCategory.FirstCategoryId equals fc.FirstCategoryId
                                 where p.Status == 1 &&
                                 (p.PostTitle.Contains(searchText) || p.Description.Contains(searchText) || p.Author.NickName.Contains(searchText))
                                 select new PostInfoViewModel
                                 {
                                     PostId = p.PostId,
                                     PostTitle = p.PostTitle,
                                     SecondCategory = new SecondCategory
                                     {
                                         SecondCategoryId = sc.SecondCategoryId,
                                         SecondCategoryName = sc.SecondCategoryName,
                                         Status = sc.Status,
                                         FirstCategory = new FirstCategory
                                         {
                                             FirstCategoryId = fc.FirstCategoryId,
                                             FirstCategoryName = fc.FirstCategoryName,
                                             Status = fc.Status
                                         }
                                     },
                                     Description = p.Description,
                                     CreateDate = p.CreateDate,
                                     UpdateDate = p.UpdateDate,
                                     Status = p.Status,
                                     Author = new UserInfo
                                     {
                                         UserId = u.UserId,
                                         UserName = u.UserName,
                                         NickName = u.NickName
                                     },
                                     ReplyCount = _dataContext.ReplyInfos.Count(rp => rp.PostInfo.PostId == p.PostId),
                                     PostGoodCount = _dataContext.PostGoods.Count(pg => pg.PostInfo.PostId == p.PostId),
                                     Watch = 0,
                                     IsTop = p.IsTop,
                                     PostGood = _dataContext.PostGoods
                                     .Where(x => x.PostInfo.PostId == p.PostId && x.GoodsUser.UserId == userInfo.UserId)
                                     .FirstOrDefault()
                                 }).Skip((currentPage - 1) * pageSize).Take(pageSize);
                return postInfos.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取搜索总条数
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public int SearchPostTotalCount(string searchText)
        {
            try
            {
                return _dataContext.PostInfos.Count(p => p.Status == 1 && (p.PostTitle.Contains(searchText) || p.PostContent.Contains(searchText) || p.Author.NickName.Contains(searchText)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
    }
}