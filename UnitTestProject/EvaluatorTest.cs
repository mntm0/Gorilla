using Gorilla.Evaluating;
using Gorilla.Lexing;
using Gorilla.Objects;
using Gorilla.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class EvaluatorTest
    {
        [TestMethod]
        public void TestEvalIntegerExpression()
        {
            var tests = new (string, int)[]
            {
                ("1", 1),
                ("12", 12),
                ("-1", -1),
                ("-12", -12),
                ("1 + 2 - 3", 0),
                ("1 + 2 * 3", 7),
                ("3 * 4 / 2 + 10 - 8", 8),
                ("(1 + 2) * 3 - -1", 10),
                ("-1 * -1", 1),
                ("-10 + -1 * 2", -12),
                ("(10 + 20) / (10 - 0)", 3),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = this._TestEval(input);
                this._TestIntegerObject(evaluated, expected);
            }
        }

        private IObject _TestEval(string input)
        {
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            var evaluator = new Evaluator();
            return evaluator.Eval(root);
        }

        private void _TestIntegerObject(IObject obj, int expected)
        {
            var result = obj as IntegerObject;
            if (result == null)
            {
                Assert.Fail("object が Integer ではありません。");
            }

            Assert.AreEqual(expected, result.Value);
        }

        [TestMethod]
        public void TestEvalBooleanExpression()
        {
            var tests = new (string, bool)[]
            {
                ("true;", true),
                ("false", false),
                ("1 < 2", true),
                ("1 > 2", false),
                ("1 == 2", false),
                ("1 != 2", true),
                ("1 > 2", false),
                ("1 < 2", true),
                ("1 == 1", true),
                ("2 != 2", false),
                ("true == true", true),
                ("true != true", false),
                ("true == false", false),
                ("true != false", true),
                ("(1 > 2) == true", false),
                ("(1 > 2) != false", false),
                ("(1 < 2) == false", false),
                ("(1 > 2) != true", true),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = this._TestEval(input);
                this._TestBooleanObject(evaluated, expected);
            }
        }

        private void _TestBooleanObject(IObject obj, bool expected)
        {
            var result = obj as BooleanObject;
            if (result == null)
            {
                Assert.Fail("object が Boolean ではありません。");
            }

            Assert.AreEqual(expected, result.Value);
        }

        [TestMethod]
        public void TestEvalBangOperator()
        {
            var tests = new (string, bool)[]
            {
                ("!true", false),
                ("!false", true),
                ("!5", false),
                ("!!true", true),
                ("!!!true", false),
                ("!!5", true),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = this._TestEval(input);
                this._TestBooleanObject(evaluated, expected);
            }
        }

        [TestMethod]
        public void TestEvalIfExpression()
        {
            var tests = new (string, int?)[]
            {
                ("if (true) { 1 }", 1),
                ("if (false) { 1 }", null),
                ("if (true) { 1 } else { 2 }", 1),
                ("if (false) { 1 } else { 2 }", 2),
                ("if (5) { 1 } else { 2 }", 1),
                ("if (!5) { 1 } else { 2 }", 2),
                ("if (1 < 2) { 1 } else { 2 }", 1),
                ("if (1 > 2) { 1 } else { 2 }", 2),
                ("if (1 > 2) { 1 }", null),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = this._TestEval(input);
                if (expected.HasValue)
                {
                    this._TestIntegerObject(evaluated, expected.Value);
                }
                else
                {
                    this._TestNullObject(evaluated);
                }
            }
        }

        private void _TestNullObject(object obj)
        {
            var nullObject = obj as NullObject;
            if (nullObject == null)
            {
                Assert.Fail($"Object が Null ではありません。{obj.GetType()}");
            }
        }
    }
}
