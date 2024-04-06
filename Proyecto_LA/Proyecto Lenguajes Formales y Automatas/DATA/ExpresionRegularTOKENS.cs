using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto_Lenguajes_Formales_y_Automatas.DATA
{
        public class ExpresionRegularTOKENS
        {

            static TimeSpan timeout = new TimeSpan(0, 0, 5);

            public static bool IniciaEnToken(string linea)
            {
                return Regex.IsMatch(linea, @"^TOKENS\s*$");
            }

            public static bool ContenidoValidoTokens(string linea)
            {
                try
                {
                return Regex.IsMatch(linea, @"^(\s*(TOKEN\s*\d+\s*=\s*)(('.{1}')+|(\w+ *(\*|\|)?)+|('.{1}' *\w+ *'.{1}'(\*|\|)?)+|(\w*\s*(\(|{)(\s*\w*\s*(\(\))?(\*|\|)?)+(\)|})(\*|\|)?)+)\s*)$", RegexOptions.IgnoreCase, timeout);
            }
                catch (RegexMatchTimeoutException e)
                {
                    // Increase the timeout interval and retry.
                    return false;
                }
            }

        public static string TokenCorregido(string linea)
        {
            Regex rg = new Regex(@"({)(\s*|\t*)(RESERVADAS)(\s*|\t*)(\()(\))(\s*|\t*)(})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rg.Matches(linea);
            if (matches.Count > 0)
            {
                try
                {
                    string nuevaLinea = Regex.Replace(linea, @"({)(\s*|\t*)(RESERVADAS)(\s*|\t*)(\()(\))(\s*|\t*)(})", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));
                    return nuevaLinea;
                }
                catch (RegexMatchTimeoutException)
                {
                    return null;
                }
            }
            else
            {
                return linea;
            }
        }
    }
}