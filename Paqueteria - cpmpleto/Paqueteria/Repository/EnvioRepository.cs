using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paqueteria.Model;
using Paqueteria.Conversores;
namespace Paqueteria.Repository
{
    public class EnvioRepository : IEnvioRepository
    {
        private EnvioConversor Conversor;
        private IPaqDBFactory Factory;
        public EnvioRepository(EnvioConversor EnvioConversor, IPaqDBFactory factory)
        {
            this.Conversor = EnvioConversor;
            this.Factory = factory;
        }
        public EnvioDTO Create(EnvioDTO envio)
        {
            Envio e = Conversor.DTO2Entity(envio);
            using (var ctx = Factory.GetInstance())
            {
                e = ctx.Envios.Add(e);
                ctx.SaveChanges();
                envio = Conversor.Entity2DTO(e);
            }
            return envio;
        }
        public EnvioDTO Read(long EnvioId)
        {
            Envio envio = null;
            EnvioDTO envioDto = null;
            using (var ctx = Factory.GetInstance())
            {
                envio = ctx.Envios.Where(e => e.EnvioId == EnvioId && e.Estado != Constantes.BORRADO).FirstOrDefault();
                envioDto = Conversor.Entity2DTO(envio);
            }
            return envioDto;
        }
        //public EnvioDTO ReadFiltro(string Filtro, string Tipo)
        //{
        //    Envio envio = null;
        //    EnvioDTO envioDto = null;
        //    using (var ctx = Factory.GetInstance())
        //    {
        //        if (Tipo == "Nexpedicion")
        //        {
        //            envio = ctx.Envios.Where(e => e.NExpedicion == Filtro && e.Estado != Constantes.BORRADO).FirstOrDefault();
        //            envioDto = Conversor.Entity2DTO(envio);
        //        }
        //    }
        //    return envioDto;
        //}
        public IList<EnvioDTO> List()
        {
            IList<EnvioDTO> lista = null;
            using (var ctx = Factory.GetInstance())
            {
                lista = ctx.Envios.Where(e => e.Estado != Constantes.BORRADO).ToList().ConvertAll(e => Conversor.Entity2DTO(e));
            }
            return lista;
        }
        public IList<EnvioDTO> ReadFiltro(string Filtro, string Tipo)
        {
            IList<EnvioDTO> lista = null;
            using (var ctx = Factory.GetInstance())
            {
               
                if (Tipo == "Nexpedicion")
                {

                    lista = ctx.Envios.Where(e => e.NExpedicion == Filtro && e.Estado != Constantes.BORRADO).ToList().ConvertAll(e => Conversor.Entity2DTO(e));
                }
                if (Tipo == "Agencia")
                {

                    lista = ctx.Envios.Where(e => e.Agencia == Filtro && e.Estado != Constantes.BORRADO).ToList().ConvertAll(e => Conversor.Entity2DTO(e));
                }
                if (Tipo == "Poblacion")
                {

                    lista = ctx.Envios.Where(e => e.Poblacion == Filtro && e.Estado != Constantes.BORRADO).ToList().ConvertAll(e => Conversor.Entity2DTO(e));
                }
                if (Tipo == "Cautonoma")
                {

                    lista = ctx.Envios.Where(e => e.Cautonoma == Filtro && e.Estado != Constantes.BORRADO).ToList().ConvertAll(e => Conversor.Entity2DTO(e));
                }
                if (Tipo == "Ciudad")
                {

                    lista = ctx.Envios.Where(e => e.Ciudad == Filtro && e.Estado != Constantes.BORRADO).ToList().ConvertAll(e => Conversor.Entity2DTO(e));
                }
                if (Tipo == "Incidencias")
                {

                    lista = ctx.Envios.Where(e => e.Incidencias == Filtro && e.Estado != Constantes.BORRADO).ToList().ConvertAll(e => Conversor.Entity2DTO(e));
                }
            }
            return lista;
        }
        public EnvioDTO Update(EnvioDTO envio)
        {
            Envio nuevoEnvio = Conversor.DTO2Entity(envio);
            EnvioDTO cambio = null;
            using (var context = Factory.GetInstance())
            {
                Envio antiguoenvio = context.Envios.Find(envio.EnvioId);
                context.Entry(antiguoenvio).CurrentValues.SetValues(nuevoEnvio);
                context.SaveChanges();
                cambio = Conversor.Entity2DTO(antiguoenvio);
            }
            return cambio;
        }

        public EnvioDTO Delete(long EnvioId)
        {
            Envio envio = null;
            EnvioDTO dto = null;
            using (var ctx = Factory.GetInstance())
            {
                envio = ctx.Envios.Find(EnvioId);
                if (envio != null)
                {
                    ctx.Entry(envio).Entity.Estado = Constantes.BORRADO;
                    envio = ctx.Envios.Find(envio.EnvioId);
                }
                dto = Conversor.Entity2DTO(envio);
                ctx.SaveChanges();
            }

            return dto;
        }

    }
}
