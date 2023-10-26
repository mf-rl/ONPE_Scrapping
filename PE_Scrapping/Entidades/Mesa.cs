using Newtonsoft.Json;
using System.Collections.Generic;

namespace PE_Scrapping.Entidades
{
    public class Mesa
    {
        [JsonProperty("mesasVotacion")]
        public List<MesasVotacion> MesasVotacion { get; set; }
    }
    public class MesasVotacion
    {
        public string NUMMESA { get; set; }
        public string PROCESADO { get; set; }
        public string IMAGEN { get; set; }
    }
}
