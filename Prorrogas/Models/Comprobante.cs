using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prorrogas.Models
{
    public class Comprobante
    {
        string tipoProrroga;
        string ci;
        string estudiante;
        string carrera;
        string fechaRegistro;
        string materia;
        string modulo;
        string turno;
        string aula;
        string fechaLimite;
        string usuario;

        public string TipoProrroga { get => tipoProrroga; set => tipoProrroga = value; }
        public string Ci { get => ci; set => ci = value; }
        public string Estudiante { get => estudiante; set => estudiante = value; }
        public string Carrera { get => carrera; set => carrera = value; }
        public string FechaRegistro { get => fechaRegistro; set => fechaRegistro = value; }
        public string Materia { get => materia; set => materia = value; }
        public string Modulo { get => modulo; set => modulo = value; }
        public string Turno { get => turno; set => turno = value; }
        public string Aula { get => aula; set => aula = value; }
        public string FechaLimite { get => fechaLimite; set => fechaLimite = value; }
        public string Usuario { get => usuario; set => usuario = value; }
    }
}