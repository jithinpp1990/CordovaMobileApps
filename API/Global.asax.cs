using CCBankWebAPI.Process;
using Microsoft.Practices.Unity;
using System.Web.Http;
using System.Web.Http.Dependencies;

namespace CCBankWebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
