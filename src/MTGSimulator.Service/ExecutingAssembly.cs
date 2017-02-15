using System;
using System.Reflection;

namespace MTGSimulator.Service
{
    public static class ExecutingAssembly
    {
        public static string Namespace { get; private set; }
        public static Assembly Assembly { get; private set; }

        public static void SetCurrentAssembly(Type type)
        {
            Assembly = type.GetTypeInfo().Assembly;
            Namespace = type.Namespace;
        }
    }
}