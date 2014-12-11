using MsilInterpreterLib;

namespace MsilInterpreter.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            var runtime = new Runtime();
            runtime.LoadAssembly("../../../BplusTreeApp/bin/Debug/BplusTreeApp.exe");
            runtime.StartExecution(args);
        }
    }
}
