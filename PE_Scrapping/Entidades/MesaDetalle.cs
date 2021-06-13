using System.Collections.Generic;

namespace PE_Scrapping.Entidades
{
    public class MesaDetalle
    {
        public Procesos procesos { get; set; }
    }
    public class Procesos
    {
        public GeneralCon generalCon { get; set; }
        public GeneralPre generalPre { get; set; }
        public GeneralPar generalPar { get; set; }
        public string asistioNoVoto { get; set; }
        public string realImages { get; set; }
    }
    public class TipoProceso
    {
        public List<Voto> votos { get; set; }
        public List<object> resoluciones { get; set; }
        public string imageActa { get; set; }
    }
    public class GeneralCon : TipoProceso
    {
        public DetalleProceso congresal { get; set; }
    }
    public class GeneralPar : TipoProceso
    {
        public DetalleProceso parlamento { get; set; }
    }
    public class GeneralPre : TipoProceso
    {
        public DetalleProceso presidencial { get; set; }
    }
    public class DetalleProceso
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
    public class Voto
    {
        public string CCODI_AUTO { get; set; }
        public string AUTORIDAD { get; set; }
        public int NLISTA { get; set; }
        public string congresal { get; set; }
        public string CON_VALIDOS { get; set; }
        public string CON_EMITIDOS { get; set; }
        public string TOTAL_VOTOS1 { get; set; }
        public string TOTAL_VOTOS2 { get; set; }
        public string TOTAL_VOTOS3 { get; set; }
        public string TOTAL_VOTOS4 { get; set; }
        public string TOTAL_VOTOS5 { get; set; }
        public string TOTAL_VOTOS6 { get; set; }
        public string TOTAL_VOTOS7 { get; set; }
        public string TOTAL_VOTOS8 { get; set; }
        public string TOTAL_VOTOS9 { get; set; }
        public string TOTAL_VOTOS10 { get; set; }
        public string TOTAL_VOTOS11 { get; set; }
        public string TOTAL_VOTOS12 { get; set; }
        public string TOTAL_VOTOS13 { get; set; }
        public string TOTAL_VOTOS14 { get; set; }
        public string TOTAL_VOTOS15 { get; set; }
        public string TOTAL_VOTOS16 { get; set; }
        public string TOTAL_VOTOS17 { get; set; }
        public string TOTAL_VOTOS18 { get; set; }
        public string TOTAL_VOTOS19 { get; set; }
        public string TOTAL_VOTOS20 { get; set; }
        public string TOTAL_VOTOS21 { get; set; }
        public string TOTAL_VOTOS22 { get; set; }
        public string TOTAL_VOTOS23 { get; set; }
        public string TOTAL_VOTOS24 { get; set; }
        public string TOTAL_VOTOS25 { get; set; }
        public string TOTAL_VOTOS26 { get; set; }
        public string TOTAL_VOTOS27 { get; set; }
        public string TOTAL_VOTOS28 { get; set; }
        public string TOTAL_VOTOS29 { get; set; }
        public string TOTAL_VOTOS30 { get; set; }
        public string TOTAL_VOTOS31 { get; set; }
        public string TOTAL_VOTOS32 { get; set; }
        public string TOTAL_VOTOS33 { get; set; }
        public string TOTAL_VOTOS34 { get; set; }
        public string TOTAL_VOTOS35 { get; set; }
        public string TOTAL_VOTOS36 { get; set; }
    }
}
