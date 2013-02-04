using System;
using System.Diagnostics;
using System.Collections.Generic;

class Program {
    static List<double> results = new List<double>();
    static Dictionary<string, double> fields = new Dictionary<string, double>();
    //static List<UserField> userFields = new List<UserField>();

    static void PrintCredentials() {
        Console.WriteLine("Arithmetic Calculator by Lev Dubinets");
        Console.WriteLine("Version 1.7");
    }

    #region Help Management
    static void PrintHelp() {
        Console.WriteLine("------------Help------------");
        Console.WriteLine("+  | Plus       (3+5)");
        Console.WriteLine("-  | Minus      (3-5)");
        Console.WriteLine("*  | Multiply   (3*5)");
        Console.WriteLine("/  | Divide     (3/5)");
        Console.WriteLine("!  | Factorial  (3!)");
        Console.WriteLine("^  | Power      (3^5)");
        Console.WriteLine(":= | Assignment (a := 5)");
        Console.WriteLine("Other info:");
        Console.WriteLine("- Everything is case insensitive");
        Console.WriteLine("- Enter in \"clear\" or \"clr\" to clear the console.");
        Console.WriteLine("- Enter a blank line or \"exit\" to exit the program.");
        Console.WriteLine("- Enter \"P:\" in start to rewrite the expression using prefix/polish notation.");
        Console.WriteLine("- Enter \"():\" in start to rewrite the expression with bracket normalization");
        Console.WriteLine("  example: \"(): 2 + 3 * 7 ^ 4\" will result in \"(2+(3*(7^4))) = 7205\"");
        Console.WriteLine("- Enter \"F:\" in start to calculate the expression, and then factor it.");
        Console.WriteLine("- Enter \"functions\" or \"?f\" to access the list of supporred functions.");
        Console.WriteLine("- The default fields are \"e\" and \"pi\", but user fields can be created");
        Console.WriteLine("  using the assignment operator");
        Console.WriteLine("- Multiplication is implicit");
        Console.WriteLine("- Previous results are saved, and you can use them in expressions. The");
        Console.WriteLine("  first result is $1, the second is $2, and so on. \"$8 + 1\" is a valid");
        Console.WriteLine("  expression as long as there have been at least 8 results.");
    }

    static void PrintCHelp() {
        Console.WriteLine("------------Help------------");
        Console.WriteLine("Operators:");
        Console.WriteLine("  +, -, *, /, !, ^, :=, +=, -=, *=, /=");
        Console.WriteLine("Input Modifiers:");
        Console.WriteLine("  p:, ():, f:");
        Console.WriteLine("Commands:");
        Console.WriteLine("  clr/clear, exit/blank line");
        Console.WriteLine("Fields:");
        Console.WriteLine("  e, pi, user-defined by using assignment operators");
        Console.WriteLine("Help:");
        Console.WriteLine("  help/?, functions/?f");
        Console.WriteLine("Info:");
        Console.WriteLine("- Case insensitive, implicit multiplication, previous results are saved as $n");
    }

    static void PrintCFunctions() {
        Console.WriteLine("----------Functions---------");
        //Console.WriteLine("abs(1), acos(1), asin(1), atan(1), ceiling(1), cosh(1), cos(1), exp(1), floor(1), gcf(2), rem(2), log(2), max(un
    }

    static void PrintFunctions() {
        Console.WriteLine("----------Functions---------");
        Console.WriteLine("Parameters are separated by commas.");
        Console.WriteLine("Example: max(1,4,2,5,2,7) will return 7.");
        Console.WriteLine("Func Name(# of parameters) | Purpose");
        Console.WriteLine("abs(1)     | Returns absolute value.");
        Console.WriteLine("acos(1)    | Returns arc cosine of the specified angle in radians.");
        Console.WriteLine("asin(1)    | Returns arc sine of the specified angle in radians.");
        Console.WriteLine("atan(1)    | Returns arc tangent of the specified angle in radians.");
        Console.WriteLine("ceiling(1) | Returns the specified number rounded up.");
        Console.WriteLine("cosh(1)    | Returns the hyperbolic cosine of the specified angle in radians.");
        Console.WriteLine("cos(1)     | Returns the cosine of the specified angle in radians.");
        Console.WriteLine("exp(1)     | Returns e to the power of the specified number.");
        Console.WriteLine("floor(1)   | Returns the specified number rounded down.");
        Console.WriteLine("gcf(2)     | Returns the greatest common factor of the two specified numbers.");
        Console.WriteLine("rem(2)     | Returns the remainder of number1 / number 2");
        Console.WriteLine("log(2)     | Returns the logarithm of number in the base of number 2.");
        Console.WriteLine("max(unlmtd)| Returns the maximum of all the specified numbers.");
        Console.WriteLine("min(unlmtd)| Returns the minimum of all the specified numbers.");
        Console.WriteLine("rand(0)    | Returns a random number between 0.0 and 1.0.");
        Console.WriteLine("sin(1)     | Returns the sine of the specified angle in radians.");
        Console.WriteLine("sinh(1)    | Returns the hyperbolic sine of the specified angle in radians.");
        Console.WriteLine("sqrt(1)    | Returns the square root of the specified number.");
        Console.WriteLine("tan(1)     | Returns the tangent of the specified angle in radians.");
        Console.WriteLine("tanh(1)    | Returns the hyperbolic tangent of the specified angle in radians.");
        Console.WriteLine("todeg(1)   | Converts the specified amount in radians to degrees.");
        Console.WriteLine("torad(1)   | Converts the specified amount in degrees to radians.");
    }
    #endregion

