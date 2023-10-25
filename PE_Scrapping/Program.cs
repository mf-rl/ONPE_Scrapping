using System;
using PE_Scrapping.Screens;
using PE_Scrapping.Funciones;
using YPandar.Common.Functional;
using PE_Scrapping.Entidades;

namespace PE_Scrapping
{
    internal static class Program
    {        
        static void Main()
        {
            InputParameters input = new();

            Console.Title = Constants.APP_TITLE;
            FunctionalHandler.WriteLines(new string[]
            {
                Messages.DOUBLE_LINE(),
                Constants.APP_TITLE.PadLeft(41, ' ')
            });

            using (ElectionType screen = new()) { input.ElectionType = screen.Show(); }
            using (ProcessType screen = new()) { input.ProcessType = screen.Show(); }            

            FunctionalHandler.ExecuteActionIf(
                () =>
                {
                    using PartialType screen = new(); input.PartialType = screen.Show();
                },
                () =>
                {
                    return input.ProcessType.Equals(Constants.ProcesoParcial);
                }
            );
            FunctionalHandler.ExecuteActionIf(
                () =>
                {
                    using TableNumber screen = new(); input.TableNumber = screen.Show();
                },
                () =>
                {
                    return input.PartialType.Equals(Constants.ProcesoMesa);
                }
            );
            FunctionalHandler.ExecuteActionIf(
                () =>
                {
                    using UbigeoCode screen = new(); input.UbigeoCode = screen.Show();
                },
                () =>
                {
                    return input.PartialType.Equals(Constants.ProcesoUbigeo);
                }
            );
            MainProcess.ExecuteProcess(input);
            FunctionalHandler.WriteLines(new string[] {
                Messages.PROCESS_FINISHED,
                Messages.PRESS_ANY_KEY,
            });
            Console.ReadKey();
        }
    }
}
