namespace PE_Scrapping.Entidades
{
    public class AppConfig
    {
        public Api Api { get; set; }
        public string DataBaseName { get; set; }
        public string SavePath { get; set; }
        public bool DownloadFiles { get; set; }
        public int MilisecondsWait { get; set; }
        public string ChromeDriverPath { get; set; }
        public bool SaveJson { get; set; }
        public bool SaveData { get; set; }
    }
    public class Api
    {
        public EndPointSet First { get; set; }
        public EndPointSet Second { get; set; }
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
}
