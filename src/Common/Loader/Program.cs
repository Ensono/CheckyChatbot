﻿using Colorful;
using ManyConsole;

namespace Loader {
    public class Program {
        public static int Main(string[] args) {
            var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
            return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
        }
    }
}