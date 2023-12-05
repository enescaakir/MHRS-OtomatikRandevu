namespace MHRS_OtomatikRandevu.Utils
{
    public static class ConsoleUtil
    {
        public static void WriteText(string text, int delay)
        {
            Console.WriteLine(text);
            Thread.Sleep(delay);
        }
    }
}