namespace PE_Scrapping.Funciones
{
    public static class Messages
    {
        public static string SELECT_PROCESS_INPUT = "Seleccionar proceso:";
        public static string FIRST_STAGE_OPTION = "1: Resultados 1ra Vuelta";
        public static string SECOND_STAGE_OPTION = "2: Resultados 2da Vuelta";

        public static string SELECT_PROCESS_TYPE_INPUT = "Seleccionar tipo de proceso:";
        public static string TOTAL_PROCESS_OPTION = "1: Total";
        public static string PARTIAL_PROCESS_OPTION = "2: Parcial";

        public static string SELECT_PARTIAL_OPTION = "Seleccionar opción:";
        public static string UBIGEO_OPTION = "1: Por Ubigeo";
        public static string TABLE_OPTION = "2: Por Mesa";

        public static string INPUT_TABLE_NUMBER = "Ingresar número de mesa:";
        public static string INPUT_UBIGEO_CODE = "Ingresar código de ubigeo:";

        public static string SELECT_OPTION_AND_ENTER = "Digitar opción y presionar <Enter>:";
        public static string WAIT_FOR_ANSWER = "Respuesta: ";

        public static string INITIALIZING_DRIVER = "Inicializando ChromeDriver...";
        public static string DRIVER_INITIATED = "ChromeDriver Iniciado.";
        public static string PROCESS_FINISHED = "Finalizado. :)";
        public static string PRESS_ANY_KEY = "Pulsar cualquier tecla para continuar...";
        public static string READING_DATA = "Leyendo datos de web apis...";
        public static string READING = "Leyendo: {0}";
        public static string TABLE_NUMBER = "Mesa N° {0}";

        public static string UBIGEO_INVALID = "Error: código de ubigeo inválido.";
        public static string UBIGEO_NOT_FOUND = "Error: No se encontró ubigeo.";
        public static string TABLE_NOT_FOUND = "Error: No se encontró mesa.";

        public static string DOUBLE_LINE()
            { return "=".PadRight(51, '='); }
    }
}
