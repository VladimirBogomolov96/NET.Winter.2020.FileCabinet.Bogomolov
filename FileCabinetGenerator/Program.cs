using CommandLine;
using System;

namespace FileCabinetGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Options options = GetCommandLineArguments(args);
        }

        private static Options GetCommandLineArguments(string[] args)
        {
            Options options = new Options();
            var result = Parser.Default.ParseArguments<Options>(args).WithParsed(parsed => options = parsed);
            return options;
        }
    }
}
