using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fileSplit
{
    class BasicFileSplit
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Name of File to split?");
            string fileName = Console.ReadLine();
            Console.WriteLine("Size of split files? (in MB)");
            string fileSizeInput = Console.ReadLine();
            int fileSizeParsed = 0;
            Int32.TryParse(fileSizeInput, out fileSizeParsed);
            fileSizeParsed = fileSizeParsed * 1000000; // 1000000 for MB, 1000 for KB
            Console.WriteLine("Output folder path?");
            string folderPath = Console.ReadLine();
            SplitFile(fileName, fileSizeParsed, folderPath);
            Console.WriteLine("File Splitting Complete");
        }

        public static void SplitFile(string inputFile, int chunkSize, string path)
        {
            const int BUFFER_SIZE = 20 * 1024;
            byte[] buffer = new byte[BUFFER_SIZE];

            using (Stream input = File.OpenRead(inputFile))
            {
                int index = 0;
                while (input.Position < input.Length)
                {
                    using (Stream output = File.Create(path + "\\" + index))
                    {
                        int remaining = chunkSize, bytesRead;
                        while (remaining > 0 && (bytesRead = input.Read(buffer, 0,
                                Math.Min(remaining, BUFFER_SIZE))) > 0)
                        {
                            output.Write(buffer, 0, bytesRead);
                            remaining -= bytesRead;
                        }
                    }
                    index++;
                    Thread.Sleep(500); // buffer to help other processes get IO
                }
            }
        }
    }
}
