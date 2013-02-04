using System;

interface IBuilder<T> {
    T Number(string literal);
    T Binary(LexType op, T left, T right);
    T Unary(LexType op, T expr);
    //bool IsField(string name);
    T Field(string name);
    T Assignment(LexType op, string name, T value);
    T String(string str);
    T Function(string name, System.Collections.Generic.List<T> args);
    T Variable(string name);
    T End(T expr);
}
