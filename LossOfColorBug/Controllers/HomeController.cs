using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LossOfColorBug.Services;

namespace LossOfColorBug.Controllers {
    public class HomeController : Controller {

        private ThumbnailService _thumbnailService;

        public HomeController() {
            _thumbnailService = new ThumbnailService();
        }

        public ActionResult Index() {
            return View();
        }
        public ActionResult GetThumbnail(string file) {
            var imageStream = new MemoryStream();
            using (Image image = _thumbnailService.GetImage(Server.MapPath(file))) {
                using (
                    var thumbnail = _thumbnailService.GetThumbnailImage(image, 800, 800,
                                                                        ThumbnailService.ResizeMode.Loose, Color.White)) {
                    thumbnail.Save(imageStream, ImageFormat.Jpeg);
                }
            }
            imageStream.Position = 0;
            return File(imageStream, "image/jpeg");
        }
    }
}