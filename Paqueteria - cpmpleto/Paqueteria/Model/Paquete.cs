using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paqueteria.Model
{
    public class Paquete
    {
        [Key]
        public long IncidenciaId { get; set; }
        public int FMercancia { get; set; }
        public int RMercancia { get; set; }
        public int DMercancia { get; set; }
        public int AMercancia { get; set; }
        public string NExpedicion { get; set; }
        public long EnvioId { get; set; }

        public Paquete() 
        {
            this.IncidenciaId = -1;
        }
    }
}
