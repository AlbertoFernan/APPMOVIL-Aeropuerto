using System;
using System.Collections.Generic;
using System.Text;

namespace APPMOVIL.Models
{
   public class Partidas
    {

        public int Id { get; set; }
        public string Destino { get; set; } = "";
        public string Vuelo { get; set; } = "";
        public string Puerta { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime Tiempo { get; set; }

    }
}
