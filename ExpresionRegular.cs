using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Lenguajes_Formales_y_Automatas.ARBOL
{
    public class ExpresionRegular
    {
        /// <summary>
        /// Lista de objetos Symbol (que contiene expresiones regulares)
        /// </summary>
        public List<Symbol> Expresion { get; set; }

        /// <summary>
        /// INFO: IMPORTANTE
        /// Lista de simbolos con sus ids
        /// </summary>
        public Dictionary<string, int> SimbolsTokenId { get; set; }
        /// <summary>
        /// Constructor de RegularExpression
        /// </summary>
        public ExpresionRegular()
        {
            this.Expresion = new List<Symbol>();
            this.SimbolsTokenId = new Dictionary<string, int>();
        }
        /// <summary>
        /// Procesa una lista de cadenas y realiza operaciones en cada línea.
        /// </summary>
        /// <param name="list">La lista de cadenas a procesar.</param>
        public void ProcessRawLists(List<string> list)
        {
            int pos, posEqual;
            string tempExpresion;
            Symbol tempSymbol;
            foreach (var line in list)
            {
                int tokenId = ValidacionDAL.GetTokenId(line);
                posEqual = 0;//Se inicia a buscar el caracter '='
                //Mientas el indice posEqual sea menor que el largo de la cadena line y el caracter en la posicion posEqual no sea  '=' se incrementa posEqual
                while (posEqual < line.Length && line[posEqual] != '=')
                {
                    posEqual++;
                }
                pos = line.Length - 1;//Se inicia la lecura al final de la cadena line
                tempExpresion = "";
                //Mientras pos sea mayor que el indice posEqual se almacena la nueva cadena
                while (pos > posEqual)
                {
                    tempExpresion = line[pos] + tempExpresion;
                    pos--;
                }
                tempSymbol = new Symbol(tempExpresion);//Se crea un simbolo con la cadena sin identificador
                tempSymbol.tokenID = tokenId; //    Info le asigmanos el id del token
                Expresion.Add(tempSymbol);
                SimbolsTokenId.Add(tempSymbol.Value, tokenId);
            }
        }
        public void ProcessRawLists(List<string> SETS, List<string> TOKENS)
        {
            List<Symbol> nonTerminalSymbols = new List<Symbol>();
            Symbol tempSymbol;
            string tempSymb;
            int posSymb;
            foreach (var symb in SETS)
            {
                tempSymb = "";
                posSymb = 0;
                do
                {
                    if (symb[posSymb] != ' ' && symb[posSymb] != '\n' && symb[posSymb] != '\t')
                    {
                        tempSymb += symb[posSymb];
                    }
                } while (symb[posSymb] != '=');
                tempSymbol = new Symbol(tempSymb);
                nonTerminalSymbols.Add(tempSymbol);
            }
            bool checkNTS;
            foreach (var token in TOKENS)
            {
                checkNTS = false;
                foreach (var nTS in nonTerminalSymbols)
                {
                    if (token.Contains(nTS.Value))
                    {
                        checkNTS = true;
                        break;
                    }
                }
                if (checkNTS)
                {
                    //verificar simbolos no terminales
                }
                else
                {
                    //verificacion solo con simbolos terminales
                }
            }
        }

    }
    /// <summary>
    /// Objeto Symbol
    /// </summary>
    public class Symbol
    {
        /// <summary>
        /// Obtiene o establece el valor del símbolo.
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Obtiene o establece el ID del token asociado al símbolo.
        /// </summary>
        public int tokenID { get; set; }
        /// <summary>
        /// Inicializa una nueva instancia de la clase Symbol con el valor especificado.
        /// </summary>
        /// <param name="value">El valor del símbolo.</param>
        public Symbol(string value)
        {
            this.Value = value;
        }
        /// <summary>
        /// Inicializa una nueva instancia de la clase Symbol con el valor especificado.
        ///</summary>
        ///<param name="value">El valor del símbolo.</param>
        public Symbol(char value)
        {
            this.Value = value.ToString();
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Symbol con el valor especificado e indetificador de token
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tokenId"></param>
        public Symbol(string value, int tokenId)
        {
            this.Value = value;
            this.tokenID = tokenId;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Symbol con el valor especificado e indetificador de token
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tokenId"></param>
        public Symbol(char value, int tokenId)
        {
            this.Value = value.ToString();
            this.tokenID = tokenId;
        }
    }
    /// <summary>
    /// La clase NodoM representa un nodo en un árbol binario.
    /// </summary>
    public class NodoM
    {
        /// <summary>
        /// Obtiene o establece el valor del nodo.
        /// </summary>
        public Symbol Valor { get; set; }
        /// <summary>
        /// Obtiene o establece el nodo izquierdo del nodo actual.
        /// </summary>
        public NodoM NodoIzquierdo { get; set; }
        /// <summary>
        /// Obtiene o establece el nodo derecho del nodo actual.
        ///</summary>
        public NodoM NodoDerecho { get; set; }
        public List<int> First { get; set; }
        public List<int> Last { get; set; }
        public bool Nullable { get; set; }
    }
}
