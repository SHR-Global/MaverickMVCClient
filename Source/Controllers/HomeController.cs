using System.Configuration;
using System.Web.Mvc;

namespace MaverickMVCClient.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private readonly string wsIBEUri = ConfigurationManager.AppSettings["windsurfer:IBEUri"];
        private readonly string wsHotelGroupID = ConfigurationManager.AppSettings["windsurfer:hotelGroupID"];
        private readonly string wsHotelCode = ConfigurationManager.AppSettings["windsurfer:hotelCode"];
        public ActionResult Index()
        {
            // If the user is logged in pass the performAuthCheck param to IBE for auto-login
            var authCheckParam = (Request.IsAuthenticated && User != null && User.Identity != null ? "&performAuthCheck=true" : "");

            ViewBag.IBEURLChain = $"{wsIBEUri}/default.aspx?hgID={wsHotelGroupID}{authCheckParam}";
            ViewBag.IBEURLHotel = $"{wsIBEUri}/index.aspx?pcode={wsHotelCode}&hgID={wsHotelGroupID}{authCheckParam}";
            
            return View();
        }
    }
}