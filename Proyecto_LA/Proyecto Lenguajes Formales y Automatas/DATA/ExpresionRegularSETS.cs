using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto_Lenguajes_Formales_y_Automatas.DATA
{
    public class ExpresionRegularSETS
    {
        

        //Comprueba si una cadena determinada consta únicamente de espacios en blanco
        public static bool TabOrEspaceOrEnter(string linea)
        {
            return Regex.IsMatch(linea, @"^\s*$");
        }

        public static bool IniciaEnSets(string linea)
        {
            return Regex.IsMatch(linea, @"^SETS\s*$");
        }

        public static bool ContenidoValidoSets(string linea)
        {
            return Regex.IsMatch(linea, @"^(\s*[A-Z]+ *= *)(('\w+'((\+|\.{2})?'\w+')*)\s*|(CHR\(\d+\)((\+|\.{2})?CHR\(\d+\))*\s*))$");
        }

        public static string GetContenedidoSet(string linea)
        {
            Regex rg = new Regex("((\\s*'\\w'\\s*((\\+|\\.{2})\\s*'\\w'\\s*)*)|(\\s*CHR\\(\\d+\\)\\s*((\\+|\\.{2})\\s*CHR\\(\\d+\\)\\s*)*))");
            MatchCollection matchCollection = rg.Matches(linea);
            if (matchCollection.Count < 0)
                return string.Empty;
            else
                return matchCollection[0].Value;
        }
    }
}
