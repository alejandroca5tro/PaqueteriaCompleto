using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paqueteria.Model;
using Paqueteria.Conversores;

namespace Paqueteria.Repository
{
    public class PaqueteRepository : IPaqueteRepository
    {
        private PaqueteConversor paqueteConversor;
        private IPaqDBFactory dbFactory;
        public PaqueteRepository(PaqueteConversor paqueteConversor, IPaqDBFactory dbFactory)
        {
            this.paqueteConversor = paqueteConversor;
            this.dbFactory = dbFactory;
        }
        public PaqueteDTO Create(PaqueteDTO paquete)
        {
            Paquete entity = paqueteConversor.DTO2Entity(paquete);
            PaqueteDTO dto = null;
            using (var ctx = dbFactory.GetInstance())
            {
                entity = ctx.Paquetes.Add(entity);
                if (entity == null)
                {
                    throw new PaqueteException("No se pudo crear el paquete.");
                }
                ctx.SaveChanges();
                dto = paqueteConversor.Entity2DTO(entity);
            }
            return dto;
        }

        public PaqueteDTO Read(long paqueteId)
        {
            Paquete entity = null;
            using (var ctx = dbFactory.GetInstance())
            {
                entity = ctx.Paquetes.Find(paqueteId);
            }
            PaqueteDTO dto = paqueteConversor.Entity2DTO(entity);
            return dto;
        }

        public IList<PaqueteDTO> List()
        {
            IList<PaqueteDTO> lista = new List<PaqueteDTO>();
            using (var ctx = dbFactory.GetInstance())
            {
                foreach (Paquete paquete in ctx.Paquetes.ToList<Paquete>())
                {
                    lista.Add(paqueteConversor.Entity2DTO(paquete));
                }
            }
            return lista;
        }

        public IList<PaqueteDTO> List(long envioId)
        {
            IList<PaqueteDTO> lista = new List<PaqueteDTO>();
            using (var ctx = dbFactory.GetInstance())
            {
                foreach (Paquete paquete in ctx.Paquetes.Where(p => p.EnvioId == envioId).ToList<Paquete>())
                {
                    lista.Add(paqueteConversor.Entity2DTO(paquete));
                }
            }
            return lista;
        }

        public PaqueteDTO Update(PaqueteDTO paquete)
        {
            PaqueteDTO dto = null;
            Paquete entity = paqueteConversor.DTO2Entity(paquete);
            using (var ctx = dbFactory.GetInstance())
            {
                Paquete old = ctx.Paquetes.Find(paquete.IncidenciaId);
                if (old == null)
                {
                    throw new PaqueteException("El paquete que intentó actualizar no existe.");
                }
                ctx.Entry(old).CurrentValues.SetValues(entity);
                entity = ctx.Paquetes.Find(paquete.IncidenciaId);
                ctx.SaveChanges();
            }
            dto = paqueteConversor.Entity2DTO(entity);
            return dto;
        }

        public PaqueteDTO Delete(long paqueteId)
        {
            Paquete entity = null;
            using (var ctx = dbFactory.GetInstance())
            {
                entity = ctx.Paquetes.Find(paqueteId);
                if (entity != null)
                {
                    ctx.Paquetes.Remove(entity);
                    ctx.SaveChanges();
                }
            }
            PaqueteDTO dto = paqueteConversor.Entity2DTO(entity);
            return dto;
        }
    }
}
