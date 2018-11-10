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

        private string RemoveSpaces(string equation)
        {
            return equation.Replace(" ", string.Empty);
        }

        private bool ParenthesisIsBalanced(string equation)
        {
            return equation.Count(x => x == '(') == equation.Count(x => x == ')');
        }

        private string EvaluateParenthisedPiecesOfEquation(string equation)
        {
            while (equation.Contains("("))
            {
                var length = 0;
                var startIndex = 0;
                var equationIndex = 0;
                foreach (var character in equation)
                {
                    switch (character)
                    {
                        case '(':
                            startIndex = equationIndex + 1;
                            length = 0;
                            break;
                        case ')' when length == 0:
                            length = equationIndex - startIndex;
                            break;
                    }

                    equationIndex += 1;
                }

                if (length > 0)
                {
                    var subEquation = equation.Substring(startIndex, length);
                    var result = LeftToRightCalculate(subEquation);
                    equation = equation.Replace("(" + subEquation + ")", result);
                }
            }

            return equation;
        }

        private string LeftToRightCalculate(string equation)
        {
            if (equation.Length == 0) return string.Empty;
            var result = 0;
            try
            {
                var operation = string.Empty;
                var number = string.Empty;
                var equationIndex = 0;
                var operationIndex = -1;
                foreach (var character in equation)
                {
                    if (CharacterIsNumber(number, equationIndex, operationIndex, character))
                    {
                        number += character;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(number)) result = PerformOperation(result, operation, number);
                        operation = character.ToString();
                        if (ConsecutiveOperators(equationIndex, operationIndex)) return $"Invalid operation ({operation}).";
                        operationIndex = equationIndex;
                        number = string.Empty;
                    }

                    equationIndex += 1;
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

        private bool CharacterIsNumber(string number, int equationIndex, int operationIndex, char character)
        {
            return char.IsNumber(character) || number == string.Empty && character == '-' && equationIndex - 1 == operationIndex;
        }

        private bool ConsecutiveOperators(int equationIndex, int operationIndex)
        {
            return equationIndex - 1 == operationIndex;
        }

        private int PerformOperation(int subResult, string operation, string stringNumber)
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