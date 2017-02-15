using System;

namespace MTGSimulator.Hubs
{
    public class HubException : Exception
    {
        public HubException(string message) : base(message)
        {
        }
    }
}