using SOLID.SRP;
using System.Diagnostics;

namespace ConsoleApp;

public class Program
{
    static void Main(string[] args)
    {
        Journal journal = new Journal();
        journal.add("I went to college");
        journal.add("I ate icecream");

        Console.WriteLine(journal);

        Persistence p = new Persistence();
        string filename = Path.GetFullPath(@"C:\Users\ERFAN\Desktop\C# Practicing\Projects\DesignPatterns\journal.txt");

        Console.WriteLine(filename);


        p.Save(filename, journal);

        Process.Start(new ProcessStartInfo
        {
            FileName = filename,
            UseShellExecute = true // This is required to open .txt files
        });
    }
}
