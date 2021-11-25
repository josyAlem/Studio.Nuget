using LoggerUtility;
using System;

namespace TestApp
{
    public static class TestLog
    {
        public static void Exec() {
            var x = new ConsoleLog();
            x.WriteToConsole("hi there");
        }    }
}
