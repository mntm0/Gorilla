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
            var enviroment = new Enviroment();
            return evaluator.Eval(root, enviroment);
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
                Assert.Fail($"Object が Null ではありません。{obj?.GetType()}");
            }
        }

        [TestMethod]
        public void TestEvalReturnStatement()
        {
            var tests = new(string, int)[]
            {
                ("return 10;", 10),
                ("return 10; 1234;", 10),
                ("2*3; return 10; 1234;", 10),
                ("return 100/10", 10),
                (@"if (true) {
                       if (true) {
                           return 10;
                       }
                       0;
                   }", 10),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = this._TestEval(input);
                this._TestIntegerObject(evaluated, expected);
            }
        }

        [TestMethod]
        public void TestErrorHandling()
        {
            var tests = new(string, string)[]
            {
                ("5 + true;", "型のミスマッチ: INTEGER + BOOLEAN"),
                ("5 + true; 5;", "型のミスマッチ: INTEGER + BOOLEAN"),
                ("-true", "未知の演算子: -BOOLEAN"),
                ("true + false", "未知の演算子: BOOLEAN + BOOLEAN"),
                ("if (true) { true * false; }", "未知の演算子: BOOLEAN * BOOLEAN"),
                (@"if (true) {
                       if (true) {
                           return false / false;
                       }
                       0;
                   }", "未知の演算子: BOOLEAN / BOOLEAN"),
                ("-true + 100", "未知の演算子: -BOOLEAN"),
                ("foo", "識別子が見つかりません。: foo"),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = this._TestEval(input);
                var error = evaluated as Error;
                if (error == null)
                {
                    Assert.Fail($"エラーオブジェクトではありません。({evaluated?.GetType()})");
                }

                Assert.AreEqual(error.Message, expected);
            }
        }

        [TestMethod]
        public void TestEvalLetStatement()
        {
            var tests = new(string, int)[]
            {
                ("let a = 1; a;", 1),
                ("let a = 1 + 2 * 3; a;", 7),
                ("let a = 1; let b = a; b;", 1),
                ("let a = 1; let b = 2; let c = a + b; c;", 3),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = this._TestEval(input);
                this._TestIntegerObject(evaluated, expected);
            }
        }

        [TestMethod]
        public void TestEvalFunctionObject()
        {
            var input = "fn(x) { x + 2; }";
            var evaluated = this._TestEval(input);

            var fn = evaluated as FunctionObject;
            if (fn == null)
            {
                Assert.Fail($"オブジェクトが関数ではありません。({fn?.GetType()})");
            }

            Assert.AreEqual(fn.Parameters.Count, 1);
            Assert.AreEqual(fn.Parameters[0].ToCode(), "x");
            Assert.AreEqual(fn.Body.ToCode(), "{(x + 2) }");
        }

        [TestMethod]
        public void TestFunctionApplication()
        {
            var tests = new(string, int)[]
            {
                ("let identity = fn(x) { x }; identity(10);", 10),
                ("let identity = fn(x) { return x; }; identity(10);", 10),
                ("let double = fn(x) { x * 2; }; double(10);", 20),
                ("let add = fn(x, y) { x + y; }; add(10, 20);", 30),
                ("let add = fn(x, y) { x + y; }; add(add(10, 20), 30 + 40);", 100),
                ("fn(x) { x; }(10);", 10),
                (@"fn(x) { x; }(10);", 10),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = this._TestEval(input);
                this._TestIntegerObject(evaluated, expected);
            }
        }

        [TestMethod]
        public void TestStringLiteral()
        {
            var tests = new(string, string)[]
            {
                ("\"foo\";", "foo"),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = this._TestEval(input);
                this._TestStringObject(evaluated, expected);
            }
        }

        private void _TestStringObject(IObject obj, string expected)
        {
            var result = obj as StringObject;
            if (result == null)
            {
                Assert.Fail("object が String ではありません。");
            }

            Assert.AreEqual(expected, result.Value);
        }
    }
}
