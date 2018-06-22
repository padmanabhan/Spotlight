using System;

namespace FizzBuzz
{
    class Program
    {
        static void Main(string[] args)
        {
            for(int iCount = 1; iCount <= 100; iCount++)
            {
                var result = (iCount % 3 == 0 && iCount % 5 == 0) ? "FizzBuzz"
                    : (iCount % 5 == 0) ? "Buzz" : (iCount % 3 == 0) ? "Fizz" : iCount.ToString();

                Console.WriteLine(result);
            }

            Console.Read();
        }
    }
}
