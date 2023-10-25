using PE_Scrapping.Funciones;
using System.Collections.Generic;

namespace PE_Scrapping.Screens
{
    public class PartialType : BaseScreen
    {
        public PartialType()
        {
            ScreenMessage = new string[]
            {
                Messages.DOUBLE_LINE(),
                Messages.SELECT_PARTIAL_OPTION,
                Messages.UBIGEO_OPTION,
                Messages.TABLE_OPTION,
                Messages.DOUBLE_LINE(),
                Messages.SELECT_OPTION_AND_ENTER
            };
            PosibleInputs = new List<string>() { Constants.ProcesoUbigeo, Constants.ProcesoMesa };
        }
    }
}
