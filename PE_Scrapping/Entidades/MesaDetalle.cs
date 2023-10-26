using Newtonsoft.Json;
using System.Collections.Generic;

namespace PE_Scrapping.Entidades
{
    public class MesaDetalle
    {
        [JsonProperty("procesos")]
        public Procesos Procesos { get; set; }
    }
    public class Procesos
    {
        [JsonProperty("generalCon")]
        public GeneralCon GeneralCon { get; set; }
        [JsonProperty("generalPre")]
        public GeneralPre GeneralPre { get; set; }
        [JsonProperty("generalPar")]
        public GeneralPar GeneralPar { get; set; }
    }
    public class TipoProceso
    {
        [JsonProperty("votos")]
        public List<Voto> Votos { get; set; }
        [JsonProperty("imageActa")]
        public string ImageActa { get; set; }
    }
    public class GeneralCon : TipoProceso
    {
        [JsonProperty("congresal")]
        public DetalleProceso Congresal { get; set; }
    }
    public class GeneralPar : TipoProceso
    {
        [JsonProperty("parlamento")]
        public DetalleProceso Parlamento { get; set; }
    }
    public class GeneralPre : TipoProceso
    {
        [JsonProperty("presidencial")]
        public DetalleProceso Presidencial { get; set; }
    }
    public class DetalleProceso
    {
        public string CCODI_UBIGEO { get; set; }
        public string CCOPIA_ACTA { get; set; }
        public string NNUME_HABILM { get; set; }
        public string TOT_CIUDADANOS_VOTARON { get; set; }
    }
    public class Voto
    {
        public string AUTORIDAD { get; set; }
        public string NLISTA { get; set; }
        [JsonProperty("congresal")]
        public string Congresal { get; set; }
    }
}
