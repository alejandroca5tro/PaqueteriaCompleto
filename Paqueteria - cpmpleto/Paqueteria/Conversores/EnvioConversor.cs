using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paqueteria.Model;
namespace Paqueteria.Conversores
{
    public class EnvioConversor : IConversor<Envio, EnvioDTO>
    {
        private PaqueteConversor paqueteConversor;
        public EnvioConversor(PaqueteConversor paqueteConversor)
        {
            this.paqueteConversor = paqueteConversor;
        }
        public EnvioDTO Entity2DTO(Envio entity)
        {

            EnvioDTO dto = null;
            if (entity != null)
            {
                dto = new EnvioDTO();
                dto.EnvioId = entity.EnvioId;
                dto.DestinatarioId = entity.DestinatarioId;
                dto.FechaEntrega = entity.FechaEntrega;
                dto.Estado = entity.Estado;
                dto.Incidencias = entity.Incidencias;
               dto.NExpedicion = entity.NExpedicion;
               dto.Volumen     = entity.Volumen;
               dto.NBulto = entity.NBulto;
               dto.TipoPorte   = entity.TipoPorte;
                dto.Kilos =         entity.Kilos;
                dto.Remitente      = entity.Remitente     ;
                dto.Destinatario   = entity.Destinatario  ;
                dto.Poblacion      = entity.Poblacion     ;
                dto.Cautonoma      = entity.Cautonoma     ;
                dto.Ciudad         = entity.Ciudad        ;
                dto.Observaciones  = entity.Observaciones ;
                dto.Agencia        = entity.Agencia       ;
                if (entity.Paquetes != null && entity.Paquetes.Count != 0)
                {
                    dto.Paquetes = (ICollection<PaqueteDTO>)entity.Paquetes.Select<Paquete, PaqueteDTO>(p => paqueteConversor.Entity2DTO(p));
                }
            }
            return dto;
        }

        public Envio DTO2Entity(EnvioDTO dto)
        {
            Envio e = null;
            if (dto != null)
            {
                e = new Envio();
                e.EnvioId = dto.EnvioId;
                e.DestinatarioId = dto.DestinatarioId;
                e.FechaEntrega = dto.FechaEntrega;
                e.Estado = dto.Estado;

               e.Incidencias=dto.Incidencias;
               e.NExpedicion=dto.NExpedicion;
               e.Volumen=dto.Volumen    ;
               e.NBulto=dto.NBulto     ;
               e.TipoPorte = dto.TipoPorte;

               e.Kilos = dto.Kilos;
               e.Remitente      = dto.Remitente       ;
               e.Destinatario   = dto.Destinatario    ;
               e.Poblacion      = dto.Poblacion       ;
               e.Cautonoma      = dto.Cautonoma       ;
               e.Ciudad         = dto.Ciudad          ;
               e.Observaciones  = dto.Observaciones   ;
               e.Agencia        = dto.Agencia         ;
                if (dto.Paquetes != null && dto.Paquetes.Count != 0)
                {
                    e.Paquetes = (ICollection<Paquete>)dto.Paquetes.Select<PaqueteDTO, Paquete>(p => paqueteConversor.DTO2Entity(p));
                }
            }
            return e;
        }
    }
}
