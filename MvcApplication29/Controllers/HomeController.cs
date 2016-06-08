using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageShare.Data;
using MvcApplication29.Models;

namespace MvcApplication29.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var manager = new ImageShareManager(Properties.Settings.Default.ConStr);
            var viewModel = new IndexViewModel();
            viewModel.MostRecent = manager.GetFiveMostRecent();
            viewModel.MostPopular = manager.GetFiveMostPopular();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase image, string firstName, string lastName)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            image.SaveAs(Server.MapPath("~/Images/") + fileName);
            Image newImage = new Image
            {
                FirstName = firstName,
                LastName = lastName,
                ImageFileName = fileName,
            };
            var manager = new ImageShareManager(Properties.Settings.Default.ConStr);
            manager.AddImage(newImage);
            var viewModel = new UploadViewModel();
            viewModel.Image = newImage;
            viewModel.HostName = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
            return View(viewModel);
        }

        public ActionResult ShowImage(int id)
        {
            var manager = new ImageShareManager(Properties.Settings.Default.ConStr);
            manager.IncrementCount(id);
            var image = manager.GetImage(id);
            var viewModel = new ShowImageViewModel();
            viewModel.Image = image;
            return View(viewModel);
        }

        public ActionResult GetCount(int id)
        {
            var manager = new ImageShareManager(Properties.Settings.Default.ConStr);
            Image image = manager.GetImage(id);
            return Json(new { viewCount = image.ViewCount }, JsonRequestBehavior.AllowGet);
        }

    }
}
