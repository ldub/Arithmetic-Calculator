using System;
using System.Text;
using System.Collections.Generic;

class PolishBuilder : IBuilder<string> {
    public string Number(string number) {
        return number;
    }

    public string Variable(string name) {
        return '$' + name;
    }

    public string Field(string name) {
        return name;
    }

    public string Assignment(LexType op, string name, string value) {
        return Scanner.OpNames[(int)op] + ' ' + name + ' ' + value;
    }

    public string String(string str) {
        throw new Exception("Strings are not (yet) allowed in polish");
    }

    public string Function(string name, List<string> args) {
        int limit = 0;
        StringBuilder sb = new StringBuilder();
        sb.Append(name);
        while (limit < args.Count) {
            sb.Append(args[limit]);
            sb.Append(' ');
            limit++;
        }
        return sb.ToString();
    }

    public string Binary(LexType op, string left, string right) {
        return Scanner.OpNames[(int)op] + ' ' + left + ' ' + right;
    }

    public string Unary(LexType op, string expr) {
        switch (op) {
        case LexType.Minus:
        case LexType.Plus:
            return Scanner.OpNames[(int)op] + ' ' + expr;
        case LexType.Exclamation:
            return expr + '!';
        default:
            throw new Exception("Internal Error. Invalid unary operator.");
        }
    }

    public string End(string expr) {
        return expr;
    }
}
