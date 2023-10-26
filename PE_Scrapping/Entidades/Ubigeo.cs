using Newtonsoft.Json;
using System.Collections.Generic;

namespace PE_Scrapping.Entidades
{
    public class Ubigeo
    {
        [JsonProperty("ubigeos")]
        public Ubigeos Ubigeos { get; set; }
    }
    public class Ubigeos
    {
        [JsonProperty("nacional")]
        public Nacional Nacional { get; set; }
        [JsonProperty("extranjero")]
        public Extranjero Extranjero { get; set; }
    }
    public class Nacional
    {
        [JsonProperty("departments")]
        public List<Department> Departments { get; set; }
        [JsonProperty("provinces")]
        public List<Province> Provinces { get; set; }
        [JsonProperty("districts")]
        public List<District> Districts { get; set; }
    }

    public class Extranjero
    {
        [JsonProperty("continents")]
        public List<Department> Continents { get; set; }
        [JsonProperty("countries")]
        public List<Province> Countries { get; set; }
        [JsonProperty("states")]
        public List<District> States { get; set; }
    }
    public class Department
    {
        public string CDGO_DEP { get; set; }
        public string DESC_DEP { get; set; }
        public string CDGO_PADRE { get; set; }
    }
    public class Province
    {
        public string CDGO_PROV { get; set; }
        public string DESC_PROV { get; set; }
        public string CDGO_PADRE { get; set; }
    }
    public class District
    {
        public string CDGO_DIST { get; set; }
        public string DESC_DIST { get; set; }
        public string CDGO_PADRE { get; set; }
    }
}
