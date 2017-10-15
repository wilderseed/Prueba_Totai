using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seed_v1.Models
{
    public class UsrLoginViewModel
    {
        //UsuarioModels
        public string Empresa { get; set; }
        public string Nombre { get; set; }
        public string Contrasenia { get; set; }
        public bool OkRecordar { get; set; }
        public string ReturnUrl { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public bool IsMobile { get; set; }
    }
}