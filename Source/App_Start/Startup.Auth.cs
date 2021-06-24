using System;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

using System.Net.Http;
using System.Security.Claims;
using Maverick.Models;
using Microsoft.Owin.Security.Notifications;
using System.Web;
using Microsoft.AspNet.Identity;
using IdentityModel;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace MaverickMVCClient
{
    public partial class Startup
    {
        // These values are stored in Web.config. Make sure you update them!
        private static readonly string clientId = ConfigurationManager.AppSettings["maverick:ClientId"];
        private static readonly string authority = ConfigurationManager.AppSettings["maverick:AuthUri"];
        private static readonly string clientSecret = ConfigurationManager.AppSettings["maverick:ClientSecret"];
        private static readonly string redirectUri = ConfigurationManager.AppSettings["maverick:RedirectUri"];
        private static readonly string postLogoutRedirectUri = ConfigurationManager.AppSettings["maverick:PostLogoutRedirectUri"];
        public static readonly string apiUri = ConfigurationManager.AppSettings["maverick:APIUri"];
        public static readonly string wsHotelGroupCode = ConfigurationManager.AppSettings["windsurfer:hotelGroupCode"];

        public static readonly Newtonsoft.Json.JsonSerializerSettings DeserializationSettings = new Newtonsoft.Json.JsonSerializerSettings
        {
            DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
            ContractResolver = new Microsoft.Rest.Serialization.ReadOnlyJsonContractResolver(),
            Converters = new System.Collections.Generic.List<Newtonsoft.Json.JsonConverter>
                    { new Microsoft.Rest.Serialization.Iso8601TimeSpanConverter() }
        };

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions ());

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Authority = authority,
                RedirectUri =  string.Format(redirectUri, wsHotelGroupCode),
                ResponseType = OpenIdConnectResponseType.Code,
                ResponseMode = OpenIdConnectResponseMode.Query,
                Scope = OpenIdConnectScope.OpenIdProfile + " profileID restApi",
                PostLogoutRedirectUri = string.Format(postLogoutRedirectUri, wsHotelGroupCode),
                Caption = "Maverick",
                RedeemCode = true,
                SaveTokens = false,
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    ValidateIssuer = true,
                    ValidIssuer = authority
                },

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = OnAuthorizationCodeReceived,

                    RedirectToIdentityProvider = OnRedirectToIdentityProvider,

                    AuthenticationFailed = OnAuthenticationFailed,

                    SecurityTokenValidated = OnSecurityTokenValidated
                },
            });
        }

        private Task OnSecurityTokenValidated(SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            var identity = context.AuthenticationTicket.Identity;
            //Add the access_token for future API calls
            identity.AddClaim(new Claim("access_token", context.ProtocolMessage.AccessToken));

            // Get user profile from Maverick
            string profileID = string.Empty;
            var profileIDClaim = identity.FindFirst("profileID");
            if (profileIDClaim != null)
                profileID = profileIDClaim.Value;

            if (!string.IsNullOrEmpty(profileID))
            {
                GuestProfile profile = null;
                
                var maverickAPIClient = new HttpClient();
                maverickAPIClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.ProtocolMessage.AccessToken);

                var apiResponse = maverickAPIClient.GetAsync($"{apiUri}/guest?GuestID={profileID}").Result;

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

                    context.Response.Redirect($"/?errormessage=" + HttpUtility.UrlEncode($"Maverick Get Profile Info operation returned an invalid status code '{statusCode}' : {responseContent}"));
                    return Task.CompletedTask;
                }

                // Deserialize Response
                if ((int)statusCode == 200)
                {
                    responseContent = apiResponse.Content.ReadAsStringAsync().Result;
                    try
                    {
                        profile = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<GuestProfile>(responseContent, Startup.DeserializationSettings);
                    }
                    catch (Newtonsoft.Json.JsonException)
                    {
                        context.Response.Redirect($"/?errormessage=" + HttpUtility.UrlEncode($"Unable to deserialize the Maverick Get Profile Info response : {responseContent}"));
                        return Task.CompletedTask;

                    }
                }

                // Save the Maverick Profile and points info in session
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                    HttpContext.Current.Session["MaverickProfile"] = profile;

                // Override some of profile info as custom claims
                var nameClaim = identity.FindFirst("name");
                if (nameClaim != null && !string.IsNullOrEmpty($"{profile.FirstName} {profile.LastName}"))
                    identity.RemoveClaim(nameClaim);
                var emailClaim = identity.FindFirst("email");
                if (emailClaim != null && !string.IsNullOrEmpty(profile.Email))
                    identity.RemoveClaim(emailClaim);

                // Add the Maverick Profile Information
                var claims = new List<Claim>
                {
                    new Claim("name", $"{profile.FirstName} {profile.LastName}"),
                    new Claim("firstname", profile.FirstName),
                    new Claim("lastname", profile.LastName),
                    new Claim("email", profile.Email),
                    new Claim("phone", profile.PhoneNumber),
                    new Claim("MemberID", profile.LoyaltyMemberID),
                    new Claim("MemberLevel", profile.MemberLevel)
                };

                // Add all the custom claims
                identity.AddClaims(claims);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Perform processing upon receiving auth code from authentication server and then fetch Maverick profile information
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification context)
        {
            // get code_verifier
            var codeVerifier = RetrieveCodeVerifier(context);

            // attach code_verifier
            context.TokenEndpointRequest.SetParameter("code_verifier", codeVerifier);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handle redirect to authentication server requests to add teh id_token hint in case of logout calls
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task OnRedirectToIdentityProvider(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            // Generate Code challenge for PKCE
            if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
            {
                // generate code verifier and code challenge
                var codeVerifier = CryptoRandom.CreateUniqueId(32);

                string codeChallenge;
                using (var sha256 = SHA256.Create())
                {
                    var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                    codeChallenge = Base64Url.Encode(challengeBytes);
                }

                // set code_challenge parameter on authorization request
                context.ProtocolMessage.SetParameter("code_challenge", codeChallenge);
                context.ProtocolMessage.SetParameter("code_challenge_method", "S256");

                // remember code verifier in cookie (adapted from OWIN nonce cookie)
                // see: https://github.com/scottbrady91/Blog-Example-Classes/blob/master/AspNetFrameworkPkce/ScottBrady91.BlogExampleCode.AspNetPkce/Startup.cs#L85
                RememberCodeVerifier(context, codeVerifier);
            }

            // If signing out, add the id_token_hint
            if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
            {
                var idTokenClaim = context.OwinContext.Authentication.User.FindFirst("id_token");

                if (idTokenClaim != null)
                {
                    context.ProtocolMessage.IdTokenHint = idTokenClaim.Value;
                }

            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handle failed authentication requests by redirecting the user to the home page with an error in the query string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }


        private void RememberCodeVerifier(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> n, string codeVerifier)
        {
            var properties = new AuthenticationProperties();
            properties.Dictionary.Add("cv", codeVerifier);
            n.Options.CookieManager.AppendResponseCookie(
                n.OwinContext,
                GetCodeVerifierKey(n.ProtocolMessage.State),
                Convert.ToBase64String(Encoding.UTF8.GetBytes(n.Options.StateDataFormat.Protect(properties))),
                new CookieOptions
                {
                    SameSite = Microsoft.Owin.SameSiteMode.None,
                    HttpOnly = false,
                    Secure = true,
                    Expires = DateTime.UtcNow + n.Options.ProtocolValidator.NonceLifetime
                });
        }

        private string RetrieveCodeVerifier(AuthorizationCodeReceivedNotification n)
        {
            string key = GetCodeVerifierKey(n.ProtocolMessage.State);

            string codeVerifierCookie = n.Options.CookieManager.GetRequestCookie(n.OwinContext, key);
            if (codeVerifierCookie != null)
            {
                var cookieOptions = new CookieOptions
                {
                    SameSite = Microsoft.Owin.SameSiteMode.None,
                    HttpOnly = false,
                    Secure = true
                };

                n.Options.CookieManager.DeleteCookie(n.OwinContext, key, cookieOptions);
            }

            var cookieProperties = n.Options.StateDataFormat.Unprotect(Encoding.UTF8.GetString(Convert.FromBase64String(codeVerifierCookie)));
            cookieProperties.Dictionary.TryGetValue("cv", out var codeVerifier);

            return codeVerifier;
        }

        private string GetCodeVerifierKey(string state)
        {
            using (var hash = SHA256.Create())
            {
                return OpenIdConnectAuthenticationDefaults.CookiePrefix + "cv." + Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(state)));
            }
        }
    }
}