using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paqueteria.Model
{
    public class Envio
    {
        [Key]
        public long EnvioId { get; set; }
        public ICollection<Paquete> Paquetes { get; set; }
        public long DestinatarioId { get; set; }
        public DateTime FechaEntrega { get; set; }
        public int Estado { get; set; }
        public string Incidencias { get; set; }
        public string NExpedicion { get; set; }
        public string Volumen { get; set; }
        public string NBulto { get; set; }
        public string TipoPorte { get; set; }
        public string Kilos { get; set; }
        public string Remitente { get; set; }
        public string Destinatario { get; set; }
        public string Poblacion { get; set; }
        public string Cautonoma { get; set; }
        public string Ciudad { get; set; }
        public string Observaciones { get; set; }
        public string Agencia { get; set; }

        public Envio()
        {
            this.EnvioId = -1;
            this.Estado = 0;
        }
    }
}
