using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paqueteria.Model;

namespace Paqueteria.Repository
{
    public interface IPaqueteRepository
    {
        PaqueteDTO Create(PaqueteDTO paquete);
        PaqueteDTO Read(long paqueteId);
        IList<PaqueteDTO> List();
        IList<PaqueteDTO> List(long envioId);
        PaqueteDTO Update(PaqueteDTO paquete);
        PaqueteDTO Delete(long paqueteId);

    }
}
