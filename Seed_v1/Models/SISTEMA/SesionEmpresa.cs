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
    
    public partial class SesionEmpresa
    {
        public int IdSesion { get; set; }
        public int IdEmpresa { get; set; }
        public bool Activa { get; set; }
    
        public virtual Empresa Empresa { get; set; }
        public virtual Sesion Sesion { get; set; }
    }
}
