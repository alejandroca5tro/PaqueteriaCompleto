using Paqueteria;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaqueteriaTest
{
    public class PaqDBInitializer : DropCreateDatabaseAlways<PaqDB>
    {
        protected override void Seed(PaqDB context)
        {
            base.Seed(context);
        }
    }
}
