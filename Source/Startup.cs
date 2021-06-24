using Microsoft.Owin;
using Owin;
using System.Net;

[assembly: OwinStartupAttribute(typeof(MaverickMVCClient.Startup))]
namespace MaverickMVCClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            #if (DEBUG)
            // Ignore any SSL validation errors, should NOT be used on Prod
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            #endif

            // Allow TLS Protocols only
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            
            ConfigureAuth(app);
        }
    }
}
