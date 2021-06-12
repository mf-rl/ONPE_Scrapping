using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PE_Scrapping.Entidades
{
    public class Mesa
    {
        public string Numero { get; set; }
        public bool Procesada { get; set; }
        public string FP { get; set; }
        public string PL { get; set; }
        public string VB { get; set; }
        public string VN { get; set; }
        public string VI { get; set; }
        public string amb_cod { get; set; }
        public string dep_cod { get; set; }
        public string pro_cod { get; set; }
        public string dis_cod { get; set; }
        public string loc_cod { get; set; }
        public bool save_success { get; set; }
    }
}
