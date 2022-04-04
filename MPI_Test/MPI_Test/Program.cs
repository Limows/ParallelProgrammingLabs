using System;
using System.IO;
using MPI;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MPI_Test
{
    class Program
    {
        public static void ReadSLAE(out double[,] A, out double[] b, int n, string FileName)
        {
            A = new double[n, n];
            b = new double[n];
            string RawText = "";
            string[] Strings = new string[n];
            int i = 0, j = 0;

            Console.WriteLine("\nReading data from file: " + FileName + "\n");

            try
            {
                using (StreamReader stream = File.OpenText(FileName))
                {
                    RawText = stream.ReadToEnd();
                }
            }
            catch
            {
                Console.WriteLine("File not exist or corrupted\n");
                throw;
            }

            Strings = RawText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string str in Strings)
            {
                string[] Numbers = str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                j = 0;

                Console.WriteLine(str);

                foreach (string number in Numbers)
                {
                    if (j < n)
                    {
                        A[i, j] = Convert.ToDouble(number);

                        j++;
                    }
                    else
                        b[i] = Convert.ToDouble(number);
                }

                i++;
            }

        }

        public static bool TestSymmetry(in double[,] A)  //проверка симметричности матрицы
        {
            int n = A.GetLength(0);

            for (int i = 0; i < n - 1; i++)
                for (int j = i + 1; j < n; j++)
                    if (A[i, j] != A[j, i])
                        return false;

            return true;
        }

        public static bool TestSylvester(in double[,] A) //проверка критерия Сильвестра
        {
            return true;
        }

        public static void WriteRoots(in double[] x, string FileName)
        {
            int n = x.Length;

            Console.WriteLine("\nWriting data to file: " + FileName + "\n");

            try
            {
                using (StreamWriter stream = File.CreateText(FileName))
                {
                    foreach (double number in x)
                    {
                        Console.WriteLine(number);
                        stream.WriteLine(number);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Unable to create output file");

                foreach (double number in x)
                {
                    Console.WriteLine(number);
                }
            }
        }

        static void Main(string[] args)
        {
            double Tolerance = 0.1; //допуск
            double[] x = new double[0];
            string FileName;
            string OutFileName;
            int n;
            Stopwatch Watch = new Stopwatch();
            TimeSpan Time;

            if (args.Length > 1)
            {
                FileName = args[0];
                n = Convert.ToInt32(args[1]);
                if (args.Length > 2)
                    OutFileName = args[2];
                else
                    OutFileName = "roots.txt";
            }
            else
            {
                Console.WriteLine("Enter name of file, that contains matrix and right side");
                Console.Write("File name: ");
                FileName = Console.ReadLine();

                Console.WriteLine("\nEnter size of matrix");
                Console.Write("Size: ");
                n = Convert.ToInt32(Console.ReadLine());

                OutFileName = "roots.txt";
            }

            //try
            //{
                ReadSLAE(out double[,] A, out double[] b, n, FileName);

                if (TestSymmetry(in A))
                {
                    if (TestSylvester(in A))
                    {
                        Watch.Start();

                        MPI.Environment.Run(ref args, communicator =>
                        {
                            n = b.Length;
                            x = new double[n];

                            if (A.GetLength(0) != n)
                                throw new InvalidOperationException("Matrices cannot be multiplied");

                            for (int i = 0; i < n; i++)
                                for (int j = 0; j < n; j++)
                                    x[i] += A[i, j] * b[j];
                        });

                        Watch.Stop();
                        Time = Watch.Elapsed;

                        WriteRoots(in x, OutFileName);

                        Console.WriteLine
                        (
                            "\nRunTime: " +
                            String.Format
                            (
                                "{0:00}:{1:00}:{2:00}.{3:00}",
                                Time.Hours,
                                Time.Minutes,
                                Time.Seconds,
                                Time.Milliseconds / 10
                            )
                        );
                        Console.WriteLine("Ticks: " + Time.Ticks);
                    }
                    else
                        Console.WriteLine("\nMatrix does not satisfy the Sylvester criterion");
                }
                else
                    Console.WriteLine("\nMatrix is not symmetrical");
            //}
            //catch
            //{
            //    Console.WriteLine("\nCritical error while processing. Check your SLAE file");
            //}
        }
    }
}
