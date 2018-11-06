using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommandLineCalculator
{
    [TestClass]
    public class CalculatorTests
    {
        [TestMethod]
        [DataRow("", "")]
        [DataRow("", " ")]
        [DataRow("1", "1")]
        [DataRow("123", "123")]
        [DataRow("2", "1+1")]
        [DataRow("444", "123+321")]
        [DataRow("15", "1+2 + 3 +4+5")]
        [DataRow("5", "1+2 + 3 +4-5")]
        [DataRow("50", "1+2 + 3 +4*5")]
        [DataRow("2", "1+2 + 3 +4/5")]
        [DataRow("2", "1+2+2 + 3 +4/5")]
        [DataRow("3", "1+2+3 + 3 +4%5")]
        [DataRow("1", "1+2+5 % 3 +4%5")]
        public void CalculationsTests(string expectedResult, string equation)
        {
            var calc = new Calculator();
            var result = calc.Calculate(equation);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow("3", "1+5-(6/2)")]
        [DataRow("6", "10+5-((6/2)*(6/2))")]
        [DataRow("36", "10+5-(6/2)*(6/2)")]
        public void CalulationsWithBracketsTests(string expectedResult, string equation)
        {
            var calc = new Calculator();
            var result = calc.Calculate(equation);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
