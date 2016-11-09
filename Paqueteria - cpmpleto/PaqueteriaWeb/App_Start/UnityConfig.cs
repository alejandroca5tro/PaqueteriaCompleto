using Microsoft.Practices.Unity;
using Paqueteria;
using System.Web.Http;
using Unity.WebApi;

namespace PaqueteriaWeb
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            UnityContainerFactory unityFactory = new UnityContainerFactory();
            var container = unityFactory.GetInstance();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}