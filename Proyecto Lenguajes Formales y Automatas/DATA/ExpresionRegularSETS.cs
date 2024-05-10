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
            //Verificacion inicial
            bool validation = Regex.IsMatch(linea, @"^(\s*[A-Z]+\s*=\s*)((\s*'\w'\s*((\+|\.{2})\s*'\w'\s*)*)|(\s*CHR\(\d+\)\s*((\+|\.{2})\s*CHR\(\d+\)\s*)*))$");
            //si la verificacion inicial falla se intenta recorrer la linea buscansdo el indice del error
            try
            {
                int indice = 0;//indice del error
                if (!validation)
                {
                    //Se utilizan fragmentos de la expresion regular original
                    //SeVerifica la izquierda del igual (el identificador)
                    Regex regex = new Regex(@"(\s*[A-Z]+\s*=\s*)");
                    Match match = regex.Match(linea);
                    //Si la izquierda del igual es correcta se verifica la derecha
                    if (match.Success)
                    {
                        indice += (match.Length - 1);//avanza el indice del error hasta la posicion final
                        linea = linea[indice..];//recorta la linea desde el indice del error hasta el final de la linea.
                        int countCHAR = 0;
                        bool comillaAbierta = false, comillaNext = false, cerrarParentesis = false, readingCHAR = false, doblePunto = false, unir = false, digitNext = false; ;
                        //La linea debe iniciar en ''' o '\t' o ' '
                        if (linea[0] == '\'' || linea[0] == '\t' || linea[0] == ' ')
                        {
                            //recorre cada caracter de la linea
                            foreach (char caracter in linea)
                            {
                                //Inicia bloque de verificaciones activadas con el anterior caracter
                                //Valida si el caracter actual debe ser una comilla
                                if (comillaNext)
                                {
                                    if (caracter != '\'')
                                    {
                                        throw new Exception($"No se pudo completar la asignación. Columna: {indice}");
                                    }
                                }
                                //Valida si el caracter acutal debe ser un caracter para unir simbolos terminales
                                else if (unir)
                                {
                                    if (caracter == '.' || caracter == '+')
                                    {
                                        unir = false;
                                    }
                                    else throw new Exception($"No se pudo completar la asignación. Columna: {indice}");
                                }
                                //Valida si el caracter actual debe ser un digito y que no sea un parentesis cerrado
                                else if (digitNext && caracter != ')')
                                {
                                    if (!Regex.IsMatch(caracter.ToString(), @"\d"))
                                        throw new Exception($"No se pudo completar la asignación. Columna: {indice}");
                                }
                                //Valida si el caracter actual debe ser un digito y si se debe ser un parentesis cerrado
                                else if (digitNext && cerrarParentesis)
                                {
                                    if (caracter != ')')
                                        throw new Exception($"No se pudo completar la asignación. Columna: {indice}");
                                    else
                                    {
                                        digitNext = false;
                                        cerrarParentesis = false;
                                        readingCHAR = false;
                                        countCHAR = 0;
                                        unir = true;
                                    }
                                }
                                //Valida si el caracter actual debe ser un punto
                                else if (doblePunto)
                                {
                                    if (caracter != '.')
                                    {
                                        throw new Exception($"No se pudo completar la asignación. Columna: {indice}");
                                    }
                                }
                                //Inicia Bloque de validaciones para el caracter actual
                                //Cuando el caracter actual es una comilla
                                if (caracter == '\'')
                                {
                                    //Si ya se ha leido una comilla y no debe recibir una comilla en el caracter actual
                                    if (comillaAbierta && !comillaNext)
                                    {
                                        throw new Exception($"No se pudo completar la asignación. Columna: {indice}");
                                    }
                                    //Si se estaba leyendo un CHR y se encuentra una comilla
                                    if (readingCHAR) readingCHAR = false;
                                    //Si ya se ha leido una comilla que abre
                                    if (comillaAbierta)
                                    {
                                        comillaAbierta = false;
                                        comillaNext = false;
                                        unir = true;
                                    }
                                    else
                                    { comillaAbierta = true; }
                                }
                                //Si el caracter actual es un .
                                else if (caracter == '.')
                                {
                                    if (doblePunto) doblePunto = false;
                                    else doblePunto = true;
                                }
                                //Si el caracter actual es 'C' o si se esta leyendo 'CHR(\d+)'
                                else if (caracter == 'C' || readingCHAR)
                                {
                                    if (!readingCHAR)
                                    {
                                        countCHAR++;
                                        readingCHAR = true;
                                    }
                                    else
                                    {
                                        if (caracter == 'H' || caracter == 'R' || Regex.IsMatch(caracter.ToString(), @"\d")) countCHAR++;
                                        else if (caracter == '(')
                                        {
                                            countCHAR++;
                                            digitNext = true;
                                            cerrarParentesis = true;
                                        }
                                        else throw new Exception($"No se pudo completar la asignación. Columna: {indice}");
                                    }
                                }
                                //Caso para reconocer caracteres validos
                                else if (caracter == '+' || caracter == ')') ;
                                //Si el caracter actual es cualquier caracter menos el salto de linea o caracteres de \s
                                else if (Regex.IsMatch(caracter.ToString(), @".") && !Regex.IsMatch(caracter.ToString(), @"\s"))
                                {
                                    if (!comillaAbierta)
                                    {
                                        indice++;
                                        throw new Exception($"No se pudo completar la asignación. Columna: {indice}");
                                    }
                                    else comillaNext = true;
                                }
                                //se aumenta el indice
                                indice++;
                            }
                        }
                        else throw new Exception($"No se pudo completar la asignación. Columna: {indice}");
                        if (comillaAbierta || doblePunto || cerrarParentesis) throw new Exception($"No se pudo completar la asignación. Columna: {indice}");
                    }
                    else //Si la izquierda tiene alguna discrepancia
                    {
                        //Se verifica el identificador
                        regex = new Regex(@"(\b[A-Z]+\b)");
                        match = regex.Match(linea);
                        //Si el identificador es correcto, se verifica el caracter '='
                        if (match.Success)
                        {
                            regex = new Regex(@"(\b=\b)");
                            match = regex.Match(linea);
                            if (!match.Success) throw new Exception($"No se encontró el caracter '='. Columna: {indice}");

                        }
                        else throw new Exception($"No se encontró el identificador del set. Columna: {indice}");
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return validation;
        }
        public static string GetElemento(string linea)
        {
            Regex rg = new Regex("(\\s*'(\\w|.)'\\s*)");
            MatchCollection mC = rg.Matches(linea);

            if (mC.Count < 0)
                return string.Empty;
            else
            {
                var elemtento = mC[0].Groups[2].Value;
                if (elemtento.Length > 1)
                    elemtento = elemtento.Replace(" ", "");

                return elemtento;
            }
        }

        public static string GetEtiquetaSet(string linea)
        {
            Regex rg = new Regex("(\\s*[A-Z]+\\s*=\\s*)");
            MatchCollection mC = rg.Matches(linea);

            string etiqueta = mC[0].Value;

            if (etiqueta.Contains(" "))
                etiqueta = etiqueta.Replace(" ", "");

            if (etiqueta.Contains("="))
                etiqueta = etiqueta.Replace("=", "");

            if (etiqueta.Contains("\t"))
                etiqueta = etiqueta.Replace("\t", "");


            return etiqueta;
        }
        public static bool EsChar(string linea)
        {
            return Regex.IsMatch(linea, @"\s*(CHR)\s*");
        }

        public static int GetValorChar(string linea)
        {
            Regex rg = new Regex("\\s*[0-9]*\\s*");
            MatchCollection mC = rg.Matches(linea);


            int respuesta = 0;
            foreach (var valor in mC)
            {
                var esNumero = int.TryParse(valor.ToString(), out respuesta);
                if (esNumero)
                    break;
            }

            return respuesta;
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
