using Seed_v1.Models.SISTEMA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Seed_v1.Controllers
{
    public class SesionController : Controller
    {
        private SISTEMAEntities dbSistema;

        private int idSesion;

        public int IdSesion { get { return idSesion; } }

        public SesionController(SISTEMAEntities dbSistema)
        {
            this.dbSistema = dbSistema;
        }
    }
}