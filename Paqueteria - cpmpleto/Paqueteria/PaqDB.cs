using Paqueteria.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paqueteria
{
    public class PaqDB: DbContext
    {
        public PaqDB(SqlConnection connection)
            : base(connection, false)
        {

        }
        public PaqDB(bool res)
            :base("PaqDB")
        {

        }


        public DbSet<Paquete> Paquetes { get; set; }
        public DbSet<Envio> Envios { get; set; }

        [ThreadStatic]
        public static SqlConnection connection = null;
    }
}
