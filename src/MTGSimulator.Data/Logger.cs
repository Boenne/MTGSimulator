using System;

namespace MTGSimulator.Data
{
    public interface ILogger
    {
        void Error(string message, Exception e);
    }

    public class Logger : ILogger
    {
        public void Error(string message, Exception e)
        {
            Console.WriteLine($"{message}. {e.Message}. {e.StackTrace}");
        }
    }
}