using Microsoft.EntityFrameworkCore;
using ayeonbbsbackend.ModelDbContext;
using ayeonbbsbackend.Models;
using ayeonbbsbackend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ayeonbbsbackend.Services
{
    public class ReplyService
    {
        private readonly DataContext _dataContext;
        public ReplyService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public List<ReplyInfoViewModel> GetReplies(ReplyInfo replyInfo, UserInfo userInfo, int currentPage = 1, int pageSize = 10)
        {
            try
            {
                var result = (from rp in _dataContext.ReplyInfos
                              join u in _dataContext.UserInfos
                              on rp.ReplyUser.UserId equals u.UserId
                              where rp.ParentReplyId == replyInfo.ParentReplyId && rp.PostInfo.PostId == replyInfo.PostInfo.PostId
                              orderby rp.ReplyId ascending
                              select new ReplyInfoViewModel
                              {
                                  ReplyId = rp.ReplyId,
                                  ReplyContent = rp.ReplyContent,
                                  ReplyDate = rp.ReplyDate,
                                  ReplyUser = new UserInfo
                                  {
                                      UserId = u.UserId,
                                      NickName = u.NickName,
                                      Avatar = u.Avatar
                                  },
                                  InnerReplyCount = _dataContext.ReplyInfos.Count(irp => irp.ParentReplyId == rp.ReplyId),
                                  Floor = rp.Floor,
                                  ReplyGoodCount = _dataContext.ReplyGoods.Count(rg => rg.ReplyInfo.ReplyId == rp.ReplyId),
                                  ReplyGood = _dataContext.ReplyGoods
                                  .Where(rg => rg.ReplyInfo.ReplyId == rp.ReplyId && rg.GoodsUser.UserId == userInfo.UserId)
                                  .FirstOrDefault(),
                                  RepliedUserInfo = _dataContext.UserInfos.FirstOrDefault(ru => ru.UserId == rp.RepliedUserId)
                              }).Skip((currentPage - 1) * pageSize).Take(pageSize);
                return result.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        public List<ReplyInfoViewModel> GetInnerReplies(ReplyInfo replyInfo, UserInfo userInfo, int currentPage = 1, int pageSize = 10)
        {
            try
            {
                var result = (from rp in _dataContext.ReplyInfos
                              join u in _dataContext.UserInfos
                              on rp.ReplyUser.UserId equals u.UserId
                              where rp.ParentReplyId == replyInfo.ReplyId
                              orderby rp.ReplyId ascending
                              select new ReplyInfoViewModel
                              {
                                  ReplyId = rp.ReplyId,
                                  ReplyContent = rp.ReplyContent,
                                  ReplyDate = rp.ReplyDate,
                                  ReplyUser = new UserInfo
                                  {
                                      UserId = u.UserId,
                                      NickName = u.NickName,
                                      Avatar = u.Avatar
                                  },
                                  Floor = rp.Floor,
                                  InnerReplyCount = _dataContext.ReplyInfos.Count(irp => irp.RepliedId == rp.ReplyId),
                                  ReplyGoodCount = _dataContext.ReplyGoods.Count(rg => rg.ReplyInfo.ReplyId == rp.ReplyId),
                                  ReplyGood = _dataContext.ReplyGoods
                                  .Where(rg => rg.ReplyInfo.ReplyId == rp.ReplyId && rg.GoodsUser.UserId == userInfo.UserId)
                                  .FirstOrDefault(),
                                  RepliedUserInfo = _dataContext.UserInfos.FirstOrDefault(ru => ru.UserId == rp.RepliedUserId)
                              }).Skip((currentPage - 1) * pageSize).Take(pageSize);
                return result.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        public int GetParentReplyCount(ReplyInfo replyInfo)
        {
            try
            {
                if (replyInfo == null || replyInfo.PostInfo == null)
                {
                    return 0;
                }
                return _dataContext.ReplyInfos.Count(rp => rp.PostInfo.PostId == replyInfo.PostInfo.PostId && rp.ParentReplyId == 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        public int AddReplyInfo(ReplyInfo replyInfo)
        {
            try
            {
                _dataContext.Entry(replyInfo.PostInfo).State = EntityState.Unchanged; // 不修改帖子
                _dataContext.Entry(replyInfo.ReplyUser).State = EntityState.Unchanged; //不修改用户
                _dataContext.ReplyInfos.Add(replyInfo);
                return _dataContext.SaveChanges() > 0 ? replyInfo.ReplyId : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        public ReplyInfoViewModel GetReplyInfo(ReplyInfo replyInfo, UserInfo userInfo)
        {
            try
            {
                var result = (from rp in _dataContext.ReplyInfos
                              join u in _dataContext.UserInfos
                              on rp.ReplyUser.UserId equals u.UserId
                              where rp.ReplyId == replyInfo.ReplyId
                              select new ReplyInfoViewModel
                              {
                                  ReplyId = rp.ReplyId,
                                  ReplyContent = rp.ReplyContent,
                                  ReplyDate = rp.ReplyDate,
                                  ReplyUser = new UserInfo
                                  {
                                      UserId = u.UserId,
                                      NickName = u.NickName,
                                      Avatar = u.Avatar
                                  },
                                  InnerReplyCount = _dataContext.ReplyInfos.Count(irp => irp.ParentReplyId == rp.ReplyId),
                                  Floor = rp.Floor,
                                  ReplyGoodCount = _dataContext.ReplyGoods.Count(rg => rg.ReplyInfo.ReplyId == rp.ReplyId),
                                  ReplyGood = _dataContext.ReplyGoods
                                  .Where(rg => rg.ReplyInfo.ReplyId == rp.ReplyId && rg.GoodsUser.UserId == userInfo.UserId)
                                  .FirstOrDefault(),
                                  RepliedUserInfo = _dataContext.UserInfos.FirstOrDefault(ru => ru.UserId == rp.RepliedUserId)
                              }).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
    }
}
