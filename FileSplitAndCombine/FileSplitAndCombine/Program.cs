using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileSplitAndCombine
{
    class BasicFileSplitAndCombine
    {
        public static int BUFFER_SIZE { get; private set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Split File (1) or Combine Files (2)?");
            string splitOrCombineInput = Console.ReadLine();
            Console.Clear();
        newInput:
            int splitOrCombine = 0;
            Int32.TryParse(splitOrCombineInput, out splitOrCombine);
            if (splitOrCombine == 1)
            {
                PromptToSplit();
            }
            else if (splitOrCombine == 2)
            {
                PromptToCombine();
            }
            else
            {
                Console.WriteLine("Incorrect input. Please enter either '1', or '2'.");
                splitOrCombineInput = Console.ReadLine();
                Console.Clear();
                goto newInput;

            }


        }
        public static void PromptToSplit()
        {
            Console.WriteLine("Name of File to split?");
            string fileName = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Size of split files? (in MB)");
            string fileSizeInput = Console.ReadLine();
            Console.Clear();
            int fileSizeParsed = 0;
            Int32.TryParse(fileSizeInput, out fileSizeParsed);
            fileSizeParsed = fileSizeParsed * 1000000; // 1000000 for MB, 1000 for KB
            Console.WriteLine("Output folder path?");
            string folderPath = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("File splitting of " + fileName + " to " + folderPath + " at " + fileSizeInput + " MB has begun.");
            SplitFile(fileName, fileSizeParsed, folderPath);
            Console.WriteLine("Files split successfully.");
        }

        public static void PromptToCombine()
        {
            Console.WriteLine("Folder of files to combine path?");
            string folderPath = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Name of output file?");
            string outputFileName = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("File combination of " + folderPath + " to " + outputFileName + " has begun.");
            CombineFile(folderPath, "*", outputFileName);
            Console.WriteLine("Files combined successfully.");
        }

        public static void SplitFile(string inputFile, int chunkSize, string path)
        {

            const int BUFFER_SIZE = 20 * 1024;
            byte[] buffer = new byte[BUFFER_SIZE];
            FileInfo f = new FileInfo(inputFile);
            int inputFileSize = (int)f.Length;
            int totalFiles = inputFileSize / chunkSize;

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
                    Thread.Sleep(500); // Buffer to help other processes get IO
                    drawTextProgressBar(index, totalFiles);

                }
            }
        }

        private static void CombineFile(string inputDirectoryPath, string inputFileNamePattern, string outputFilePath)
        {
            string[] inputFilePaths = Directory.GetFiles(inputDirectoryPath, inputFileNamePattern);
            Console.WriteLine("Number of files: {0}.", inputFilePaths.Length);
            int fileBeingProcessed = 1;
            using (var outputStream = File.Create(outputFilePath))
            {
                foreach (var inputFilePath in inputFilePaths)
                {
                    using (var inputStream = File.OpenRead(inputFilePath))
                    {
                        // Buffer size can be passed as the second argument.
                        inputStream.CopyTo(outputStream);
                    }
                    drawTextProgressBar(fileBeingProcessed, inputFilePaths.Length);
                    fileBeingProcessed++;
                }
            }
        }

        private static void drawTextProgressBar(int progress, int total)
        {
            // Draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            // Draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            // Draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.CursorLeft = position++;
                Console.Write("-");
            }

            // Draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }
    }

}

