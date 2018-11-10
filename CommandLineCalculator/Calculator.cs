using System;
using System.Linq;

namespace CommandLineCalculator
{
    public class Calculator
    {
        public string Calculate(string equation)
        {
            equation = RemoveSpaces(equation);
            if (!ParenthesisIsBalanced(equation)) return "Invalid expression.";
            equation = EvaluateParenthisedPiecesOfEquation(equation);
            return LeftToRightCalculate(equation);
        }

        private static string RemoveSpaces(string equation)
        {
            return equation.Replace(" ", string.Empty);
        }

        private static bool ParenthesisIsBalanced(string equation)
        {
            return equation.Count(x => x == '(') == equation.Count(x => x == ')');
        }

        private static string EvaluateParenthisedPiecesOfEquation(string equation)
        {
            while (equation.Contains("("))
            {
                var startPosition = 0;
                var length = 0;
                var equationPosition = 0;
                foreach (var character in equation)
                {
                    if (character == '(')
                    {
                        startPosition = equationPosition + 1;
                        length = 0;
                    }

                    if (character == ')' && length == 0)
                    {
                        length = equationPosition - startPosition;
                    }
                    equationPosition += 1;
                }

                if (length > 0)
                {
                    var subEquation = equation.Substring(startPosition, length);
                    var result = LeftToRightCalculate(subEquation);
                    equation = equation.Replace("(" + subEquation + ")", result);
                }
            }

            return equation;
        }

        private static string LeftToRightCalculate(string equation)
        {
            if (equation.Length == 0) return string.Empty;
            var result = 0;
            var operation = string.Empty;
            var number = string.Empty;
            try
            {
                var equationPosition = 0;
                var operationPosition = -1;
                foreach (var character in equation)
                {
                    if (CharacterIsNumber(number, equationPosition, operationPosition, character))
                    {
                        number += character;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(number)) result = PerformOperation(result, operation, number);
                        operation = character.ToString();
                        if (ConsecutiveOperators(equationPosition, operationPosition)) return $"Invalid operation ({operation}).";
                        operationPosition = equationPosition;
                        number = string.Empty;
                    }

                    equationPosition += 1;
                }

                result = PerformOperation(result, operation, number);
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message;
            }
            catch (Exception)
            {
                return "Invalid expression.";
            }
            return $"{result}";
        }

        private static bool CharacterIsNumber(string number, int equationPosition, int operationPosition, char character)
        {
            return char.IsNumber(character) || number == string.Empty && character == '-' && equationPosition - 1 == operationPosition;
        }

        private static bool ConsecutiveOperators(int equationPosition, int operationPosition)
        {
            return equationPosition - 1 == operationPosition;
        }

        private static int PerformOperation(int subResult, string operation, string stringNumber)
        {
            if (string.IsNullOrEmpty(operation)) return int.Parse(stringNumber);
            switch (operation)
            {
                case "+":
                    return subResult + int.Parse(stringNumber);
                case "-":
                    return subResult - int.Parse(stringNumber);
                case "*":
                    return subResult * int.Parse(stringNumber);
                case "/":
                    return subResult / int.Parse(stringNumber);
                case "%":
                    return subResult % int.Parse(stringNumber);
                default:
                    throw new InvalidOperationException($"Invalid operation ({operation}).");
            }
        }
    }
}