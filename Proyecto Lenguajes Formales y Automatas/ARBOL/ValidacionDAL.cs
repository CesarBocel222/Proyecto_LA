using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto_Lenguajes_Formales_y_Automatas.ARBOL
{
    public static class ValidacionDAL
    {

        public static int GetTokenId(string linea)
        {
            MatchCollection mc;
            Regex rgTokenPart = new Regex("(TOKEN\\s*\\d+\\s*=\\s*)");
            mc = rgTokenPart.Matches(linea);

            if (mc.Count < 0)
                return -1;

            string tokenPart = mc[0].Value;
            Regex rgTokenIdPart = new Regex("[0-9]*");
            mc = rgTokenIdPart.Matches(tokenPart);

            for (int i = 0; i < mc.Count; i++)
            {
                if (!string.IsNullOrEmpty(mc[i].Value))
                    return Convert.ToInt32(mc[i].Value);
            }

            return -1;
        }

        public static bool EsCaracterAlfabeto(string linea)
        {
            MatchCollection mc;
            Regex rg = new Regex("([a-z]|[A-Z])+");
            mc = rg.Matches(linea);
            if (mc.Count > 0)
                return true;
            else
                return false;
        }
    }
}
