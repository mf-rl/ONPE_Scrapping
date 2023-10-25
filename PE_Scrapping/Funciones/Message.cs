namespace PE_Scrapping.Funciones
{
    public static class Messages
    {
        public const string SELECT_PROCESS_INPUT = "Seleccionar proceso:";
        public const string FIRST_STAGE_OPTION = "1: Resultados 1ra Vuelta";
        public const string SECOND_STAGE_OPTION = "2: Resultados 2da Vuelta";

        public const string SELECT_PROCESS_TYPE_INPUT = "Seleccionar tipo de proceso:";
        public const string TOTAL_PROCESS_OPTION = "1: Total";
        public const string PARTIAL_PROCESS_OPTION = "2: Parcial";

        public const string SELECT_PARTIAL_OPTION = "Seleccionar opción:";
        public const string UBIGEO_OPTION = "1: Por Ubigeo";
        public const string TABLE_OPTION = "2: Por Mesa";

        public const string INPUT_TABLE_NUMBER = "Ingresar número de mesa:";
        public const string INPUT_UBIGEO_CODE = "Ingresar código de ubigeo:";

        public const string SELECT_OPTION_AND_ENTER = "Digitar opción y presionar <Enter>:";
        public const string WAIT_FOR_ANSWER = "Respuesta: ";
                
        public const string PROCESS_FINISHED = "Finalizado. :)";
        public const string PRESS_ANY_KEY = "Pulsar cualquier tecla para continuar...";
        public const string READING_DATA = "Leyendo datos de web apis...";
        public const string READING = "Leyendo: {0}";
        public const string TABLE_NUMBER = "Mesa N° {0}";

        public const string UBIGEO_INVALID = "Error: código de ubigeo inválido.";
        public const string UBIGEO_NOT_FOUND = "Error: No se encontró ubigeo.";
        public const string TABLE_NOT_FOUND = "Error: No se encontró mesa.";

        public static string DOUBLE_LINE()
            { return "=".PadRight(51, '='); }
    }
}
