using System;
using System.Collections.Generic;
using YPandar.Common.Functional;
using PE_Scrapping.Funciones;

namespace PE_Scrapping.Screens
{
    public class BaseScreen : IDisposable
    {
        private bool disposed = false;
        public string[] ScreenMessage { get; set; }
        public List<string> PosibleInputs { get; set; } = new List<string>();
        public string SelectedInput { get; set; } = string.Empty;
        public string Show()
        {
            FunctionalHandler.RepeatActionIf(
                () =>
                {
                    FunctionalHandler.WriteLines(ScreenMessage);
                    SelectedInput = FunctionalHandler.GetUserInput(Messages.WAIT_FOR_ANSWER);
                }, CheckInputs
            );
            return SelectedInput;
        }
        
        public Func<bool> CheckInputs { get; set; }
                
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {                
                disposed = true;
            }
        }
                
        ~BaseScreen()
        {
            Dispose(false);
        }
    }
}