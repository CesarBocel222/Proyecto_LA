using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Lenguajes_Formales_y_Automatas
{
    public class Gramatica
    {

        public List<string> Grupo { get; set; }

        public Gramatica()
        {
            Grupo = new List<string>();
        }
    }

    public class Mensaje
    {
        public bool OcurrioError { get; set; }
        public string Texto { get; set; }
        public int Row { get; set; }
    }
}
