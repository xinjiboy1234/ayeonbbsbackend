using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ayeonbbsbackend.Models;
using ayeonbbsbackend.Services;
using ayeonbbsbackend.Utils;
using ayeonbbsbackend.ViewModels;
using System;
using System.Collections.Generic;

namespace ayeonbbsbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly PostGoodService _postGoodService;
        private readonly IConfiguration _configuration;

        public PostController(PostService postService, PostGoodService postGoodService, IConfiguration configuration)
        {
            _postService = postService;
            _postGoodService = postGoodService;
            _configuration = configuration;
        }

        // GET
        [HttpGet("getposts/{currPage}/{pageSize}")]
        public ActionResult GetPosts(int currPage, int pageSize)
        {
            if (currPage <= 0 || pageSize <= 0 || pageSize > 100)
            {
                return null;
            }
            var headerStr = Request.Headers["Authorization"];
            var jwtHelper = new JWTHelper(_configuration);
            var user = new UserInfo
            {
                UserId = jwtHelper.GetJWTUserData(headerStr)
            };
            var p = new PostInfo
            {
                Status = 1
            };
            return Ok(new
            {
                posts = _postService.GetPostInfos(p, user, true, currPage, pageSize),
                totalCount = _postService.GetPostInfosTotalCount(p)
            });
        }

        #region 根据大分类获取帖子
        [HttpGet("getpostsbyfirstcategoryid/{categoryId}/{currPage}/{pageSize}")]
        public ActionResult GetPostByFirstCategory(int categoryId, int currPage, int pageSize)
        {
            try
            {
                if (categoryId <= 0 || currPage <= 0 || pageSize <= 0 || pageSize > 100)
                {
                    return null;
                }
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                var p = new PostInfo
                {
                    Status = 1,
                    SecondCategory = new SecondCategory
                    {
                        FirstCategory = new FirstCategory
                        {
                            FirstCategoryId = categoryId
                        }
                    }
                };
                return Ok(new
                {
                    posts = _postService.GetPostInfos(p, user, true, currPage, pageSize),
                    totalCount = _postService.GetPostInfosTotalCount(p)
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 根据二级分类获取帖子

        [HttpGet("getpostsbysecondcategoryid/{categoryId}/{currPage}/{pageSize}")]
        public ActionResult GetPostBySecondCategory(int categoryId, int currPage, int pageSize)
        {
            try
            {
                if (categoryId <= 0 || currPage <= 0 || pageSize <= 0 || pageSize > 100)
                {
                    return null;
                }
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                var p = new PostInfo
                {
                    Status = 1,
                    SecondCategory = new SecondCategory
                    {
                        SecondCategoryId = categoryId
                    }
                };
                return Ok(new
                {
                    posts = _postService.GetPostInfos(p, user, true, currPage, pageSize),
                    totalCount = _postService.GetPostInfosTotalCount(p)
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 发表帖子

        [HttpPost("writepost")]
        [Authorize]
        public ActionResult WritePost([FromBody]PostInfo postInfo)
        {
            try
            {
                if (postInfo == null)
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据不能为空！"
                    });
                if (string.IsNullOrEmpty(postInfo.PostTitle))
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "标题不能为空！"
                    });
                }
                if (postInfo.SecondCategory.SecondCategoryId <= 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "分类为空！"
                    });
                }
                if (string.IsNullOrEmpty(HtmlFillter.ReplaceHtmlToBlankExCludetImg(postInfo.PostContent.Trim()).Trim()))
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "内容为空！"
                    });
                }
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                postInfo.Author = user;
                postInfo.Status = 1;
                postInfo.CreateDate = DateTime.Now;
                postInfo.UpdateDate = DateTime.Now;
                // 过滤 html 后的内容
                //var pc = HtmlFillter.ReplaceHtmlToBlank(postInfo.PostContent);
                //描述
                //postInfo.Description = pc.Length > 50 ? pc.Substring(0, 50) + " ..." : pc;
                postInfo.Description = HtmlFillter.ReplaceHtmlToBlank(postInfo.PostContent);
                var result = _postService.AddPostInfo(postInfo);
                if (result > 0)
                {
                    return Ok(new
                    {
                        mark = "1",
                        postId = result,
                        msg = "发布成功！"
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

        /// <summary>
        /// 保存帖子
        /// </summary>
        /// <param name="postInfo"></param>
        /// <returns></returns>
        [HttpPost("savepost")]
        [Authorize]
        public ActionResult SavePost([FromBody]PostInfo postInfo)
        {
            try
            {
                if (postInfo == null)
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求内容为空！"
                    });

                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                postInfo.Author = user;
                postInfo.Status = 2; //保存
                postInfo.CreateDate = DateTime.Now;
                postInfo.UpdateDate = DateTime.Now;
                if (!string.IsNullOrEmpty(postInfo.Description))
                {
                    postInfo.Description = HtmlFillter.ReplaceHtmlToBlank(postInfo.PostContent);
                }
                var result = _postService.AddPostInfo(postInfo);
                if (result > 0)
                {
                    return Ok(new
                    {
                        mark = "1",
                        postId = result,
                        msg = "保存成功！"
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

        #endregion

        #region 热门帖子

        [HttpGet("gethotposts")]
        public ActionResult<IEnumerable<PostInfoViewModel>> GetHotPostInfos()
        {
            try
            {
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                return Ok(_postService.GetHotPostInfos(user));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        #endregion

        #region 获取帖子详细

        [HttpGet("getpostbyid/{postId}")]
        public ActionResult<PostInfoViewModel> GetPostInfo(int postId)
        {
            try
            {
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                var post = new PostInfo
                {
                    PostId = postId,
                    Author = user,
                    Status = 1
                };
                var p = _postService.GetPostInfo(post);
                p.PostGood = _postGoodService.GetPostGood(new PostGood
                {
                    PostInfo = post,
                    GoodsUser = user
                });
                return p;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        #endregion

        #region 获取保存的帖子

        [HttpGet("getpostinfo/{postId}")]
        [Authorize]
        public ActionResult<PostInfoViewModel> GetPostInfoByPostId(int postId)
        {
            try
            {
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                var post = new PostInfo
                {
                    PostId = postId,
                    Author = user,
                    Status = 2
                };
                return _postService.GetPostInfo(post, user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        #endregion

        /// <summary>
        /// 根据用户ID获取帖子
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="currPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("getpostsbyuser/{userid}/{currPage}/{pageSize}")]
        public ActionResult GetPostsByUserInfo(int userId, int currPage, int pageSize)
        {
            try
            {
                if (userId <= 0 || currPage <= 0 || pageSize <= 0 || pageSize > 100)
                {
                    return Ok();
                }
                var headerStr = Request.Headers["Authorization"];
                var loginFlag = false;
                if (!string.IsNullOrEmpty(headerStr))
                {
                    var jwtHelper = new JWTHelper(_configuration);
                    if (jwtHelper.JWTvalidate(headerStr))
                        loginFlag = true;
                }
                return Ok(new
                {
                    posts = _postService.GetPostsByUserInfo(userId, loginFlag, currPage, pageSize),
                    totalCount = _postService.GetPostInfosTotalCount(userId)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(ex);
            }
        }
        /// <summary>
        /// 删除帖子
        /// </summary>
        /// <param name="postInfo"></param>
        /// <returns></returns>
        [HttpPost("deletepostinfo")]
        [Authorize]
        public ActionResult DeletePostInfo([FromBody]PostInfo postInfo)
        {
            try
            {
                if (postInfo == null || postInfo.PostId <= 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据异常"
                    });
                }
                postInfo.Status = 3;
                postInfo.IsTop = 999;
                var result = _postService.DeletePostInfo(postInfo);
                if (result > 0)
                {
                    return Ok(new
                    {
                        mark = "1",
                        msg = "成功"
                    });
                }
                return Ok(new
                {
                    mark = "2",
                    msg = "失败"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(ex);
            }
        }
        /// <summary>
        /// 管理页面获取帖子信息
        /// </summary>
        /// <param name="postInfoViewModel"></param>
        /// <returns></returns>
        [HttpPost("manage/getposts")]
        [Authorize]
        public ActionResult GetPosts([FromBody]PostInfoViewModel postInfoViewModel)
        {
            if (postInfoViewModel == null)
            {
                return null;
            }
            var headerStr = Request.Headers["Authorization"];
            var jwtHelper = new JWTHelper(_configuration);
            var user = new UserInfo
            {
                UserId = jwtHelper.GetJWTUserData(headerStr)
            };
            return Ok(new
            {
                posts = _postService.GetPostInfos(postInfoViewModel, user, postInfoViewModel.PageViewModel.CurrentPage, postInfoViewModel.PageViewModel.PageSize),
                totalCount = _postService.GetPostInfosTotalCount(postInfoViewModel, user)
            });
        }
        /// <summary>
        /// 获取置顶帖子数量
        /// </summary>
        /// <returns></returns>
        [HttpGet("manage/gettoppostcount")]
        [Authorize]
        public ActionResult<int> GetTopPostCount()
        {
            try
            {
                return _postService.GetTopPostCount();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 22222;
            }
        }

        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="postInfo"></param>
        /// <returns></returns>
        [HttpPost("manage/settop")]
        [Authorize]
        public ActionResult SetTop([FromBody]PostInfo postInfo)
        {
            try
            {
                if (postInfo == null || postInfo.PostId <= 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据异常"
                    });
                }
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                if (_postService.IsManager(postInfo, user))
                {
                    _postService.SetTop(postInfo);
                    return Ok(new
                    {
                        mark = "1",
                        msg = "成功"
                    });
                }
                return Ok(new
                {
                    mark = "2",
                    msg = "失败"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(ex);
            }
        }

        /// <summary>
        /// 置顶下架
        /// </summary>
        /// <param name="postInfo"></param>
        /// <returns></returns>
        [HttpPost("manage/droptop")]
        [Authorize]
        public ActionResult DropTop([FromBody]PostInfo postInfo)
        {
            try
            {
                if (postInfo == null || postInfo.PostId <= 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据异常"
                    });
                }
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                if (_postService.IsManager(postInfo, user))
                {
                    _postService.DropTop(postInfo);
                    return Ok(new
                    {
                        mark = "1",
                        msg = "成功"
                    });
                }
                return Ok(new
                {
                    mark = "2",
                    msg = "失败"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(ex);
            }
        }
        /// <summary>
        /// 恢复帖子状态 删除 => 发布
        /// </summary>
        /// <param name="postInfo"></param>
        /// <returns></returns>
        [HttpPost("manage/recoverypoststatus")]
        [Authorize]
        public ActionResult RecoveryPostStatus([FromBody]PostInfo postInfo)
        {
            try
            {
                if (postInfo == null || postInfo.PostId <= 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据异常"
                    });
                }
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                if (_postService.IsManager(postInfo, user))
                {
                    _postService.RecoveryPostStatus(postInfo);
                    return Ok(new
                    {
                        mark = "1",
                        msg = "成功"
                    });
                }
                return Ok(new
                {
                    mark = "2",
                    msg = "失败"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(ex);
            }
        }
        /// <summary>
        /// 批量删除帖子
        /// </summary>
        /// <param name="postIds"></param>
        /// <returns></returns>
        [HttpPost("manage/postmultydelete")]
        [Authorize]
        public ActionResult PostMultyDelete([FromBody]List<int> postIds)
        {
            try
            {
                if (postIds == null || postIds.Count <= 0)
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据为空"
                    });
                var result = _postService.PostMultyDelete(postIds);
                if (result > 0)
                    return Ok(new
                    {
                        mark = "1",
                        msg = "成功"
                    });

                return Ok(new
                {
                    mark = "2",
                    msg = "失败"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok();
            }
        }

        /// <summary>
        /// 搜索方法
        /// </summary>
        /// <param name="postInfoViewModel"></param>
        /// <returns></returns>
        [HttpPost("searchpost")]
        public ActionResult SearchPost([FromBody]PostInfoViewModel postInfoViewModel)
        {
            try
            {
                if (postInfoViewModel == null)
                    return null;
                if (string.IsNullOrEmpty(postInfoViewModel.PostTitle))
                    return null;
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var user = new UserInfo
                {
                    UserId = jwtHelper.GetJWTUserData(headerStr)
                };
                if (postInfoViewModel.PageViewModel == null)
                    return Ok(new
                    {
                        posts = _postService.SearchPost(postInfoViewModel.PostTitle, user, 1, 20),
                        totalCount = _postService.SearchPostTotalCount(postInfoViewModel.PostTitle)
                    });
                return Ok(new
                {
                    posts = _postService.SearchPost(postInfoViewModel.PostTitle, user, postInfoViewModel.PageViewModel.CurrentPage, postInfoViewModel.PageViewModel.PageSize),
                    totalCount = _postService.SearchPostTotalCount(postInfoViewModel.PostTitle)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}