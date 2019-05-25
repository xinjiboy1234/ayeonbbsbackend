using Microsoft.EntityFrameworkCore;
using ayeonbbsbackend.ModelDbContext;
using ayeonbbsbackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ayeonbbsbackend.Services
{
    public class ReplyGoodService
    {
        private readonly DataContext _dataContext;
        public ReplyGoodService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public int AddReplyGood(ReplyGood replyGood)
        {
            try
            {
                _dataContext.Entry(replyGood.ReplyInfo).State = EntityState.Unchanged;
                _dataContext.Entry(replyGood.GoodsUser).State = EntityState.Unchanged;
                _dataContext.Add(replyGood);
                var result = _dataContext.SaveChanges() > 0 ? replyGood.GoodsId : 0;
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
        /// <summary>
        /// 验证是否点过赞
        /// </summary>
        /// <param name="postGood"></param>
        /// <returns></returns>
        public ReplyGood GetReplyGood(ReplyGood replyGood)
        {
            try
            {
                return _dataContext.ReplyGoods
                    .Where(x => x.ReplyInfo.ReplyId == replyGood.ReplyInfo.ReplyId
                    && x.GoodsUser.UserId == replyGood.GoodsUser.UserId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
        /// <summary>
        /// 取消赞
        /// </summary>
        /// <param name="postGood"></param>
        /// <returns></returns>
        public int DeleteReplyGood(ReplyGood replyGood)
        {
            try
            {
                //_dataContext.Entry(postGood.PostInfo).State = EntityState.Unchanged;
                //_dataContext.Entry(postGood.GoodsUser).State = EntityState.Unchanged;
                _dataContext.Remove(replyGood);
                var result = _dataContext.SaveChanges();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 根据回复ID获取点赞数量
        /// </summary>
        /// <param name="repyId"></param>
        /// <returns></returns>
        public int GetReplyGoodCountByReplyId(int repyId)
        {
            try
            {
                return _dataContext.ReplyGoods.Count(x => x.ReplyInfo.ReplyId == repyId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
    }
}
