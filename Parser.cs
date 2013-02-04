using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

class Parser<T> {
    private static bool[] isOperand = {
       /* BeginFile   */ false,
       /* EOF         */ false,
       /* Number      */ true,
       /* Plus        */ false,
       /* Minus       */ false,
       /* Star        */ false,
       /* Slash       */ false,
       /* Power       */ false,
       /* Exclamation */ false,
       /* OpenBrace   */ true,
       /* CloseBrace  */ false,
       /* DoubleQuote */ false,
       /* Comma       */ false,
       /* Identifier  */ true,
       /* EqualSign   */ false,
       /* MinusSign   */ false,
       /* PlusSign    */ false,
       /* SlashSign   */ false,
       /* StarSign    */ false,
       /* Variable    */ true,
    };
    private Scanner scanner;
    IBuilder<T> builder;
    //private int possibleErrorPosition = 0;

    public Parser(IBuilder<T> builder) {
        this.builder = builder;
    }

    // Expression ::= Add
    public T Parse(string expression) {
        this.scanner = new Scanner(expression);
        scanner.MoveNext();
        try {
            T result = builder.End(ParseAssignment());
            Debug.Assert(scanner.CurrentLexema.LexType == LexType.EOF, "With this grammar and implicit multiplication, this is impossible");
            return result;
        } catch (CalcException) {
            throw;
        } catch (Exception e) {
            throw new CalcException("Cannot recognize expression", scanner.CurrentLexema.Start, e);
        }
    }

    //Assignment ::= Add | Field ':=' Assignment
    private T ParseAssignment() {
        //T result;
        int identifierStart = scanner.CurrentLexema.Start;
        if (scanner.CurrentLexema.LexType == LexType.Identifier) {
            string name = scanner.CurrentLexema.Value;

            LexType lextype = scanner.NextLexema.LexType;
            switch (lextype) {
            case LexType.MinusEqual:
            case LexType.PlusEqual:
            case LexType.SlashEqual:
            case LexType.StarEqual:
            case LexType.EqualSign:
                scanner.MoveNext(/*Identifier*/);
                scanner.MoveNext(/*EqualSign*/);
                return builder.Assignment(lextype, name, ParseAdd());
            case LexType.OpenBrace:
                scanner.MoveNext();
                return ParseFunction(name);
            case LexType.EOF:
                scanner.MoveNext();
                return ParseField(name);
            }
        }
        //result = ParseAdd();
        //LexType nextLextype = scanner.CurrentLexema.LexType;
        //switch (nextLextype) {
        //case LexType.MinusEqual:
        //case LexType.PlusEqual:
        //case LexType.SlashEqual:
        //case LexType.StarEqual:
        //case LexType.EqualSign:
        //    throw new CalcException("Invalid identifier", identifierStart, null);
        //}
        //return result;
        return ParseAdd();
    }

    // Add ::= Mul | Add ('+' | '-') Mul
    private T ParseAdd() {
        T result = ParseMul();
        while (true) {
            LexType lexType = scanner.CurrentLexema.LexType;
            switch (lexType) {
            case LexType.Plus:
            case LexType.Minus:
                scanner.MoveNext();
                result = builder.Binary(lexType, result, ParseMul());
                break;
            default:
                return result;
            }
        }
    }
    // Mul ::= Power | Mul('*'|'/') Power | Mul Power'
    private T ParseMul() {
        T result = ParsePwr();
        while (true) {
            LexType lexType = scanner.CurrentLexema.LexType;
            switch (lexType) {
            case LexType.Star:
            case LexType.Slash:
                scanner.MoveNext();
                result = builder.Binary(lexType, result, ParsePwr());
                break;
            default:
                if (isOperand[(int)lexType]) {
                    result = builder.Binary(LexType.Star, result, ParsePwr());
                    break;
                } else {
                    return result;
                }
            }
        }
    }
    // Power::= Unary | Unary ^ Power
    private T ParsePwr() {
        T result = ParseUnary();
        LexType lexType = scanner.CurrentLexema.LexType;
        if (lexType == LexType.Power) {
            scanner.MoveNext();
            result = builder.Binary(lexType, result, ParsePwr());
        }
        return result;
    }


    // Unary ::= Factorial | ('+' | '-') Factorial
    private T ParseUnary() {
        LexType lexType = scanner.CurrentLexema.LexType;
        switch (lexType) {
        case LexType.Plus:
        case LexType.Minus:
            scanner.MoveNext();
            T result = builder.Unary(lexType, ParseUnary());
            return result;
        default:
            return ParseFact();
        }
    }

