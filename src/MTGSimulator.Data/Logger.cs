using System;
using System.Diagnostics;

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
            if(Debugger.IsAttached)
                Debug.WriteLine($"{message}. {e.Message}. {e.StackTrace}");
        }
    }
}