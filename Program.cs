using System;
using System.Text;

namespace Calculator
{

    public class Program
    {
        public static void Main(String[] args)
        {
            // Function call.
            Test("a()", "a()");
            Test("a(b)", "a(b)");
            Test("a(b, c)", "a(b, c)");
            Test("a(b)(c)", "a(b)(c)");
            Test("a(b) + c(d)", "(a(b) + c(d))");
            Test("a(b ? c : d, e + f)", "a((b ? c : d), (e + f))");

            // Unary precedence.
            Test("~!-+a", "(~(!(-(+a))))");
            Test("a!!!", "(((a!)!)!)");

            // Unary and binary predecence.
            Test("-a * b", "((-a) * b)");
            Test("!a + b", "((!a) + b)");
            Test("~a ^ b", "((~a) ^ b)");
            Test("-a!", "(-(a!))");
            Test("!a!", "(!(a!))");

            // Binary precedence.
            Test("a = b + c * d ^ e - f / g", "(a = ((b + (c * (d ^ e))) - (f / g)))");

            // Binary associativity.
            Test("a = b = c", "(a = (b = c))");
            Test("a + b - c", "((a + b) - c)");
            Test("a * b / c", "((a * b) / c)");
            Test("a ^ b ^ c", "(a ^ (b ^ c))");

            // Conditional operator.
            Test("a ? b : c ? d : e", "(a ? b : (c ? d : e))");
            Test("a ? b ? c : d : e", "(a ? (b ? c : d) : e)");
            Test("a + b ? c * d : e / f", "((a + b) ? (c * d) : (e / f))");

            // Grouping.
            Test("a + (b + c) + d", "((a + (b + c)) + d)");
            Test("a ^ (b + c)", "(a ^ (b + c))");
            Test("(!a)!", "((!a)!)");

            // Show the results.
            if (sFailed == 0)
            {
                Console.WriteLine("Passed all " + sPassed + " tests.");
            }
            else
            {
                Console.WriteLine("----");
                Console.WriteLine("Failed " + sFailed + " out of " + (sFailed + sPassed) + " tests.");
            }
        }

        /**
         * Parses the given chunk of code and verifies that it matches the expected
         * pretty-printed result.
         */
        public static void Test(String source, String expected)
        {
            Lexer lexer = new Lexer(source);
            Parser parser = new BantamParser(lexer);

            try
            {
                IExpression result = parser.ParseExpression();
                string actual = result.ToString();

                if (expected == actual)
                {
                    sPassed++;
                }
                else
                {
                    sFailed++;
                    Console.WriteLine("[FAIL] Expected: " + expected);
                    Console.WriteLine("         Actual: " + actual);
                }
            }
            catch (ParseException ex)
            {
                sFailed++;
                Console.WriteLine("[FAIL] Expected: " + expected);
                Console.WriteLine("          Error: " + ex.Message);
            }
        }

        private static int sPassed = 0;
        private static int sFailed = 0;
    }
}
