using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto_Lenguajes_Formales_y_Automatas.ARBOL
{
    /// <summary>
    /// Arbol de expresiones
    /// </summary>
    public class ArbolExpresion
    {
        //Diccionario con los operadores de expresiones regulares, y su precedencia
        protected Dictionary<char, int> precedencia = new Dictionary<char, int>
            {
            //A menor el valor del operador mayor su precedencia
                {'(', 0},
                {'{', 0},
                {')', 0},
                {'}', 0},
                {'+', 1},
                {'*', 2},
                {'?', 2},
                {' ', 3},
                {'|', 4},
                {'#', 5}
            };

        // Paso 1: Inicialización de los parametros del arbol
        //Pila de operadores
        private Stack<Symbol> T = new Stack<Symbol>();
        //Pila de árboles
        private Stack<NodoM> S = new Stack<NodoM>();
        //Raiz del árbol
        public NodoM root = new NodoM();
        //Declaración de la tabla de Follow
        public Dictionary<int, List<int>> FollowTable { get; set; }
        //Declaración de la tabla de estados/transiciones 
        public Dictionary<List<int>, Dictionary<string, List<int>>> DFATable;
        public List<int> EstadosAceptacion = new List<int>();

        //Declaracion de Tokens y sus Ids
        public List<InfoVM> InfoSimbolos = new List<InfoVM>();
        public List<string> SimbolosTerminales = new List<string>();

        //Declaro mi estructura que me va servir para el saber el movimiento de estados
        public List<Estados> EstadosVMov = new List<Estados>();

        //Linea actual del archivo .txt
        private int line;
        //Contador de comillas para simbolos terminales
        private bool comillaAbierta = false;
        /// <summary>
        /// Método que crece la raiz del árbol utilizando los tokens de la gramatica
        /// </summary>
        /// <param name="parametros">Objeto de tipo expresion regular que tiene una lista con los tokens de la gramatica</param>
        /// <param name="countSets">El numero de lineas en sets para contabilizar la linea actual</param>
        /// <returns>Retorna la raiz del árbol ya enraizado</returns>
        public NodoM CreateTree(ExpresionRegular parametros, int countSets)
        {
            //Declaracion del simbolo
            Symbol symbol;

            line = countSets + 2;
            for (int i = 0; i < parametros.Expresion.Count; i++)
            {
                line++;
                //Cambia el simbolo por el elemento en la posicion i de la lista de espresiones regulares
                symbol = parametros.Expresion.ElementAt(i);
                //Se inicializa un nodo temporal llamando a la funcion CreateTree(symbol)
                //Se inicializa un nodo auxiliar como un nodo nuevo
                NodoM temp = CreateTree(symbol), aux = new NodoM();
                if (temp != null)
                {
                    if (root.NodoIzquierdo == null)
                    {
                        root.Valor = new Symbol('|');
                        root.NodoIzquierdo = temp;
                    }
                    else if (root.NodoDerecho == null)
                    {
                        root.NodoDerecho = temp;
                    }
                    else
                    {
                        //Se asigna a la izquierda del nodo auxiliar la raiz actual
                        aux.NodoIzquierdo = root;
                        //Se asigna el valor '|' al nodo auxiliar
                        aux.Valor = new Symbol('|');
                        //Se asigna el nodo temporal a la derecha del nodo auxiliar
                        aux.NodoDerecho = temp;
                        //Se asigna el nodo auxiliar a la raiz
                        root = aux;
                    }
                }
            }
            //Paso 2: Se añade el simbolo final a la raiz del árbol
            //Se inicializa un nodo auxiliar como un nodo nuevo
            NodoM agregarSimboloFinal = new NodoM();
            //Se asigna el valor ' ' al nodo auxiliar
            agregarSimboloFinal.Valor = new Symbol(' ');
            //Se inicializa la derecha del nodo auxiliar como un nodo nuevo
            agregarSimboloFinal.NodoDerecho = new NodoM();
            //Se asigna '#' a la derecha del nodo auxiliar
            agregarSimboloFinal.NodoDerecho.Valor = new Symbol('#');
            //Se asigna la raiz actual a la izquierda del nodo auxiliar
            agregarSimboloFinal.NodoIzquierdo = root;
            //Se asigna el nodo auxiliar a la raiz del árbol
            root = agregarSimboloFinal;
            //Se retorna la raiz
            return root;
        }
        /// <summary>
        /// Crea un árbol en base a una expresion regular almacenada en un objeto simbolo
        /// </summary>
        /// <param name="simbolo">Objeto de tipo simbolo que contiene una expresion regular</param>
        /// <returns>Retorna un nodo que almacena la expresion regular</returns>
        public NodoM CreateTree(Symbol simbolo)
        {
            //Si la cadena tiene espacios  al inicio los elimina
            while (simbolo.Value.Length > 0 && simbolo.Value[0] == ' ')
            {
                simbolo.Value = simbolo.Value.Substring(1);
            }
            //Se extrae la expresion regular del objeto tipo Symbol
            var expresion = simbolo.Value;
            var tokenId = Convert.ToInt32(simbolo.tokenID); //   INFO: Se agrego el tokenId
            //Se inicializa un nodo como un nuevo nodo
            NodoM tree = new NodoM();
            try
            {
                // Paso 3: Mientras existan tokens en la expresión regular
                for (int i = 0; i < expresion.Length; i++)
                {
                    var token = expresion[i];
                    // Paso 4: Si token es un símbolo terminal
                    if (i > 0 && EsTerminal(expresion, i))
                    {
                        NodoM arbol = new NodoM();
                        arbol.Valor = new Symbol(token, tokenId); // INFO: Asignamos el valor del token en char con su identificador de token
                        S.Push(arbol);
                    }
                    //Paso 5: Si el token es un simbolo no terminal 
                    else if (EsNoTerminal(expresion, i))
                    {
                        string tempToken = CreateNT(expresion, i);
                        NodoM arbol = new NodoM();
                        arbol.Valor = new Symbol(tempToken, tokenId); // INFO: Asignamos el valor del token en string con su identificador de token
                        //  INFO Buscar guardar el token con su id
                        S.Push(arbol);
                        i += (tempToken.Length - 1);
                    }
                    // Paso 5: Si token es un paréntesis izquierdo
                    else if (token == '(' || token == '{')
                    {
                        T.Push(new Symbol(token));
                    }
                    // Paso 6: Si token es un paréntesis derecho
                    else if (token == ')' || token == '}')
                    {
                        //Mientras la pila T tenga mas de 0 eleentos y el primer valor no sea '(' o '{'
                        while (T.Count > 0 && T.Peek().Value != "(" && T.Peek().Value != "{")
                        {
                            //Se inicializa un caracter operador con el primer elemento de T
                            char operador = T.Pop().Value[0];
                            if (!precedencia.ContainsKey(operador))
                            {
                                throw new Exception("Error 1: Operador no reconocido. operador: " + operador + "token: " + token);
                            }
                            //Se inicializa un nodo como un nodo nuevo temporal
                            NodoM temp = new NodoM();
                            //Se asigna al valor del nodo temporal el operador
                            temp.Valor = new Symbol(operador);
                            if (S.Count < 2)
                            {
                                throw new Exception("Error 2: Faltan operandos. expresion: " + expresion + " i: " + i + " token: " + token);
                            }
                            //Se asigna a la derecha del nodo temporal el valor de S.Pop()
                            temp.NodoDerecho = S.Pop();
                            //Se asigna a la izquierda del nodo temporal el valor de S.Pop()
                            temp.NodoIzquierdo = S.Pop();
                            //Se introduce el nodo temporal en la pila S
                            S.Push(temp);
                        }
                        if (T.Count == 0)
                        {
                            throw new Exception("Error 3: Faltan operandos. expresion: " + expresion + "token: " + token);
                        }
                        T.Pop(); // Sacar el paréntesis izquierdo
                    }
                    // Paso 7: Si token es un operador
                    else if (precedencia.ContainsKey(token))
                    {
                        //Si el operador es unario
                        if (EsUnario(token))
                        {
                            //Se inicializa un nodo temporal como nuevo nodo
                            NodoM arbol = new NodoM();
                            //Se asigna al valor del nodo temporal el operador
                            arbol.Valor = new Symbol(token, tokenId);   //INFO: Se le agrega al operador unario el token id del que pertenece
                            if (S.Count == 0)
                            {
                                throw new Exception("Error 4: Faltan operandos. expresion: " + expresion + "token: " + token);
                            }
                            //Se asigna a la izquierda del nodo temporal el valor de S.Pop()
                            arbol.NodoIzquierdo = S.Pop();
                            //Se introduce a la pila S el nodo temporal
                            S.Push(arbol);
                        }
                        else
                        {
                            //Si el operador no es un espacio o si es un espacio y el contador i es menor a la longitud de la expresion -1 y el simbolo siguiente al actual no pertenece a la precedencia o es un parentesis abierto y el simbolo anterior al actial no pertenece a la precedencia o un parentesis abierto o las pilas S y T tienen 0 elementos
                            if ((token == ' ' && i < expresion.Length - 1 && (!precedencia.ContainsKey(expresion[i + 1]) || expresion[i + 1] == '(') && (!precedencia.ContainsKey(expresion[i - 1]) || expresion[i - 1] == '(')) || token != ' ' || (S.Count > 0 && T.Count == 0))
                            {
                                //Mientras la pila T tenga mas de 0 elemntos y el valor del primer elemento de T no dea '(' y el token pertenezca a la precedencia y la precedencia del token sea mayor o igual  al valor de la primera posicion de T
                                while (T.Count > 0 && T.Peek().Value[0] != '(' && precedencia.ContainsKey(token) && precedencia[token] >= precedencia[T.Peek().Value[0]])
                                {
                                    //Se asigna a un caracter operador  el valor de T.Pop().Value[0]
                                    char operador = T.Pop().Value[0];
                                    if (!precedencia.ContainsKey(operador))
                                    {
                                        throw new Exception("Error 5: Operador no reconocido. operador: " + operador);
                                    }
                                    //Se inicializa un nodo temporal como un nodo nuevo
                                    NodoM temp = new NodoM();
                                    //Se asigna el valor del operador al nodo temporal
                                    temp.Valor = new Symbol(operador);
                                    if (S.Count < 2)
                                    {
                                        throw new Exception("Error 6: Faltan operandos. expresion: " + expresion + " i: " + i + " token: " + token);
                                    }
                                    //Se Asigna a la derecha del noso temporal el valor de S.Pop()
                                    temp.NodoDerecho = S.Pop();
                                    //Se Asigna a la irquierda del noso temporal el valor de S.Pop()
                                    temp.NodoIzquierdo = S.Pop();
                                    //Se introduce el nodo temporal a S
                                    S.Push(temp);
                                }
                                //Se introduce el operador a la pila T
                                T.Push(new Symbol(token)); //  INFO: Se convertira el operador de concatenacion
                            }
                        }
                    }
                    else if (token == '\'')
                    {
                        //Si el indicador de comilla abierto es falso
                        if (!comillaAbierta)
                        {
                            //Se asigna true al indicador de comillas abiertas
                            comillaAbierta = true;
                            //Si la pila S tiene 1 elemento y la pila T tiene 0 elementos se introduce un operador de concatenacion
                            if (S.Count == 1 && T.Count == 0)
                            {
                                T.Push(new Symbol(' '));
                            }
                            //Si la pila T tiene 1 elemento y el primer elemento de la pila T es ' ' y la pila S tiene 2 o mas elementos
                            else if (T.Count == 1 && T.Peek().Value[0] == ' ' && S.Count >= 2)
                            {
                                NodoM temp = new NodoM();
                                temp.Valor = T.Pop();
                                temp.NodoDerecho = S.Pop();
                                temp.NodoIzquierdo = S.Pop();
                                S.Push(temp);
                            }
                        }
                        else
                        {
                            //Se asigna false al indicador de comillas abiertas
                            comillaAbierta = false;
                            //Si la pila S es mayor o igual a 2 y la pila T tiene 0 elementos
                            if (S.Count >= 2 && T.Count == 0)
                            {
                                NodoM temp = new NodoM();
                                temp.Valor = new Symbol(' ');
                                temp.NodoDerecho = S.Pop();
                                temp.NodoIzquierdo = S.Pop();
                                S.Push(temp);
                            }
                        }
                    }
                    //Si el token forma parte del conjunto \s
                    else if (Regex.IsMatch(token.ToString(), @"\s*")) ;
                    else
                    {
                        throw new Exception("Error 8: Token no reconocido. token: " + token + ". expresion: " + expresion);
                    }
                }
                //paso 8
                while (T.Count > 0)
                {
                    NodoM temp = new NodoM();
                    temp.Valor = T.Pop();
                    if (temp.Valor.Value != "(" && S.Count >= 2)
                    {
                        temp.NodoDerecho = S.Pop();
                        temp.NodoIzquierdo = S.Pop();
                        S.Push(temp);
                    }
                }
                //Paso 9: Asignar el arbol a retornar
                if (S.Count == 1)
                    tree = S.Pop();
                else
                {
                    tree = null;
                    throw new Exception("Error 9: Faltan operandos. expresion: " + expresion);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ms: {ex.Message}" + " linea: " + line);
            }
            return tree;
        }
        /// <summary>
        /// Determina si un caracter de la cadena es terminal 
        /// </summary>
        /// <param name="exp">expresion regular para analizar</param>
        /// <param name="i">Indice del caracter a analizar</param>
        /// <returns>Retorna: true: si es una caracter terminal, false: si no es un caracter terminal</returns>
        public bool EsTerminal(string exp, int i)
        {
            if (comillaAbierta && exp[i] != '\'')
            {
                return true;
            }
            //Si el caracter actual es una collima y el contador i no esta al final de la cadena
            else if (exp[i] == '\'' && i - 1 > 0 && i + 1 < exp.Length)
            {
                //Si El caracter que sigue al actual es una comilla y el caracter previo al actual es una collima
                if (exp[i - 1] == '\'' && exp[i + 1] == '\'')
                {
                    return true;
                }
                else return false;
            }
            else return false;
        }
        /// <summary>
        /// Determina si un caracter es no terminal
        /// </summary>
        /// <param name="exp">expresion regular para analizar</param>
        /// <param name="i">Indice del caracter a analizar</param>
        /// <returns>Retorna: true: si el caracter es no terminal, false: si el caracter no es no terminal</returns>
        public bool EsNoTerminal(string exp, int i)
        {
            //Se inicializa un objeto Regex regex con una expresion regular para identificar caracteres no terminales
            Regex regex = new Regex(@"\b[A-Z]+\b(\(\))?");
            //Se inicializa un objeto MatchCollection simboloNT como una coleccion de matches en la espresion regular
            MatchCollection simbolosNT = regex.Matches(exp);
            //Si hay mas de 0 coincidencias
            if (simbolosNT.Count > 0)
            {
                //Por cada match en la coleccion de matches
                foreach (Match match in simbolosNT)
                {
                    //Si el indice inicialdel match es igual al contador i
                    if (match.Index == i)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Determina si un operador es unario
        /// </summary>
        /// <param name="t">Operador de tipo caracter</param>
        /// <returns>Retorna: true: si el operador es unario, false: si el operador no es unario</returns>
        public bool EsUnario(char t)
        {
            if (t == '+' || t == '*' || t == '?') return true;
            return false;
        }
        /// <summary>
        /// Cra un simbolo no terminal a partir de la expresion y un indice
        /// </summary>
        /// <param name="exp">Expresion regular a analizar</param>
        /// <param name="i">Indice inicial del simbolo no terminal</param>
        /// <returns>Retorna un string que contiene el simbolo no terminal</returns>
        public string CreateNT(string exp, int i)
        {
            string result = "";
            Regex regex = new Regex(@"\b[A-Z]+\b(\(\))?");
            MatchCollection simbolosNT = regex.Matches(exp);
            if (simbolosNT.Count > 0)
            {
                foreach (Match match in simbolosNT)
                {
                    if (match.Index == (i))
                    {
                        result = match.Value;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Metodo de inicio del metodo recursivo del mismo nombre
        /// </summary>
        public void RecorridoInOrden()
        {
            RecorridoInOrden(this.root);
        }
        /// <summary>
        /// Recorrido recursivo del arbol
        /// </summary>
        /// <param name="node">Nodo a recorrer</param>
        public void RecorridoInOrden(NodoM node)
        {
            if (node != null)
            {
                RecorridoInOrden(node.NodoIzquierdo);
                if (node.Valor.Value == " ")
                {
                    Console.Write("°");
                    //ActualizarInfoSimbolos("°", node.Valor.tokenID);
                }
                else
                {
                    Console.Write($"{node.Valor.Value}");
                    //ActualizarInfoSimbolos(node.Valor.Value, node.Valor.tokenID);
                }
                RecorridoInOrden(node.NodoDerecho);
            }
        }
        /// <summary>
        /// Parametro para contar los simbolos terminales de la gramatica
        /// </summary>
        int firstLastCount = 0;
        /// <summary>
        /// Metodo que calcula El First, Last, Nullable y Follow de cada nodo del árbol
        /// </summary>
        /// <param name="node">Nodo a calcular</param>
        public void CalculateFirstLastNullableFollow(NodoM node)
        {
            node.First = new List<int>();
            node.Last = new List<int>();
            //Si el nodo es una hoja
            if (node.NodoIzquierdo == null)
            {
                firstLastCount++;
                node.First.Add(firstLastCount);
                node.Last.Add(firstLastCount);
            }
            //Si el valor del nodo es un '|', que represneta o exclusivo
            else if (node.Valor.Value == '|'.ToString())
            {
                //First: Union de firsts de izq y der
                foreach (var firstIz in node.NodoIzquierdo.First)
                {
                    node.First.Add(firstIz);
                }
                foreach (var firstDe in node.NodoDerecho.First)
                {
                    node.First.Add(firstDe);
                }
                //Last: Union de lasts de izq y der
                foreach (var lastIz in node.NodoIzquierdo.Last)
                {
                    node.Last.Add(lastIz);
                }
                foreach (var lastDe in node.NodoDerecho.Last)
                {
                    node.Last.Add(lastDe);
                }
                //Nullable: si izq o der es nullable node lo es
                if (node.NodoIzquierdo.Nullable || node.NodoDerecho.Nullable) node.Nullable = true;
            }
            //Si el valor del nodo es un '?', que representa opcional
            else if (node.Valor.Value == '?'.ToString())
            {
                //First: firsts de izq
                foreach (var firstIz in node.NodoIzquierdo.First)
                {
                    node.First.Add(firstIz);
                }
                //Last: Lasts de izq
                foreach (var lastIz in node.NodoIzquierdo.Last)
                {
                    node.Last.Add(lastIz);
                }
                //Nullable
                node.Nullable = true;
            }
            //Si el valor del nodo es un ' ', que representa concatenacion
            else if (node.Valor.Value == ' '.ToString())
            {
                //First
                if (node.NodoIzquierdo.Nullable)
                {
                    //First: Union de firsts de izq y der
                    foreach (var firstIz in node.NodoIzquierdo.First)
                    {
                        node.First.Add(firstIz);
                    }
                    foreach (var firstDe in node.NodoDerecho.First)
                    {
                        node.First.Add(firstDe);
                    }

                }
                else
                {
                    //First: firsts de izq
                    foreach (var firstIz in node.NodoIzquierdo.First)
                    {
                        node.First.Add(firstIz);
                    }
                }
                //Last
                if (node.NodoDerecho.Nullable)
                {
                    //Last: Union de lasts de izq y der
                    foreach (var lastIz in node.NodoIzquierdo.Last)
                    {
                        node.Last.Add(lastIz);
                    }
                    foreach (var lastDe in node.NodoDerecho.Last)
                    {
                        node.Last.Add(lastDe);
                    }
                }
                else
                {
                    //Last: lasts de der
                    foreach (var lastDe in node.NodoDerecho.Last)
                    {
                        node.Last.Add(lastDe);
                    }
                }
                //Nullable
                if (node.NodoIzquierdo.Nullable && node.NodoDerecho.Nullable) node.Nullable = true;

                //Por cada elemnto del Last de la izquierda
                foreach (int key in node.NodoIzquierdo.Last)
                {
                    //si no existe el elemento entre las llaves se incluye
                    if (!FollowTable.ContainsKey(key)) FollowTable.Add(key, new List<int>());
                    //Por cada elemento del First de la derecha
                    foreach (int value in node.NodoDerecho.First)
                    {
                        //si no existe entre los elemnto de la llave se incluye
                        if (!FollowTable[key].Contains(value)) FollowTable[key].Add(value);
                    }
                }
            }
            //Si el valor del nodo es un '*', que representa cerradura esrella 
            else if (node.Valor.Value == '*'.ToString())
            {
                //First: firsts de izq
                foreach (var firstIz in node.NodoIzquierdo.First)
                {
                    node.First.Add(firstIz);
                }
                //Last: Lasts de izq
                foreach (var lastIz in node.NodoIzquierdo.Last)
                {
                    node.Last.Add(lastIz);
                }
                //Nullable
                node.Nullable = true;
                //Follow
                //Por cada elemnto del Last de la izquierda
                foreach (int key in node.NodoIzquierdo.Last)
                {
                    //si no existe el elemento entre las llaves se incluye
                    if (!FollowTable.ContainsKey(key)) FollowTable.Add(key, new List<int>());
                    //Por cada elemento del First de la izquierda
                    foreach (int value in node.NodoIzquierdo.First)
                    {
                        //si no existe entre los elemnto de la llave se incluye
                        if (!FollowTable[key].Contains(value)) FollowTable[key].Add(value);
                    }
                }
            }
            //Si el valor del nodo es un '+', que representa cierre positivo
            else if (node.Valor.Value == '+'.ToString())
            {
                //First: firsts de izq
                foreach (var firstIz in node.NodoIzquierdo.First)
                {
                    node.First.Add(firstIz);
                }
                //Last: Lasts de izq
                foreach (var lastIz in node.NodoIzquierdo.Last)
                {
                    node.Last.Add(lastIz);
                }
                //Nullable
                if (node.NodoIzquierdo.Nullable) node.Nullable = true;
                //Follow
                //Por cada elemnto del Last de la izquierda
                foreach (int key in node.NodoIzquierdo.Last)
                {
                    //si no existe el elemento entre las llaves se incluye
                    if (!FollowTable.ContainsKey(key)) FollowTable.Add(key, new List<int>());
                    //Por cada elemento del First de la izquierda
                    foreach (int value in node.NodoIzquierdo.First)
                    {
                        //si no existe entre los elemnto de la llave se incluye
                        if (!FollowTable[key].Contains(value)) FollowTable[key].Add(value);
                    }
                }
            }
        }
        /// <summary>
        /// Metodo que llama al metodo recursivo PrintTree
        /// </summary>
        public void verArbol()
        {
            PrintTree(root, 0);
        }
        /// <summary>
        /// Metodo recursivo que imprime el arbol de forma en que se vean representados hijo derecho e izquierdo visualmente
        /// </summary>
        /// <param name="node">Nodo a imprimir</param>
        /// <param name="level">Int auxiliar para determinar el nivel en que se encuentra el nodo</param>
        private void PrintTree(NodoM node, int level = 0)
        {
            if (node == null)
                return;
            PrintTree(node.NodoDerecho, level + 1);
            if (node.Valor.Value == " ") Console.WriteLine(new string(' ', 4 * level) + '°');
            else if (node.NodoIzquierdo == null) Console.WriteLine(new string(' ', 4 * level) + '\'' + node.Valor.Value + '\'');
            else Console.WriteLine(new string(' ', 4 * level) + node.Valor.Value);
            PrintTree(node.NodoIzquierdo, level + 1);
        }
        /// <summary>
        /// Identifija y almacena recursivamente las hojas del árbol
        /// </summary>
        /// <param name="node"> nodo a analizar</param>
        public void IdentifyLeafs(NodoM node)
        {
            if (node == null) return;
            IdentifyLeafs(node.NodoIzquierdo);
            IdentifyLeafs(node.NodoDerecho);
            //Si el nodo es una hoja
            if (node.NodoIzquierdo == null)
            {
                hojas.Add(node.First[0], node.Valor.Value);
            }

        }
        /// <summary>
        /// Diccionario de hojas del arbol con estructura. {First de la hoja, valor de la hoja}
        /// </summary>
        private Dictionary<int, string> hojas = new Dictionary<int, string>();
        /// <summary>
        /// Genera la tabla de tansiciones del automata finito determinista
        /// </summary>
        public void GenerateDFATable()
        {
            //Paso 1
            //Se inicia una pila para los extados por visitar
            Stack<List<int>> noVisitados = new Stack<List<int>>();
            //Paso 2
            noVisitados.Push(root.First); //se agrega el estado inicial como un estado sin visitar
            //Se crea una lista para los estados ya visitados
            List<List<int>> visitados = new List<List<int>>();
            //Se identifican las hojas (simbolos del alfabeto del automata) con sus first
            IdentifyLeafs(root);
            //Se inicializa el estado actual
            List<int> estadoActual;
            //Paso 3
            //Mientras existan estados por visitar
            while (noVisitados.Count > 0)
            {
                //Paso a
                //Se saca el estado actual de la pila de estados no visitados
                estadoActual = noVisitados.Pop();
                //Paso b
                //Si el estado actual no existe entre los estados visitados
                if (!visitados.Contains(estadoActual))
                {
                    //Paso c
                    //Se añade a los estados visitados
                    visitados.Add(estadoActual);
                    //Paso d
                    //Se inicializa un diccionario para establecer las transiciones del estado actual
                    Dictionary<string, List<int>> transicionesEstadoActual = new Dictionary<string, List<int>>();
                    //por cada llave en el estado actual
                    for (int i = 0; i < estadoActual.Count; i++)
                    {
                        var followKey = estadoActual[i];
                        //Si la llave existe: se añade el follow del valor actual a la misma llave
                        if (transicionesEstadoActual.ContainsKey(hojas[followKey]))
                        {
                            //por cada valor en el follow de la llave actual
                            foreach (int val in FollowTable[followKey])
                            {
                                //Si no existen en el la lista de la transicion
                                if (!transicionesEstadoActual[hojas[followKey]].Contains(val))
                                {
                                    transicionesEstadoActual[hojas[followKey]].Add(val);

                                }
                            }
                            transicionesEstadoActual[hojas[followKey]].Sort();
                        }
                        else
                        {
                            if (followKey < hojas.Count) transicionesEstadoActual.Add(hojas[followKey], FollowTable[followKey]);
                        }
                    }

                    //Paso e
                    var estadosGenerados = transicionesEstadoActual.Values;
                    foreach (var potencialEstadoNuevo in estadosGenerados)
                    {
                        bool encontrado = false;
                        //Compara con visitados
                        foreach (var estadoVisitado in visitados)
                        {
                            if (estadoVisitado.SequenceEqual(potencialEstadoNuevo))
                            {
                                encontrado = true;
                                break;
                            }
                        }
                        //Compara con noVisitados
                        if (!encontrado)
                        {
                            foreach (var estadoNoVisitado in noVisitados)
                            {
                                if (estadoNoVisitado.SequenceEqual(potencialEstadoNuevo))
                                {
                                    encontrado = true;
                                    break;
                                }
                            }
                        }
                        //Añade a noVisitados si no está en visitados ni en noVisitados
                        if (!encontrado)
                        {
                            noVisitados.Push(potencialEstadoNuevo);
                        }
                    }
                    //Paso f
                    DFATable.Add(estadoActual, transicionesEstadoActual);
                }
            }
        }

        public void printFollow()
        {
            Console.WriteLine("\nFOLLOW");
            //Lista para ordenar los followa
            List<int> orden = new List<int>(FollowTable.Keys);
            orden.Sort();
            //Se imprime la tabla de follow
            foreach (var clave in orden)
            {
                Console.Write("\nClave: {0}, Follow: ", clave);
                for (int i = 0; i < FollowTable[clave].Count; i++)
                {
                    Console.Write("{0}", FollowTable[clave][i]);
                    if (i < FollowTable[clave].Count - 1)
                        Console.Write(", ");
                }
            }
        }
        public void printDFATable()
        {
            Console.WriteLine("Tabla de Transiciones");
            //Se imprime la tabla de transiciones
            bool aceptacion;
            foreach (var estado in DFATable.Keys)
            {
                aceptacion = estado.Contains(root.NodoDerecho.First[0]);
                Console.Write("\n\n" + "Estado: {");
                for (int i = 0; i < estado.Count; i++)
                {
                    Console.Write("{0}", estado[i]);
                    if (i < estado.Count - 1)
                        Console.Write(", ");
                }
                Console.Write("}}, (aceptacion: {0}) :\n", aceptacion);
                var keys = DFATable[estado].Keys.ToList();
                for (int j = 0; j < keys.Count; j++)
                {
                    var simbolo = keys[j];
                    Console.Write("Con simbolo: {0} , transiciona a: {{", simbolo);
                    for (int i = 0; i < DFATable[estado][simbolo].Count; i++)
                    {
                        Console.Write("{0}", DFATable[estado][simbolo][i]);
                        if (i < DFATable[estado][simbolo].Count - 1)
                            Console.Write(", ");
                    }
                    Console.Write("}");
                    if (j < keys.Count - 1)
                        Console.Write(", ");
                }
            }
        }
        /// <summary>
        /// Genera la tabla de follow, la imprime, llama al metodo para generar la tabla de transiciones y la imprime
        /// </summary>
        public void GenerateDFA()
        {
            //Se inicializa la tabla de follow del arbol, lista de simbolos
            FollowTable = new Dictionary<int, List<int>>();
            //Se recorre el arbol para generar la tabla de follows
            TaversalCalculateFLNF(this.root);

            //Se inicializa el diccionario que representa la tabla de transiciones del automata
            DFATable = new Dictionary<List<int>, Dictionary<string, List<int>>>();
            //Llama al metodo que genera la tabla de transiciones
            GenerateDFATable();
        }
        /// <summary>
        /// Recorrido que llama la funcion para calcular First, Last, Nullable y follow de cada nodo
        /// </summary>
        /// <param name="node"></param>
        private void TaversalCalculateFLNF(NodoM node)
        {
            if (node != null)
            {
                TaversalCalculateFLNF(node.NodoIzquierdo);
                TaversalCalculateFLNF(node.NodoDerecho);
                //CalculateFirstLastNullableFollow
                CalculateFirstLastNullableFollow(node);
            }
        }
        public void printNodes()
        {
            printNodes(root);
        }
        public void printNodes(NodoM node)
        {
            if (node != null)
            {
                printNodes(node.NodoIzquierdo);
                writeNodeInfo(node);
                printNodes(node.NodoDerecho);
            }
        }
        /// <summary>
        /// Imprime el nodoy su informacion
        /// </summary>
        /// <param name="node">nodo a imprimir</param>
        public void writeNodeInfo(NodoM node)
        {
            Console.Write("\nNodo: {0}, Null: {1}, First: {{", node.Valor.Value, node.Nullable);
            for (int i = 0; i < node.First.Count; i++)
            {
                Console.Write("{0}", node.First[i]);
                if (i < node.First.Count - 1)
                    Console.Write(", ");
            }
            Console.Write("}, Last: {");
            for (int i = 0; i < node.Last.Count; i++)
            {
                Console.Write("{0}", node.Last[i]);
                if (i < node.Last.Count - 1)
                    Console.Write(", ");
            }
            Console.Write("}");

        }
        public List<InfoVM> SymbolsToInfoVM(List<Symbol> list)
        {
            InfoVM tempInfo;
            foreach (var symbol in list)
            {
                tempInfo = new InfoVM();
                tempInfo.SimboloId = symbol.tokenID;
                if (symbol.Value.Contains("'"))
                {
                    if (!Regex.IsMatch(symbol.Value, @"'\''"))
                    {
                        tempInfo.SimboloValor = symbol.Value.Replace("'", "");
                    }
                    else
                    {
                        tempInfo.SimboloValor = "'";
                    }
                }
                else tempInfo.SimboloValor = symbol.Value;
                InfoSimbolos.Add(tempInfo);
            }
            return InfoSimbolos;
        }

        /// <summary>
        /// Almacena los simbolos
        /// </summary>
        public void AlmacenarSimbolos()
        {
            foreach (var infoSim in InfoSimbolos)
            {
                if (ValidacionDAL.EsCaracterAlfabeto(infoSim.SimboloValor))
                    continue;
                else
                {
                    var simbolos = infoSim.SimboloValor.ToArray();
                    foreach (var simbolo in simbolos)
                    {
                        if (!SimbolosTerminales.Exists(x => x == simbolo.ToString()))
                            SimbolosTerminales.Add(simbolo.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Nos sirve para saber que informacion de los estados para movernos
        /// </summary>
        public void LlenarMovimientosEstado()
        {
            EstadosVMov = new List<Estados>();
            int estadoId = 0;
            foreach (var padre in DFATable)
            {
                Estados estado = new Estados();
                estado.Estado = estadoId;

                foreach (var hijo in padre.Value)
                {
                    var simbolo = new Simbolos();
                    simbolo.Token = hijo.Key;
                    simbolo.Transicion = GetEstadoSiguiente(hijo.Value);
                    estado.simbolos.Add(simbolo);
                }
                EstadosVMov.Add(estado);
                estadoId++;
            }
        }

        //  TODO: Pendiente de revisar
        /// <summary>
        /// estadoSiguiente != -1, encontro transicion, de lo contrario hay que ver que hacemos
        /// </summary>
        /// <param name="transiciones"></param>
        /// <returns></returns>
        private int GetEstadoSiguiente(List<int> transiciones)
        {
            int estadoSiguiente = 0;
            bool encontrado = false;
            string transicionStr = String.Join(',', transiciones);

            foreach (var estadoTransicion in DFATable.Keys)
            {
                string estadoTransicionesStr = String.Join(',', estadoTransicion);
                if (estadoTransicionesStr.Equals(transicionStr))
                {
                    encontrado = true;
                    break;
                }
                estadoSiguiente++;
            }

            return encontrado ? estadoSiguiente : -1;
        }

        public void LlenarEstadosAceptacion()
        {
            EstadosAceptacion = new List<int>();
            int numEstado = 0;

            foreach (var llave in DFATable.Keys)
            {
                if (llave.Contains(root.NodoDerecho.First[0]))
                    EstadosAceptacion.Add(numEstado);
                numEstado++;
            }
        }
    }
}
