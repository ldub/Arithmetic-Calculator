using System;
using System.Text;
using System.Collections.Generic;

class BracketsBuilder : IBuilder<string> {
    public string Number(string number) {
        return number;
    }

    public string Variable(string name) {
        return '$' + name;
    }

    public string Assignment(LexType op, string name, string value) {
        return '(' + name + Scanner.OpNames[(int)op] + value + ')';
    }

    public string Field(string name) {
        return name;
    }

    public string String(string str) {
        throw new Exception("Strings are not (yet) allowed in brackets");
    }

    public string Function(string name, List<string> args) {
        int limit = 0;
        StringBuilder sb = new StringBuilder();
        sb.Append(name);
        sb.Append('(');
        while (limit < args.Count) {
            if (limit != 0) {
                sb.Append(",");
            }
            sb.Append(args[limit]);
            limit++;
        }
        sb.Append(')');
        return sb.ToString();
    }

    public string Binary(LexType op, string left, string right) {
        return '(' + left + Scanner.OpNames[(int)op] + right + ')';
    }

    public string Unary(LexType op, string expr) {
        switch (op) {
        case LexType.Minus:
        case LexType.Plus:
            return '(' + Scanner.OpNames[(int)op] + expr + ')';
        case LexType.Exclamation:
            return '(' + expr + '!' + ')';
        default:
            throw new Exception("Internal Error. Invalid unary operator.");
        }
    }

    public string End(string expr) {
        return expr;
    }
}
