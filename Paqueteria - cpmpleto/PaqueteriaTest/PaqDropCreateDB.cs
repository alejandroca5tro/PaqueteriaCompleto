using Paqueteria;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaqueteriaTest
{
    public class PaqDropCreateDB : PaqDB
    {
        public PaqDropCreateDB(SqlConnection connection)
            : base(connection)
        {

        }
        public PaqDropCreateDB(bool res)
            : base(res)
        {
            Database.SetInitializer<PaqDB>(new PaqDBInitializer());
        }
    }
}
