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

    public List<Gramatica> ReadFile(string filePath)
    {
        var lineaActual = "";
        var tieneSets = false;
        List<Gramatica> ListaLists = new List<Gramatica>();
        try
        {
            if (!File.Exists(filePath))
                throw new Exception("El archivo no existe");

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
                        //  TODO: No agregar espacios, tab o enters
                        if (!ExpresionRegularSETS.TabOrEspaceOrEnter(lineaActual))
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
        return ListaLists;
    }

    /// <summary>
    /// Verifica si la gramatica proporcionada es válida
    /// </summary>
    /// <param name="SETS_TOKENS_ACTIONS_ERRORS">Lista de listas con la gramática separada por partes</param>
    public bool VerifyGramar(List<Gramatica> SETS_TOKENS_ACTIONS_ERRORS)
    {
        try
        {
            if (SETS_TOKENS_ACTIONS_ERRORS.Count < 2)
                throw new Exception("A la gramatica le faltan grupos");
            bool respuesta = false;
            foreach (var gramarParts in SETS_TOKENS_ACTIONS_ERRORS)//se recorre la lista de las partes, parte por parte para su verificacion
            {
                respuesta = checksDealer(gramarParts.Grupo);
                if (!respuesta)
                    throw new Exception("Gramática incorrecta.");
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

    static bool checksDealer(List<string> list)
    {
        int actualRow = 0;

        if (ExpresionRegularSETS.IniciaEnSets(list[0]))
        {
            var verificationResult = checkPattern(list, actualRow);

            //verificationResult debe interpretarse para saber si hay un error si es asi detener la verificacion
            if (verificationResult.OcurrioError)
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine(verificationResult.Texto);
                return false;
            }
            else
            {
                actualRow = verificationResult.Row;
                return true;
            }
        }
        else if (ExpresionRegularTOKENS.IniciaEnToken(list[0]))
        {
            //  TODO: Aun falta revisarlo xd
            var verificationResult = checkPattern(list, actualRow);
            if (verificationResult.OcurrioError)
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine(verificationResult.Texto);
                return false;
            }
            else
            {
                actualRow = verificationResult.Row;
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
                    var verificationResult = checkPattern(list, actualRow);
                    if (verificationResult.OcurrioError)
                    {
                        Console.WriteLine("-------------------------------------------------------");
                        Console.WriteLine(verificationResult.Texto);
                        return false;
                    }
                    else
                    {
                        actualRow = verificationResult.Row;
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
            var verificationResult = checkPattern(list, actualRow);
            if (verificationResult.OcurrioError)
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine(verificationResult.Texto);
                return false;
            }
            else
            {
                actualRow = verificationResult.Row;
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Identifica si la lista
    /// </summary>
    /// <param name="list">Lista de SETS, TOKENS, ACTIONS o ERRORS</param>
    /// <param name="pattern">patrón verificador</param>
    /// <param name="fila">fila en la que inician los chequeos</param>
    /// <returns>Devuelve un string con: 1. la información de fallo 2. la fila actual</returns>
    static Mensaje checkPattern(List<string> list, int fila)
    {
        var mensaje = new Mensaje();
        var caso = "";
        int row = fila; //indica la fila actual
        try
        {
            if (list.Count > 1 || ExpresionRegularERROR.IniciaEnError(list[0]))//si existe almenos 1 elemento más que el indicador de la parte
            {
                foreach (string element in list) //recorre la lista elemento por elemento
                {
                    if (string.IsNullOrEmpty(caso))
                    {
                        caso = element;
                        caso = caso.Replace(" ", ""); //TODO: Removemos espacios en blanco si trae xds
                    }

                    switch (caso)
                    {
                        case "SETS":

                            if (ExpresionRegularSETS.IniciaEnSets(element) || ExpresionRegularSETS.ContenidoValidoSets(element))
                                row++;
                            else
                            {
                                row++;
                                throw new Exception($"Error en Seccion = {caso}. Fila = {row}");
                            }

                            break;

                        case "TOKENS":

                            if (ExpresionRegularTOKENS.IniciaEnToken(element) || ExpresionRegularTOKENS.ContenidoValidoTokens(element))
                                row++;
                            else
                            {
                                row++;
                                throw new Exception($"Error en Seccion = {caso}. Fila = {row}");
                            }
                            break;
                        case "ACTIONS":

                            if (ExpresionRegularACTION.IniciaEnAction(element) || ExpresionRegularACTION.ContenidoValidoActions(element))
                                row++;
                            else
                            {
                                row++;
                                throw new Exception($"Error en Seccion = {caso}. Fila = {row}");
                            }
                            break;
                        case @"(\s*\w*ERROR\w*\s*=)(\s*\d*\s*)":

                            if (ExpresionRegularERROR.IniciaEnError(element) || ExpresionRegularERROR.ContenidoValidoError(element))
                                row++;
                            else
                            {
                                row++;
                                throw new Exception($"Error en Seccion = {caso}. Fila = {row}");
                            }
                            break;
                    }
                }
            }
            else
                throw new Exception("No hay suficientes elementos para verificar");

            mensaje.Row = row;
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
