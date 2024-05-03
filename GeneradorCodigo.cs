using Proyecto_Lenguajes_Formales_y_Automatas.ARBOL;
using Proyecto_Lenguajes_Formales_y_Automatas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Runtime.InteropServices;
using Proyecto_Lenguajes_Formales_y_Automatas.Data;

namespace ProjectoLFA.Data
{
    public class GeneradorCodigo
    {
        //  TODO: Metodo para escribir archivo en python
        InfoMetodosVM infoMetodos;

        public GeneradorCodigo(InfoMetodosVM infoMetodos)
        {
            this.infoMetodos = infoMetodos;
        }

        public Mensaje Generar()
        {
            var respuestaGeneracion = new Mensaje();
            var librerias = new GeneradorVM();
            var estructura = new GeneradorVM();
            var enter = new GeneradorVM();
            var clase = new GeneradorVM();
            try
            {
                //  TODO: Path
                var nombreArchivo = "Automata.py";
                var pathFile = "";
                var directorio = AppDomain.CurrentDomain.BaseDirectory;
                directorio = Path.Combine(directorio, "PythonResult");

                if (!Directory.Exists(directorio))
                    Directory.CreateDirectory(directorio);

                pathFile = Path.Combine(directorio, nombreArchivo);

                //  TODO: Definimos la libreria
                librerias.Comentario = "# Librerias";
                librerias.Definicion = GetLibreriasPython();
                //Console.WriteLine(librerias.Definicion);

                //  TODO: Definimos la clase
                clase.Identacion = 0;
                clase.Comentario = "# Defincion de la clase";
                clase.Definicion = "class Automata:";
                clase.AgregarEspacio = true;

                //  TODO: Definimos el main del proyecto
                var main = new GeneradorVM();
                main.Identacion = 2;
                main.Comentario = "# Definimos el main del proyecto";
                main.Definicion = "\tdef main():";
                main.AgregarEspacio = true;
                clase.Hijos.Add(main);

                //  TODO: Informacion del main
                var mainContent = new GeneradorVM();
                mainContent = GenerarContenidoMain(main.Identacion);
                main.Hijos.Add(mainContent);

                //  TODO: Metodos que utiliza el main

                //  TODO: Metodo indentificar los simbolos
                var metodoTerminalChar = new GeneradorVM();
                metodoTerminalChar = GenerarMetodoIndentificarTerminalCaracter(main.Identacion);
                clase.Hijos.Add(enter);
                clase.Hijos.Add(metodoTerminalChar);

                //  TODO: Metodo idetentifica los sets
                var metodoSet = new GeneradorVM();
                metodoSet = GenerarMetodoSet(main.Identacion);
                clase.Hijos.Add(metodoSet);

                //  TODO: Metodo identifica las reservadas
                var metodoReservadas = new GeneradorVM();
                metodoReservadas = GenerarMetodoReservadas(main.Identacion);
                clase.Hijos.Add(metodoReservadas);

                //  TODO: Metodo identifica los tokens
                var metodoIdentificarTokens = new GeneradorVM();
                metodoIdentificarTokens = GenerarMetodoIdentificarTokens(main.Identacion);
                clase.Hijos.Add(metodoIdentificarTokens);

                //  TODO: Metodo identifica si es estado final
                var metodoEsEstadoFinal = new GeneradorVM();
                metodoEsEstadoFinal = GenerarEsEstadoFinal(main.Identacion);
                clase.Hijos.Add(metodoEsEstadoFinal);

                //  TODO: Agregamos la clase al contenido
                estructura.Identacion = 0;
                estructura.AgregarEspacio = true;
                estructura.Hijos.Add(clase);

                //  TODO: Agregamos la libreria al contenido
                librerias.Hijos.Add(estructura);
                //  TODO: Generamos el archivo
                //  TODO: Escribir el archivo;
                ManejadoArchivo manejadoArchivo = new ManejadoArchivo(pathFile);
                manejadoArchivo.ADAN(librerias);
                manejadoArchivo.Finalizar();
                /*
                var contenido = new StringBuilder();
                var comentario = librerias.Comentario;
                contenido.Append(comentario);
                contenido.Append(Environment.NewLine);
                contenido.Append(librerias.Definicion);
                main.Hijos.Add(mainContent);
                foreach (var Estructura in librerias.Hijos)
                {
                    contenido.Append(Estructura.Definicion);
                    contenido.Append(Environment.NewLine);
                    foreach (var Clase in Estructura.Hijos)
                    {
                        contenido.Append(Clase.Definicion);
                        contenido.Append(Environment.NewLine);
                        if (Clase.Hijos.Count > 0)
                        {
                            foreach (var subHijo in Clase.Hijos)
                            {
                                contenido.Append(subHijo.Definicion);
                                contenido.Append(Environment.NewLine);

                                if (subHijo.Hijos.Count > 0)
                                {
                                    foreach (var hojas in subHijo.Hijos)
                                    {
                                        if (hojas.Hijos.Count > 0)
                                        {
                                            contenido.Append(hojas.Definicion);
                                            contenido.Append(Environment.NewLine);
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                Console.WriteLine(contenido.ToString());
                File.WriteAllText(pathFile, contenido.ToString());
                */

                respuestaGeneracion.OcurrioError = false;
                respuestaGeneracion.Texto = "Se ha generado el archivo correctamente";
            }
            catch (Exception ex)
            {
                respuestaGeneracion.OcurrioError = true;
                respuestaGeneracion.Texto = ex.Message;
            }

            return respuestaGeneracion;
        }

