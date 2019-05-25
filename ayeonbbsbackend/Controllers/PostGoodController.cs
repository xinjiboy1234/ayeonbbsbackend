using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ayeonbbsbackend.Models;
using ayeonbbsbackend.Services;
using ayeonbbsbackend.Utils;

namespace ayeonbbsbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostGoodController : ControllerBase
    {
        private readonly PostGoodService _postGoodService;
        private readonly IConfiguration _configuration;

        public PostGoodController(PostGoodService postGoodService, IConfiguration configuration)
        {
            _postGoodService = postGoodService;
            _configuration = configuration;
        }

        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="postGood"></param>
        /// <returns></returns>
        [HttpPost("addpostgood")]
        [Authorize]
        public ActionResult AddPostGood([FromBody] PostInfo postInfo)
        {
            try
            {
                if (postInfo == null || postInfo.PostId <= 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "帖子信息为空！"
                    });
                }
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                var postGood = new PostGood
                {
                    PostInfo = postInfo,
                    GoodsUser = user,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };
                var pg = _postGoodService.GetPostGood(postGood);
                if (pg != null && pg.GoodsId > 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "已经点过了！"
                    });
                }
                var result = _postGoodService.AddPostGood(postGood);
                if (result <= 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "处理失败！"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        mark = "1",
                        result,
                        msg = "成功！"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(new
                {
                    mark = "2",
                    msg = ex.Message
                });
            }
        }

        /// <summary>
        /// 取消赞
        /// </summary>
        /// <param name="postInfo"></param>
        /// <returns></returns>
        [HttpPost("deletepostgood")]
        [Authorize]
        public ActionResult DeletePostGood([FromBody] PostGood postGood)
        {
            try
            {
                if (postGood == null || postGood.GoodsId <= 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求信息异常！"
                    });
                }
                var result = _postGoodService.DeletePostGood(postGood);
                if (result <= 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "处理失败！"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        mark = "1",
                        msg = "成功！"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(new
                {
                    mark = "2",
                    msg = ex.Message
                });
            }
        }

        /// <summary>
        /// 获取点赞次数
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpGet("getpostgoodcount/{postId}")]
        public ActionResult<int> GetPostGoodCountByPostId(int postId)
        {
            try
            {
                return postId <= 0 ? 0 : _postGoodService.GetPostGoodCount(new PostInfo { PostId = postId });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }
    }
}