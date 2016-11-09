using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paqueteria.Model;
using Paqueteria.Repository;

namespace Paqueteria.Service
{
    public class PaqueteService: IPaqueteService
    {
        private IPaqueteRepository _paqueteRepository;
        public PaqueteService(IPaqueteRepository paqueteRepository)
        {
            _paqueteRepository = paqueteRepository;
        }
        public PaqueteDTO Create(PaqueteDTO paquete)
        {
            return _paqueteRepository.Create(paquete);
        }

        public PaqueteDTO Read(long paqueteId)
        {
            return _paqueteRepository.Read(paqueteId);
        }

        public IList<PaqueteDTO> List()
        {
            return _paqueteRepository.List();
        }

        public IList<PaqueteDTO> List(long envioId)
        {
            return _paqueteRepository.List(envioId);
        }

        public PaqueteDTO Update(PaqueteDTO paquete)
        {
            return _paqueteRepository.Update(paquete);
        }

        public PaqueteDTO Delete(long paqueteId)
        {
            return _paqueteRepository.Delete(paqueteId);
        }
    }
}
