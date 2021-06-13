using System.Collections.Generic;

namespace PE_Scrapping.Entidades
{
    public class Ubigeo
    {
        public Ubigeos ubigeos { get; set; }
    }
    public class Ubigeos
    {
        public Nacional nacional { get; set; }
        public Extranjero extranjero { get; set; }
    }
    public class Nacional
    {
        public List<Department> departments { get; set; }
        public List<Province> provinces { get; set; }
        public List<District> districts { get; set; }
    }

    public class Extranjero
    {
        public List<Department> continents { get; set; }
        public List<Province> countries { get; set; }
        public List<District> states { get; set; }
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
