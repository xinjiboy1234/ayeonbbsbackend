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
    public class ReplyGoodController : ControllerBase
    {
        private readonly ReplyGoodService _replyGoodService;
        private readonly IConfiguration _configuration;

        public ReplyGoodController(ReplyGoodService replyGoodService, IConfiguration configuration)
        {
            _replyGoodService = replyGoodService;
            _configuration = configuration;
        }

        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="postGood"></param>
        /// <returns></returns>
        [HttpPost("addreplygood")]
        [Authorize]
        public ActionResult AddReplyGood([FromBody] ReplyInfo replyInfo)
        {
            try
            {
                if (replyInfo == null || replyInfo.ReplyId <= 0)
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
                var replyGood = new ReplyGood
                {
                    ReplyInfo = replyInfo,
                    GoodsUser = user
                };
                var pg = _replyGoodService.GetReplyGood(replyGood);
                if (pg != null && pg.GoodsId > 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "已经点过了！"
                    });
                }
                var result = _replyGoodService.AddReplyGood(replyGood);
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
                throw ex;
            }
        }

        /// <summary>
        /// 取消赞
        /// </summary>
        /// <param name="postInfo"></param>
        /// <returns></returns>
        [HttpPost("deletereplygood")]
        [Authorize]
        public ActionResult DeleteReplyGood([FromBody] ReplyGood replyGood)
        {
            try
            {
                if (replyGood == null || replyGood.GoodsId <= 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求信息异常！"
                    });
                }
                var result = _replyGoodService.DeleteReplyGood(replyGood);
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
                throw ex;
            }
        }

        [HttpGet("getreplygoodcount/{replyId}")]
        public ActionResult<int> GetReplyGoodCount(int replyId)
        {
            try
            {
                var result = _replyGoodService.GetReplyGoodCountByReplyId(replyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 0;
        }
    }
}