        private string GetLibreriasPython()
        {
            var librerias = new GeneradorVM();
            librerias.Definicion = "import sys";
            librerias.Definicion += Environment.NewLine;
            librerias.Definicion += "import re";
            return librerias.Definicion;
        }

        private GeneradorVM GenerarContenidoMain(int identacion)
        {
            var mainContent = new GeneradorVM();
            mainContent.Comentario = "\t\t#Inicializamos los valores ";
            mainContent.Definicion = "\t\tset = set()";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\tterminales = []";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\treservadas = []";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\ttokens = []";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\testadoFinal = False";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\testado = 0";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\tfor linea in sys.stdin:";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\tlinea = linea.strip()";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\tif linea == \"\":";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\tcontinue";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\tif estado == 0:";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\tif IdentificarSet(linea):";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\t\testado = 1";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\telif IdentificarTerminalCaracter(linea):";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\t\testado = 2";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\telif IdentificarReservadas(linea):";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\t\testado = 3";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\telif estado == 1:";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\tif EsEstadoFinal(linea):";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\t\testadoFinal = True";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\t\testado = 0";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\telse:";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\t\testado = 1";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\telif estado == 2:";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\tif EsEstadoFinal(linea):";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\t\testadoFinal = True";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\t\testado = 0";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\telse:";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\t\testado = 2";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\telif estado == 3:";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\tif EsEstadoFinal(linea):";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\t\testadoFinal = True";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\t\testado = 0";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\telse:";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\t\t\testado = 3";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\tif estadoFinal:";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\tIdentificarTokenID()";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\tprint(\"Tokens: \" + str(tokens))";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\tprint(\"Reservadas: \"+ str(reservadas))";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\tprint(\"Terminales: \" + str(terminales))";
            mainContent.Definicion += Environment.NewLine;
            mainContent.Definicion += "\t\t\tprint(\"Set: \" + str(set))";
            return mainContent;
        }

        private GeneradorVM GenerarMetodoSet(int identacion)
        {
            var metodoSet = new GeneradorVM();
            metodoSet.Identacion = identacion;
            metodoSet.Comentario = "#Metodo para identificar los simbolos";
            metodoSet.Definicion += Environment.NewLine;
            metodoSet.Definicion = "\tdef IdentificarSet(linea):";
            metodoSet.Definicion += Environment.NewLine;
            metodoSet.Definicion += "\t\tset.add(linea)";
            metodoSet.Definicion += Environment.NewLine;
            metodoSet.Definicion += "\t\treturn True";
            return metodoSet;
        }

        private GeneradorVM GenerarMetodoIndentificarTerminalCaracter(int identacion)
        {
            var metodoTerminalChar = new GeneradorVM();
            metodoTerminalChar.Identacion = identacion;
            metodoTerminalChar.Comentario = "# Metodo para identificar los terminales";
            metodoTerminalChar.Definicion += Environment.NewLine;
            metodoTerminalChar.Definicion = "\tdef IdentificarTerminalCaracter(linea):";
            metodoTerminalChar.Definicion += Environment.NewLine;
            metodoTerminalChar.Definicion += "\t\tif re.match(r'^[a-zA-Z]+$', linea):";
            metodoTerminalChar.Definicion += Environment.NewLine;
            metodoTerminalChar.Definicion += "\t\t\tterminales.append(linea)";
            metodoTerminalChar.Definicion += Environment.NewLine;
            metodoTerminalChar.Definicion += "\t\treturn True";
            return metodoTerminalChar;
        }

        private GeneradorVM GenerarMetodoReservadas(int identacion)
        {
            var metodoReservadas = new GeneradorVM();
            metodoReservadas.Identacion = identacion;
            metodoReservadas.Comentario = "# Metodo para identificar las reservadas";
            metodoReservadas.Definicion += Environment.NewLine;
            metodoReservadas.Definicion = "\tdef IdentificarReservadas(linea):";
            metodoReservadas.Definicion += Environment.NewLine;
            metodoReservadas.Definicion += "\t\tif linea in reservadas:";
            metodoReservadas.Definicion += Environment.NewLine;
            metodoReservadas.Definicion += "\t\treturn True";
            return metodoReservadas;
        }

        private GeneradorVM GenerarMetodoIdentificarTokens(int identacion)
        {
            var metodoIdentificarTokens = new GeneradorVM();
            metodoIdentificarTokens.Identacion = identacion;
            metodoIdentificarTokens.Comentario = "# Metodo para identificar los tokens";
            metodoIdentificarTokens.Definicion += Environment.NewLine;
            metodoIdentificarTokens.Definicion = "\tdef IdentificarTokenID():";
            metodoIdentificarTokens.Definicion += Environment.NewLine;
            metodoIdentificarTokens.Definicion += "\t\tfor token in terminales:";
            metodoIdentificarTokens.Definicion += Environment.NewLine;
            metodoIdentificarTokens.Definicion += "\t\t\tif token not in set:";
            metodoIdentificarTokens.Definicion += Environment.NewLine;
            metodoIdentificarTokens.Definicion += "\t\t\t\ttokens.append(token)";
            metodoIdentificarTokens.Definicion += Environment.NewLine;
            metodoIdentificarTokens.Definicion += "\t\tfor reservada in reservadas:";
            metodoIdentificarTokens.Definicion += Environment.NewLine;
            metodoIdentificarTokens.Definicion += "\t\t\tif reservada not in set:";
            metodoIdentificarTokens.Definicion += Environment.NewLine;
            metodoIdentificarTokens.Definicion += "\t\t\t\ttokens.append(reservada)";
            return metodoIdentificarTokens;
        }

        private GeneradorVM GenerarEsEstadoFinal(int identacion)
        {
            var metodoEsEstadoFinal = new GeneradorVM();
            metodoEsEstadoFinal.Identacion = identacion;
            metodoEsEstadoFinal.Comentario = "# Metodo para identificar si es estado final";
            metodoEsEstadoFinal.Definicion += Environment.NewLine;
            metodoEsEstadoFinal.Definicion = "\tdef EsEstadoFinal(linea):";
            metodoEsEstadoFinal.Definicion += Environment.NewLine;
            metodoEsEstadoFinal.Definicion += "\t\tif linea in infoMetodos.EstadosFinales:";
            metodoEsEstadoFinal.Definicion += Environment.NewLine;
            metodoEsEstadoFinal.Definicion += "\t\treturn True";
            return metodoEsEstadoFinal;
        }
    }
}