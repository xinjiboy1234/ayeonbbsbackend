using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ayeonbbsbackend.Models;
using ayeonbbsbackend.Services;
using ayeonbbsbackend.ViewModels;

namespace ayeonbbsbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecondCategoryController : Controller
    {
        private readonly SecondCategoryService _secondCategoryService;

        public SecondCategoryController(SecondCategoryService secondCategoryService)
        {
            _secondCategoryService = secondCategoryService;
        }
        /// <summary>
        /// 根据大分类获取二级分类
        /// </summary>
        /// <param name="fid">大分类ID</param>
        /// <returns></returns>
        [HttpGet("getsecondcategories/{fid}")]
        public ActionResult<IEnumerable<SecondCategory>> GetSecondCategoriesByFirstCategoryId(int fid)
        {
            try
            {
                return Ok(_secondCategoryService.GetSecondCategories(new SecondCategoryViewModel
                {
                    FirstCategory = new FirstCategory
                    {
                        FirstCategoryId = fid
                    }
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok();
            }
        }
        /// <summary>
        /// 获取二级分类，管理页面
        /// </summary>
        /// <param name="secondCategoryViewModel"></param>
        /// <returns></returns>
        [HttpPost("manage/getsecondcategories")]
        public ActionResult GetSecondCategories([FromBody] SecondCategoryViewModel secondCategoryViewModel)
        {
            try
            {
                var result = _secondCategoryService.GetSecondCategories(secondCategoryViewModel,
                    secondCategoryViewModel.PageViewModel.CurrentPage,
                    secondCategoryViewModel.PageViewModel.PageSize);
                var totalCount = _secondCategoryService.GetTotalCountOfSecondCategories(secondCategoryViewModel);

                return Ok(new
                {
                    secondCategories = result,
                    totalCount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok();
            }
        }

        [HttpPost("manage/addsecondcategory")]
        [Authorize]
        public ActionResult AddSecondCategory([FromBody] SecondCategory secondCategory)
        {
            try
            {
                if (secondCategory == null || secondCategory.FirstCategory == null || secondCategory.FirstCategory.FirstCategoryId <= 0)
                {
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据为空"
                    });
                }
                secondCategory.CreateDate = DateTime.Now;
                secondCategory.UpdateDate = DateTime.Now;
                secondCategory.Status = 1;
                var result = _secondCategoryService.AddSecondCategory(secondCategory);
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
                return Ok(new
                {
                    mark = "2",
                    msg = ex.Message
                });
            }
        }

        /// <summary>
        /// 修改大分类
        /// </summary>
        /// <returns></returns>
        [HttpPost("manage/changesecondcategory")]
        public ActionResult ChangeSecondCategory([FromBody]SecondCategory secondCategory)
        {
            try
            {
                if (secondCategory == null || secondCategory.SecondCategoryId <= 0 || string.IsNullOrEmpty(secondCategory.SecondCategoryName))
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据为空！"
                    });

                if (_secondCategoryService.ChangeSecondCategory(secondCategory))
                    return Ok(new
                    {
                        mark = "1",
                        msg = "成功！"
                    });

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
                    msg = ex.Message
                });
            }
        }

        /// <summary>
        /// 获取二级分类
        /// </summary>
        /// <returns></returns>
        [HttpGet("getallsecondcategories")]
        public ActionResult<IEnumerable<SecondCategory>> GetSecondCategories()
        {
            try
            {
                var result = _secondCategoryService.GetSecondCategories();
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(ex);
            }
        }

        /// <summary>
        /// 删除二级分类
        /// </summary>
        /// <param name="secondCategory"></param>
        /// <returns></returns>
        [HttpPost("manage/deletesecondcategory")]
        [Authorize]
        public ActionResult DeleteSecondCategory([FromBody]SecondCategory secondCategory)
        {
            try
            {
                if (secondCategory == null || secondCategory.SecondCategoryId <= 0)
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据为空！"
                    });
                secondCategory.Status = 3; // 改变状态
                if (_secondCategoryService.ChangeSecondCategoryStatus(secondCategory))
                {
                    return Ok(new
                    {
                        mark = "1",
                        msg = "成功！"
                    });
                }
                return Ok();
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
        /// 恢复二级分类状态
        /// </summary>
        /// <param name="secondCategory"></param>
        /// <returns></returns>
        [HttpPost("manage/recoverysecondcategorystatus")]
        [Authorize]
        public ActionResult RecoverySecondCategoryStatus([FromBody]SecondCategory secondCategory)
        {
            try
            {
                if (secondCategory == null || secondCategory.SecondCategoryId <= 0)
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据为空！"
                    });
                secondCategory.Status = 1; // 改变状态
                if (_secondCategoryService.ChangeSecondCategoryStatus(secondCategory))
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
    }
}