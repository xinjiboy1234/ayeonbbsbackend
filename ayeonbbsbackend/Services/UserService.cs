using ayeonbbsbackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ayeonbbsbackend.ModelDbContext;
using ayeonbbsbackend.ViewModels;

namespace ayeonbbsbackend.Services
{
    public class UserService
    {
        private readonly DataContext _dbContext;

        public UserService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region 获取用户信息

        public UserInfoViewModel GetUserInfo(UserInfo userInfo)
        {
            try
            {
                if (userInfo == null || string.IsNullOrEmpty(userInfo.LoginId) ||
                    string.IsNullOrEmpty(userInfo.Password))
                    return null;
                // 根据账号密码获取用户信息
                var user = from u in _dbContext.UserInfos
                           where u.LoginId == userInfo.LoginId && u.Password == userInfo.Password && u.Status == 1
                           select new UserInfoViewModel
                           {
                               UserId = u.UserId,
                               NickName = u.NickName,
                               Avatar = u.Avatar
                           };
                return user.FirstOrDefault(); //返回第一条
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region 获取用户列表

        public List<UserInfo> GetUserInfos(UserInfo userInfo)
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return null;
        }

        #endregion

        #region 添加用户

        public int AddUser(UserInfo userInfo)
        {
            try
            {
                if (userInfo != null)
                {
                    _dbContext.Add(userInfo);
                    return _dbContext.SaveChanges() > 0 ? userInfo.UserId : 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return 0;
        }

        #endregion
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserInfo GetUserInfo(int userId)
        {
            try
            {
                return _dbContext.UserInfos.Where(u => u.UserId == userId)
                    .Select(u => new UserInfo
                    {
                        UserId = u.UserId,
                        NickName = u.NickName,
                        Avatar = u.Avatar
                    }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        public UserInfoViewModel GetUserInfoByUserId(int userId)
        {
            try
            {
                return _dbContext.UserInfos.Where(u => u.UserId == userId)
                    .Select(u => new UserInfoViewModel
                    {
                        UserId = u.UserId,
                        NickName = u.NickName,
                        Avatar = u.Avatar
                    }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取可管理分类信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<PostManager> GetPostManagers(int userId)
        {
            try
            {
                return _dbContext.PostManagers.Where(pm => pm.UserInfo.UserId == userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取可发布分类信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<UserPublishCategory> GetUserPublishCategories(int userId)
        {
            try
            {
                return _dbContext.UserPublishCategories.Where(up => up.UserInfo.UserId == userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取可管理的模块
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<int> GetManageCategories(int userId)
        {
            try
            {
                var query = from sc in _dbContext.SecondCategories
                            join pm in _dbContext.PostManagers
                            on sc.SecondCategoryId equals pm.SecondCategory.SecondCategoryId
                            where pm.UserInfo.UserId == userId
                            select sc.SecondCategoryId;
                return query;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取可发布的模块
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<int> GetPublishCategories(int userId)
        {
            try
            {
                var query = from sc in _dbContext.SecondCategories
                            join up in _dbContext.UserPublishCategories
                            on sc.SecondCategoryId equals up.SecondCategory.SecondCategoryId
                            where up.UserInfo.UserId == userId
                            select sc.SecondCategoryId;
                return query;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取用户发表过的二级分类
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<SecondCategory> GetSecondCategoriesByUser(int userId)
        {
            try
            {
                //var result = _dbContext.PostInfos.Join(_dbContext.SecondCategories, p => p.SecondCategory.SecondCategoryId, sc => sc.SecondCategoryId, (p, sc) => p.SecondCategory.SecondCategoryId == sc.SecondCategoryId);
                var query = (from p in _dbContext.PostInfos
                             join sc in _dbContext.SecondCategories
                             on p.SecondCategory.SecondCategoryId equals sc.SecondCategoryId
                             where p.Author.UserId == userId && p.Status == 1
                             select new SecondCategory
                             {
                                 SecondCategoryId = sc.SecondCategoryId,
                                 SecondCategoryName = sc.SecondCategoryName
                             }).Distinct();
                return query;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 根据用户获取点赞数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetPostGoodCountByUser(int userId)
        {
            try
            {
                return _dbContext.PostGoods.Count(pg => pg.PostInfo.Author.UserId == userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 根据用户获取被回复数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetReplyCountByUser(int userId)
        {
            try
            {
                return _dbContext.ReplyInfos.Count(rp => rp.PostInfo.Author.UserId == userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 管理模块获取用户信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<UserInfoViewModel> ManageGetUserInfos(UserInfo userInfo, int currentPage = 1, int pageSize = 10)
        {
            try
            {
                var query = _dbContext.UserInfos
                            .Select(u => new UserInfoViewModel
                            {
                                UserId = u.UserId,
                                NickName = u.NickName,
                                Avatar = u.Avatar,
                                CreateDate = u.CreateDate,
                                Status = u.Status,
                                PostManageCount = _dbContext.PostManagers.Count(pm => pm.UserInfo.UserId == u.UserId),
                                PublishCategoryCount = _dbContext.UserPublishCategories.Count(up => up.UserInfo.UserId == u.UserId),
                                PostCount = _dbContext.PostInfos.Count(p => p.Author.UserId == u.UserId)
                            }).Skip((currentPage - 1) * pageSize).Take(pageSize);
                if (userInfo != null)
                {
                    if (userInfo.Status > 0)
                    {
                        query = query.Where(uv => uv.Status == userInfo.Status);
                    }
                    if (!string.IsNullOrEmpty(userInfo.NickName))
                    {
                        query = query.Where(uv => uv.NickName.Contains(userInfo.NickName));
                    }
                }

                return query;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 更改用户信息(管理)
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public bool MangeChangeUserInfo(UserInfo userInfo)
        {
            try
            {
                var user = _dbContext.UserInfos.Where(u => u.UserId == userInfo.UserId).FirstOrDefault();
                var pms = _dbContext.PostManagers.Where(pm => pm.UserInfo.UserId == userInfo.UserId);
                _dbContext.PostManagers.RemoveRange(pms);
                var ups = _dbContext.UserPublishCategories.Where(up => up.UserInfo.UserId == userInfo.UserId);
                _dbContext.UserPublishCategories.RemoveRange(ups);
                user.PostManagers = userInfo.PostManagers;
                user.UserPublishCategories = userInfo.UserPublishCategories;
                foreach (var pm in user.PostManagers)
                {
                    _dbContext.Entry(pm.SecondCategory).State = EntityState.Unchanged;
                }
                foreach (var up in user.UserPublishCategories)
                {
                    _dbContext.Entry(up.SecondCategory).State = EntityState.Unchanged;
                }
                _dbContext.UserInfos.Update(user);
                return _dbContext.SaveChanges() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 更改用户信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public bool ChangeUserInfo(UserInfo userInfo)
        {
            try
            {
                var user = _dbContext.UserInfos.Where(u => u.UserId == userInfo.UserId).FirstOrDefault();
                user.Avatar = string.IsNullOrEmpty(userInfo.Avatar) ? user.Avatar : userInfo.Avatar;
                user.NickName = string.IsNullOrEmpty(userInfo.NickName) ? user.NickName : userInfo.NickName.Trim();
                _dbContext.UserInfos.Update(user);
                return _dbContext.SaveChanges() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
        /// <summary>
        /// 获取用户总数
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public int GetUserTotalCount(UserInfo userInfo)
        {
            try
            {
                var query = _dbContext.UserInfos.Where(u => 1 == 1);
                if (userInfo != null)
                {
                    if (userInfo.Status > 0)
                    {
                        query.Where(u => u.Status == userInfo.Status);
                    }
                    if (!string.IsNullOrEmpty(userInfo.NickName))
                    {
                        query.Where(u => u.NickName.Contains(userInfo.NickName.Trim()));
                    }
                    return query.Count();
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 批量删除用户
        /// </summary>
        /// <param name="userInfos"></param>
        /// <returns></returns>
        public int MultyDeleteUserInfo(List<int> userIds)
        {
            try
            {
                var userInfos = _dbContext.UserInfos.Where(u => userIds.Contains(u.UserId) && u.IsSupperManager == 1);
                if (userInfos == null)
                {
                    return 0;
                }
                foreach (var u in userInfos)
                {
                    u.Status = 3;
                }
                _dbContext.UpdateRange(userInfos);
                return _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 恢复用户状态
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public int RecoveryUserStatus(UserInfo userInfo)
        {
            try
            {
                var user = _dbContext.UserInfos.Where(u => u.UserId == userInfo.UserId).FirstOrDefault();
                user.Status = 1;
                _dbContext.Update(user);
                return _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public int DeleteUserInfo(UserInfo userInfo)
        {
            try
            {
                var user = _dbContext.UserInfos.Where(u => u.UserId == userInfo.UserId).FirstOrDefault();
                user.Status = 3;
                _dbContext.Update(user);
                return _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
        /// <summary>
        /// 验证昵称是否存在
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        public bool IsNickNameExist(UserInfo userInfo)
        {
            try
            {
                // 验证时排除本人，避免不修改昵称的情况
                return _dbContext.UserInfos.Count(u => u.NickName == userInfo.NickName.Trim() && u.UserId != userInfo.UserId) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
        /// <summary>
        /// 验证用户名是否存在
        /// </summary>
        /// <param name="loginId"></param>
        /// <returns></returns>
        public bool IsLoginIdExist(string loginId)
        {
            try
            {
                return _dbContext.UserInfos.Count(u => u.LoginId == loginId) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
        
        /// <summary>
        /// 验证邮箱是否注册过
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public bool IsEmailExist(UserInfo userInfo)
        {
            try
            {
                return _dbContext.UserInfos.Count(u => u.Email == userInfo.Email.Trim()) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 是否是帖子管理员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsPostManager(int userId)
        {
            try
            {
                return _dbContext.PostManagers.Count(pm => pm.UserInfo.UserId == userId) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsSupperManager(int userId)
        {
            try
            {
                return _dbContext.UserInfos.Count(u => u.UserId == userId && u.IsSupperManager == 1) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
        /// <summary>
        /// 验证老密码
        /// </summary>
        /// <param name="passwordViewModel"></param>
        /// <returns></returns>
        public bool ValidationPassword(PasswordViewModel passwordViewModel)
        {
            try
            {
                return _dbContext.UserInfos.Where(u => u.UserId == passwordViewModel.UserId)
                    .Select(u => u.Password).FirstOrDefault() == passwordViewModel.OldPassword ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 变更密码
        /// </summary>
        /// <param name="passwordViewModel"></param>
        /// <returns></returns>
        public bool ChangePassword(PasswordViewModel passwordViewModel)
        {
            try
            {
                var user = _dbContext.UserInfos.Where(u => u.UserId == passwordViewModel.UserId).FirstOrDefault();
                user.Password = passwordViewModel.NewPassword;
                _dbContext.Update(user);
                return _dbContext.SaveChanges() > 0 ? true: false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 根据用户名获取用户信息
        /// </summary>
        /// <param name="loginId"></param>
        /// <returns></returns>
        public UserInfo GetUserInfo(string loginId)
        {
            try
            {
                if (string.IsNullOrEmpty(loginId))
                    return null;
                return _dbContext.UserInfos.Where(u => u.LoginId == loginId).FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 激活用户的方法
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool ActiveUser(int userId)
        {
            try
            {
                var user = _dbContext.UserInfos.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                    return false;
                user.Status = 1;
                _dbContext.Update(user);
                return _dbContext.SaveChanges() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
    }
}