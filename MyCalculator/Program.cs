using System;
using CommandLineCalculator;

namespace MyCalculator
{
    class Program
    {
        static void Main()
        {
            string equation;
            var calculator = new Calculator();
            do
            {
                Console.Write("calculator> ");
                equation = Console.ReadLine();
                var result = calculator.Calculate(equation);
                Console.WriteLine(result);
            } while (equation?.ToLower() != "exit");
        }
    }
}
