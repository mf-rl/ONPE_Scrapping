using PE_Scrapping.Funciones;
using System.Collections.Generic;

namespace PE_Scrapping.Screens
{
    public class ElectionType : BaseScreen
    {
        public ElectionType()
        {
            ScreenMessage = new string[]
            {
                Messages.DOUBLE_LINE(),
                Messages.SELECT_PROCESS_INPUT,
                Messages.FIRST_STAGE_OPTION,
                Messages.SECOND_STAGE_OPTION,
                Messages.DOUBLE_LINE(),
                Messages.SELECT_OPTION_AND_ENTER
            };
            PosibleInputs = new List<string>() { Constants.ProcesarPrimeraV, Constants.ProcesarSegundaV };
            CheckInputs = ValidateInput;
        }
        private bool ValidateInput() => !PosibleInputs.Exists(i => SelectedInput.Equals(i));
    }
}
