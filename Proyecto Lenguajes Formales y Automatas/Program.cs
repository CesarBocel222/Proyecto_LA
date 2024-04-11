using Proyecto_Lenguajes_Formales_y_Automatas.ARBOL;
using Proyecto_Lenguajes_Formales_y_Automatas.DATA;
using System;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    //Se uso un metodo de github https://github.com/SantiagoBocel/Lenguajes para las verificaciones
    //Archivos
    // "C:/Users/50255/Desktop/URL/Septimo semestre/Lenguajes Formales y Automatas/Proyecto Lenguajes Formales y Automatas/Archivos/prueba3.txt";
    //"C:/Users/50255/Desktop/URL/Septimo semestre/Lenguajes Formales y Automatas/Proyecto Lenguajes Formales y Automatas/Archivos/GRAMATICA.txt"
    //"C:/Users/50255/Desktop/URL/Septimo semestre/Lenguajes Formales y Automatas/Proyecto Lenguajes Formales y Automatas/Archivos/GRAMATICA-3.txt"
    //"C:/Users/50255/Desktop/URL/Septimo semestre/Lenguajes Formales y Automatas/Proyecto Lenguajes Formales y Automatas/Archivos/GRAMATICA-4.txt"
    //"C:/Users/50255/Desktop/URL/Septimo semestre/Lenguajes Formales y Automatas/Proyecto Lenguajes Formales y Automatas/Archivos/GRAMATICA-5.txt"
    //"C:/atchivo.txt"
    static void Main()
    {
        var pathFile = "";
        Console.WriteLine("_______Bienvenido al Generador de Scanner: Lenguajes Formales y Automatas______\n");
        Console.WriteLine("Ingrese el path del archivo: ");
        pathFile = Console.ReadLine();
        Console.WriteLine("---------------------------------------");
        Console.WriteLine("\n\t Espere");
        do
        {
            ArbolExpresion expressionTree = new ArbolExpresion();
            if (!Regex.IsMatch(pathFile, @".+\.txt")) pathFile += ".txt";
            var al = new Verificaror();
            var gramatica = al.ReadFile(pathFile);
            if (gramatica.Count > 0)
            {
                var ordenCorrecto = al.VerifyOrderSection(gramatica);
                if (ordenCorrecto)
                    if (al.VerifyGramar(gramatica))
                        Console.WriteLine("Gramática correcta. :P");
                //Ceacion del AFD
                if (al.getTokens() != null)
                {
                    ExpresionRegular parametros = new ExpresionRegular();
                    parametros.ProcessRawLists(al.getTokens());
                    expressionTree.CreateTree(parametros, al.getSets().Count);
                    expressionTree.GenerateDFA();

                    //  TODO: Pendiente
                    expressionTree.LlenarEstadosAceptacion();
                    expressionTree.LlenarMovimientosEstado();
                    Console.WriteLine("---------------------------------------");
                    Console.WriteLine("Ingrese el número indicado para mosrar la información o 0 para salir del menu.");
                    var informacionGenerada = "";
                    while (informacionGenerada != "0")
                    {
                        Console.Write("\nNúmero indicador - Información generada\n1 - Nodos del árblo\n2 - Árbol de expresiones\n3 - Tabla de follow\n4 - Tabla de transiciones\n");
                        informacionGenerada = Console.ReadLine();
                        Console.WriteLine("---------------------------------------");
                        if (Regex.IsMatch(informacionGenerada, @"\d"))
                        {
                            switch (informacionGenerada)
                            {
                                case "0":
                                    Console.WriteLine("Regresando.");
                                    break;
                                case "1":
                                    expressionTree.printNodes();
                                    break;
                                case "2":
                                    Console.WriteLine("--------VISTA EN UNA LINEA--------");
                                    expressionTree.RecorridoInOrden();
                                    Console.WriteLine("\n--------VISTA POR NODOS--------");
                                    expressionTree.verArbol();
                                    break;
                                case "3":
                                    expressionTree.printFollow();
                                    break;
                                case "4":
                                    expressionTree.printDFATable();
                                    break;
                                default:
                                    Console.WriteLine("Opción inválida.");
                                    break;
                            }
                        }
                        else Console.WriteLine("Opción no reconocida.");
                        Console.WriteLine("\n---------------------------------------");
                    }
                }



                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("\nIngrese q para salir o el path de un nuevo archivo");
                pathFile = Console.ReadLine();
                al.nullSets();
                al.nullTokens();
                Console.WriteLine("\n\t Espere... :)");
            }
            else
            {
                Console.WriteLine("Ingrese el nombre de otro archivo");
                pathFile = Console.ReadLine();
            }
        } while (!Regex.IsMatch(pathFile, @"[Qq]"));
    }
}