    // Factorial ::= SubExp |  SubExp '!'
    private T ParseFact() {
        T result = ParseBracedExpr();
        while (scanner.CurrentLexema.LexType == LexType.Exclamation) {
            result = builder.Unary(LexType.Exclamation, result);
            scanner.MoveNext();
        }
        return result;
    }

    // SubExp ::= '(' expr+ ')' | Function '(' | Field | Variable
    private T ParseBracedExpr() {
        if (scanner.CurrentLexema.LexType == LexType.OpenBrace) {
            int subExpStart = scanner.CurrentLexema.Start;
            scanner.MoveNext();
            T result = ParseAssignment();
            if (scanner.CurrentLexema.LexType == LexType.EOF) {
                throw new CalcException("Missing ')'", subExpStart, null);
            }
            Debug.Assert(scanner.CurrentLexema.LexType == LexType.CloseBrace); //It works. BUT I DONT KNOW WHY!!!!! ؐ_ؐ
            scanner.MoveNext();
            return result;
        }
        return ParseVariable();
    }

    // Field' ::= chars+
    private T ParseField(string name) {
        T result = builder.Field(name);
        return result;
    }

    // Function ::= chars+ '(' expr+ [, expr+]* ')'
    private T ParseFunction(string name) {
        scanner.MoveNext();
        int funcStart = scanner.CurrentLexema.Start;
        List<T> args = new List<T>();
        if (scanner.CurrentLexema.LexType != LexType.CloseBrace) {
            //T innerResult = ParseAdd();
            while (true) {
                T innerResult = ParseAdd();
                if (scanner.CurrentLexema.LexType == LexType.CloseBrace) {
                    args.Add(innerResult);
                    break;
                } else if (scanner.CurrentLexema.LexType == LexType.Comma) {
                    args.Add(innerResult);
                    scanner.MoveNext();
                    //throw new CalcException("Syntax error", scanner.CurrentLexema.Start, null);
                } else if (scanner.CurrentLexema.LexType == LexType.EOF) {
                    throw new CalcException("Missing ')'", funcStart, null);
                } else {
                    Debug.Assert(false, "With this grammar, it is impossible to get here. Because of the implicit multiplication");
                }
            }
            //args.Add(innerResult);
        }
        T result = builder.Function(name, args);
        Debug.Assert(scanner.CurrentLexema.LexType == LexType.CloseBrace);
        scanner.MoveNext();
        return result;
    }

    // Variable ::= '$' number | String
    private T ParseVariable() {
        if (scanner.CurrentLexema.LexType == LexType.Variable) {
            string name = scanner.CurrentLexema.Value;
            T result = builder.Variable(name);
            scanner.MoveNext();
            return result;
        }
        return ParseString();
    }

    // String ::= '"'chars+'"' | Field
    private T ParseString() {
        if (scanner.CurrentLexema.LexType == LexType.DoubleQuote) {
            int stringStart = scanner.CurrentLexema.Start;
            scanner.MoveNext();
            StringBuilder sb = new StringBuilder();
            while (true) {
                LexType lextype = scanner.CurrentLexema.LexType;
                switch (lextype) {
                case LexType.DoubleQuote:
                    T result = builder.String(sb.ToString());
                    scanner.MoveNext();
                    return result;
                case LexType.EOF:
                    throw new CalcException("Missing '\"'", stringStart, null);
                default:
                    sb.Append(scanner.CurrentLexema.Value);
                    scanner.MoveNext();
                    break;
                }
            }
        }
        return ParseField();
    }

    // Field ::= chars+ | num
    private T ParseField() {
        if (scanner.CurrentLexema.LexType == LexType.Identifier) {
            if (scanner.NextLexema.LexType == LexType.EqualSign) {
                return ParseAssignment();
            }
            string name = scanner.CurrentLexema.Value;
            scanner.MoveNext();
            return ParseField(name);
        }

        return ParseNumber();
    }

    // num ::= .number Exp? | num (.(num)?)? exp?
    private T ParseNumber() {
        if (scanner.CurrentLexema.LexType != LexType.Number) {
            throw new CalcException("Syntax error", scanner.CurrentLexema.Start, null);
        }
        T result = builder.Number(scanner.CurrentLexema.Value);
        scanner.MoveNext();
        return result;
    }
}
