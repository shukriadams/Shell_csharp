
using System.Reflection;

namespace MadScience_Shell;
public class Program
{
    static void Main()
    {
        Shell shell = new Shell("ls .");
        int result = shell.Run();
        System.Diagnostics.Debug.Assert(result == 0);
        Console.WriteLine($"Expected file list {shell.Out}");

        shell = new Shell("do a fail thing");
        result = shell.Run();
        System.Diagnostics.Debug.Assert(result != 0);
        Console.WriteLine($"Expected error: {shell.Err}");

    }
}