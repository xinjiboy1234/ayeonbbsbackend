using System;
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
    public class FirstCategoryController : ControllerBase
    {
        private readonly FirstCategoryService _firstCategoryService;

        public FirstCategoryController(FirstCategoryService firstCategoryService)
        {
            _firstCategoryService = firstCategoryService;
        }
        /// <summary>
        /// 获取大分类
        /// </summary>
        /// <param name="firstCategoryViewModel"></param>
        /// <returns></returns>
        [HttpPost("getfirstcategories")]
        public ActionResult GetFirstCategories([FromBody] FirstCategoryViewModel firstCategoryViewModel)
        {
            try
            {
                var result = _firstCategoryService.GetFirstCategories(firstCategoryViewModel,
                    firstCategoryViewModel.PageViewModel.CurrentPage,
                    firstCategoryViewModel.PageViewModel.PageSize);
                var totalCount = _firstCategoryService.GetTotalCountOfFristCategories(firstCategoryViewModel);
                return Ok(new
                {
                    firstCategories = result,
                    totalCount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok();
            }
        }
        /// <summary>
        /// 获取全部使用中的帖子， 前台菜单使用
        /// </summary>
        /// <returns></returns>
        [HttpGet("getallfirstcategories")]
        public ActionResult GetFirstCategories()
        {
            try
            {
                return Ok(_firstCategoryService.GetFirstCategories());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok();
            }
        }
        /// <summary>
        /// 获取大分类，单个信息
        /// </summary>
        /// <param name="firstCategory"></param>
        /// <returns></returns>
        [HttpGet("getFirstCategory")]
        public ActionResult GetFirstCategory(FirstCategory firstCategory)
        {
            try
            {
                _firstCategoryService.GetFirstCategory(firstCategory);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Ok();
        }
        /// <summary>
        /// 添加大分类
        /// </summary>
        /// <param name="firstCategory"></param>
        /// <returns></returns>
        [HttpPost("manage/addfirstcategory")]
        [Authorize]
        public ActionResult AddFirstCategory([FromBody] FirstCategory firstCategory)
        {
            try
            {
                if (firstCategory == null)
                    return null;
                if (string.IsNullOrEmpty(firstCategory.FirstCategoryName))
                    return Ok(new {mark = "2", msg = "分类名称不可为空"});
                //初始化数据
                firstCategory.Status = 1;
                firstCategory.CreateDate = DateTime.Now;
                firstCategory.UpdateDate = DateTime.Now;
                var fcId = _firstCategoryService.AddFirstCategory(firstCategory);
                return Ok(new {mark = "1", msg = "success", fcid = fcId});
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok();
            }
        }
        /// <summary>
        /// 修改大分类
        /// </summary>
        /// <returns></returns>
        [HttpPost("manage/changefirstcategory")]
        public ActionResult ChangeFirstCategory([FromBody]FirstCategory firstCategory)
        {
            try
            {
                if (firstCategory == null || firstCategory.FirstCategoryId <= 0 || string.IsNullOrEmpty(firstCategory.FirstCategoryName))
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据为空！"
                    });

                if(_firstCategoryService.ChangeFirstCategory(firstCategory))
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
                return Ok();
            }
        }

        /// <summary>
        /// 删除大分类
        /// </summary>
        /// <param name="firstCategory"></param>
        /// <returns></returns>
        [HttpPost("manage/deletefirstcategory")]
        [Authorize]
        public ActionResult DeleteFirstCategory([FromBody] FirstCategory firstCategory)
        {
            try
            {
                if (firstCategory == null || firstCategory.FirstCategoryId <= 0)
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据为空！"
                    });
                firstCategory.Status = 3; // 改变状态
                if (_firstCategoryService.ChangeFirstCategoryStatus(firstCategory))
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
                return Ok();
            }
        }

        /// <summary>
        /// 恢复帖子
        /// </summary>
        /// <param name="firstCategory"></param>
        /// <returns></returns>
        [HttpPost("manage/recoveryfirstcategorystatus")]
        [Authorize]
        public ActionResult RecoveryFirstCategoryStatus([FromBody]FirstCategory firstCategory)
        {
            try
            {
                if (firstCategory == null || firstCategory.FirstCategoryId <= 0)
                    return Ok(new
                    {
                        mark = "2",
                        msg = "请求数据为空！"
                    });
                firstCategory.Status = 1; // 改变状态
                if (_firstCategoryService.ChangeFirstCategoryStatus(firstCategory))
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
                return Ok();
            }
        }
    }
}