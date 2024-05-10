
using Proyecto_Lenguajes_Formales_y_Automatas.ARBOL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto_Lenguajes_Formales_y_Automatas
{
    public class GeneradorVM
    {
        public int Identacion { get; set; }
        public string Comentario { get; set; }
        public string Definicion { get; set; }
        public bool AgregarEspacio { get; set; }
        public List<GeneradorVM> Hijos { get; set; }

        public GeneradorVM()
        {
            Hijos = new List<GeneradorVM>();
        }
    }

    public class InfoMetodosVM
    {
        /// <summary>
        /// Estos son para los metodos
        /// </summary>
        public List<string> Sets { get; set; }
        public List<InfoVM> Tokens { get; set; }
        public List<string> Simbolos { get; set; }
        public List<string> Reservadas { get; set; }

        /// <summary>
        /// Estos son para el contenido
        /// </summary>
        public List<int> EstadosAceptacion { get; set; }
        public List<Estados> EstadosVM { get; set; }

        public InfoMetodosVM()
        {
            Sets = new List<string>();
            Tokens = new List<InfoVM>();
            Simbolos = new List<string>();
            Reservadas = new List<string>();
            EstadosAceptacion = new List<int>();
            EstadosVM = new List<Estados>();
        }
    }
}

