using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seed_v1.Models
{
    public class LytMenuViewModel
    {
        public IEnumerable<LytMenuItemViewModel> MenuItems { get; set; }
    }

    public class LytMenuItemViewModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string LinkText { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public int Nivel { get; set; }
        public string Url { get; set; }
        public string CssClass { get; set; }
        public string CssClassRoot { get; set; }
    }
}