using Maverick.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MaverickMVCClient.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private static readonly string currencyCode = ConfigurationManager.AppSettings["maverick:pointsCurrencyCode"];

        // GET: Profile
        public ActionResult Index()
        {
            var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;
            string accessToken = userClaims?.FindFirst("access_token")?.Value;
            GuestProfile profile = (GuestProfile)Session["MaverickProfile"];
            
            //You get the user's first and last name below:
            ViewBag.Name = userClaims?.FindFirst("name")?.Value;

            // The few custom claims
            ViewBag.Email = userClaims?.FindFirst("email")?.Value;
            ViewBag.Phone = userClaims?.FindFirst("phone")?.Value;
            ViewBag.MemberID = userClaims?.FindFirst("MemberID")?.Value;
            ViewBag.MemberLevel = userClaims?.FindFirst("MemberLevel")?.Value;

            // Get more detailed information from session profile object
            if (profile != null)
            {
                ViewBag.Address = $"{profile.Address1} {profile.Address2}";
                ViewBag.City = profile.City;
                ViewBag.Region = profile.RegionCode ?? profile.Region;
                ViewBag.Country = profile.Country;

                // Get points information
                var pointsInfo = GetPointsInfo(profile.LoyaltyMemberID, currencyCode, accessToken);
                if (pointsInfo != null)
                {
                    ViewBag.TotalPoints = pointsInfo.TotalNumberOfPoints ?? 0;
                    ViewBag.PointsAmount = pointsInfo.TotalAmount ?? 0;
                    ViewBag.PointsCurrency = pointsInfo.CurrencyCode;
                }
            }

            return View();
        }

        private GuestPointsInfo GetPointsInfo(string profileID, string currencyCode, string accessToken)
        {
            GuestPointsInfo pointsInfo = null;
            var maverickAPIClient = new HttpClient();
            maverickAPIClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var apiResponse = maverickAPIClient.GetAsync($"{Startup.apiUri}/points/GetPoints?LoyaltyMemberID={profileID}&CurrencyCode={currencyCode}").Result;

            System.Net.HttpStatusCode statusCode = apiResponse.StatusCode;
            string responseContent;
            if ((int)statusCode != 200 && (int)statusCode != 400)
            {
                if (apiResponse.Content != null)
                    responseContent = apiResponse.Content.ReadAsStringAsync().Result;
                else
                    responseContent = string.Empty;

                if (apiResponse != null)
                    apiResponse.Dispose();

                Response.Redirect($"/?errormessage=" + HttpUtility.UrlEncode($"Maverick Get Points Info operation returned an invalid status code '{statusCode}' : {responseContent}"));
                return null;
            }

            // Deserialize Response
            if ((int)statusCode == 200)
            {
                responseContent = apiResponse.Content.ReadAsStringAsync().Result;
                try
                {
                    pointsInfo = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<GuestPointsInfo>(responseContent, Startup.DeserializationSettings);
                }
                catch (Newtonsoft.Json.JsonException)
                {
                    Response.Redirect($"/?errormessage=" + HttpUtility.UrlEncode($"Unable to deserialize the Maverick Get Points Info response : {responseContent}"));
                    return null;
                }
            }

            return pointsInfo;
        }
    }
}