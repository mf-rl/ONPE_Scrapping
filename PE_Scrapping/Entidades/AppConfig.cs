﻿namespace PE_Scrapping.Entidades
{
    public class AppConfig
    {
        public Api Api { get; set; }
        public string DataBaseName { get; set; }
        public string SavePath { get; set; }
        public bool DownloadFiles { get; set; }
        public bool SaveJson { get; set; }
        public bool SaveData { get; set; }
        public string JsonFileExtension { get; set; }
        public string FilePath { get; set; }
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
        public string Id { get; set; }
        public string FileStorage { get; set; }
    }
    public class RequestParameters
    {
        public string UbigeoCode { get; set; }
        public string LocaleCode { get; set; }
        public string TableCode { get; set; }
    }
}
