using Paqueteria.Model;
using Paqueteria.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PaqueteriaWeb.Controllers
{
    public class PaqueteController: ApiController
    {
        private IPaqueteService _paqueteService;
        public PaqueteController(IPaqueteService paqueteService)
        {
            _paqueteService = paqueteService;
        }
        // GET api/paquete
        public IEnumerable<PaqueteDTO> Get()
        {
            return _paqueteService.List();
        }

        // GET api/paquete/5
        public PaqueteDTO Get(int id)
        {
            return _paqueteService.Read(id);
        }

        // POST api/paquete
        public PaqueteDTO Post(PaqueteDTO value)
        {
            return _paqueteService.Create(value);
        }

        // PUT api/values/5
        public PaqueteDTO Put(int id, PaqueteDTO value)
        {
            value.IncidenciaId = id;
            return _paqueteService.Update(value);
        }

        // DELETE api/values/5
        public PaqueteDTO Delete(int id)
        {
            return _paqueteService.Delete(id);
        }
    }
}