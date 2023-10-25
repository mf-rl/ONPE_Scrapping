namespace PE_Scrapping.Entidades
{
    public class AppConfig
    {
        public Api Api { get; set; }
        public string DataBaseName { get; set; }
        public string SavePath { get; set; }
        public bool DownloadFiles { get; set; }
        public int MilisecondsWait { get; set; }
        public bool SaveJson { get; set; }
        public bool SaveData { get; set; }
        public string JsonFileExtension { get; set; }
    }
    public class Api
    {
        public EndPointSet First { get; set; }
        public EndPointSet Second { get; set; }
        public RequestParameters RequestParameters { get; set; }
    }
    public class EndPointSet
    {
        public string Title { get; set; }
        public string BaseUri { get; set; }
        public string Ubigeo { get; set; }
        public string Locale { get; set; }
        public string Table { get; set; }
        public string TableDetail { get; set; }
        public string BodyTag { get; set; }
    }
    public class RequestParameters
    {
        public string UbigeoCode { get; set; }
        public string LocaleCode { get; set; }
        public string TableCode { get; set; }
    }
}
