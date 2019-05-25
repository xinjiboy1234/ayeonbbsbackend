using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ayeonbbsbackend.Models;
using ayeonbbsbackend.Services;
using ayeonbbsbackend.Utils;
using ayeonbbsbackend.ViewModels;

namespace ayeonbbsbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReplyController : ControllerBase
    {
        private readonly ReplyService _replyService;
        private readonly IConfiguration _configuration;

        public ReplyController(ReplyService replyService, IConfiguration configuration)
        {
            _replyService = replyService;
            _configuration = configuration;
        }

        [HttpGet("getrepliesbypostid/{postId}/{currPage}/{pageSize}")]
        public ActionResult<ReplyInfoViewModelResponse> GetRepliesByPostId(int postId, int currPage, int pageSize)
        {
            try
            {
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                // 获取一级回复
                var replyInfo = new ReplyInfo
                {
                    ParentReplyId = 0,
                    PostInfo = new PostInfo
                    {
                        PostId = postId
                    }
                };
                var result = _replyService.GetReplies(replyInfo, user, currPage, pageSize);
                var replyInfoViewModelRes = new ReplyInfoViewModelResponse();
                foreach (var r in result)
                {
                    r.ReplyInfoViewModels = _replyService.GetInnerReplies(new ReplyInfo { ReplyId = r.ReplyId }, user, 1, pageSize);
                }
                replyInfoViewModelRes.ReplyInfoViewModels = result;
                replyInfoViewModelRes.ReplyTotalCount = _replyService.GetParentReplyCount(replyInfo);
                return replyInfoViewModelRes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        [HttpGet("getrepliesbyreplyid/{replyId}/{currPage}/{pageSize}")]
        public ActionResult<ReplyInfoViewModelResponse> GetRepliesByReplyId(int replyId, int currPage, int pageSize)
        {
            try
            {
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                var result = _replyService.GetInnerReplies(new ReplyInfo { ReplyId = replyId }, user, currPage, pageSize);
                var replyInfoViewModelRes = new ReplyInfoViewModelResponse
                {
                    ReplyInfoViewModels = result
                };
                return replyInfoViewModelRes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        [HttpGet("getreplybyid/{replyId}")]
        public ActionResult<ReplyInfoViewModelResponse> GetReplyInfoById(int replyId)
        {
            try
            {
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                var result = _replyService.GetReplyInfo(new ReplyInfo { ReplyId = replyId }, user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        [HttpPost("addreplyinfo")]
        [Authorize]
        public ActionResult AddReplyInfo([FromBody]ReplyInfoViewModel replyInfoViewModel)
        {
            try
            {
                #region 数据校验
                if (replyInfoViewModel == null || string.IsNullOrEmpty(HtmlFillter.ReplaceHtmlToBlank(replyInfoViewModel.ReplyContent)))
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "回复内容为空！"
                    });
                }

                if (replyInfoViewModel.PostInfo == null)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "帖子为空！"
                    });
                }

                if (replyInfoViewModel.PostInfo.PostId <= 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "帖子为空！"
                    });
                }

                if (replyInfoViewModel.RepliedId != null)
                {
                    if ((replyInfoViewModel.RepliedId <= 0 && replyInfoViewModel.RepliedUserInfo == null)
                        || (replyInfoViewModel.RepliedUserInfo.UserId <= 0 && replyInfoViewModel.RepliedId > 0))
                    {
                        return Ok(new
                        {
                            mark = "2",
                            msg = "用户回复关系异常！"
                        });
                    }
                    replyInfoViewModel.RepliedUserId = replyInfoViewModel.RepliedUserInfo.UserId;
                }
                #endregion

                #region 根据token获得用户ID
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                #endregion
                replyInfoViewModel.ReplyUser = user;
                replyInfoViewModel.ReplyDate = DateTime.Now;
                replyInfoViewModel.UpdateDate = DateTime.Now;
                var result = _replyService.AddReplyInfo(replyInfoViewModel);
                if (result > 0)
                {
                    return Ok(new
                    {
                        mark = "1",
                        msg = "成功！"
                    });
                }
                return Ok(new
                {
                    mark = "2",
                    msg = "处理失败！"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    mark = "2",
                    msg = ex.Message
                });
            }
        }
    }
}