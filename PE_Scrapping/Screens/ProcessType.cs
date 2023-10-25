using PE_Scrapping.Funciones;
using System.Collections.Generic;

namespace PE_Scrapping.Screens
{
    public class ProcessType : BaseScreen
    {
        public ProcessType()
        {
            ScreenMessage = new string[]
            {
                Messages.DOUBLE_LINE(),
                Messages.SELECT_PROCESS_TYPE_INPUT,
                Messages.TOTAL_PROCESS_OPTION,
                Messages.PARTIAL_PROCESS_OPTION,
                Messages.DOUBLE_LINE(),
                Messages.SELECT_OPTION_AND_ENTER
            };
            PosibleInputs = new List<string>() { Constants.ProcesoTotal, Constants.ProcesoParcial };
        }
    }
}
