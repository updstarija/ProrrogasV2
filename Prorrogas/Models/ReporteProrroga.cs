using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prorrogas.Models
{
    public class ReporteProrroga
    {
        int id;
        string fechaRegistro;
        string fechaLimite;
        string estudiante;
        string ci;
        string carrera;
        string modulo;
        string materia;
        string aula;
        string turno;
        string estado;
        string tipoProrroga;

        public int Id { get => id; set => id = value; }
        public string FechaRegistro { get => fechaRegistro; set => fechaRegistro = value; }
        public string FechaLimite { get => fechaLimite; set => fechaLimite = value; }
        public string Estudiante { get => estudiante; set => estudiante = value; }
        public string Ci { get => ci; set => ci = value; }
        public string Carrera { get => carrera; set => carrera = value; }
        public string Modulo { get => modulo; set => modulo = value; }
        public string Materia { get => materia; set => materia = value; }
        public string Aula { get => aula; set => aula = value; }
        public string Turno { get => turno; set => turno = value; }
        public string Estado { get => estado; set => estado = value; }
        public string TipoProrroga { get => tipoProrroga; set => tipoProrroga = value; }
    }
}