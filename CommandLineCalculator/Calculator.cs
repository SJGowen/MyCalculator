using System.Collections.Generic;
using System.Linq;

namespace CommandLineCalculator
{
    public class Calculator
    {
        private readonly string _invalidExpression = "Invalid expression.";
        private bool _floatingPointExpression;

        public string Calculate(string equation)
        {
            equation = RemoveSpaces(equation);
            _floatingPointExpression = equation.Contains('.');
            if (!ParenthesisIsBalanced(equation)) return _invalidExpression;
            equation = EvaluateParenthesisedPiecesOfEquation(equation);
            return WeightedCalculate(equation);
        }

        private string RemoveSpaces(string equation)
        {
            return equation.Replace(" ", string.Empty);
        }

        private bool ParenthesisIsBalanced(string equation)
        {
            return equation.Count(x => x == '(') == equation.Count(x => x == ')');
        }

        private string EvaluateParenthesisedPiecesOfEquation(string equation)
        {
            while (equation.Contains("("))
            {
                var length = 0;
                var startIndex = 0;
                var equationIndex = 0;
                foreach (var character in equation)
                {
                    if (character == '(')
                    {
                        startIndex = equationIndex + 1;
                        length = 0;
                    }
                    else if (character == ')' && length == 0)
                    {
                        length = equationIndex - startIndex;
                    }

                    equationIndex += 1;
                }

                if (length <= 0) continue;
                var subEquation = equation.Substring(startIndex, length);
                var result = WeightedCalculate(subEquation);
                equation = equation.Replace("(" + subEquation + ")", result);
            }

            return equation;
        }

        private string WeightedCalculate(string equation)
        {
            if (equation.Length == 0) return string.Empty;
            if (equation.Contains(_invalidExpression)) return _invalidExpression;

            var bits = BreakUpEquation(equation);
            ApplyNegatives(bits);
            if (bits.Count % 2 == 0) return _invalidExpression;
            if (bits.Count <= 1) return bits.Count == 1 ? bits[0] : _invalidExpression;
            CondenseListByDoing(bits, "*");
            CondenseListByDoing(bits, "/");
            CondenseListByDoing(bits, "%");
            CondenseListByDoing(bits, "+");
            CondenseListByDoing(bits, "-");
            return bits.Count == 1 ? bits[0] : _invalidExpression;
        }

        private void CondenseListByDoing(List<string> bits, string mathsOperator)
        {
            while (bits.IndexOf(mathsOperator) > 0)
            {
                var indexOfOperator = bits.IndexOf(mathsOperator);
                if (_floatingPointExpression)
                {
                    ConsolidateAsFloat(bits, mathsOperator, indexOfOperator);
                }
                else
                {
                    ConsolidateAsInt(bits, mathsOperator, indexOfOperator);
                }
            }
        }

        private void ConsolidateAsInt(IList<string> bits, string mathsOperator, int indexOfOperator)
        {
            if (!int.TryParse(bits[indexOfOperator - 1], out var integer1) ||
                (!int.TryParse(bits[indexOfOperator + 1], out var integer2))) return;
            switch (mathsOperator)
            {
                case "*":
                    bits[indexOfOperator] = (integer1 * integer2).ToString();
                    break;
                case "/":
                    bits[indexOfOperator] = (integer1 / integer2).ToString();
                    break;
                case "%":
                    bits[indexOfOperator] = (integer1 % integer2).ToString();
                    break;
                case "+":
                    bits[indexOfOperator] = (integer1 + integer2).ToString();
                    break;
                case "-":
                    bits[indexOfOperator] = (integer1 - integer2).ToString();
                    break;
                default:
                    bits[indexOfOperator] = _invalidExpression;
                    break;
            }

            bits.RemoveAt(indexOfOperator + 1);
            bits.RemoveAt(indexOfOperator - 1);
        }

