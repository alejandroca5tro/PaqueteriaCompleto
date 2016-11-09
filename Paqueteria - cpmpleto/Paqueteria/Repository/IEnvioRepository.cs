using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paqueteria.Model;
namespace Paqueteria.Repository
{
    public interface IEnvioRepository
    {
        EnvioDTO Create(EnvioDTO envio);
        EnvioDTO Read(long EnvioId);
        IList<EnvioDTO> ReadFiltro(string Filtro, string Tipo);
        EnvioDTO Update(EnvioDTO envio);
        EnvioDTO Delete(long EnvioId);
        IList<EnvioDTO> List();
    }
}
