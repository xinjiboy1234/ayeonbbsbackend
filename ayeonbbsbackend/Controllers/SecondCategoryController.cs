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
        /// ���ݴ�����ȡ��������
        /// </summary>
        /// <param name="fid">�����ID</param>
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
        /// ��ȡ�������࣬����ҳ��
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
                        msg = "��������Ϊ��"
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
                        msg = "�ɹ�"
                    });
                return Ok(new
                {
                    mark = "2",
                    msg = "ʧ��"
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
        /// �޸Ĵ����
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
                        msg = "��������Ϊ�գ�"
                    });

                if (_secondCategoryService.ChangeSecondCategory(secondCategory))
                    return Ok(new
                    {
                        mark = "1",
                        msg = "�ɹ���"
                    });

                return Ok(new
                {
                    mark = "2",
                    msg = "ʧ�ܣ�"
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
        /// ��ȡ��������
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
        /// ɾ����������
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
                        msg = "��������Ϊ�գ�"
                    });
                secondCategory.Status = 3; // �ı�״̬
                if (_secondCategoryService.ChangeSecondCategoryStatus(secondCategory))
                {
                    return Ok(new
                    {
                        mark = "1",
                        msg = "�ɹ���"
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
        /// �ָ���������״̬
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
                        msg = "��������Ϊ�գ�"
                    });
                secondCategory.Status = 1; // �ı�״̬
                if (_secondCategoryService.ChangeSecondCategoryStatus(secondCategory))
                {
                    return Ok(new
                    {
                        mark = "1",
                        msg = "�ɹ���"
                    });
                }
                return Ok(new
                {
                    mark = "2",
                    msg = "ʧ��"
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