using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Seed_v1.Models;
using Seed_v1.Models.SISTEMA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Seed_v1.Controllers
{
    public class UsuarioController : Controller
    {
        public enum Estado
        {
            Inactivo = 0,
            Activo = 1
        };

        private SISTEMAEntities dbSistema;
        private SesionController sesionController;

        private string userName;
        private string idAUSession, idAUsuario;
        private int idUsuario;

        private UsrLoginViewModel usrLogin;

        public UsuarioController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
            dbSistema = new SISTEMAEntities();
            sesionController = new SesionController(dbSistema);
        }

        public UsuarioController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            UserManager.PasswordValidator = new MinimumLengthValidator(4);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string returnUrl)
        {
            userName = User.Identity.Name;

            if (!(userName == null || userName == ""))
            {
                return RedirectToAction("Panel", "Usuario");
            }

            if (returnUrl == null)
            {
                returnUrl = returnUrl = Url.Action("Panel", "Usuario");
            }

            usrLogin = new UsrLoginViewModel { ReturnUrl = returnUrl };
            return View(usrLogin);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Panel()
        {
            userName = User.Identity.Name;

            if (!(userName == null || userName == ""))
            {
                return View();
            }

            return RedirectToAction("Login", "Usuario");
        }

        public ActionResult Logout(string newUrl)
        {
            var browser = Request.Browser;
            var userAgent = Request.UserAgent;

            var index = userAgent.IndexOf("(");
            userAgent = userAgent.Substring(index + 1, userAgent.Length - (index + 1));

            index = userAgent.IndexOf(")");
            userAgent = userAgent.Substring(0, index);

            userName = User.Identity.Name;
            idAUSession = Session.SessionID;

            idAUsuario = (dbSistema.AspNetUsers.Where(au => au.UserName == userName).FirstOrDefault()).Id;
            idUsuario = (dbSistema.Usuario.Where(u => u.IdAspNetUser == idAUsuario).FirstOrDefault()).Id;

            var sesion = dbSistema.Sesion.Where(s => s.IdAspNetSession == idAUSession
                && s.IdUsuario == idUsuario).FirstOrDefault();

            sesion.FechaFin = DateTime.Now;
            dbSistema.SaveChanges();

            //BITACORA

            var usuarioBitacora = new UsuarioBitacora
            {
                Operacion = "Usuarios/Logout",
                Descripcion = "Cierre de Sesión. Id: " + sesion.Id + ", Navegador: " + browser.Browser
                    + " (" + browser.Type + "), IP: " + Request.UserHostAddress + ", Device: " + userAgent,
                Tipo = 1,
                UsuaReg = userName,
                FechaReg = DateTime.Now
            };

            dbSistema.UsuarioBitacora.Add(usuarioBitacora);
            dbSistema.SaveChanges();

            AuthenticationManager.SignOut();
            return Redirect(newUrl);
        }

        [HttpPost]
        public ActionResult Login2(string jsonModel)
        {
            var browser = Request.Browser;
            var userAgent = Request.UserAgent;

            var index = userAgent.IndexOf("(");
            userAgent = userAgent.Substring(index + 1, userAgent.Length - (index + 1));

            index = userAgent.IndexOf(")");
            userAgent = userAgent.Substring(0, index);

            var model = JsonConvert.DeserializeObject<UsrLoginViewModel>(jsonModel);
            userName = model.Nombre.ToLower();
            
            if (model.Nombre.ToLower() != "sistemas")
            {
                userName = model.Empresa.ToLower() + "_" + model.Nombre.ToLower();
            }

            idAUsuario = (dbSistema.AspNetUsers.Where(au => au.UserName == userName).FirstOrDefault()).Id;

            var usuario = dbSistema.Usuario.Where(u => u.IdAspNetUser == idAUsuario
                && u.Estado == (int)Estado.Activo).FirstOrDefault();

            if (usuario != null)
            {
                if (model.Nombre.ToLower() == "sistemas")
                {
                    var empresa = usuario.UsuarioEmpresa.Select(ue => ue.Empresa)
                        .Where(e => e.DBName == model.Empresa.ToUpper()).FirstOrDefault();

                    if (empresa == null)
                    {
                        //BITACORA

                        var usuarioBitacora = new UsuarioBitacora
                        {
                            Operacion = "Usuarios/Login",
                            Descripcion = "Inicio de Sesión. Error: Empresa inexistente, Navegador: " + browser.Browser
                                + "(" + browser.Type + "), IP: " + Request.UserHostAddress + ", Device: " + userAgent,
                            Tipo = 0,
                            UsuaReg = userName,
                            FechaReg = DateTime.Now
                        };

                        dbSistema.UsuarioBitacora.Add(usuarioBitacora);
                        dbSistema.SaveChanges();

                        return Json(new { ok = false, message = "Empresa inexistente." });
                    }
                }

                var user = UserManager.Find(userName, model.Contrasenia);

                if (user != null)
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                    var identity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = model.OkRecordar }, identity);

                    //SESION

                    idAUSession = Session.SessionID;
                    HttpContext.Session["SESSION_ID"] = idAUSession;

                    var sesion = dbSistema.Sesion.Where(s => s.IdUsuario == usuario.Id)
                        .OrderByDescending(s => s.FechaFin).FirstOrDefault();

                    if (sesion != null)
                    {
                        if (sesion.IdAspNetSession != idAUSession)
                        {
                            sesion.FechaFin = DateTime.Now;
                            dbSistema.SaveChanges();

                            //BITACORA

                            var usuarioBitacora = new UsuarioBitacora
                            {
                                Operacion = "Usuarios/Logout",
                                Descripcion = "Cierre de Sesión. Id: " + sesion.Id + ", Navegador: " + browser.Browser
                                    + " (" + browser.Type + "), IP: " + Request.UserHostAddress + ", Device: " + userAgent,
                                Tipo = 1,
                                UsuaReg = userName,
                                FechaReg = DateTime.Now
                            };

                            dbSistema.UsuarioBitacora.Add(usuarioBitacora);
                            dbSistema.SaveChanges();

                            var sesionEmpresa = sesion.SesionEmpresa.Where(se => se.Activa).FirstOrDefault();
                            var empresa = sesionEmpresa.Empresa;

                            sesionEmpresa.Activa = false;
                            dbSistema.SaveChanges();

                            sesion = new Sesion
                            {
                                IdAspNetSession = idAUSession,
                                IdUsuario = usuario.Id,
                                TipoBrowser = browser.Type,
                                Browser = browser.Browser,
                                IpAddress = Request.UserHostAddress,
                                UserAgent = userAgent,
                                ScreenWidth = model.ScreenWidth,
                                ScreenHeight = model.ScreenHeight,
                                IsMobile = model.IsMobile,
                                FechaInicio = DateTime.Now,
                                FechaFin = DateTime.Now
                            };

                            dbSistema.Sesion.Add(sesion);
                            dbSistema.SaveChanges();

                            if (empresa.DBName == model.Empresa.ToUpper())
                            {
                                sesionEmpresa = new SesionEmpresa
                                {
                                    IdSesion = sesion.Id,
                                    IdEmpresa = empresa.Id,
                                    Activa = true
                                };
                            }
                            else
                            {
                                empresa = usuario.UsuarioEmpresa.Select(ue => ue.Empresa)
                                    .Where(e => e.DBName == model.Empresa.ToUpper()).FirstOrDefault();

                                sesionEmpresa = new SesionEmpresa
                                {
                                    IdSesion = sesion.Id,
                                    IdEmpresa = empresa.Id,
                                    Activa = true
                                };
                            }

                            dbSistema.SesionEmpresa.Add(sesionEmpresa);
                            dbSistema.SaveChanges();
                        }
                        else
                        {
                            var sesionEmpresa = sesion.SesionEmpresa.Where(se => se.Activa == true).FirstOrDefault();
                            sesionEmpresa.Activa = false;

                            dbSistema.SaveChanges();

                            sesion = new Sesion
                            {
                                IdAspNetSession = idAUSession,
                                IdUsuario = usuario.Id,
                                TipoBrowser = browser.Type,
                                Browser = browser.Browser,
                                IpAddress = Request.UserHostAddress,
                                UserAgent = userAgent,
                                ScreenWidth = model.ScreenWidth,
                                ScreenHeight = model.ScreenHeight,
                                IsMobile = model.IsMobile,
                                FechaInicio = DateTime.Now,
                                FechaFin = DateTime.Now
                            };

                            dbSistema.Sesion.Add(sesion);
                            dbSistema.SaveChanges();

                            var empresa = usuario.UsuarioEmpresa.Select(ue => ue.Empresa)
                                .Where(e => e.DBName == model.Empresa.ToUpper()).FirstOrDefault();

                            sesionEmpresa = new SesionEmpresa
                            {
                                IdSesion = sesion.Id,
                                IdEmpresa = empresa.Id,
                                Activa = true
                            };

                            dbSistema.SesionEmpresa.Add(sesionEmpresa);
                            dbSistema.SaveChanges();
                        }
                    }
                    else
                    {
                        sesion = new Sesion
                        {
                            IdAspNetSession = idAUSession,
                            IdUsuario = usuario.Id,
                            TipoBrowser = browser.Type,
                            Browser = browser.Browser,
                            IpAddress = Request.UserHostAddress,
                            UserAgent = userAgent,
                            ScreenWidth = model.ScreenWidth,
                            ScreenHeight = model.ScreenHeight,
                            IsMobile = model.IsMobile,
                            FechaInicio = DateTime.Now,
                            FechaFin = DateTime.Now
                        };

                        dbSistema.Sesion.Add(sesion);
                        dbSistema.SaveChanges();

                        var empresa = usuario.UsuarioEmpresa.Select(ue => ue.Empresa)
                                    .Where(e => e.DBName == model.Empresa.ToUpper()).FirstOrDefault();

                        var sesionEmpresa = new SesionEmpresa
                        {
                            IdSesion = sesion.Id,
                            IdEmpresa = empresa.Id,
                            Activa = true
                        };

                        dbSistema.SesionEmpresa.Add(sesionEmpresa);
                        dbSistema.SaveChanges();
                    }

                    //BITACORA

                    var usuarioBitacora2 = new UsuarioBitacora
                    {
                        Operacion = "Usuarios/Login",
                        Descripcion = "Inicio de Sesión. Navegador: " + browser.Browser + " (" + browser.Type
                            + "), IP: " + Request.UserHostAddress + ", Device: " + userAgent,
                        Tipo = 1,
                        UsuaReg = userName,
                        FechaReg = DateTime.Now
                    };

                    dbSistema.UsuarioBitacora.Add(usuarioBitacora2);
                    dbSistema.SaveChanges();

                    return Json(new { ok = true, newUrl = model.ReturnUrl });
                }

                //BITACORA

                var usuarioBitacora3 = new UsuarioBitacora
                {
                    Operacion = "Usuarios/Login",
                    Descripcion = "Inicio de Sesión. Error: Contraseña incorrecta, Navegador: " + browser.Browser
                        + "(" + browser.Type + "), IP: " + Request.UserHostAddress + ", Device: " + userAgent,
                    Tipo = 0,
                    UsuaReg = userName,
                    FechaReg = DateTime.Now
                };

                dbSistema.UsuarioBitacora.Add(usuarioBitacora3);
                dbSistema.SaveChanges();

                return Json(new { ok = false, message = "Contraseña incorrecta." });
            }

            //BITACORA

            var usuarioBitacora4 = new UsuarioBitacora
            {
                Operacion = "Usuarios/Login",
                Descripcion = "Inicio de Sesión. Error: Usuario no válido, Navegador: " + browser.Browser
                    + "(" + browser.Type + "), IP: " + Request.UserHostAddress + ", Device: " + userAgent,
                Tipo = 0,
                UsuaReg = userName,
                FechaReg = DateTime.Now
            };

            dbSistema.UsuarioBitacora.Add(usuarioBitacora4);
            dbSistema.SaveChanges();

            return Json(new { ok = false, message = "Usuario no válido." });
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        #region Crypto Methods
        private string Encrypt(string clearText)
        {
            string EncryptionKey = "SEED_V1_11111100001";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }

                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return clearText;
        }

        private string Decrypt(string cipherText)
        {
            string EncryptionKey = "SEED_V1_11111100001";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }

                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipherText;
        }
        #endregion
    }
}