using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paqueteria.Model
{
    public class PaqueteException : Exception
    {
        public PaqueteException()
            : base()
        {

        }
        public PaqueteException(string msg)
            : base(msg)
        {

        }
    }
}
