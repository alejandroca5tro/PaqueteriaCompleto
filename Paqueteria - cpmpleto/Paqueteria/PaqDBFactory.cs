using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paqueteria
{
    public class PaqDBFactory : IPaqDBFactory
    {
      
        public PaqDB GetInstance()
        {
            return new PaqDB(PaqDB.connection);
        }
    }
}
