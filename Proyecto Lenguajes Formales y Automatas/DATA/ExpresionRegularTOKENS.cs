using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
                return Regex.IsMatch(linea, @"^(\s*(TOKEN\s*\d+\s*=\s*)(('.{1}')+|(\w+ (\|\|)?)+|('.{1}' \w+ *'.{1}'(\|\|)?)+|(\w*\s*(\(|{)(\s*\w*\s*(\(\))?(\|\|)?)+(\)|})(\|\|)?)+)\s*)$", RegexOptions.IgnoreCase, timeout);
            }
            catch (RegexMatchTimeoutException e)
            {
                // Increase the timeout interval and retry.
                return false;
            }
        }

    }
}
