using System.Collections.Generic;

namespace PE_Scrapping.Entidades
{
    public class MesaDetalle
    {
        public Procesos procesos { get; set; }
    }
    public class Procesos
    {
        public GeneralPre generalPre { get; set; }
        public string asistioNoVoto { get; set; }
        public string realImages { get; set; }
    }
    public class GeneralPre
    {
        public Presidencial presidencial { get; set; }
        public List<Voto> votos { get; set; }
        public List<object> resoluciones { get; set; }
        public string imageActa { get; set; }
    }
    public class Voto
    {
        public string CCODI_AUTO { get; set; }
        public string AUTORIDAD { get; set; }
        public string NLISTA { get; set; }
        public string congresal { get; set; }
        public string CON_VALIDOS { get; set; }
        public string CON_EMITIDOS { get; set; }
    }
    public class Presidencial
    {
        public string CCODI_UBIGEO { get; set; }
        public string TNOMB_LOCAL { get; set; }
        public string TDIRE_LOCAL { get; set; }
        public string CCENT_COMPU { get; set; }
        public string DEPARTAMENTO { get; set; }
        public string PROVINCIA { get; set; }
        public string DISTRITO { get; set; }
        public string CCOPIA_ACTA { get; set; }
        public int NNUME_HABILM { get; set; }
        public string OBSERVACION { get; set; }
        public string OBSERVACION_TXT { get; set; }
        public int N_CANDIDATOS { get; set; }
        public int TOT_CIUDADANOS_VOTARON { get; set; }
    }
}
