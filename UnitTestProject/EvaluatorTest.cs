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
            var tests = new(string, int)[]
            {
                ("1", 1),
                ("12", 12),
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
            var tests = new(string, bool)[]
            {
                ("true;", true),
                ("false", false),
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
    }
}
