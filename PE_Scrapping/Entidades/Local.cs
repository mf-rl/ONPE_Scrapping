using Newtonsoft.Json;
using System.Collections.Generic;

namespace PE_Scrapping.Entidades
{
    public class Local
    {
        [JsonProperty("locales")]
        public List<Locale> Locales { get; set; }
    }
    public class Locale
    {
        public string CCODI_LOCAL { get; set; }
        public string CCODI_UBIGEO { get; set; }
        public string TNOMB_LOCAL { get; set; }
        public string TDIRE_LOCAL { get; set; }
    }
}
