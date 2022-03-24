﻿using System;
using System.IO;
using System.Text;

namespace MatrixGenerator
{
    class Program
    {   
        public static double[,] RandMatrixGen(int n, int Max, int Min, int Seed)
        {
            double[,] Matrix = new double[n,n];
            Random RandSeed = new Random(Seed);
            Random RandNumber = new Random(RandSeed.Next());

            Max = Max * 1000;
            Min = Min * 1000;

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    Matrix[i, j] = RandNumber.Next(Min, Max) / 1000.0;

            return Matrix;
        }

        public static double[] RandVectorGen(int n, int Max, int Min, int Seed)
        {
            double[] Vector = new double[n];
            Random RandSeed = new Random(Seed);
            Random RandNumber = new Random(RandSeed.Next());

            Max = Max * 1000;
            Min = Min * 1000;

            for (int i = 0; i < n; i++)
                Vector[i] = RandNumber.Next(Min, Max) / 1000;

            return Vector;
        }

        public static void WriteToFile(in double[,] Matrix, in double[] Vector, string FileName)
        {
            int n = Vector.Length;
            StringBuilder Str = new StringBuilder();
            string str;

            using (StreamWriter Writer = File.CreateText(FileName))
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                        Str.Append(Matrix[i, j].ToString("0.###") + ";");

                    Str.Append(Vector[i]);
                    str = Str.ToString();

                    Console.WriteLine(str);
                    Writer.WriteLine(str);
                }
            }
        }

        public static double[,] MakeMatrixPD(in double[,] Matrix)
        {
            int n = Matrix.GetLength(0);
            double[,] Transposed = new double[n, n];
            double[,] PDMatrix = new double[n, n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    Transposed[i, j] = Matrix[j, i];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int k = 0; k < n; k++)
                        PDMatrix[i, j] += Transposed[i, k] * Matrix[k, j];

            return PDMatrix;
        }

        static void Main(string[] args)
        {
            int n, Max, Min, Seed;
            string FileName;
            double[,] Matrix;
            double[] Vector;

            if (args.Length > 4)
            {
                FileName = args[0];
                n = Convert.ToInt32(args[1]);
                Min = Convert.ToInt32(args[2]);
                Max = Convert.ToInt32(args[3]);
                Seed = Convert.ToInt32(args[4]);
            }
            else
            {
                Console.WriteLine("Enter name of file to write result");
                Console.Write("File name: ");
                FileName = Console.ReadLine();

                Console.WriteLine("\nEnter size of matrix");
                Console.Write("Size: ");
                n = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("\nEnter minimum random value");
                Console.Write("Min: ");
                Min = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("\nEnter maximum random value");
                Console.Write("Max: ");
                Max = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("\nEnter seed");
                Console.Write("Seed: ");
                Seed = Convert.ToInt32(Console.ReadLine());

            }

            Matrix = RandMatrixGen(n, Max, Min, Seed);
            Vector = RandVectorGen(n, Max, Min, Seed);

            Matrix = MakeMatrixPD(in Matrix);

            WriteToFile(in Matrix, in Vector, FileName);

        }
    }
}
