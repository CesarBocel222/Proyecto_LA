using System;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
       

    }

    static string checkPattern(List<string> list, string pattern, int fila)
    {
        string verificationString = ""; //Debe contener la informacion sobre la validación
        if (list.Count > 1)//si existe almenos 1 elemento más que el indicador de la parte
        {
            int row = fila; //indica la fila actual
            foreach (string element in list) //recorre la lista elemento por elemento
            {
                if (!Regex.IsMatch(element, pattern))//si el elemento no cumple con el patron
                {
                    //verificar que columna falla
                }
                row++;//aumenta el indicador de la fila actual
            }
            return verificationString;
        }
        else return "No hay suficientes elementos para verificar";

    }
    // Distribuidor de partes de la gramatica con su respectivo patron
    // <param name="list">Recibe una lista que contiene la parte de la gramática que se verificará</param>
    static void checksDealer(List<string> list)
    {
        string setsPattern = @"(\s*[A-Z]+ *= *)(('\w+'((\+|\.{2})?'\w+')*)\s*|(CHR\(\d+\)((\+|\.{2})?CHR\(\d+\))*)|\s)";
        string tokensPattern = @"(TOKEN\s*\d+\s*=\s*)(('.{1}')+|(\w+ *(\*|\|)?)+|('.{1}' *\w+ *'.{1}'(\*|\|)?)+|(\w*\s*(\(|{)(\s*\w*\s*(\(\))?(\*|\|)?)+(\)|})(\*|\|)?)+)";
        string actionsPattern = @"([A-Z0-9_]+\(\))|({\s*)|(}\s*)|(\s*\d*\s*=\s*'[A-Z]*'\s*)|\s*";
        string errorsPattern = @"(\s*\w*ERROR\w*\s*=)(\s*\d*\s*)";
        int actualRow = 0;
        if (Regex.IsMatch(list[0], @"SETS\s*"))//Si el primer elemento de la lista es SETS 
        {
            var verificationResult = checkPattern(list, setsPattern, actualRow);
            //verificationResult debe interpretarse para saber si hay un error si es asi detener la verificacion

        }
        else if (Regex.IsMatch(list[0], @"TOKENS\s*"))//Si el primer elemento de la lista es TOKENS
        {
            var verificationResult = checkPattern(list, tokensPattern, actualRow);
        }
        else if (Regex.IsMatch(list[0], @"ACTIONS\s*"))//Si el primer elemento de la lista es ACTIONS
        {
            foreach (string reservadasSerach in list)
            {
                if (Regex.IsMatch(reservadasSerach, @"\s*RESERVADAS.*"))//Verifica si existe la funcion RESERVADAS
                {
                    var verificationResult = checkPattern(list, actionsPattern, actualRow);
                }
                else
                {
                    Console.WriteLine("Falta la funcion reservadas no es una gramática válida");
                }
            }

        }
        else if (Regex.IsMatch(list[0], @"ERROR.*"))//Si el primer elemento de la lista es un ERROR
        {
            var verificationResult = checkPattern(list, errorsPattern, actualRow);
        }
    }
}