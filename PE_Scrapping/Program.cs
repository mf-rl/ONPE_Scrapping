using System;
using PE_Scrapping.Funciones;

namespace PE_Scrapping
{
    class Program
    {
        static void Main()
        {
            string opt = string.Empty;
            string sel = string.Empty;
            string tip_pro = string.Empty;
            string mesa_sel = string.Empty;

            Console.Title = Constants.APP_TITLE;
            Handler.WriteLines(new string[]
            {
                Messages.DOUBLE_LINE(),
                Constants.APP_TITLE.PadLeft(41, ' ')
            });
            Handler.RepeatActionIf(
                () =>
                {
                    Handler.WriteLines(new string[]{
                        Messages.DOUBLE_LINE(),
                        Messages.SELECT_PROCESS_INPUT,
                        Messages.FIRST_STAGE_OPTION,
                        Messages.SECOND_STAGE_OPTION,
                        Messages.DOUBLE_LINE(),
                        Messages.SELECT_OPTION_AND_ENTER
                    });
                    opt = Handler.GetUserInput(Messages.WAIT_FOR_ANSWER);
                },
                () =>
                {
                    return !opt.Equals(Constants.ProcesarPrimeraV) && !opt.Equals(Constants.ProcesarSegundaV);
                }
            );
            Handler.RepeatActionIf(
                () =>
                {
                    Handler.WriteLines(new string[]
                    {
                        Messages.DOUBLE_LINE(),
                        Messages.SELECT_PROCESS_TYPE_INPUT,
                        Messages.TOTAL_PROCESS_OPTION,
                        Messages.PARTIAL_PROCESS_OPTION,
                        Messages.DOUBLE_LINE(),
                        Messages.SELECT_OPTION_AND_ENTER
                    });
                    tip_pro = Handler.GetUserInput(Messages.WAIT_FOR_ANSWER);
                },
                () =>
                {
                    return !tip_pro.Equals(Constants.ProcesoTotal) && !tip_pro.Equals(Constants.ProcesoParcial);
                }
            );
            Handler.ExecuteActionIf(
                () =>
                {
                    Handler.RepeatActionIf(
                        () =>
                        {
                            Handler.WriteLines(new string[]
                            {
                                Messages.DOUBLE_LINE(),
                                Messages.SELECT_PARTIAL_OPTION,
                                Messages.UBIGEO_OPTION,
                                Messages.TABLE_OPTION,
                                Messages.DOUBLE_LINE(),
                                Messages.SELECT_OPTION_AND_ENTER
                            });
                            sel = Handler.GetUserInput(Messages.WAIT_FOR_ANSWER);
                        },
                        () =>
                        {
                            return !sel.Equals(Constants.ProcesoUbigeo) && !sel.Equals(Constants.ProcesoMesa);
                        }
                    );
                },
                () =>
                {
                    return tip_pro.Equals(Constants.ProcesoParcial);
                }
            );
            Handler.ExecuteActionIf(
                () =>
                {
                    Handler.WriteLines(new string[] { Messages.DOUBLE_LINE() });
                    Handler.RepeatActionIf(
                        () =>
                        {
                            Handler.WriteLines(new string[] { Messages.INPUT_TABLE_NUMBER });
                            mesa_sel = Handler.GetUserInput(Messages.WAIT_FOR_ANSWER);
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
            Handler.ExecuteActionIf(
                () =>
                {
                    Handler.WriteLines(new string[] { Messages.DOUBLE_LINE() });
                    Handler.RepeatActionIf(
                        () =>
                        {
                            Handler.WriteLines(new string[] { Messages.INPUT_UBIGEO_CODE });
                            mesa_sel = Handler.GetUserInput(Messages.WAIT_FOR_ANSWER);
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
            Handler.WriteLines(new string[] {
                Messages.PROCESS_FINISHED,
                Messages.PRESS_ANY_KEY,
            });
            Console.ReadKey();
        }
    }
}
