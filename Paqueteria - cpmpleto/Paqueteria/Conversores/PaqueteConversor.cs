using Paqueteria.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paqueteria.Conversores
{
    public class PaqueteConversor : IConversor<Paquete, PaqueteDTO>
    {

        public PaqueteDTO Entity2DTO(Paquete entity)
        {
            PaqueteDTO resultado = null;
            if (entity != null)
            {
                resultado = new PaqueteDTO();
                resultado.IncidenciaId  = entity.IncidenciaId;
                resultado.FMercancia  =entity.FMercancia;
                resultado.RMercancia  =entity.RMercancia;
                resultado.DMercancia  =entity.DMercancia;
                resultado.AMercancia  =entity.AMercancia;
                resultado.NExpedicion = entity.NExpedicion;
                resultado.EnvioId = entity.EnvioId;      
            }             
            return resultado;
        }

        public Paquete DTO2Entity(PaqueteDTO dto)
        {
            Paquete resultado = null;
            if (dto != null)
            {
                resultado = new Paquete();
                resultado.IncidenciaId = dto.IncidenciaId;
                resultado.FMercancia   = dto.FMercancia;
                resultado.RMercancia  =   dto.RMercancia;
                resultado.DMercancia  =  dto.DMercancia;
                resultado.AMercancia   = dto.AMercancia;
                resultado.NExpedicion = dto.NExpedicion;
                resultado.EnvioId = dto.EnvioId;

            }   
            return resultado;
        }
    }
}
