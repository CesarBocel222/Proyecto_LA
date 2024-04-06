using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto_Lenguajes_Formales_y_Automatas.DATA
{
    public class ExpresionRegularACTION
    {

        public static bool IniciaEnAction(string linea)
        {
            return Regex.IsMatch(linea, @"^ACTIONS\s*$");
        }
        public static bool ContenidoValidoActions(string linea)
        {
            var matchs = Regex.Matches(linea, @"^\s*([A-Z0-9_]+\(\))|({\s*)|(}\s*)|(\s*\d*\s*=\s*'[A-Z]*'\s*)|\s*$");
            if (matchs.Count < 0 && matchs == null)
                return false;

            return !string.IsNullOrEmpty(matchs[0].Value);

        }
    }
}
