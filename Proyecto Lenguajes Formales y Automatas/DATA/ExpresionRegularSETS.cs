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
        //Con ayuda de Github https://github.com/diegoazurdia1998/Proyecto-LenguajesFormalesYAutomatas/blob/Fase1/ProjectoLFA/ProjectoLFA/Data/Validaciones.cs
        //El tiempo que espera en detectar una coincidencia 
        static TimeSpan timeout = new TimeSpan(0, 0, 5);

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
    }
}
