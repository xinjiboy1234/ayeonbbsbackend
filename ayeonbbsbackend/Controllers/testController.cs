using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ayeonbbsbackend.Models;

namespace ayeonbbsbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class testController : ControllerBase
    {
        #region 获取大分类
        // GET api/values
        [HttpGet("getfirstcategories")]
        public ActionResult<IEnumerable<FirstCategory>> Get()
        {
            try
            {
                var c = new List<FirstCategory>
                {
                    new FirstCategory
                    {
                        FirstCategoryId = 100, FirstCategoryName = "美食", Status = 1, SecondCategories = null
                    },
                    new FirstCategory
                    {
                        FirstCategoryId = 200, FirstCategoryName = "家庭", Status = 1, SecondCategories = null
                    },
                    new FirstCategory
                    {
                        FirstCategoryId = 300, FirstCategoryName = "影视娱乐", Status = 1, SecondCategories = null
                    }
                };
                return c;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion
        
        #region 获取二级分类
        [HttpGet("getsecondcategories/{cid}")]
        public ActionResult<IEnumerable<SecondCategory>> GetSecondCategories(int cid)
        {
            try
            {
                var sc = new List<SecondCategory> {
                    new SecondCategory
                    {
                        SecondCategoryId = 500,
                        SecondCategoryName = "鸡腿",
                        Status = 1,
                        FirstCategory = new FirstCategory {
                            FirstCategoryId = 100,
                            FirstCategoryName = "美食",
                            Status = 1,
                            SecondCategories = null
                        }
                    },
                    new SecondCategory
                    {
                        SecondCategoryId = 7000,
                        SecondCategoryName = "瓜子",
                        Status = 1,
                        FirstCategory = new FirstCategory {
                            FirstCategoryId = 100,
                            FirstCategoryName = "美食",
                            Status = 1,
                            SecondCategories = null
                        }
                    },
                    new SecondCategory
                    {
                        SecondCategoryId = 666,
                        SecondCategoryName = "良率",
                        Status = 1,
                        FirstCategory = new FirstCategory
                        {
                            FirstCategoryId = 200,
                            FirstCategoryName = "家庭",
                            Status = 1,
                            SecondCategories = null
                        }
                    },
                    new SecondCategory
                    {
                        SecondCategoryId = 777,
                        SecondCategoryName = "方法",
                        Status = 1,
                        FirstCategory = new FirstCategory
                        {
                            FirstCategoryId = 200,
                            FirstCategoryName = "家庭",
                            Status = 1,
                            SecondCategories = null
                        }
                    },
                    new SecondCategory
                    {
                        SecondCategoryId = 888,
                        SecondCategoryName = "损失",
                        Status = 1,
                        FirstCategory = new FirstCategory
                        {
                            FirstCategoryId = 300,
                            FirstCategoryName = "影视娱乐",
                            Status = 1,
                            SecondCategories = null
                        }
                    }
                };
                return sc.Where(x => x.FirstCategory.FirstCategoryId == cid).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion

        #region 模拟登录
        [HttpPost("dologin")]
        public ActionResult<UserInfo> Login([FromBody]UserInfo userInfo)
        {
            try
            {
                if (userInfo.LoginId != null)
                {
                    if (userInfo.LoginId == "1"&&userInfo.Password=="1")
                    {
                        return new UserInfo
                        {
                            UserId = 111,
                            UserName = "心机Boy",
                            Avatar = "",
                            Password = userInfo.Password,
                            LoginId = userInfo.LoginId,
                            NickName = "心机Boy"
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return null;
        }
        #endregion
        
        #region 上传图片
        [HttpPost("upload")]
        public ActionResult<Object> UploadFile()
        {
            try
            {
                var cols = Request.Form.Files;
                if (cols == null || cols.Count == 0)
                {
                    return new {error="error"};
                }

                foreach (var file in cols)
                {
                    using (var stream = new FileStream("uploadimg/1.png",FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                return new {result="http://localhost:5000/uploadimg/1.png"};
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
        #endregion

        #region 获取可发表分类
        
        public ActionResult<IEnumerable<SecondCategory>> GetWriteCategories()
        {
            try
            {
                // 返回mock数据
                return new List<SecondCategory>
                {
                    new SecondCategory
                    {
                        SecondCategoryId = 500,
                        SecondCategoryName = "鸡腿",
                        Status = 1,
                        FirstCategory = new FirstCategory {
                            FirstCategoryId = 100,
                            FirstCategoryName = "美食",
                            Status = 1,
                            SecondCategories = null
                        }
                    },
                    new SecondCategory
                    {
                        SecondCategoryId = 7000,
                        SecondCategoryName = "瓜子",
                        Status = 1,
                        FirstCategory = new FirstCategory {
                            FirstCategoryId = 100,
                            FirstCategoryName = "美食",
                            Status = 1,
                            SecondCategories = null
                        }
                    },
                    new SecondCategory
                    {
                        SecondCategoryId = 888,
                        SecondCategoryName = "损失",
                        Status = 1,
                        FirstCategory = new FirstCategory
                        {
                            FirstCategoryId = 300,
                            FirstCategoryName = "影视娱乐",
                            Status = 1,
                            SecondCategories = null
                        }
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
        #endregion
        
        
    }
}