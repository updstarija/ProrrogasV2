//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Prorrogas.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MateriaContenido
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MateriaContenido()
        {
            this.GrupoAperturado = new HashSet<GrupoAperturado>();
            this.Materia = new HashSet<Materia>();
            this.MateriaContenido1 = new HashSet<MateriaContenido>();
        }
    
        public short Id { get; set; }
        public string Sigla { get; set; }
        public string Nombre { get; set; }
        public Nullable<short> IdDireccionArea { get; set; }
        public Nullable<short> IdEstado { get; set; }
        public Nullable<decimal> Costo { get; set; }
        public Nullable<short> Creditos { get; set; }
        public Nullable<short> HorasTeoricasSemanal { get; set; }
        public Nullable<short> HorasPracticasSemanal { get; set; }
        public Nullable<short> TotalHoras { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public int IdModeloEstudio { get; set; }
        public string Competencia { get; set; }
        public Nullable<int> IdTipoMateriaContenido { get; set; }
        public string UrlPrograma { get; set; }
        public Nullable<int> CantidadModulo { get; set; }
        public Nullable<short> GrupoAcademicoId { get; set; }
        public Nullable<short> MateriaContenidoPadreId { get; set; }
        public Nullable<short> IdiomaId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrupoAperturado> GrupoAperturado { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Materia> Materia { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MateriaContenido> MateriaContenido1 { get; set; }
        public virtual MateriaContenido MateriaContenido2 { get; set; }
    }
}
