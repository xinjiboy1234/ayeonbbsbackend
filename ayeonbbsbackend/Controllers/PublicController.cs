using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ayeonbbsbackend.Utils;

namespace ayeonbbsbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PublicController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 验证Token，验证成功后，过期就签发新的Token
        /// </summary>
        /// <returns></returns>
        [HttpPost("refreshjwttoken")]
        [Authorize]
        public ActionResult ValidateUserLoginStatus()
        {
            try
            {
                var requestHeader = Request.Headers["Authorization"];
                var jwtTokenUtil = new JWTHelper(_configuration);
                if (string.IsNullOrEmpty(requestHeader))
                    return BadRequest(new
                    {
                        mark = "2",
                        msg = "无效请求"
                    });
                var newToken = jwtTokenUtil.RefreshJWTToken(requestHeader);
                return string.IsNullOrEmpty(newToken) ?
                    Ok(new { mark = "1", msg = "success", jwttoken = "" })
                    : Ok(new { mark = "1", msg = "success", jwttoken = newToken });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// 验证是否登录
        /// </summary>
        /// <returns></returns>
        [HttpGet("isuserlogin/{userId}")]
        public ActionResult IsLogin(int userId)
        {
            try
            {
                if (userId <= 0)
                    return Ok(new
                    {
                        mark = false,
                        msg = "无效请求"
                    });
                var requestHeader = Request.Headers["Authorization"].ToString();
                var jwtTokenUtil = new JWTHelper(_configuration);
                if (string.IsNullOrEmpty(requestHeader))
                    return Ok(new
                    {
                        mark = false,
                        msg = "无效请求"
                    });
                if (string.IsNullOrEmpty(requestHeader) || requestHeader.Split("Bearer ").Length < 2)
                    return Ok(new
                    {
                        mark = false,
                        msg = "无效请求"
                    });
                var tokens = requestHeader.Split("Bearer ")[1].Trim().Split('.'); // 获取JWT段
                if (tokens.Length < 3)
                    return Ok(new
                    {
                        mark = false,
                        msg = "无效请求"
                    });
                var payLoad = Base64UrlEncoder.Decode(tokens[1]);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(payLoad);
                var uId = int.TryParse(dict[ClaimTypes.UserData], out _) ? int.Parse(dict[ClaimTypes.UserData]) : 0;
                if (uId <= 0)
                    return Ok(new
                    {
                        mark = false,
                        msg = "无效请求"
                    });
                if(uId == userId)
                    return Ok(new
                    {
                        mark = true,
                        mgs = "已登录"
                    });

                return Ok(new
                {
                    mark = false,
                    msg = "无效请求"
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}