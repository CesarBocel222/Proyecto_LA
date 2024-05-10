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
using Proyecto_Lenguajes_Formales_y_Automatas.DATA;

namespace ProjectoLFA.Data
{
    public class GeneradorCodigo
    {
        //  TODO: Metodo para escribir archivo en java
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
            var constructor = new GeneradorVM();
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
                librerias.Identacion = 0;
                librerias.Definicion = GetLibreriasPython();

                //  TODO: Definimos la clase
                clase.Identacion = 0;
                clase.Comentario = "# Defincion de la clase";
                clase.Definicion = "class Automata:";

                //  TODO: Definimos el main del proyecto
                var main = new GeneradorVM();
                main.Identacion = 2;
                main.Comentario = "# Definimos el main del proyecto";
                main.Definicion = "def main(self):";

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
                clase.Hijos.Add(enter);
                clase.Hijos.Add(metodoSet);

                //  TODO: Metedo identifica el token de la reservada
                var metodoReservada = new GeneradorVM();
                metodoReservada = GenerarMetodoReservadas(main.Identacion);
                clase.Hijos.Add(enter);
                clase.Hijos.Add(metodoReservada);

                //  TODO: Metodo identifica los tokens id de una cadena
                var metodoTokensID = new GeneradorVM();
                metodoTokensID = GenerarMetodoIdentificarTokens(main.Identacion);
                clase.Hijos.Add(enter);
                clase.Hijos.Add(metodoTokensID);

                //  TODO: Metodo que identifica si nos quedamos en un estaod final
                var metodoEstadoFinal = new GeneradorVM();
                metodoEstadoFinal = GenerarEsEstadoAceptacion(main.Identacion);
                clase.Hijos.Add(enter);
                clase.Hijos.Add(metodoEstadoFinal);

                //  INFO: Agregamos todo lo que va llevar el archivo en orden
                estructura.Hijos.Add(librerias);
                estructura.Hijos.Add(clase);

                //Creacion del constructor 
                constructor.Comentario = "# Constructores de clase";
                constructor.Definicion = GetMain();
                estructura.Hijos.Add(constructor);


