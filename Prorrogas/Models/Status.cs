using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prorrogas.Models
{
    public class Status
    {
        private int tipo;
        private string msj;
        private int id;

        public int Tipo { get => tipo; set => tipo = value; }
        public string Msj { get => msj; set => msj = value; }
        public int Id { get => id; set => id = value; }
    }
}