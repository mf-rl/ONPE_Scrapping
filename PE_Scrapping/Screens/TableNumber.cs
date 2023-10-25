using PE_Scrapping.Funciones;

namespace PE_Scrapping.Screens
{
    public class TableNumber : BaseScreen
    {
        public TableNumber()
        {
            ScreenMessage = new string[] { Messages.DOUBLE_LINE(), Messages.INPUT_TABLE_NUMBER };
            CheckInputs = ValidateInput;
        }
        private bool ValidateInput() => string.IsNullOrEmpty(SelectedInput);
    }
}
