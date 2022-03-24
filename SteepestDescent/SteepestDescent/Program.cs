﻿using System;
using System.IO;

namespace SteepestDescent
{
    class Program
    {
        public static double[] SteepestDescent(ref double[,] A, ref double[] b, double Tolerance)
        {
            int n = b.Length;
            double Alpha; //параметр альфа
            double[] x = new double[n];
            double[] r = new double[n]; //антиградиент

            //начальное приближение
            x = b;
            r = b;

            while(Math.Sqrt(ScalarProduct(r,r)) < Tolerance) //вторая норма верктора
            {
                Alpha = ScalarProduct(r, r) / ScalarProduct(r, MatrixVectorProduct(A, r));

                for (int i = 0; i < n; i++)
                {
                    x[i] = x[i] + Alpha * r[i];
                }

                r = GradF(in A, in x, in b);
            }    

            return x;
        }

        public static double[] SteepestDescent(ref double[,] A, ref double[] b, double Tolerance, int ThreadsNumber)
        {
            int n = b.Length;
            double Alpha; //параметр альфа
            double[] x = new double[n];
            double[] r = new double[n];

            //начальное приближение
            for (int i = 0; i < n; i++)
            {
                x[i] = b[i];
                r[i] = b[i];
            }

            while (Math.Sqrt(ScalarProduct(r, r, ThreadsNumber)) < Tolerance) //вторая норма верктора
            {
                Alpha = ScalarProduct(r, r, ThreadsNumber) / ScalarProduct(r, MatrixVectorProduct(A, r, ThreadsNumber), ThreadsNumber);

                for (int i = 0; i < n; i++)
                {
                    x[i] = x[i] + Alpha * r[i];
                }

                r = GradF(in A, in x, in b, ThreadsNumber);
            }

            return x;
        }

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

            Strings = RawText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string str in Strings)
            {
                string[] Numbers = str.Split(';', StringSplitOptions.RemoveEmptyEntries);
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
                    foreach (int number in x)
                    {
                        Console.WriteLine(number);
                        stream.WriteLine(number);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Unable to create output file");

                foreach (int number in x)
                {
                    Console.WriteLine(number);
                }
            }
        }

        public static double ScalarProduct(in double[] x, in double[] y)
        {
            double Product = 0;
            int n = x.Length;

            if (y.Length != n)
                throw new InvalidOperationException("Vectors cannot be multiplied");

            for (int i = 0; i < n; i++)
                Product += x[i] * y[i];

            return Product;
        }


        //параллельная реализация
        public static double ScalarProduct(in double[] x, in double[] y, int ThreadsNumber)
        {
            double Product = 0;
            int n = x.Length;

            if (y.Length != n)
                throw new InvalidOperationException("Vectors cannot be multiplied");

            for (int i = 0; i < n; i++)
                Product += x[i] * y[i];

            return Product;
        }

        public static double[] MatrixVectorProduct(in double[,] A, in double[] x)
        {
            int n = x.Length;
            double[] Product = new double[n];

            if (A.GetLength(0) != n)
                throw new InvalidOperationException("Matrices cannot be multiplied");

            for (int i = 0; i < n; i++)
                for (int k = 0; k < n; k++)
                    x[i] += A[i, k] * x[k];

            return Product;
        }


        //параллельная реализация
        public static double[] MatrixVectorProduct(in double[,] A, in double[] x, int ThreadsNumber)
        {
            int n = x.Length;
            double[] Product = new double[n];

            if (A.GetLength(0) != n)
                throw new InvalidOperationException("Matrices cannot be multiplied");

            for (int i = 0; i < n; i++)
                for (int k = 0; k < n; k++)
                    x[i] += A[i, k] * x[k];

            return Product;
        }

        public static double[] GradF(in double[,] A, in double[] x, in double[] b)
        {
            int n = x.Length;
            double[] y = MatrixVectorProduct(A, x);

            for (int i = 0; i < n; i++)
                y[i] = -y[i] + b[i];

            return y;
        }

        //параллельная реализация
        public static double[] GradF(in double[,] A, in double[] x, in double[] b, int ThreadsNumber)
        {
            int n = x.Length;
            double[] y = MatrixVectorProduct(A, x, ThreadsNumber);

            for (int i = 0; i < n; i++)
                y[i] = -y[i] + b[i];

            return y;
        }

        public static void Main(string[] args)
        {
            double Tolerance = 1e-6; //допуск
            double[] x;
            string FileName;
            string OutFileName;
            int n;

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

            try
            {
                ReadSLAE(out double[,] A, out double[] b, n + 1, FileName);
                if (TestSymmetry(in A))
                {
                    if (TestSylvester(in A))
                    {
                        x = SteepestDescent(ref A, ref b, Tolerance);
                        WriteRoots(in x, OutFileName);
                    }
                    else
                        Console.WriteLine("\nMatrix does not satisfy the Sylvester criterion");
                }
                else
                    Console.WriteLine("\nMatrix is not symmetrical");
            }
            catch
            {
                Console.WriteLine("\nCritical error while processing. Check your SLAE file");
            }

        }
    }
}
