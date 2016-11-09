using Paqueteria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaqueteriaTest
{
    public class UnityContainerTestingFactory : UnityContainerFactory
    {
        public override IPaqDBFactory CreateDBFactory()
        {
            return new PaqDropCreateDBFactory();
        }
    }
}
