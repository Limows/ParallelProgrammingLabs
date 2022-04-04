using System;
using System.Text;
using MPI;

namespace MPI_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            MPI.Environment.Run(ref args, communicator =>
            {
                Console.WriteLine
                (
                    "Hello, from process number " + 
                    communicator.Rank + " of " + 
                    communicator.Size
                );

                Console.ReadLine();
            });

        }
    }
}
