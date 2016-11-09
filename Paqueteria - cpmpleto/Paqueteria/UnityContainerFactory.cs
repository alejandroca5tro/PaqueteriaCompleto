using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Paqueteria.Conversores;
using Paqueteria.Repository;
using Paqueteria.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paqueteria
{
    public class UnityContainerFactory
    {
        public virtual IPaqDBFactory CreateDBFactory()
        {
            return new PaqDBFactory();
        }

        public virtual InterceptionBehavior CreateInterceptionBehavior()
        {
            return new InterceptionBehavior(new InterceptorTransactionBehavior());
        }


        public IUnityContainer GetInstance()
        {
            UnityContainer container = new UnityContainer();

            container.AddNewExtension<Interception>();

            Interceptor interceptor = new Interceptor(new InterfaceInterceptor());
            InterceptionBehavior interceptionBehavior = CreateInterceptionBehavior();

            // Envios
            container.RegisterType<IEnvioRepository, EnvioRepository>(interceptor, interceptionBehavior);
            container.RegisterType<IEnvioService, EnvioService>(interceptor, interceptionBehavior);

            // Paquetes
            container.RegisterType<IPaqueteRepository, PaqueteRepository>(interceptor, interceptionBehavior);
            container.RegisterType<IPaqueteService, PaqueteService>(interceptor, interceptionBehavior);

            // Conversores
            PaqueteConversor paqueteConversor = new PaqueteConversor();
            EnvioConversor envioConversor = new EnvioConversor(paqueteConversor);
            container.RegisterInstance(typeof(PaqueteConversor), paqueteConversor);
            container.RegisterInstance(typeof(EnvioConversor), envioConversor);

            // dbFactory

            container.RegisterType<IPaqDBFactory, PaqDBFactory>();
            return container;
        }
    }
}