        private void ConsolidateAsFloat(IList<string> bits, string mathsOperator, int indexOfOperator)
        {
            if (!float.TryParse(bits[indexOfOperator - 1], out var float1) ||
                (!float.TryParse(bits[indexOfOperator + 1], out var float2))) return;
            switch (mathsOperator)
            {
                case "*":
                    bits[indexOfOperator] = (float1 * float2).ToString("F");
                    break;
                case "/":
                    bits[indexOfOperator] = (float1 / float2).ToString("F");
                    break;
                case "%":
                    bits[indexOfOperator] = (float1 % float2).ToString("F");
                    break;
                case "+":
                    bits[indexOfOperator] = (float1 + float2).ToString("F");
                    break;
                case "-":
                    bits[indexOfOperator] = (float1 - float2).ToString("F");
                    break;
                default:
                    bits[indexOfOperator] = _invalidExpression;
                    break;
            }

            bits.RemoveAt(indexOfOperator + 1);
            bits.RemoveAt(indexOfOperator - 1);
        }

        private void ApplyNegatives(IList<string> bits)
        {
            var bitIndex = 0;
            if (bits.IndexOf("-") == -1) return;
            while (bitIndex < bits.Count)
            {
                if (bits[bitIndex] == "-")
                {
                    if (_floatingPointExpression)
                    {
                        if (bitIndex == 0 || !float.TryParse(bits[bitIndex - 1], out var _))
                        {
                            if (float.TryParse(bits[bitIndex + 1], out var float1))
                            {
                                bits[bitIndex + 1] = (float1 * -1).ToString("F");
                                bits.RemoveAt(bitIndex);
                            }
                        }
                    }
                    else
                    {
                        if (bitIndex == 0 || !int.TryParse(bits[bitIndex - 1], out var _))
                        {

                            if (int.TryParse(bits[bitIndex + 1], out var integer1))
                            {
                                bits[bitIndex + 1] = (integer1 * -1).ToString();
                                bits.RemoveAt(bitIndex);
                            }
                        }
                    }
                }

             bitIndex++;
                
            }
        }

        private List<string> BreakUpEquation(string equation)
        {
            var bit = string.Empty;
            var bits = new List<string>();
            foreach (var character in equation)
            {
                if (int.TryParse(character.ToString(), out var _) || character == '.')
                {
                    bit += character;
                }
                else
                {
                    if (bit != string.Empty)
                    {
                        bits.Add(bit);
                        bit = string.Empty;
                    }

                    bits.Add(character.ToString());
                }
            }

            if (bit != string.Empty) bits.Add(bit);

            return bits;
        }

        //private string LeftToRightCalculate(string equation)
        //{
        //    if (equation.Length == 0) return string.Empty;
        //    var result = 0;
        //    try
        //    {
        //        var operation = string.Empty;
        //        var number = string.Empty;
        //        var equationIndex = 0;
        //        var operationIndex = -1;
        //        foreach (var character in equation)
        //        {
        //            if (CharacterIsNumber(number, equationIndex, operationIndex, character))
        //            {
        //                number += character;
        //            }
        //            else
        //            {
        //                if (!string.IsNullOrEmpty(number)) result = PerformOperation(result, operation, number);
        //                operation = character.ToString();
        //                if (ConsecutiveOperators(equationIndex, operationIndex)) return $"Invalid operation ({operation}).";
        //                operationIndex = equationIndex;
        //                number = string.Empty;
        //            }

        //            equationIndex += 1;
        //        }

        //        result = PerformOperation(result, operation, number);
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return ex.Message;
        //    }
        //    catch (Exception)
        //    {
        //        return invalidExpression;
        //    }
        //    return $"{result}";
        //}

        //private bool CharacterIsNumber(string number, int equationIndex, int operationIndex, char character)
        //{
        //    return char.IsNumber(character) || number == string.Empty && character == '-' && equationIndex - 1 == operationIndex;
        //}

        //private bool ConsecutiveOperators(int equationIndex, int operationIndex)
        //{
        //    return equationIndex - 1 == operationIndex;
        //}

        //private int PerformOperation(int subResult, string operation, string stringNumber)
        //{
        //    if (string.IsNullOrEmpty(operation)) return int.Parse(stringNumber);
        //    switch (operation)
        //    {
        //        case "+":
        //            return subResult + int.Parse(stringNumber);
        //        case "-":
        //            return subResult - int.Parse(stringNumber);
        //        case "*":
        //            return subResult * int.Parse(stringNumber);
        //        case "/":
        //            return subResult / int.Parse(stringNumber);
        //        case "%":
        //            return subResult % int.Parse(stringNumber);
        //        default:
        //            throw new InvalidOperationException($"Invalid operation ({operation}).");
        //    }
        //}
    }
}