    static double CalculateExpression(string expr) {
        Parser<double> calc = new Parser<double>(new CalcBuilder(results, ref fields));
        double result = calc.Parse(expr);
        Console.WriteLine("${0} = {1}", results.Count + 1, result);
        results.Add(result);
        return result;
    }

    static double NormCalc(string expr) {
        try {
            return CalculateExpression(expr);
        } catch (CalcException ce) {
            PrintException(ce, 2, expr.Length);
            return double.NaN;
        }
    }

    private static void PrintException(CalcException ce, int offset, int exprLength) {
        Console.Write(new string(' ', offset));
        Console.Write(new String('-', ce.ErrorPosition));
        Console.Write('^');
        int tail = exprLength - (ce.ErrorPosition + 1);
        if (tail > 0) {
            //Tail can be negative because we consider End Of File behind the string 
            Console.Write(new String('-', tail));
        }
        Console.WriteLine();
        Exception e = ce;
        for (int depth = 0; e != null; depth++, e = e.InnerException) {
            Console.Write(new string(' ', depth));
            Console.WriteLine(e.Message);
        }
    }

    static void TestCalc(string expr, double expectedResult) {
        double result = NormCalc(expr);
        if (result != expectedResult) {
            Console.WriteLine("You are stupid");
            Console.WriteLine("The expected result was: {0}", expectedResult);
        }
    }

    private static void CommandCalc(string expr) {
        try {
            Parser<string> command = new Parser<string>(new CommandBuilder());
            Console.WriteLine(command.Parse(expr));
        } catch (CalcException ce) {
            PrintException(ce, 3, expr.Length);
        }
    }

    private static void PolishCalc(string expr) {
        try {
            double result = CalculateExpression(expr);

            Parser<string> polish = new Parser<string>(new PolishBuilder());
            Console.Write(polish.Parse(expr));
            Console.Write(" = ");

            Console.WriteLine(result);
            return;
        } catch (CalcException ce) {
            PrintException(ce, 4, expr.Length);
        }
    }

    private static void BracketsCalc(string expr) {
        try {
            //double result = CalculateExpression(expr);

            Parser<string> brackets = new Parser<string>(new BracketsBuilder());
            Console.Write(brackets.Parse(expr));
            Console.Write(" = ");

            //Console.WriteLine(result);
            Console.WriteLine(CalculateExpression(expr));
            return;
        } catch (CalcException ce) {
            PrintException(ce, 5, expr.Length);
        }
    }

    private static void FactorCalc(string expr) {
        try {
            double result = CalculateExpression(expr);
            Factorization.Factor(result);
        } catch (CalcException ce) {
            //Console.WriteLine(e);
            PrintException(ce, 4, expr.Length);
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }
    }

    private static void InitializeFields() {
        fields.Add("e", Math.E);
        fields.Add("pi", Math.PI); 
    }

    static void Main() {
        InitializeFields();
        PrintCredentials();
        Console.WriteLine("Enter 'help' or '?' to access help");
        while (true) {
            Console.Write("> ");
            string line = Console.ReadLine().ToLower();
            if (line == "" || line == "exit") {
                return;
            } else if (line == "help" || line == "?") {
                PrintCHelp();
            } else if (line == "functions" || line == "?f") {
                PrintFunctions();
            } else if (line == "clear" || line == "clr") {
                Console.Clear();
                PrintCredentials();
                Console.WriteLine("Enter 'help' or '?' to access help");
                results.Clear();
            } else if (line == "test") {
                //when writing test cases, make sure that all the chars are lowercase.
                TestCalc("   (2 *   (7))+6", (2 * (7)) + 6);
                TestCalc("sin(9)", Math.Sin(9));
                TestCalc("max(3)",3);
                TestCalc("max(9,23,-0,0, -1,-214,50)", 50);
                TestCalc("min(4,3,2,-9)", -9);
                TestCalc("log(8,9)", Math.Log(8,9));
            } else if (line.StartsWith(":")) {
                CommandCalc(line.Substring(1));
            } else if (line.StartsWith("p:")) {
                PolishCalc(line.Substring(2));
            } else if (line.StartsWith("():")) {
                BracketsCalc(line.Substring(3));
            } else if (line.StartsWith("f:")) {
                FactorCalc(line.Substring(2));
            } else {
                NormCalc(line);
            }
        }
    }
}