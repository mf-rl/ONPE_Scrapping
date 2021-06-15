using System.Collections.Generic;

namespace PE_Scrapping.Entidades
{
    public class Mesa
    {
        public List<MesasVotacion> mesasVotacion { get; set; }
    }
    public class MesasVotacion
    {
        public string NUMMESA { get; set; }
        public string PROCESADO { get; set; }
        public string IMAGEN { get; set; }
    }
}
