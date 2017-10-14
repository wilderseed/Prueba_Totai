using Seed_v1.Models;
using Seed_v1.Models.SISTEMA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Seed_v1.Controllers
{
    public class HomeController : Controller
    {
        private SISTEMAEntities dbSistema;

        private string userName;
        private string idAUSession, idAUsuario;
        private int idUsuario;
        private string data, consulta;

        public HomeController()
        {
            dbSistema = new SISTEMAEntities();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _EmpresaMenuPartial(string currentUrl)
        {
            CargarMenuEmpresa(currentUrl);
            return Content(data);
        }
        
        private void CargarMenuEmpresa(string currentUrl)
        {
            userName = User.Identity.Name;
            idAUSession = Session.SessionID;

            idAUsuario = (dbSistema.AspNetUsers.Where(au => au.UserName == userName).FirstOrDefault()).Id;
            idUsuario = (dbSistema.Usuario.Where(u => u.IdAspNetUser == idAUsuario).FirstOrDefault()).Id;

            var sesion = dbSistema.Sesion.Where(s => s.IdAspNetSession == idAUSession
                && s.IdUsuario == idUsuario).OrderByDescending(s => s.FechaFin).FirstOrDefault();

            consulta = "SELECT e.Id AS Id, '' AS Codigo, e.DBName AS LinkText, 'Empresa' AS ControllerName, 0 AS Nivel, '' AS ClassRoot"
                    + ", 'Conectar' AS ActionName, '/Empresa/Connect/' + CAST(e.Id AS NVARCHAR(20)) + '?returnUrl=" + currentUrl
                    + "' AS Url, '#1ab394' AS CssClass, '' AS CssClassRoot"
                + " FROM dbo.Empresa e INNER JOIN dbo.UsuarioEmpresa ue ON e.Id = ue.IdEmpresa"
                + " WHERE e.Id NOT IN (SELECT IdEmpresa FROM dbo.SesionEmpresa WHERE IdSesion = " + sesion.Id
                + " AND Activa = 1) ORDER BY e.DBName";

            var menuItems = dbSistema.Database.SqlQuery<LytMenuItemViewModel>(consulta).ToList();
            var empresa = (sesion.SesionEmpresa.Where(se => se.Activa == true).FirstOrDefault().Empresa);

            data = "<li id=\"empMenu\" class=\"dropdown\">"
                    + "<a class=\"dropdown-toggle\" href=\"#\"" + (menuItems.Count() > 0 ? " data-toggle=\"dropdown\"" : "") + ">"
                        + "<i class=\"fa fa-home\"></i> " + empresa.DBName
                    + "</a>";

            if (menuItems.Count() > 0)
            {
                data = data + "<ul id=\"empMenu\" class=\"dropdown-menu animated fadeInRight m-t-xs scroll-menu\">";

                foreach (var mi in menuItems)
                {
                    data = data
                                + "<li>"
                                    + "<a title=\"" + mi.ActionName + "\" href=\"" + mi.Url + "\"><i class=\"fa fa-plug\" style=\"color: " + mi.CssClass
                                        + "\"></i> " + mi.LinkText
                                    + "</a>"
                                + "</li>";
                }

                data = data + "</ul>"
                    + "</li>";
            }
        }
    }
}