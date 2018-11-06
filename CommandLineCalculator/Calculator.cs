using System;

namespace CommandLineCalculator
{
    public class Calculator
    {
        public string Calculate(string equation)
        {
            equation = equation.Replace(" ", string.Empty);
            while (equation.Contains("("))
            {
                var startPos = 0;
                var length = 0;
                var posInEquation = 0;
                foreach (var character in equation)
                {
                    if (character == '(')
                    {
                        startPos = posInEquation + 1;
                        length = 0;
                    }

                    if (character == ')')
                    {
                        if (length == 0) length = posInEquation - startPos;
                    }
                    posInEquation += 1;
                }

                if (length > 0)
                {
                    var oldString = equation.Substring(startPos, length);
                    var result = LeftToRightCalculate(oldString);
                    equation = equation.Replace("(" + oldString + ")", result);
                }
            }

            return LeftToRightCalculate(equation);
        }

        private string LeftToRightCalculate(string equation)
        {
            if (equation.Length == 0) return string.Empty;
            var result = 0;
            var operation = string.Empty;
            var number = string.Empty;
            try
            {
                var posInEquation = 0;
                var operationPos = -1;
                foreach (var character in equation)
                {
                    if (char.IsNumber(character) ||
                        number == string.Empty && character == '-' && posInEquation - 1 == operationPos)
                    {
                        number += character;
                    }
                    else
                    {
                        result = PerformOperation(result, operation, number);
                        number = string.Empty;
                        switch (character)
                        {
                            case '+':
                            case '-':
                            case '*':
                            case '/':
                            case '%':
                                if (posInEquation - 1 == operationPos) return "Invalid operation.";
                                operation = character.ToString();
                                operationPos = posInEquation;
                                break;
                            default:
                                return "Invalid operation.";
                        }
                    }

                    posInEquation += 1;
                }

                result = PerformOperation(result, operation, number);
            }
            catch (Exception)
            {
                return "Invalid expression.";
            }
            return $"{result}";
        }

        private static int PerformOperation(int subResult, string operation, string strNumber)
        {
            switch (operation)
            {
                case "-":
                    return subResult - int.Parse(strNumber);
                case "*":
                    return subResult * int.Parse(strNumber);
                case "/":
                    return subResult / int.Parse(strNumber);
                case "%":
                    return subResult % int.Parse(strNumber);
                default:
                    return (strNumber == "") ? subResult : subResult + int.Parse(strNumber);
            }
        }
    }
}