using System;
using System.Collections.Generic;

class CalcBuilder : IBuilder<double> {
    private List<double> results;
    private Dictionary<string, double> fields;

    public CalcBuilder(List<double> results, ref Dictionary<string, double> fields) {
        this.results = results;
        this.fields = fields;
    }

    public double Number(string number) {
        return double.Parse(number);
    }

    public double Assignment(LexType op, string name, double value) {
        switch (op) {
        case LexType.EqualSign:
            fields[name] = value;
            return value;
        case LexType.MinusEqual:
            if (fieldDefined(name)) {
                fields[name] -= value;
            } else {
                throw new Exception("Cannot do an operation on an uninitialized field");
            }
            return fields[name];
        case LexType.PlusEqual:
            if (fieldDefined(name)) {
                fields[name] += value;
            } else {
                throw new Exception("Cannot do an operation on an uninitialized field");
            }
            return fields[name];
        case LexType.SlashEqual:
            if (fieldDefined(name)) {
                fields[name] /= value;
            } else {
                throw new Exception("Cannot do an operation on an uninitialized field");
            }
            return fields[name];
        case LexType.StarEqual:
            if (fieldDefined(name)) {
                fields[name] *= value;
            } else {
                throw new Exception("Cannot do an operation on an uninitialized field");
            }
            return fields[name];
        default:
            throw new Exception("Invalid assignment operator");
        }
    }

    public double String(string str) {
        throw new Exception("Strings are not (yet) allowed in calc");
    }

    public double Field(string name) {
        double value = 0;
        if (fields.TryGetValue(name, out value)) {
            return value;
        } else {
            throw new Exception("Field Name");
        }
    }

    /// <summary>
    /// Determines whether or not a field is already defined.
    /// </summary>
    private bool fieldDefined(string name) {
        if (fields.ContainsKey(name)) {
            return true;
        }
        return false;
    }

    public double Variable(string name) {
        int resultNumber = int.Parse(name) - 1;
        if (0 <= resultNumber && resultNumber < results.Count) {
            return results[resultNumber];
        }
        throw new Exception("Inexisting previous result.");
        //return '$' + name;
    }

    public double Function(string name, List<double> args) {
        switch (name) {
        case "abs":
            Functions.CheckArgs(args, 1, "Absolute value");
            return Math.Abs(args[0]);
        case "acos":
            Functions.CheckArgs(args, 1, "Arc cosine");
            return Math.Acos(args[0]);
        case "asin":
            Functions.CheckArgs(args, 1, "Arc sine");
            return Math.Asin(args[0]);
        case "atan":
            Functions.CheckArgs(args, 1, "Arc tangent");
            return Math.Atan(args[0]);
        case "avrg":
            return Functions.Average(args);
        case "ceiling":
            Functions.CheckArgs(args, 1, "Ceiling");
            return Math.Ceiling(args[0]);
        case "cos":
            Functions.CheckArgs(args, 1, "Cosine");
            return Math.Cos(args[0]);
        case "cosh":
            Functions.CheckArgs(args, 1, "Hyperbolic cosine");
            return Math.Cosh(args[0]);
        case "exp":
            Functions.CheckArgs(args, 1, "Exp");
            return Math.Exp(args[0]);
        case "floor":
            Functions.CheckArgs(args, 1, "Floor");
            return Math.Floor(args[0]);
        case "gcf":
            Functions.CheckArgs(args, 2, "Greatest common factor");
            return Functions.GCF(args[0], args[1], true);
        case "rem":
            Functions.CheckArgs(args, 2, "Division with remainder");
            return Math.IEEERemainder(args[0], args[1]);
        case "lcm":
            Functions.CheckArgs(args, 2, "Least common multiple");
            return Functions.LCM(args[0], args[1]);
        case "log":
            Functions.CheckArgs(args, 2, "Logarithm");
            return Math.Log(args[0], args[1]);
        case "max":
            return Functions.Max(args);
        case "min":
            return Functions.Min(args);
        case "rand":
            Functions.CheckArgs(args, 0, "Random number");
            return new Random().NextDouble();
        case "sin":
            Functions.CheckArgs(args, 1, "Sine");
            return Math.Sin(args[0]);
        case "sinh":
            Functions.CheckArgs(args, 1, "Hyperbolic sine");
            return Math.Sinh(args[0]);
        case "sqrt":
            Functions.CheckArgs(args, 1, "Square root");
            return Math.Sqrt(args[0]);
        case "tan":
            Functions.CheckArgs(args, 1, "Tangent");
            return Math.Tan(args[0]);
        case "tanh":
            Functions.CheckArgs(args, 1, "Hyperbolic tangent");
            return Math.Tanh(args[0]);
        case "todeg":
            Functions.CheckArgs(args, 1, "Radian to degree");
            return Functions.ToDegree(args[0]);
        case "torad":
            Functions.CheckArgs(args, 1, "Degree to radian");
            return Functions.ToRadian(args[0]);
        default:
            throw new Exception("Function name");
        //throw new Exception("Unrecognized function name");
        }
    }


    public double Binary(LexType op, double left, double right) {
        switch (op) {
        case LexType.Minus:
            return left - right;
        case LexType.Plus:
            return left + right;
        case LexType.Star:
            return left * right;
        case LexType.Slash:
            return left / right;
        case LexType.Power:
            return Math.Pow(left, right);
        default:
            throw new Exception("Internal Error. Invalid binary operator.");
        }
    }

    public double Unary(LexType op, double expr) {
        switch (op) {
        case LexType.Minus:
            return -expr;
        case LexType.Plus:
            return expr;
        case LexType.Exclamation:
            return Functions.Factorial(expr);
        default:
            throw new Exception("Internal Error. Invalid unary operator.");
        }
    }

    public double End(double expr) {
        return expr;
    }
}
