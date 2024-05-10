using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Lenguajes_Formales_y_Automatas.ARBOL
{
    public class Estados
    {
        public int Estado { get; set; }
        public List<Simbolos> simbolos { get; set; }

        public Estados()
        {
            simbolos = new List<Simbolos>();
        }
    }

    public class Simbolos
    {
        public string Token { get; set; }
        public int Transicion { get; set; }
    }
}
