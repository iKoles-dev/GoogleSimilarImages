using System;

namespace ImageLibrary
{
    using ImageLibrary.Control;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter work directory.");
            string workDirectory = Console.ReadLine().Replace("\"", "").Replace("'", "");
            LibraryController library = new LibraryController(workDirectory);
            library.Start();
            Console.WriteLine("Task completed successfully!");
            Console.ReadLine();
        }
    }
}
