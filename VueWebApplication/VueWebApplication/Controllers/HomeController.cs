using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VueWebApplication.Caching;
using VueWebApplication.Models;

namespace VueWebApplication.Controllers
{
    public class HomeController : Controller
    {
        //private static List<VueTestModel> testModel = new List<VueTestModel>();

        public HomeController()
        {

        }

        //----------------------------------------------------------------------------------------------



        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }



        public ActionResult TryVue()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoadData()
        {
            RedisCacheProvider cacheProvider = new RedisCacheProvider();

            List<VueTestModel> basicData = new List<VueTestModel>();

            //Dictionary<string, VueTestModel> dic = new Dictionary<string, VueTestModel>();

            if (!cacheProvider.Exists("VueTest:LoadData:VueTestModel"))
            {
                basicData = new List<VueTestModel>
                {
                    new VueTestModel { Name="rico", Age=35, Birthday=DateTime.Now.ToShortDateString() },
                    new VueTestModel {Name="sherry",Age=30,Birthday=DateTime.Now.AddDays(-1).ToShortDateString() },
                    new VueTestModel {Name="fifi",Age=4,Birthday=DateTime.Now.AddMonths(-10).ToShortDateString() },
                    new VueTestModel {Name="rico2",Age=34,Birthday=DateTime.Now.AddDays(-2).ToShortDateString() },
                    new VueTestModel {Name="sherry2",Age=29,Birthday=DateTime.Now.AddDays(-2).ToShortDateString() },
                    new VueTestModel {Name="fifi2",Age=3,Birthday=DateTime.Now.AddMonths(-20).ToShortDateString() },
                    new VueTestModel {Name="rico3",Age=33,Birthday=DateTime.Now.AddDays(-3).ToShortDateString() },
                    new VueTestModel {Name="sherry3",Age=28,Birthday=DateTime.Now.AddDays(-3).ToShortDateString() },
                    new VueTestModel {Name="fifi3",Age=2,Birthday=DateTime.Now.AddMonths(-30).ToShortDateString()},
                    new VueTestModel {Name="Vue",Age=1,Birthday=DateTime.Now.AddMonths(-60).ToShortDateString() }
                };
                
            }
            

            var cache = cacheProvider.ListGetOrSave<VueTestModel>("VueTest:LoadData:VueTestModel", basicData);
            //var cache = cacheProvider.GetOrSet<List<VueTestModel>>("VueTest:VueTestModel", basicData);
            
            var result = JsonConvert.SerializeObject(cache);

            return Json(result);
        }

        [HttpGet]
        public ActionResult CreateTestData(string name, int age, string birthday)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(birthday) || age == null || age <= 0)
            {
                return Json(new { success = false, message = "請輸入正確內容" }, JsonRequestBehavior.AllowGet);
            }

            RedisCacheProvider cacheProvider = new RedisCacheProvider();
            cacheProvider.ListInsert(
                "VueTest:LoadData:VueTestModel",
                new VueTestModel { Name = name, Age = age, Birthday = birthday });

            return Json(new { success = true, message = "新增完成" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UpdateTestData(long index, string name, int age, string birthday)
        {
            RedisCacheProvider cacheProvider = new RedisCacheProvider();

            //if (string.IsNullOrEmpty(name))
            //{
            //    return Json(new { success = false, message = "請輸入姓名" }, JsonRequestBehavior.AllowGet);
            //}
            //if (!cache.Where(x => x.Name.ToLower() == name.ToLower()).Any())
            //{
            //    return Json(new { success = false, message = "找不到資料" }, JsonRequestBehavior.AllowGet);
            //}
            //var data = cache.Where(x => x.Name.ToLower() == name.ToLower())
            //    .FirstOrDefault();

            cacheProvider.ListUpdate(
                index, 
                "VueTest:LoadData:VueTestModel", 
                new VueTestModel { Name = name, Age = age, Birthday = birthday, Changed = false });
            

            return Json(new { success = true, message = "更新完成" }, JsonRequestBehavior.AllowGet);
        }

    }
}