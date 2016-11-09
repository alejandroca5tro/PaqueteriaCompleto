using Paqueteria.Model;
using Paqueteria.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PaqueteriaWeb.Controllers
{
    public class EnvioController : ApiController
    {
        private IEnvioService _envioService;
        private IPaqueteService _paqueteService;
        public EnvioController(IEnvioService envioService, IPaqueteService paqueteService)
        {
            _envioService = envioService;
            _paqueteService = paqueteService;
        }
        // GET api/envio
        public IEnumerable<EnvioDTO> Get()
        {
            return _envioService.List();
        }

        // GET api/envio/5
        public EnvioDTO Get(int id)
        {
            return _envioService.Read(id);
        }
        //GET api/envio/filtro
        [Route("~/api/envio/filtro/{Filtro}/{Tipo}/")]
        public IEnumerable<EnvioDTO> ReadFiltroFiltro(string Filtro,string Tipo)
        {
            return _envioService.ReadFiltro(Filtro,Tipo);
        }

        // POST api/envio
        public EnvioDTO Post(EnvioDTO value)
        {
            return _envioService.Create(value);
        }

        // PUT api/envio/5
        public EnvioDTO Put(int id, EnvioDTO value)
        {
            value.EnvioId = id;
            return _envioService.Update(value);
        }

        // DELETE api/envio/5
        public EnvioDTO Delete(int id)
        {
            return _envioService.Delete(id);
        }
        // GET api/envio/5/paquete
        [Route("~/api/envio/{id}/paquete")]
        public IEnumerable<PaqueteDTO> GetListPaquetes(int id)
        {
            return _paqueteService.List(id);
        }
    }
}