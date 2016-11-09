using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paqueteria.Repository;
using Paqueteria.Model;
namespace Paqueteria.Service
{
    public class EnvioService : IEnvioService
    {
        public IEnvioRepository Repository;

        public EnvioService(IEnvioRepository EnvioRepository)
        {
            Repository = EnvioRepository;
        }



        public EnvioDTO Create(EnvioDTO envio)
        {
            return Repository.Create(envio);
        }

        public EnvioDTO Read(long EnvioId)
        {
            return Repository.Read(EnvioId);
        }
        public IList<EnvioDTO>  ReadFiltro(string Filtro, string Tipo)
        {
            return Repository.ReadFiltro(Filtro, Tipo);
        }

        public EnvioDTO Update(EnvioDTO envio)
        {
            return Repository.Update( envio);
        }

        public EnvioDTO Delete(long EnvioId)
        {
            return Repository.Delete(EnvioId);
        }

        public IList<EnvioDTO> List()
        {
            return Repository.List();
        }
    }
}
