using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto_Lenguajes_Formales_y_Automatas.DATA
{
    public class Verificaror
    {
        private static int ROWS = 1;
        private static List<string> Tokens { get; set; }
        private static List<string> Sets { get; set; }
        private static List<string> Reservadas { get; set; }

        public Verificaror()
        {
            Tokens = null;
            Sets = null;
            Reservadas = null;
        }

        public List<string> getTokens()
        {
            return Tokens;
        }
        public void nullTokens()
        {
            Tokens = null;
        }
        public List<string> getSets()
        {
            return Sets;
        }
        public List<string> getReservadas()
        {
            return Reservadas;
        }

        public void nullSets()
        {
            Sets = null;
        }
        public bool VerifyOrderSection(List<Gramatica> gramatica)
    {
            var correcto = false;
            var contieneSets = false;
            var contieneTokens = false;
            var contieneActions = false;
            var contieneError = false;
            var contador = 0;
            try
            {
                //TODO: Verifica cada seccion de la gramatica
                foreach (var seccion in gramatica)
                {
                    foreach (var linea in seccion.Grupo)
                    {
                        if (ExpresionRegularSETS.IniciaEnSets(linea) && contador == 0)
                        {
                            //  TODO: Inidica que viene los sets en orden correcto
                            contador++;
                            contieneSets = true;
                            break;
                        }
                        else if (ExpresionRegularTOKENS.IniciaEnToken(linea) && (contador == 1 || contador == 0))
                        {
                            //  TODO: Indica que vienen los tokens en orden correcto
                            contador = contador == 0 ? contador + 2 : contador + 1;
                            contieneTokens = true;
                            break;
                        }
                        else if (ExpresionRegularACTION.IniciaEnAction(linea) && contador == 2)
                        {
                            //  TODO: Indica que luego siguen los actions en orden correcto
                            contador++;
                            contieneActions = true;
                            break;
                        }
                        else if (ExpresionRegularERROR.IniciaEnError(linea) && contador == 3)
                        {
                            contieneError = true;
                            break;
                        }
                        else if (!ExpresionRegularSETS.IniciaEnSets(linea) && !ExpresionRegularTOKENS.IniciaEnToken(linea) && !ExpresionRegularACTION.IniciaEnAction(linea) && !ExpresionRegularERROR.IniciaEnError(linea))
                        {
                            contador++;
                            break;
                        }

                        if (contador == 3)
                            break;
                    }
                }

                if (contieneSets && contieneTokens && contieneActions && contieneError || contieneActions && contieneTokens && contieneError)
                    correcto = true;
                else
                    throw new Exception("El archivo no tiene el orden correcto. SETS -> TOKENS -> ACTIONS -> ERROR | TOKENS -> ACTIONS -> ERROR ");

                return correcto;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nProblemas con la estructura del archivo");
                Console.WriteLine($"Mensaje: {ex.Message}");
                return correcto;
            }
        }

        /// <summary>
        /// Lee un archivo .txt
        /// </summary>
        /// <param name="filePath">nombre o ruta del archivo a verificar</param>
        public List<Gramatica> ReadFile(string filePath)
        {
            var lineaActual = "";
            var tieneSets = false;
            //  TODO:   Separa el archivo por seccion
            List<Gramatica> ListaLists = new List<Gramatica>();
            try
            {
                if (!File.Exists(filePath))
                    throw new Exception("El archivo no existe :(");

                //  TODO:   Lista reusable para guardar las partes de la gramatica
                Gramatica tempList = new Gramatica();

                using (StreamReader sr = new StreamReader(filePath))
                {

                    while (!sr.EndOfStream)
                    {
                        lineaActual = sr.ReadLine();

                        //  TODO: Encuentra una seccion e agregar 
                        if ((ExpresionRegularTOKENS.IniciaEnToken(lineaActual) || ExpresionRegularACTION.IniciaEnAction(lineaActual) || ExpresionRegularERROR.IniciaEnError(lineaActual)) && tempList.Grupo.Count > 0)
                        {
                            ListaLists.Add(tempList);
                            tempList = new Gramatica();
                            tempList.Grupo.Add(lineaActual); //añade el identificador a la lista
                        }
                        else
                        {
                            if (ExpresionRegularTOKENS.VerificandoEsToken(lineaActual))
                            {
                                //  !   TODO: Corrigiendo token
                                var tokenCorregido = ExpresionRegularTOKENS.TokenCorregido(lineaActual);
                                if (string.IsNullOrEmpty(tokenCorregido))
                                    throw new Exception("Problemas al corregir el token");

                                lineaActual = tokenCorregido;
                            }

                            tempList.Grupo.Add(lineaActual);
                            if (lineaActual.ToUpper().Replace(" ", "") == "SETS")
                                tieneSets = true;
                        }
                    }

                    if (ListaLists.Count == 0)
                    {
                        string error = tieneSets ? "El archivo solo tiene 'SETS'" : "El archivo tiene un formato no valido";
                        throw new Exception(error);
                    }

                    //  TODO: Si falto una seccion agregarla
                    ListaLists.Add(tempList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nProblemas con el archivo.");
                Console.WriteLine($"Mensaje: {ex.Message}");
            }
            ROWS = 0;
            return ListaLists;
        }

        /// <summary>
        /// Verifica si la grmatica proporcionada es válida
        /// </summary>
        /// <param name="SETS_TOKENS_ACTIONS_ERRORS">Lista de listas con la gramática separada por partes</param>
        public bool VerifyGramar(List<Gramatica> SETS_TOKENS_ACTIONS_ERRORS)
        {
            try
            {
                if (SETS_TOKENS_ACTIONS_ERRORS.Count < 2)
                    throw new Exception("A la gramatica le faltan grupos :(");
                bool respuesta = false;
                foreach (var section in SETS_TOKENS_ACTIONS_ERRORS)//se recorre la lista de las partes, parte por parte para su verificacion
                {
                    if (section.Grupo.Count > 1)
                    {
                        respuesta = checksDealer(section.Grupo);
                        if (!respuesta)
                            throw new Exception("Gramática incorrecta.");
                    }
                }
                if (respuesta) return true;
                else return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en las secciones del archivo");
                Console.WriteLine($"Ms: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Distribuidor de partes de la gramatica con su respectivo patron
        /// </summary>
        /// <param name="list">Recibe una lista que contiene la parte de la gramática que se verificará</param>
        static bool checksDealer(List<string> list)
        {
            if (ExpresionRegularSETS.IniciaEnSets(list[0]))
            {
                var verificationResult = checkPattern(list);

                //verificationResult debe interpretarse para saber si hay un error si es asi detener la verificacion
                if (verificationResult.OcurrioError)
                {
                    Console.WriteLine("ERROR----------------------------------------------SETS");
                    Console.WriteLine(verificationResult.Texto);
                    return false;
                }
                else
                {
                    list.RemoveAt(0); // elimina el identificador SETS de la lista
                    Sets = list;
                    return true;
                }
            }
            else if (ExpresionRegularTOKENS.IniciaEnToken(list[0]))
            {
                //  TODO: Aun falta revisarlo xd
                var verificationResult = checkPattern(list);
                if (verificationResult.OcurrioError)
                {
                    Console.WriteLine("ERROR--------------------------------------------TOKENS");
                    Console.WriteLine(verificationResult.Texto);
                    return false;
                }
                else
                {
                    list.RemoveAt(0); // elimina el identificador TOKENS de la lista
                    Tokens = list;
                    return true;
                }
            }
            else if (ExpresionRegularACTION.IniciaEnAction(list[0]))
            {
                //  TODO: Aun falta revisarlo xd
                try
                {
                    bool ReservadasKey = false;
                    foreach (string reservadasSerach in list)
                    {
                        if (Regex.IsMatch(reservadasSerach, @"\s*RESERVADAS\(\)\s*"))//Verifica si existe la funcion RESERVADAS
                        {
                            ReservadasKey = true;
                        }
                    }
                    if (ReservadasKey)
                    {
                        var verificationResult = checkPattern(list);
                        list.RemoveAt(0); // elimina el identificador Actions de la lista
                        list.RemoveAt(0); // elimina el identificador RESERVADAS de la lista
                        list.RemoveAt(0); // eliminar el identificar { de la lista
                        list.RemoveAt(list.Count - 1); // eliminar el identificar } de la lista

                        Reservadas = list;

                        if (verificationResult.OcurrioError)
                        {
                            Console.WriteLine("ERROR-------------------------------------------ACTIONS");
                            Console.WriteLine(verificationResult.Texto);
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        throw new Exception("No existe la palabra RESERVADAS que es requerida.");
                    }
                }
                catch
                {
                    return false;
                }
            }
            else if (ExpresionRegularERROR.IniciaEnError(list[0]))//Si el primer elemento de la lista es un ERROR
            {
                //  TODO: Aun falta revisarlo xd
                var verificationResult = checkPattern(list);
                if (verificationResult.OcurrioError)
                {
                    Console.WriteLine("ERROR---------------------------------------------ERRORS");
                    Console.WriteLine(verificationResult.Texto);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else return false;
        }


        /// <summary>
        /// Identifica si la lista
        /// </summary>
        /// <param name="list">Lista de SETS, TOKENS, ACTIONS o ERRORS</param>
        /// <param name="pattern">patrón verificador</param>
        /// <param name="fila">fila en la que inician los chequeos</param>
        /// <returns>Devuelve un string con: 1. la información de fallo 2. la fila actual</returns>
        static Mensaje checkPattern(List<string> list)
        {
            var mensaje = new Mensaje();
            var caso = "";
            var removelines = new List<string>();
            try
            {
                if (list.Count > 1 || ExpresionRegularERROR.IniciaEnError(list[0]))//si existe almenos 1 elemento más que el indicador de la parte
                {
                    foreach (string element in list) //recorre la lista elemento por elemento
                    {
                        if (string.IsNullOrEmpty(caso))
                        {
                            caso = element;
                            caso = caso.Replace(" ", ""); //TODO: Removemos espacios en blanco si trae
                        }
                        if (ExpresionRegularSETS.TabOrEspaceOrEnter(element))
                        {
                            removelines.Add(element);
                        }
                        else if (!string.IsNullOrEmpty(element))
                        {
                            switch (caso)
                            {
                                case "SETS":
                                    if (!ExpresionRegularSETS.IniciaEnSets(element) && !ExpresionRegularSETS.ContenidoValidoSets(element))
                                    {
                                        throw new Exception($"Error en Seccion = {caso}. Fila = {ROWS}. Elemento = {element}");
                                    }
                                    break;

                                case "TOKENS":
                                    if (!ExpresionRegularTOKENS.IniciaEnToken(element) && !ExpresionRegularTOKENS.ContenidoValidoTokens(element))
                                    {
                                        throw new Exception($"Error en Seccion = {caso}. Fila = {ROWS}. Elemento = {element}");
                                    }
                                    break;
                                case "ACTIONS":
                                    if (!ExpresionRegularACTION.IniciaEnAction(element) && !ExpresionRegularACTION.ContenidoValidoActions(element))
                                    {
                                        throw new Exception($"Error en Seccion = {caso}. Fila = {ROWS}. Elemento = {element}");
                                    }
                                    break;
                                case @"(\s*\w*ERROR\w*\s*=)(\s*\d*\s*)":
                                    if (!ExpresionRegularERROR.IniciaEnError(element) && !ExpresionRegularERROR.ContenidoValidoError(element))
                                    {
                                        throw new Exception($"Error en Seccion = {caso}. Fila = {ROWS}. Elemento = {element}");
                                    }
                                    break;
                            }
                        }
                        ROWS++;
                    }
                    foreach (var removal in removelines)
                    {
                        list.Remove(removal);
                    }
                }
                else
                    throw new Exception("No hay suficientes elementos para verificar");
                return mensaje;
            }
            catch (Exception ex)
            {
                mensaje.OcurrioError = true;

                mensaje.Texto = $"\nMensaje: {ex.Message} :(";
                return mensaje;
            }
        }
    }
}

