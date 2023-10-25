using PE_Scrapping.Funciones;

namespace PE_Scrapping.Screens
{
    public class UbigeoCode : BaseScreen
    {
        public UbigeoCode()
        {
            ScreenMessage = new string[] { Messages.DOUBLE_LINE(), Messages.INPUT_UBIGEO_CODE };
            CheckInputs = ValidateInput;
        }
        private bool ValidateInput() => string.IsNullOrEmpty(SelectedInput) || SelectedInput.Trim().Length > 6;
    }
}
