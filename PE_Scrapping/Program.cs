using System;
using PE_Scrapping.Funciones;
using CommonFuntionalMethods;
using PE_Scrapping.Screens;

namespace PE_Scrapping
{
    internal static class Program
    {
        static void Main()
        {
            string sel = string.Empty;
            
            string mesa_sel = string.Empty;

            Console.Title = Constants.APP_TITLE;
            FunctionalHandler.WriteLines(new string[]
            {
                Messages.DOUBLE_LINE(),
                Constants.APP_TITLE.PadLeft(41, ' ')
            });

            string opt = new ElectionType().Show();

            string tip_pro = new ProcessType().Show();

            FunctionalHandler.ExecuteActionIf(
                () =>
                {
                    sel = new PartialType().Show();
                },
                () =>
                {
                    return tip_pro.Equals(Constants.ProcesoParcial);
                }
            );
            FunctionalHandler.ExecuteActionIf(
                () =>
                {
                    FunctionalHandler.WriteLines(new string[] { Messages.DOUBLE_LINE() });
                    FunctionalHandler.RepeatActionIf(
                        () =>
                        {
                            FunctionalHandler.WriteLines(new string[] { Messages.INPUT_TABLE_NUMBER });
                            mesa_sel = FunctionalHandler.GetUserInput(Messages.WAIT_FOR_ANSWER);
                        },
                        () =>
                        {
                            return string.IsNullOrEmpty(mesa_sel);
                        }
                    );
                },
                () =>
                {
                    return sel.Equals(Constants.ProcesoMesa);
                }
            );
            FunctionalHandler.ExecuteActionIf(
                () =>
                {
                    FunctionalHandler.WriteLines(new string[] { Messages.DOUBLE_LINE() });
                    FunctionalHandler.RepeatActionIf(
                        () =>
                        {
                            FunctionalHandler.WriteLines(new string[] { Messages.INPUT_UBIGEO_CODE });
                            mesa_sel = FunctionalHandler.GetUserInput(Messages.WAIT_FOR_ANSWER);
                        },
                        () =>
                        {
                            return string.IsNullOrEmpty(mesa_sel) || mesa_sel.Trim().Length > 6;
                        }
                    );
                },
                () =>
                {
                    return sel.Equals(Constants.ProcesoUbigeo);
                }
            );
            MainProcess.ExecuteProcess(opt, tip_pro, sel, mesa_sel);
            FunctionalHandler.WriteLines(new string[] {
                Messages.PROCESS_FINISHED,
                Messages.PRESS_ANY_KEY,
            });
            Console.ReadKey();
        }
    }
}
