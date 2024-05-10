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
            bool validation = Regex.IsMatch(linea, @"^(\s*(TOKEN\s*\d+\s*=\s*)(('.{1}'\s*)+|(\w+\s*(\*|\|)?\s*)+|('.{1}'\s*\w+\s*'.{1}'(\*|\|)?\s*)+|(\w*\s*(\(|{)(\s*\w*\s*(\(\))?(\*|\|)?)+(\)|})(\*|\|)?\s*)+)\s*)$", RegexOptions.IgnoreCase, timeout);
            try
            {
                int indice = 0;//indice del error
                if (!validation)
                {
                    Regex regex = new Regex(@"\s*(TOKEN\s*\d+\s*=\s*)");
                    Match match = regex.Match(linea);
                    if (!match.Success)
                    {
                        indice += match.Length - 1;
                        linea = linea[indice..];
                        bool comillaAbierta = false, readingNT = false, cerrarComilla = false, opFollowOp = false, parentesisAbierto = false;
                        int parentesisCount = 0;
                        foreach (char caracter in linea)
                        {
                            //Bloque de verificaciones 1
                            if (cerrarComilla)
                            {
                                if (caracter != '\'') throw new Exception($"Se esperara ''' en el token. Columna: {indice}");
                            }
                            else if (opFollowOp)
                            {
                                if (Regex.IsMatch(caracter.ToString(), @"\+|\*|\?|\|")) throw new Exception($"No pueden haber 2 operadores seguidos en el token. Columna: {indice}");
                                else opFollowOp = false;
                            }
                            //Bloque de verificaciones 2
                            if (caracter == '\'')
                            {
                                if (comillaAbierta)
                                {
                                    if (cerrarComilla)
                                    {
                                        cerrarComilla = false;
                                        comillaAbierta = false;
                                    }
                                    else cerrarComilla = true;
                                }
                                else
                                {
                                    if (readingNT) readingNT = false;
                                    comillaAbierta = true;
                                }
                            }
                            else if (Regex.IsMatch(caracter.ToString(), @"\(|\)|\{|\}"))
                            {
                                if ((caracter == '(' || caracter == '{') && !parentesisAbierto)
                                {
                                    parentesisCount++;
                                    parentesisAbierto = true;
                                }
                                else if ((caracter == ')' || caracter == '}') && parentesisAbierto)
                                {
                                    if (parentesisCount == 1) parentesisAbierto = false;
                                    else if (parentesisCount > 1) parentesisCount--;
                                }
                            }
                            else if (Regex.IsMatch(caracter.ToString(), @"[A-Z]"))
                            {
                                if (!comillaAbierta && !readingNT) readingNT = true;
                                else if (comillaAbierta) cerrarComilla = true;
                            }
                            else if (Regex.IsMatch(caracter.ToString(), @"\+|\*|\?|\||\(") && !comillaAbierta)
                            {
                                if (!opFollowOp) opFollowOp = true;
                            }
                            else if (Regex.IsMatch(caracter.ToString(), @"\s"))
                            {
                                if (readingNT) readingNT = false;
                            }
                            else if (Regex.IsMatch(caracter.ToString(), @"."))
                            {
                                if (comillaAbierta)
                                {
                                    cerrarComilla = true;
                                }
                                else
                                {
                                    throw new Exception($"Caracter inválido en el token. Columna: {indice}");
                                }
                            }

                            indice++;
                        }
                        //Fin foreach
                        if (readingNT || comillaAbierta || parentesisAbierto || parentesisCount > 0) throw new Exception($"No es posible asignar el token.");
                    }
                    else
                    {
                        regex = new Regex(@"\b?TOKEN\b");
                        match = regex.Match(linea);
                        if (match.Success)
                        {
                            regex = new Regex(@"\b=\b");
                            match = regex.Match(linea);
                            if (match.Success) throw new Exception($"No se encontró el digito en el identificador del token. Columna: {indice}");
                            else throw new Exception($"No se encontró el caracter '=' del token. Columna: {indice}");
                        }
                        else throw new Exception($"No se encontró el identificador del token. Columna: {indice}");
                    }
                }

            }
            catch (Exception e)
            {
                // Increase the timeout interval and retry.
                Console.WriteLine(e.Message);
                return false;
            }
            return validation;
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
        public static bool VerificandoEsToken(string linea)
            {
                return Regex.IsMatch(linea, @"^(\s*(TOKEN\s*\d+\s*=\s*)(('.{1}'\s*)+|(\w+\s*(\*|\|)?\s*)+|('.{1}'\s*\w+\s*'.{1}'(\*|\|)?\s*)+|(\w*\s*(\(|{)(\s*\w*\s*(\(\))?(\*|\|)?)+(\)|})(\*|\|)?\s*)+)\s*)$", RegexOptions.IgnoreCase, timeout);
            }
        public static int GetTokenIdReservada(string linea)
        {
            MatchCollection mC;
            Regex rgIdentificarP1 = new Regex("([0-9]*)\\s*\\d+\\s*=\\s*");
            mC = rgIdentificarP1.Matches(linea);

            if (mC.Count < 0)
                return -1;

            string tokenPart = mC[0].Value;
            Regex rgTokenIdPart = new Regex("[0-9]*");
            mC = rgTokenIdPart.Matches(tokenPart);

            for (int i = 0; i < mC.Count; i++)
            {
                if (!string.IsNullOrEmpty(mC[i].Value))
                    return Convert.ToInt32(mC[i].Value);
            }

            return -1;
        }
        public static string GetContenidoReservada(string linea)
        {
            MatchCollection mC;
            Regex rg = new Regex(@"\s*=\s*'[A-Z]*'\s*$");
            mC = rg.Matches(linea);

            if (mC.Count < 0)
                return null;

            var contenido = mC[0].Value;
            rg = new Regex(@"([A-Z]+)");
            mC = rg.Matches(contenido);

            if (mC.Count < 0)
                return null;

            return mC[0].Value;
        }
    }
}