                //  TODO: Escribir el archivo;
                ManejadoArchivo manejadoArchivo = new ManejadoArchivo(pathFile);
                manejadoArchivo.ADAN(estructura);
                manejadoArchivo.Finalizar();
            }
            catch (Exception ex)
            {
                respuestaGeneracion.OcurrioError = true;
                respuestaGeneracion.Texto = $"[ERROR] Mensaje: {ex.Message}";
            }
            return respuestaGeneracion;
        }

        //  TODO: Metodo que escribre lo que va ir en el main
        //  1
        private GeneradorVM GenerarContenidoMain(int identacion)
        {
            var main = new GeneradorVM();
            var instruccion = new GeneradorVM();

            //  TODO: Definicion de variables y acciones para la logica del programa

            instruccion.Identacion = identacion + 2;
            //instruccion.Definicion = "sc = input()";
            main.Hijos.Add(instruccion);

            instruccion = new GeneradorVM();
            instruccion.Identacion = identacion + 2;
            instruccion.Definicion = "print(\"Ingrese el codigo\")";
            main.Hijos.Add(instruccion);

            instruccion = new GeneradorVM();
            instruccion.Identacion = identacion + 2;
            instruccion.Definicion = "programa = input() + \" \"";
            main.Hijos.Add(instruccion);

            instruccion = new GeneradorVM();
            instruccion.Identacion = identacion + 2;
            instruccion.Comentario = "# Nos guarda la posicion de la cadena que leemos";
            instruccion.Definicion = "index = 0";
            main.Hijos.Add(instruccion);

            instruccion = new GeneradorVM();
            instruccion.Identacion = identacion + 2;
            instruccion.Comentario = "# Nos guarde el estado en que no encontramos";
            instruccion.Definicion = "estado_actual = 0";
            main.Hijos.Add(instruccion);

            instruccion = new GeneradorVM();
            instruccion.Identacion = identacion + 2;
            instruccion.Comentario = "# Finaliza la lectura del archivo con si hay un error";
            instruccion.Definicion = "ocurrio_error = False";
            main.Hijos.Add(instruccion);

            instruccion = new GeneradorVM();
            string estadosFinalStr = String.Join(',', infoMetodos.EstadosAceptacion);

            instruccion.Identacion = identacion + 2;
            instruccion.Comentario = "# Nos guarda los estados de aceptacion";
            instruccion.Definicion = (
                     new StringBuilder().Append("estados_finales = [0] + ").Append('[').Append("0,").Append(estadosFinalStr).Append(']')
                                     ).ToString();
            main.Hijos.Add(instruccion);

            instruccion = new GeneradorVM();
            instruccion.Identacion = identacion + 2;
            instruccion.Comentario = "# Este nos va servir para evaluar que token es o si es una reservada";
            instruccion.Definicion = "token = \"\"";
            main.Hijos.Add(instruccion);


            //  INFO: LOGICA DEL PROGRAMA 
            var cicloWhile = new GeneradorVM();
            var contenidoWhile = new GeneradorVM();

            cicloWhile.Identacion = identacion + 2;
            cicloWhile.Definicion = "while index < len(programa) and not ocurrio_error == False:";
            contenidoWhile = ContenidoWhile(cicloWhile.Identacion);
            cicloWhile.Hijos.Add(contenidoWhile);

            main.Hijos.Add(cicloWhile);

            //  INFO: VALICACION DE ESTADO
            var ifFinal = new GeneradorVM();
            ifFinal = GetEstadoFinalValidacion(identacion + 2);
            main.Hijos.Add(ifFinal);

            return main;
        }

        //  INFO: Metodo para escribir los sets en Python
        private GeneradorVM GenerarMetodoSet(int identacion)
        {

            var metodoSet = new GeneradorVM();
            metodoSet.Comentario = "# Metodo para identificar set";
            metodoSet.Definicion = "def identificar_set(self, caracter):";
            metodoSet.Identacion = identacion;

            int filaSet = 1;
            var nombreCaracter = "caracterValue";

            var valorCaracter = new GeneradorVM()
            {
                Comentario = $"# Valor caracter",
                Definicion = "caracterValue = ord(caracter)",
                Identacion = identacion + 2
            };

            metodoSet.Hijos.Add(valorCaracter);

            foreach (var set in infoMetodos.Sets)
            {
                var etiquetaSet = ExpresionRegularSETS.GetEtiquetaSet(set);

                var comentario = new GeneradorVM()
                {
                    Comentario = $"# SET {etiquetaSet}",
                    Identacion = identacion + 2
                };
                metodoSet.Hijos.Add(comentario);

                var contenidoSet = ExpresionRegularSETS.GetContenedidoSet(set);
                if (string.IsNullOrEmpty(contenidoSet))
                    continue;

                var grupos = contenidoSet.Split('+');

                int grupo = 1;
                foreach (var grup in grupos)
                {
                    if (grup.Contains(".."))
                    {
                        var elementos = grup.Split("..");

                        string definicionH1 = "";
                        string definicionH2 = "";

                        var esChar = ExpresionRegularSETS.EsChar(elementos[0]);
                        if (!esChar)
                        {
                            var elemt1 = ExpresionRegularSETS.GetElemento(elementos[0]);
                            var elemt2 = ExpresionRegularSETS.GetElemento(elementos[1]);
                            definicionH1 = $"limite_Inf_G{grupo}_SET{filaSet} = (int)('{elemt1}')";
                            definicionH2 = $"limite_Sup_G{grupo}_SET{filaSet} = (int)('{elemt2}')";
                        }
                        else
                        {
                            var elemt1 = ExpresionRegularSETS.GetValorChar(elementos[0]);
                            var elemt2 = ExpresionRegularSETS.GetValorChar(elementos[1]);
                            definicionH1 = $"limite_Inf_G{grupo}_SET{filaSet} = {elemt1}";
                            definicionH2 = $"limite_Sup_G{grupo}_SET{filaSet} = {elemt2}";
                        }

                        var hijo1 = new GeneradorVM()
                        {
                            Definicion = definicionH1,
                            Identacion = metodoSet.Identacion + 2
                        };

                        var hijo2 = new GeneradorVM()
                        {
                            Definicion = definicionH2,
                            Identacion = metodoSet.Identacion + 2
                        };
                        metodoSet.Hijos.Add(hijo1);
                        metodoSet.Hijos.Add(hijo2);

                        string cond0 = $"{nombreCaracter}";
                        string cond1 = $"limite_Inf_G{grupo}_SET{filaSet}";
                        string cond2 = $"limite_Sup_G{grupo}_SET{filaSet}";
                        string result = etiquetaSet;

                        var ifSet = IfSet(cond0, cond1, cond2, result, identacion + 2);
                        metodoSet.Hijos.Add(ifSet);
                    }
                    else
                    {
                        var elemt = ExpresionRegularSETS.GetElemento(grup);
                        var hijo1 = new GeneradorVM()
                        {
                            Definicion = $"limite_Unic_G{grupo}_SET{filaSet} = (int)('{elemt}')",
                            Identacion = metodoSet.Identacion + 2
                        };
                        metodoSet.Hijos.Add(hijo1);

                        string cond0 = $"{nombreCaracter}";
                        string cond1 = $"limite_Unic_G{grupo}_SET{filaSet}";
                        string result = etiquetaSet;
                        var ifSet = IfSet(cond0, cond1, condicion2: "", result, identacion + 2);
                        metodoSet.Hijos.Add(ifSet);
                    }
                    grupo++;
                }
                filaSet++;
            }

            var returnVacio = new GeneradorVM()
            {
                Identacion = identacion + 2,
                Comentario = "# No es un set",
                Definicion = "return \"\""
            };
            metodoSet.Hijos.Add(returnVacio);

            return metodoSet;
        }

        //  INFO: Metodo para identificar caracter como terminal
        private GeneradorVM GenerarMetodoIndentificarTerminalCaracter(int identacion)
        {
            var metodoTerminalChar = new GeneradorVM();

            metodoTerminalChar.Comentario = "# Metodo para identificar Terminal Char";
            metodoTerminalChar.Definicion = "def identificar_terminal(self, caracter):";
            metodoTerminalChar.Identacion = identacion;

            GeneradorVM ifValidacion;
            GeneradorVM respuestaIf;
            GeneradorVM enter = new GeneradorVM();
            foreach (var simbolo in infoMetodos.Simbolos)
            {
                ifValidacion = new GeneradorVM();
                respuestaIf = new GeneradorVM();

                ifValidacion.Identacion = identacion + 2;
                if (simbolo == "\"" || simbolo == "\'")
                {
                    ifValidacion.Definicion = $"if caracter == '\\{simbolo}': ";
                }
                else
                {
                    ifValidacion.Definicion = $"if caracter == '{simbolo}': ";
                }

                respuestaIf.Identacion = identacion + 4;
                respuestaIf.Definicion = $"return \"{simbolo}\" ";
                respuestaIf.Hijos.Add(enter);

                ifValidacion.Hijos.Add(respuestaIf);

                metodoTerminalChar.Hijos.Add(ifValidacion);
            }

            //  TODO: Si es un espacio en blanco
            ifValidacion = new GeneradorVM();
            respuestaIf = new GeneradorVM();

            ifValidacion.Identacion = identacion + 2;
            ifValidacion.Definicion = $"if caracter == ' ': ";

            respuestaIf.Identacion = identacion + 4;
            respuestaIf.Definicion = $"return \"BLANK_SPACE\" ";
            respuestaIf.Hijos.Add(enter);

            ifValidacion.Hijos.Add(respuestaIf);
            metodoTerminalChar.Hijos.Add(ifValidacion);

            //  TODO: Si no es ninguno de los anteriores
            GeneradorVM respuestaFinal = new GeneradorVM();
            respuestaFinal.Identacion = identacion + 2;
            respuestaFinal.Definicion = "return \"\" ";
            respuestaFinal.Hijos.Add(enter);
            metodoTerminalChar.Hijos.Add(respuestaFinal);

            return metodoTerminalChar;
        }

        //  INFO: Metodo para escribir los reservados en java
        private GeneradorVM GenerarMetodoReservadas(int identacion)
        {
            var metodoReservadas = new GeneradorVM();
            metodoReservadas.Comentario = "# Metodo para identificar Reservadas";
            metodoReservadas.Definicion = "def identificar_reservadas(self, comando):";
            metodoReservadas.Identacion = identacion;

            GeneradorVM enter = new GeneradorVM();
            foreach (var contenido in infoMetodos.Reservadas)
            {
                int reservadaId = ExpresionRegularTOKENS.GetTokenIdReservada(contenido);
                string reservadaContenido = ExpresionRegularTOKENS.GetContenidoReservada(contenido);

                GeneradorVM ifReservada = new GeneradorVM();
                GeneradorVM resReservada = new GeneradorVM();

                ifReservada.Identacion = identacion + 2;
                ifReservada.Definicion = $"if comando.lower() == \"{reservadaContenido}\".lower():";

                resReservada.Identacion = identacion + 4;
                resReservada.Definicion = $"return \"TOKEN {reservadaId}\"";
                resReservada.Hijos.Add(enter);

                ifReservada.Hijos.Add(resReservada);
                metodoReservadas.Hijos.Add(ifReservada);
            }

            //  TODO: Si no es ninguno de estos
            GeneradorVM respuestaFinal = new GeneradorVM();
            respuestaFinal.Identacion = identacion + 2;
            respuestaFinal.Definicion = "return \"TOKEN 4\"";

            metodoReservadas.Hijos.Add(respuestaFinal);

            return metodoReservadas;
        }

        //  INFO: Metodo para escribir los tokens en java
        private GeneradorVM GenerarMetodoIdentificarTokens(int identacion)
        {
            var metodoGetTokenId = new GeneradorVM();
            metodoGetTokenId.Comentario = "# Metodo para identificar a que token pertence";
            metodoGetTokenId.Identacion = identacion;
            metodoGetTokenId.Definicion = "def identificar_token_id(self, token):";

            var enter = new GeneradorVM();
            foreach (var token in infoMetodos.Tokens)
            {
                var ifMetod = new GeneradorVM();
                var respIf = new GeneradorVM();

                ifMetod.Identacion = identacion + 2;
                ifMetod.Definicion = $"if re.match(\"{token.SimboloValor}\", token):";

                respIf.Identacion = identacion + 4;
                respIf.Definicion = $"return \"TOKEN {token.SimboloId}\" ";
                respIf.Hijos.Add(enter);

                ifMetod.Hijos.Add(respIf);
                metodoGetTokenId.Hijos.Add(ifMetod);
            }

            //  TODO: Si no es nada de lo anterior
            var respuestaFinal = new GeneradorVM();
            respuestaFinal.Identacion = identacion + 2;
            respuestaFinal.Definicion = "return \"\" ";

            metodoGetTokenId.Hijos.Add(respuestaFinal);

            return metodoGetTokenId;
        }

        //  TODO: Metodo para buscar el si el estado es estado de aceptacion
        private GeneradorVM GenerarEsEstadoAceptacion(int identacion)
        {
            var metodoEstadoAceptacion = new GeneradorVM();
            var instruccion = new GeneradorVM();

            metodoEstadoAceptacion.Comentario = "# Nos sirve para saber si nos quedamos en un estado final";
            metodoEstadoAceptacion.Identacion = identacion;
            metodoEstadoAceptacion.Definicion = "def es_estado_final(self, estado_actual, estados_finales):";

            //  Definimos un ciclo for
            instruccion.Identacion = identacion + 2;
            instruccion.Definicion = "for i in range(len(estados_finales)):";

            var contentidoFor = new GeneradorVM();
            contentidoFor.Identacion = identacion + 4;
            contentidoFor.Definicion = "if estados_finales[i] == estado_actual:";

            var returnIf = new GeneradorVM();
            returnIf.Identacion = identacion + 6;
            returnIf.Definicion = "return True";
            instruccion.Hijos.Add(contentidoFor);
            instruccion.Hijos.Add(returnIf);

            // Si no esta en el ciclo for

            var returnDefault = new GeneradorVM();
            returnDefault.Identacion = identacion + 2;
            returnDefault.Definicion = "return False";

            //  Agregamos todo
            metodoEstadoAceptacion.Hijos.Add(instruccion);
            metodoEstadoAceptacion.Hijos.Add(returnDefault);

            return metodoEstadoAceptacion;
        }

        //  INFO: Metodo para genearIfSet
        private GeneradorVM IfSet(string condicion0, string condicion1, string condicion2, string resultado, int identacion)
        {
            var ifSet = new GeneradorVM();
            var restIf = new GeneradorVM();
            var espacio = new GeneradorVM();

            ifSet.Identacion = identacion;

            if (!string.IsNullOrEmpty(condicion2))
                ifSet.Definicion = $"if {condicion0} >= {condicion1} and {condicion0} <= {condicion2}:";
            else
                ifSet.Definicion = $"if {condicion0} >= {condicion1}:";

            restIf.Identacion = identacion + 4;
            restIf.Definicion = $"return \"{resultado}\"";
            restIf.Hijos.Add(espacio);


            ifSet.Hijos.Add(restIf);
            return ifSet;
        }

        //  INFO: Define las librerias que utilzara el programa
        private string GetLibreriasPython()
        {
            string[] librerias = new string[] {
                "import sys",
                "import re",
            };

            string libreriasStr = string.Empty;

            foreach (var libreria in librerias)
            {
                libreriasStr += $"{libreria} \n";
            }

            return libreriasStr;
        }
        private string GetMain()
        {
            string[] constructor = new string[] {
                "automata = Automata()",
                "automata.main()",
            };

            string ConstructorStr = string.Empty;

            foreach (var emilio in constructor)
            {
                ConstructorStr += $"{emilio} \n";
            }

            return ConstructorStr;
        }

        //  TODO: Metodo para identificar partes del codigo con automata
        //  2
        private GeneradorVM ContenidoWhile(int identacion)
        {
            var contenidoWhile = new GeneradorVM();
            var enter = new GeneradorVM();

            //  TODO: Obteniendo informacion
            var instruccion = new GeneradorVM();
            instruccion.Identacion = identacion + 2;
            instruccion.Definicion = "caracter = programa[index]";
            contenidoWhile.Hijos.Add(instruccion);

            instruccion = new GeneradorVM();
            instruccion.Identacion = identacion + 2;
            instruccion.Definicion = "simbolo = self.identificar_set(caracter)";
            contenidoWhile.Hijos.Add(instruccion);


            //  Todo: Haciendo validaciones
            var ifInstru = new GeneradorVM();
            var ifResp = new GeneradorVM();

            ifInstru.Identacion = identacion + 2;
            ifInstru.Comentario = "#  Tratamos de verificar si es un terminal";
            ifInstru.Definicion = "if simbolo == \"\":";

            ifResp.Identacion = identacion + 4;
            ifResp.Definicion = "simbolo = self.identificar_terminal(caracter)\r\n";
            ifInstru.Hijos.Add(ifResp);
            ifInstru.Hijos.Add(enter);

            contenidoWhile.Hijos.Add(ifInstru);

            ifInstru = new GeneradorVM();
            ifResp = new GeneradorVM();

            ifInstru.Identacion = identacion + 2;
            ifInstru.Comentario = "#  ERROR: Muestra un error al no reconocer el simbolo como terminal o como set ";
            ifInstru.Definicion = "if simbolo == \"\":";

            ifResp.Identacion = identacion + 4;
            ifResp.Definicion = "print(\"El simbolo: '{caracter}' no es reconocido\")\r\n";
            ifInstru.Hijos.Add(ifResp);
            ifInstru.Hijos.Add(enter);

            contenidoWhile.Hijos.Add(ifInstru);

            //  TODO: Aca mandamos a crear todo lo de los switc case
            var switchEstados = SwitchEstados(identacion + 2);
            contenidoWhile.Hijos.Add(switchEstados);

            //  IMPORT: Para que no se quede en ciclado debe de ir al final esto
            var contadorWhile = new GeneradorVM();
            contadorWhile.Identacion = identacion + 2;
            contadorWhile.Definicion = "index += 1";
            contenidoWhile.Hijos.Add(contadorWhile);

            return contenidoWhile;
        }
        // 3
        //Preguntar a diego
        //  TODO: switch para hacer cambios de estado
        // Python no tiene switch
        private GeneradorVM SwitchEstados(int identacion)
        {
            GeneradorVM swichEstado = new GeneradorVM();
            GeneradorVM contenidoSwitch = new GeneradorVM();

            swichEstado.Identacion = identacion;
            swichEstado.Comentario = "# Identificar en que estado estoy";
            swichEstado.Definicion = "match estado_actual:";

            //  TODO: BREAK
            var breakSwitch = new GeneradorVM();
            breakSwitch.Identacion = identacion + 4;
            //breakSwitch.Definicion = "break";

            //  TODO: lo que va llevar el primer switch
            foreach (var estadoVM in infoMetodos.EstadosVM)
            {
                var caseSwitch = new GeneradorVM();
                var contenidoCase = new GeneradorVM();

                caseSwitch.Identacion = identacion + 2;
                caseSwitch.Definicion = $"case {estadoVM.Estado}:";

                //contenidoCase.Identacion = identacion + 4;
                //contenidoCase.Definicion = $"System.out.println(\"Estoy en el Estado: {estadoVM.Estado}\");";
                contenidoCase = SwitchSimbolo(identacion + 4, estadoVM.simbolos, estadoVM.Estado);
                caseSwitch.Hijos.Add(contenidoCase);

                swichEstado.Hijos.Add(caseSwitch);
                swichEstado.Hijos.Add(breakSwitch);
            }

            return swichEstado;
        }

        //  INFO: switch para hacer cambio de simbolo
        private GeneradorVM SwitchSimbolo(int identacion, List<Simbolos> simbolos, int estado)
        {
            GeneradorVM swichSimbolo = new GeneradorVM();
            swichSimbolo.Identacion = identacion;
            swichSimbolo.Comentario = "# Identificar en que simbolo estoy";
            swichSimbolo.Definicion = "match simbolo:";

            //  TODO: BREAK
            var breakSwitch = new GeneradorVM();
            breakSwitch.Identacion = identacion + 2;
            //breakSwitch.Definicion = "break";

            string simbolosEspera = "";

            foreach (var simbolo in simbolos)
            {
                var caseSwitch = new GeneradorVM();
                var contenidoCase = new GeneradorVM();

                caseSwitch.Identacion = identacion + 2;
                if (simbolo.Token == "\"" || simbolo.Token == "\'")
                {
                    caseSwitch.Definicion = $"case \"\\{simbolo.Token}\":";
                }
                else
                {
                    caseSwitch.Definicion = $"case \"{simbolo.Token}\":";
                }


                contenidoCase.Identacion = identacion + 4;
                contenidoCase.Definicion = $"estado_actual = {simbolo.Transicion}";
                caseSwitch.Hijos.Add(contenidoCase);

                contenidoCase = new GeneradorVM
                {
                    Identacion = identacion + 4,
                    Definicion = $"token += caracter"
                };
                caseSwitch.Hijos.Add(contenidoCase);

                swichSimbolo.Hijos.Add(caseSwitch);
                swichSimbolo.Hijos.Add(breakSwitch);

                simbolosEspera += $"{simbolo.Token} ";
            }

            //  Este nos sirve para agregar la validacion de que token es
            if (estado != 0)
            {
                var casoValidacionToken = new GeneradorVM();
                var contenidoValidacionToken = new GeneradorVM();
                casoValidacionToken.Identacion = identacion + 2;
                casoValidacionToken.Definicion = "case \"BLANK_SPACE\":";

                contenidoValidacionToken = GetValidacionEspacioBlanco(identacion + 4);
                casoValidacionToken.Hijos.Add(contenidoValidacionToken);
                swichSimbolo.Hijos.Add(casoValidacionToken);
                swichSimbolo.Hijos.Add(breakSwitch);
            }

            //  Este nos sirve para genera el caso default en el switch case en caso de que no encuentre el estado
            var casoDefault = new GeneradorVM();
            var contenidoCaseDefault = new GeneradorVM();
            var mensajeDefault = "";
            casoDefault.Identacion = identacion + 2;
            casoDefault.Definicion = "case _:";

            if (string.IsNullOrEmpty(simbolosEspera) || estado == 0)
                mensajeDefault = "Simbolo no reconocido";
            else
            {            
                    mensajeDefault = $"Se esperaba el ' \\{simbolosEspera} ' ";
            }

            contenidoCaseDefault.Identacion = identacion + 4;
            contenidoCaseDefault.Definicion = $"print(\"{ mensajeDefault}\")";

            casoDefault.Hijos.Add(contenidoCaseDefault);

            //Identificamos que ocurrio un error, ocurrioError
            contenidoCaseDefault = new GeneradorVM();
            contenidoCaseDefault.Identacion = identacion + 4;
            contenidoCaseDefault.Definicion = $"ocurrioError = True";
            casoDefault.Hijos.Add(contenidoCaseDefault);

            swichSimbolo.Hijos.Add(casoDefault);
            swichSimbolo.Hijos.Add(breakSwitch);

            return swichSimbolo;
        }

        //  INFO: validaciones de black_space
        private GeneradorVM GetValidacionEspacioBlanco(int identacion)
        {
            var contenido = new GeneradorVM();

            // TODO: Definimos variable que alamcena el token id
            var variableImprimir = new GeneradorVM();
            variableImprimir.Identacion = identacion + 2;
            variableImprimir.Definicion = "tokenImprimir = self.identificar_token_id(token)";
            contenido.Hijos.Add(variableImprimir);

            // TODO: Si no es un token, buscamos que token es en reservadas
            var ifInstru = new GeneradorVM();
            var ifResp = new GeneradorVM();

            ifInstru.Identacion = identacion + 2;
            ifInstru.Comentario = "# Buscamos que token Id es en reservadas";
            ifInstru.Definicion = "if tokenImprimir == \"\":";

            ifResp.Identacion = identacion + 4;
            ifResp.Definicion = "tokenImprimir = self.identificar_reservadas(token)";
            ifInstru.Hijos.Add(ifResp);

            contenido.Hijos.Add(ifInstru);
            //

            //  TODO: Si no es un token ni un reservada, mostramos un error
            var ifError = new GeneradorVM();
            var contIfError = new GeneradorVM();

            ifError.Identacion = identacion + 2;
            ifError.Comentario = "# Error por que no es un token ni una reservada";
            ifError.Definicion = "if tokenImprimir == \"\":";

            contIfError.Identacion = identacion + 4;
            contIfError.Definicion = "print(\"El simbolo: '{token}' no es reconocido\")";
            ifError.Hijos.Add(contIfError);
            contenido.Hijos.Add(ifError);

            //  TODO: Si es un token mostramos su id
            var elseError = new GeneradorVM();
            var contenidoElseError = new GeneradorVM();

            elseError.Identacion = identacion + 2;
            elseError.Definicion = "else:";

            //  TODO: Mostrmos mensaje del token id
            contenidoElseError.Identacion = identacion + 4;
            contenidoElseError.Definicion = "print(tokenImprimir)";
            elseError.Hijos.Add(contenidoElseError);

            // TODO: Limpiamos para buscar el siguiente token
            contenidoElseError = new GeneradorVM();
            contenidoElseError.Identacion = identacion + 4;
            contenidoElseError.Definicion = "token = \"\"";
            elseError.Hijos.Add(contenidoElseError);

            //  TODO: Limpiamos el estado
            contenidoElseError = new GeneradorVM();
            contenidoElseError.Identacion = identacion + 4;
            contenidoElseError.Definicion = "estado_actual = 0";
            elseError.Hijos.Add(contenidoElseError);

            contenido.Hijos.Add(elseError);
            return contenido;
        }

        //  TODO: If de validar estado final
        private GeneradorVM GetEstadoFinalValidacion(int identacion)
        {
            var contenido = new GeneradorVM();

            var ifV = new GeneradorVM();
            var restIf = new GeneradorVM();
            var espacio = new GeneradorVM();

            //  TODO: If para validar si me quede en un  estado final
            ifV.Identacion = identacion;
            ifV.Comentario = "# Mensaje de error si nos quedamos en un estado final";
            ifV.Definicion = $"if self.es_estado_final(estado_actual, estados_finales) and index == len(programa):";

            restIf.Identacion = identacion + 4;
            restIf.Definicion = $"print(\"PROGRAMA CORRECTO. :)))))\")";
            restIf.Hijos.Add(espacio);
            ifV.Hijos.Add(restIf);

            //  TODO: Else que me dice que me quede en un estado que no es final
            var elseV = new GeneradorVM();
            var resElse = new GeneradorVM();

            elseV.Identacion = identacion;
            elseV.Definicion = "else:";

            resElse.Identacion = identacion + 4;
            resElse.Definicion = $"print(\"PROGRAMA INCORRECTO. :)))))\")";
            resElse.Hijos.Add(espacio);
            elseV.Hijos.Add(resElse);

            contenido.Hijos.Add(ifV);
            contenido.Hijos.Add(elseV);

            return contenido;
        }
    }
}