//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Seed_v1.Models.SISTEMA
{
    using System;
    using System.Collections.Generic;
    
    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            this.Sesion = new HashSet<Sesion>();
            this.UsuarioEmpresa = new HashSet<UsuarioEmpresa>();
            this.UsuarioPrivilegio = new HashSet<UsuarioPrivilegio>();
        }
    
        public int Id { get; set; }
        public string Email { get; set; }
        public string Contrasenia { get; set; }
        public int Estado { get; set; }
        public string UsuaReg { get; set; }
        public System.DateTime FechaReg { get; set; }
        public string UsuaModif { get; set; }
        public Nullable<System.DateTime> FechaModif { get; set; }
        public string IdContrasenia { get; set; }
        public string IdAspNetUser { get; set; }
        public Nullable<int> IdPersona { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sesion> Sesion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuarioEmpresa> UsuarioEmpresa { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuarioPrivilegio> UsuarioPrivilegio { get; set; }
        public virtual PersonaBase PersonaBase { get; set; }
    }
}
