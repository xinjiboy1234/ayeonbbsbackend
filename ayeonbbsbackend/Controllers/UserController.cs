using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ayeonbbsbackend.Models;
using ayeonbbsbackend.Services;
using ayeonbbsbackend.Utils;
using ayeonbbsbackend.ViewModels;

namespace ayeonbbsbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration, UserService userService)
        {
            _userService = userService;
            _configuration = configuration;
        }

        /// <summary>
        /// 用户登录控制器
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public ActionResult UserLogin([FromBody] UserInfo userInfo)
        {
            try
            {
                if (userInfo == null || string.IsNullOrEmpty(userInfo.Password) ||
                    string.IsNullOrEmpty(userInfo.LoginId))
                    return Ok(new
                    {
                        mark = 2,
                        msg = "用户名或者密码错误"
                    });
                var user = _userService.GetUserInfo(userInfo);
                if (user != null && user.UserId <= 0)
                    return Ok(new
                    {
                        mark = 2,
                        msg = "用户名或者密码错误"
                    });
                user.IsPostManager = _userService.IsPostManager(user.UserId);
                //生成 Token
                var jwtTokenUtil = new JWTHelper(_configuration);
                var token = jwtTokenUtil.GetToken(user); //生成token
                return Ok(new
                {
                    mark = 1,
                    msg = "登录成功",
                    jwttoken = token,
                    userInfo = user
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(new
                {
                    mark = 2,
                    msg = "发生错误，请联系管理员！"
                });
            }
        }
        // POST: api/User
        [HttpPost("adduser")]
        [Authorize]
        public void AddUser([FromBody] UserInfo userInfo)
        {
            try
            {
                if (userInfo == null)
                    return;

                #region 验证数据有效性

                if (string.IsNullOrEmpty(userInfo.LoginId))
                {
                }

                if (string.IsNullOrEmpty(userInfo.Password))
                {
                }

                if (string.IsNullOrEmpty(userInfo.NickName))
                {
                }

                #endregion

                #region 初始化值

                userInfo.CreateDate = DateTime.Now;
                userInfo.Status = 1;
                userInfo.UpdateDate = DateTime.Now;

                #endregion

                var result = _userService.AddUser(userInfo);
                Console.WriteLine(result.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [HttpGet("userspace/getuserdata/{userId}")]
        public ActionResult GetUserSpaceData(int userId)
        {
            try
            {
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                var loginFlag = false;
                if (userId == jwtHelper.GetJWTUserData(headerStr))
                {
                    loginFlag = true;
                }
                return Ok(new
                {
                    SecondCategories = _userService.GetSecondCategoriesByUser(userId),
                    UserInfo = _userService.GetUserInfo(userId),
                    PostGoodCount = _userService.GetPostGoodCountByUser(userId),
                    ReplyCount = _userService.GetReplyCountByUser(userId),
                    IsLogin = loginFlag
                });
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

        [HttpPost("manage/getuserinfos")]
        [Authorize]
        public ActionResult GetUserInfos([FromBody]UserInfoViewModel userInfoViewModel)
        {
            try
            {
                if (userInfoViewModel == null || userInfoViewModel.PageViewModel == null || userInfoViewModel.PageViewModel.CurrentPage <= 0
                    || userInfoViewModel.PageViewModel.PageSize <= 0 || userInfoViewModel.PageViewModel.PageSize > 100)
                    return Ok();

                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                if (!_userService.IsSupperManager(jwtHelper.GetJWTUserData(headerStr)))
                    return Ok();

                return Ok(new
                {
                    users = _userService.ManageGetUserInfos(userInfoViewModel, userInfoViewModel.PageViewModel.CurrentPage, userInfoViewModel.PageViewModel.PageSize),
                    totalCount = _userService.GetUserTotalCount(userInfoViewModel)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(ex);
            }
        }

        [HttpGet("manage/getuserinfo/{userId}")]
        [Authorize]
        public ActionResult<UserInfo> GetUserInfo(int userId)
        {
            try
            {
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                if (!_userService.IsSupperManager(jwtHelper.GetJWTUserData(headerStr)))
                    return Ok();
                return Ok(new
                {
                    user = _userService.GetUserInfo(userId),
                    manageCategories = _userService.GetManageCategories(userId),
                    publishCategories = _userService.GetPublishCategories(userId)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(ex);
            }
        }

        [HttpPost("manage/changeuserinfo")]
        [Authorize]
        public ActionResult ChangeUserInfo([FromBody]UserInfo userInfo)
        {
            try
            {
                if (userInfo == null)
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据为空"
                    });
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                if (!_userService.IsSupperManager(jwtHelper.GetJWTUserData(headerStr)))
                    return Ok(new
                    {
                        mark = "2",
                        msg = "没有权限"
                    }); ;
                if (_userService.MangeChangeUserInfo(userInfo))
                    return Ok(new
                    {
                        mark = "1",
                        msg = "成功！"
                    });
                return Ok(new
                {
                    mark = "2",
                    msg = "变更失败"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(ex);
            }
        }

        [HttpPost("changeuserinfo")]
        [Authorize]
        public ActionResult EditUserInfo([FromBody]UserInfo userInfo)
        {
            try
            {
                //请求数据不规范就返回失败
                if (userInfo == null || userInfo.UserId <= 0)
                    return Ok(new
                    {
                        mark = "2",
                        msg = "变更失败"
                    });

                var imgName = string.Empty;
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                // 请求信息和 jwt 不符合就返回失败
                if (userInfo.UserId != jwtHelper.GetJWTUserData(headerStr))
                    return Ok(new
                    {
                        mark = "2",
                        msg = "变更失败"
                    });
                //验证昵称是否存在
                if (_userService.IsNickNameExist(userInfo))
                    return Ok(new
                    {
                        mark = "2",
                        msg = "昵称已存在"
                    });
                // 如果头像存在
                if (!string.IsNullOrEmpty(userInfo.Avatar))
                {
                    var imgStrs = userInfo.Avatar.Split("data:image/png;base64,");
                    if (imgStrs.Count() >= 2)
                    {
                        imgName = Guid.NewGuid() + ".png";
                        ImageHelper.Base64ToImg(imgStrs[1], _configuration["imguploadpath"] + "/avatar/" + imgName);
                    }
                    userInfo.Avatar = imgName;
                }
                if (_userService.ChangeUserInfo(userInfo))
                    return Ok(new
                    {
                        mark = "1",
                        msg = "成功"
                    });

                return Ok(new
                {
                    mark = "2",
                    msg = "变更失败"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(new
                {
                    mark = "2",
                    msg = "变更失败"
                });
            }
        }

        /// <summary>
        /// 批量删除用户数据
        /// </summary>
        /// <param name="userInfoViewModel"></param>
        /// <returns></returns>
        [HttpPost("manage/multydeleteuserinfo")]
        [Authorize]
        public ActionResult MultyDeleteUserInfo([FromBody]UserInfoViewModel userInfoViewModel)
        {
            try
            {
                if (userInfoViewModel == null || userInfoViewModel.UserIds == null || userInfoViewModel.UserIds.Count() <= 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "失败！"
                    });
                }
                var result = _userService.MultyDeleteUserInfo(userInfoViewModel.UserIds);
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
                    msg = "失败！"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(new
                {
                    mark = "2",
                    msg = "失败！"
                });
            }
        }

        [HttpPost("manage/recoveryuserstatus")]
        [Authorize]
        public ActionResult RecoveryUserStatus([FromBody]UserInfo userInfo)
        {
            try
            {
                if (userInfo == null || userInfo.UserId <= 0)
                    return Ok(new
                    {
                        mark = "2",
                        msg = "数据为空"
                    });

                var result = _userService.RecoveryUserStatus(userInfo);
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
                return Ok();
            }
        }

        [HttpPost("manage/deleteuserinfo")]
        [Authorize]
        public ActionResult DeleteUserInfo([FromBody]UserInfo userInfo)
        {
            try
            {
                if (userInfo == null || userInfo.UserId <= 0)
                    return Ok(new
                    {
                        mark = "2",
                        msg = "数据为空"
                    });
                var result = _userService.DeleteUserInfo(userInfo);
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

        [HttpGet("getuserinfo/{userId}")]
        [Authorize]
        public ActionResult<UserInfoViewModel> GetUserInfoById(int userId)
        {
            try
            {
                if (userId <= 0)
                    return null;
                var user = _userService.GetUserInfoByUserId(userId);
                if (user != null)
                {
                    user.IsPostManager = _userService.IsPostManager(userId);
                }
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok();
            }
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [HttpPost("userregiste")]
        public ActionResult UserRegister([FromBody]UserInfo userInfo)
        {
            try
            {
                if (userInfo != null)
                {
                    if (string.IsNullOrEmpty(userInfo.LoginId))
                        return Ok(new
                        {
                            mark = "2",
                            msg = "用户名为空！"
                        });
                    //验证用户密码："^[a-zA-Z]\w{5,17}$"正确格式为：以字母开头，长度在6~18之间，只能包含字符、数字和下划线。
                    //验证Email地址："^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"。
                    //只能输入由数字和26个英文字母组成的字符串："^[A-Za-z0-9]+$
                    //验证是否含有 ^% &'',;=?$\"等字符："[^% &'',;=?$\x22]+"
                    if (!Regex.IsMatch(userInfo.LoginId, @"[a-zA-Z]\w{5,17}$"))
                        return Ok(new
                        {
                            mark = "2",
                            msg = "用户名必须是6-18位的数字字母下划线的任意组合！"
                        });
                    if (_userService.IsLoginIdExist(userInfo.LoginId))
                        return Ok(new
                        {
                            mark = "2",
                            msg = "用户名已存在！"
                        });
                    if (_userService.IsNickNameExist(userInfo))
                        return Ok(new
                        {
                            mark = "2",
                            msg = "昵称已存在！"
                        });
                    if (string.IsNullOrEmpty(userInfo.Password))
                        return Ok(new
                        {
                            mark = "2",
                            msg = "密码为空！"
                        });
                    if (userInfo.Password.Length < 6)
                        return Ok(new
                        {
                            mark = "2",
                            msg = "密码必须大于6位！"
                        });
                    if (string.IsNullOrEmpty(userInfo.Email))
                        return Ok(new
                        {
                            mark = "2",
                            msg = "邮箱为空！"
                        });
                    if (!Regex.IsMatch(userInfo.Email, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
                        return Ok(new
                        {
                            mark = "2",
                            msg = "邮箱不符合规则！"
                        });
                    if (_userService.IsEmailExist(userInfo))
                        return Ok(new
                        {
                            mark = "2",
                            msg = "邮箱已注册！"
                        });
                    userInfo.Status = 3; // 待激活状态
                    userInfo.CreateDate = DateTime.Now;
                    userInfo.UpdateDate = DateTime.Now;
                    var rsaHelper = new RSAHelper(RSAType.RSA2, Encoding.UTF8, _configuration["private_key"], _configuration["public_key"]);
                    var userId = _userService.AddUser(userInfo);
                    var url = string.Format("{0}/main/user/activeuser?key={1}", _configuration["fronturl"], rsaHelper.Encrypt(userId.ToString()).Replace("+", "%2B"));
                    if (userId > 0)
                    {
                        //MailHelper.SendEMail("系统提醒", "您注册成功，<br> 点击以下地址进行激活<br> <a href='" + fronturl + "/main/user/activeuser/" + HttpUtility.UrlEncode(rsaHelper.Encrypt(userId.ToString())).Replace("/","%%") + "'>" + fronturl + "/main/user/activeuser/" + rsaHelper.Encrypt(userId.ToString()).Replace("/", "%%") + "</a>", userInfo.Email);
                        MailHelper.SendEMail("系统提醒"
                            , string.Format("您注册成功，<br> 点击以下地址进行激活<br> <a href = '{0}'>{1}</a>", url, url), userInfo.Email);
                        
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
                return Ok(new
                {
                    mark = "2",
                    msg = "失败"
                });
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

        [HttpGet("isloginidexist/{loginId}")]
        public ActionResult<bool> IsLoginIdExist(string loginId)
        {
            try
            {
                if (string.IsNullOrEmpty(loginId))
                    return true;
                if (_userService.IsLoginIdExist(loginId))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return true;
            }
        }

        [HttpPost("isnicknameexist")]
        public ActionResult<bool> IsNickNameExist([FromBody]UserInfo userInfo)
        {
            try
            {
                if (userInfo == null)
                    return false;
                if (string.IsNullOrEmpty(userInfo.NickName))
                    return true;
                if (_userService.IsNickNameExist(userInfo))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return true;
            }
        }

        /// <summary>
        /// 验证邮箱是否注册过
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [HttpPost("isemailexist")]
        public ActionResult<bool> IsEmailExist([FromBody]UserInfo userInfo)
        {
            try
            {
                if (userInfo == null)
                    return false;
                if (string.IsNullOrEmpty(userInfo.Email))
                    return true;
                if (_userService.IsEmailExist(userInfo))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return true;
            }
        }

        /// <summary>
        /// 验证是否是超级管理员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("manage/issuppermanager/{userId}")]
        [Authorize]
        public ActionResult<bool> IsSupperManager(int userId)
        {
            try
            {
                var headerStr = Request.Headers["Authorization"];
                var jwtHelper = new JWTHelper(_configuration);
                // 请求信息和 jwt 不符合就返回失败
                if (userId != jwtHelper.GetJWTUserData(headerStr))
                    return false;
                return _userService.IsSupperManager(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok();
            }
        }

        /// <summary>
        /// 激活用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("activeuser")]
        public ActionResult ActivationUser(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return Ok(new
                    {
                        mark = "2",
                        msg = "Key为空"
                    });
                var rsaHelper = new RSAHelper(RSAType.RSA2, Encoding.UTF8, _configuration["private_key"], _configuration["public_key"]);
                var userId = rsaHelper.Decrypt(key);
                if(_userService.ActiveUser(int.Parse(userId)))
                    return Ok(new
                    {
                        mark = "1",
                        msg = "激活成功"
                    });
                else
                    return Ok(new
                    {
                        mark = "2",
                        msg = "激活失败"
                    });
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
        /// 变更密码
        /// </summary>
        /// <param name="passwordViewModel"></param>
        /// <returns></returns>
        [HttpPost("changepassword")]
        [Authorize]
        public ActionResult ChangePassword([FromBody]PasswordViewModel passwordViewModel)
        {
            try
            {
                if (passwordViewModel == null)
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据为空"
                    });
                if (passwordViewModel.UserId <= 0 || string.IsNullOrEmpty(passwordViewModel.OldPassword) || string.IsNullOrEmpty(passwordViewModel.NewPassword))
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据异常"
                    });
                if (!_userService.ValidationPassword(passwordViewModel))
                    return Ok(new
                    {
                        mark = "2",
                        msg = "原密码错误"
                    });
                if (_userService.ChangePassword(passwordViewModel))
                    return Ok(new
                    {
                        mark = "1",
                        msg = "变更成功"
                    });
                return Ok(new
                {
                    mark = "2",
                    msg = "变更失败"
                });
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

        [HttpPost("forgetpassword")]
        public ActionResult ForgetPassWord([FromBody]string loginId)
        {
            try
            {
                var user = _userService.GetUserInfo(loginId);
                if (user == null)
                    return Ok(new
                    {
                        mark = "2",
                        msg = "用户信息无效，请检查用户名"
                    });
                if (string.IsNullOrEmpty(user.Email))
                    return Ok(new
                    {
                        mark = "2",
                        msg = "用户邮箱为空，无法初始化密码"
                    });
                // 密码字符数组
                var charArr = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', ',', '#', '@', '*', '-', '=', '_', '!' };
                var newPassword = "";
                var random = new Random();
                for (var i = 0; i < 6; i++)
                {
                    newPassword += charArr[random.Next(0, charArr.Length)];
                }
                var fronturl = _configuration["fronturl"];
                MailHelper.SendEMail("系统提醒", @"您的密码初始化为 " + newPassword
                    + " 请连接到 <a href='" + fronturl + "'>" + fronturl + "</a>, 登录之后自行变更密码", user.Email);
                _userService.ChangePassword(new PasswordViewModel { UserId = user.UserId, NewPassword = newPassword });
                return Ok(new
                {
                    mark = "1",
                    msg = "发送成功"
                });
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
    }
}