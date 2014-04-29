using System;
using System.Linq;
using System.Web.Mvc;
using XHRSignaling.Models;

namespace XHRSignaling.Controllers
{
    // https://github.com/muaz-khan/XHR-Signaling
    public class HomeController : Controller
    {
        private readonly WebRTCDataContext _db = new WebRTCDataContext();
        public ActionResult Index()
        {
            return View();
        }

        // this action method takes single query-string parameter value
        // it stores it in "Message" colume under "Data" table
        [HttpPost]
        public JsonResult PostData(string message)
        {
            var data = new Data
                           {
                               Message = message,
                               Date = DateTime.Now
                           };

            _db.Datas.InsertOnSubmit(data);
            _db.SubmitChanges();

            // older data must be deleted 
            // otherwise databae file will be full of records!
            DeleteOlderData();
            
            return Json(true);
        }

        // this action method gets latest messages
        [HttpPost]
        public JsonResult GetData()
        {
            var data = _db.Datas.Where(d => d.Date.AddSeconds(2) > DateTime.Now).OrderByDescending(d => d.ID).FirstOrDefault();
            return data != null ? Json(data) : Json(false);
        }

        // this private method deletes old data
        void DeleteOlderData()
        {
            var data = _db.Datas.Where(d => d.Date.AddSeconds(2) < DateTime.Now);
            foreach(var d in data)
            {
                _db.Datas.DeleteOnSubmit(d);
            }
            _db.SubmitChanges();
        }
    }
}
