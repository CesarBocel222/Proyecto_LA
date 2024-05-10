
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;

namespace Proyecto_Lenguajes_Formales_y_Automatas.Data
{
    public class ManejadoArchivo
    {
        FileStream fs;
        StreamWriter sw;

        public ManejadoArchivo(string rutaArchivo)
        {
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("Creando archivo .py :))\n");
            if (File.Exists(rutaArchivo))
                File.Delete(rutaArchivo);

            fs = File.Open(rutaArchivo, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            sw = new StreamWriter(fs);

            Console.WriteLine("Tu archivo Python esta en: ");
            Console.WriteLine($"\t {rutaArchivo}");
        }
        public void ADAN(GeneradorVM libreria)
        {
            if (!string.IsNullOrEmpty(libreria.Comentario))
                EVA(libreria.Identacion, libreria.Comentario);

            EVA(libreria.Identacion, libreria.Definicion);        

            foreach (var hijo in libreria.Hijos)
            {
                if (hijo != null)
                {
                    ADAN(hijo);
                }
            }
        }
        private void EVA(int identacion, string cadenaInicial)
        {
            string cadenaFinal = "";
            for (int i = 0; i < identacion; i++)
                cadenaFinal += " ";

            cadenaFinal += cadenaInicial;
            sw.WriteLine(cadenaFinal);
        }

        public void Finalizar()
        {
            sw.Dispose();
            sw.Close();
        }
    }
}
