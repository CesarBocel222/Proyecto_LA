using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto_Lenguajes_Formales_y_Automatas.DATA
{
    public class ExpresionRegularERROR
    {
        public static bool IniciaEnError(string linea)
        {
            return Regex.IsMatch(linea, @"^ERROR\w*\s* =\s* \d+$");
        }

        public static bool ContenidoValidoError(string linea)
        {
            return Regex.IsMatch(linea, @"^((\s*\w*ERROR\w*\s*=)(\s*\d*\s*)|\s*)$");
        }
    }
}
