using System;
using System.Collections.Generic;
using System.Linq;
using CommonFuntionalMethods;
using PE_Scrapping.Funciones;

namespace PE_Scrapping.Screens
{
    public class BaseScreen
    {
        public string[] ScreenMessage { get; set; }
        public List<string> PosibleInputs { get; set; } = new List<string>();

        string SelectedInput = string.Empty;
        public string Show()
        {
            FunctionalHandler.RepeatActionIf(
                () =>
                {
                    FunctionalHandler.WriteLines(ScreenMessage);
                    SelectedInput = FunctionalHandler.GetUserInput(Messages.WAIT_FOR_ANSWER);
                },
                () =>
                {
                    return !CheckInputs();
                }
            );
            return SelectedInput;
        }

        private bool CheckInputs() => PosibleInputs.Exists(i => SelectedInput.Equals(i));        
    }
}