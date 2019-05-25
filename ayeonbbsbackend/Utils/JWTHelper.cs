using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ayeonbbsbackend.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ayeonbbsbackend.Utils
{
    public class JWTHelper
    {
        private readonly IConfiguration _configuration;

        public JWTHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetToken(UserInfo user)
        {
            // push the user’s name into a claim, so we can identify the user later on.
            var claims = new[]
            {
                //new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Name, user.UserId.ToString()),
                new Claim(ClaimTypes.UserData, user.UserId.ToString()),
                //new Claim(ClaimTypes.Role, admin)//在这可以分配用户角色，比如管理员 、 vip会员 、 普通用户等
            };
            //sign the token using a secret key.This secret will be shared between your API and anything that needs to check that the token is legit.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["mytoken"])); // 获取密钥
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //凭证 ，根据密钥生成
            //.NET Core’s JwtSecurityToken class takes on the heavy lifting and actually creates the token.
            /**
             * Claims (Payload)
               Claims 部分包含了一些跟这个 token 有关的重要信息。 JWT 标准规定了一些字段，下面节选一些字段:

               iss: The issuer of the token，token 是给谁的  发送者
               aud: 接收的
               sub: The subject of the token，token 主题
               exp: Expiration Time。 token 过期时间，Unix 时间戳格式
               iat: Issued At。 token 创建时间， Unix 时间戳格式
               jti: JWT ID。针对当前 token 的唯一标识
               除了规定的字段外，可以包含其他任何 JSON 兼容的字段。
             **/
            var token = new JwtSecurityToken(
                issuer: "bbs.soarwhale.com",
                audience: "bbs.soarwhale.com",
                claims: claims,
                expires: DateTime.Now.AddDays(3).ToUniversalTime(),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #region 验证JWT
        public bool JWTvalidate(string headeAuthorization)
        {
            try
            {
                if (string.IsNullOrEmpty(headeAuthorization))
                {
                    return false;
                }
                if (headeAuthorization.ToString().Split("Bearer ").Length < 2)
                {
                    return false;
                }
                if (headeAuthorization.ToString().Split("Bearer ")[1].Trim().Split('.').Length < 3)
                {
                    return false;
                }
                var tokens = headeAuthorization.ToString().Split("Bearer ")[1].Trim().Split('.'); // 获取JWT段
                var hs256 = new HMACSHA256(Encoding.UTF8.GetBytes(_configuration["mytoken"])); //获取加密算法对象
                                                                                               // 进行验证
                var flag = string.Equals(tokens[2], Base64UrlEncoder
                    .Encode(hs256.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(tokens[0], ".", tokens[1])))));
                return flag;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int GetJWTUserData(string headeAuthorization)
        {
            try
            {
                if (JWTvalidate(headeAuthorization))
                {
                    var tokens = headeAuthorization.ToString().Split("Bearer ")[1].Trim().Split('.'); // 获取JWT段
                    var hs256 = new HMACSHA256(Encoding.UTF8.GetBytes(_configuration["mytoken"])); //获取加密算法对象
                    var userId = 0;
                    // 进行验证
                    var payLoad = Base64UrlEncoder.Decode(tokens[1]);
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(payLoad);
                    userId = int.TryParse(dict[ClaimTypes.UserData], out _) ? int.Parse(dict[ClaimTypes.UserData]) : 0;
                    return userId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// 每次请求更新Token
        /// </summary>
        /// <param name="headeAuthorization"></param>
        /// <returns></returns>
        public string RefreshJWTToken(string headeAuthorization)
        {
            try
            {
                if (string.IsNullOrEmpty(headeAuthorization))
                    return string.Empty;
                if (headeAuthorization.Split("Bearer ").Length < 2)
                    return string.Empty;
                var tokens = headeAuthorization.Split("Bearer ")[1].Trim().Split('.'); // 获取JWT段
                if (tokens.Length < 3)
                    return string.Empty;
                var payLoad = Base64UrlEncoder.Decode(tokens[1]);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(payLoad);
                var userId = int.TryParse(dict[ClaimTypes.UserData], out _) ? int.Parse(dict[ClaimTypes.UserData]) : 0;
                if (userId <= 0)
                {
                    return string.Empty;
                }
                return GetToken(new UserInfo { UserId = userId });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
