namespace Calculator.DevInfo
{
    public static class DeveloperInfo
    {
        public static string Name       => "Бученков Д.Е.\n Кохан К.Д.";
        public static string Group      => "10701324";
        public static string Supervisor => "Станкевич С.Н.";
        public static string Version    => "2.0.0";

        public static string GetFullInfo() =>
            $"Разработчик: {Name}\n" +
            $"Группа: {Group}\n" +
            $"Руководитель: {Supervisor}\n" +
            $"Версия: {Version}";
    }
}