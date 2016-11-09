using Paqueteria;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaqueteriaTest
{
    public class PaqDropCreateDBFactory: IPaqDBFactory
    {
        public PaqDB GetInstance()
        {
            return new PaqDropCreateDB(true);
        }
    }
}
