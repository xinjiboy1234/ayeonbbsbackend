using Microsoft.EntityFrameworkCore;
using ayeonbbsbackend.ModelDbContext;
using ayeonbbsbackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ayeonbbsbackend.Services
{
    public class PostGoodService
    {
        private readonly DataContext _dataContext;

        public PostGoodService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public int AddPostGood(PostGood postGood)
        {
            try
            {
                _dataContext.Entry(postGood.PostInfo).State = EntityState.Unchanged;
                _dataContext.Entry(postGood.GoodsUser).State = EntityState.Unchanged;
                _dataContext.Add(postGood);
                var result = _dataContext.SaveChanges() > 0 ? postGood.GoodsId : 0;
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
        public PostGood GetPostGood(PostGood postGood)
        {
            try
            {
                return _dataContext.PostGoods
                    .Where(x => x.PostInfo.PostId == postGood.PostInfo.PostId
                    && x.GoodsUser.UserId == postGood.GoodsUser.UserId).FirstOrDefault();
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
        public int DeletePostGood(PostGood postGood)
        {
            try
            {
                _dataContext.Remove(postGood);
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
        /// 根据帖子获取点赞此暑
        /// </summary>
        /// <param name="postInfo"></param>
        /// <returns>点赞次数</returns>
        public int GetPostGoodCount(PostInfo postInfo)
        {
            try
            {
                return _dataContext.PostGoods.Count(x => x.PostInfo.PostId == postInfo.PostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
    }
}
