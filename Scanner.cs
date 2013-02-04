using System;
using System.Diagnostics;

public class Scanner {
    private char[] source;     //input array
    private int    sourcePos;  //index of current char
    private Lexema lexema;     //last lexeme that the scanner recognized
    private Lexema nextLexema; //next lexema that the scanner will recognize (LR1)

    public Scanner(string source) {
        this.source = new char[source.Length + 1];
        source.CopyTo(0, this.source, 0, source.Length);
        this.source[source.Length] = '\0';
        this.sourcePos = 0;
        this.lexema = new Lexema(LexType.BeginFile, 0);
        this.nextLexema = new Lexema(LexType.BeginFile, 0);
        MoveNext();
    }

    private void SkipSpace() {
        while (this.source[sourcePos] == ' ' || this.source[sourcePos] == '\t') {
            sourcePos++;
        }
    }

    public Lexema CurrentLexema {
        get { return lexema; }
    }

    public Lexema NextLexema {
        get { return nextLexema; }
    }

    public void MoveNext() {
        lexema = nextLexema;

        int length;
        SkipSpace();
        switch (source[sourcePos]) {
        case '\0':
            nextLexema = new Lexema(LexType.EOF, sourcePos);
            break;
        case '+':
            if (source[sourcePos + 1] == '=') {
                nextLexema = new Lexema(LexType.PlusEqual, sourcePos);
                sourcePos += 2;
                break;
            } else {
                nextLexema = new Lexema(LexType.Plus, sourcePos);
                sourcePos++;
                break;
            }
        case '-':
            if (source[sourcePos + 1] == '=') {
                nextLexema = new Lexema(LexType.MinusEqual, sourcePos);
                sourcePos += 2;
                break;
            } else {
                nextLexema = new Lexema(LexType.Minus, sourcePos);
                sourcePos++;
                break;
            }
        case '*':
            if (source[sourcePos + 1] == '=') {
                nextLexema = new Lexema(LexType.StarEqual, sourcePos);
                sourcePos += 2;
                break;
            } else {
                nextLexema = new Lexema(LexType.Star, sourcePos);
                sourcePos++;
                break;
            }
        case '/':
            if (source[sourcePos + 1] == '=') {
                nextLexema = new Lexema(LexType.SlashEqual, sourcePos);
                sourcePos += 2;
                break;
            } else {
                nextLexema = new Lexema(LexType.Slash, sourcePos);
                sourcePos++;
                break;
            }
        case '^':
            nextLexema = new Lexema(LexType.Power, sourcePos);
            sourcePos++;
            break;
        case '!':
            nextLexema = new Lexema(LexType.Exclamation, sourcePos);
            sourcePos++;
            break;
        case '(':
            nextLexema = new Lexema(LexType.OpenBrace, sourcePos);
            sourcePos++;
            break;
        case ')':
            nextLexema = new Lexema(LexType.CloseBrace, sourcePos);
            sourcePos++;
            break;
        case '"':
            nextLexema = new Lexema(LexType.DoubleQuote, sourcePos);
            sourcePos++;
            break;
        case ',':
            nextLexema = new Lexema(LexType.Comma, sourcePos);
            sourcePos++;
            break;
        case '$':
            length = ScanDigits(source, sourcePos + 1);
            if (length != 0) {
                nextLexema = new Lexema(LexType.Variable, sourcePos, new string(source, sourcePos + 1, length));
                sourcePos += 1 + length;
                break;
            }
            throw new CalcException("Syntax error", sourcePos, null);
        case ':':
            if (source[sourcePos + 1] == '=') {
                nextLexema = new Lexema(LexType.EqualSign, sourcePos);
                sourcePos += 2;
                break;
            } else {
                throw new CalcException("Syntax error", sourcePos, null);
            }
        default:
            length = ScanDouble(source, sourcePos);
            if (length != 0) {
                nextLexema = new Lexema(LexType.Number, sourcePos, new string(source, sourcePos, length));
                sourcePos += length;
                break;
            }
            length = ScanIdentifier(source, sourcePos);
            if (length != 0) {
                nextLexema = new Lexema(LexType.Identifier, sourcePos, new string(source, sourcePos, length));
                sourcePos += length;
                break;
            }
            throw new CalcException("Syntax error", sourcePos, null);
        }
    }    

    private int ScanIdentifier(char[] source, int start) {
        int currentIndex = start;
        while (char.IsLetter(source[currentIndex])) {
            currentIndex++;
        }
        return currentIndex - start;
    }

    static int ScanDigits(char[] source, int start) {
        int currentIndex = start;
        while (char.IsNumber(source[currentIndex])) {
            currentIndex++;
        }
        return currentIndex - start;
    }

    static int ScanDouble(char[] source, int start) {
        int current = start;        
        current += ScanDigits(source, start);
        if (source[current] == '.') {
            current++;
            current += ScanDigits(source, current);
            if (current - start == 1) {
                return 0;
            }
        } else {
            if (current == start) {
                return 0;
            }
        }
        if (source[current] == 'e' || source[current] == 'E') {
            current++;
            if (source[current] == '-' || source[current] == '+') {
                current++;
            }
            int power = ScanDigits(source, current);
            if (power == 0) {
                return 0;
            }
            current += power;
        }
        return current - start;
    }

    public static string[] OpNames = {
       /* BeginFile   */ "",
       /* EOF         */ "",
       /* Number      */ "",
       /* Plus        */ "+",
       /* Minus       */ "-",
       /* Star        */ "*",
       /* Slash       */ "/",
       /* Power       */ "^",
       /* Exclamation */ "!",
       /* OpenBrace   */ "(",
       /* CloseBrace  */ ")",
       /* DoubleQuote */ "\"",
       /* Comma       */ ",",
       /* Identifier  */ "",
       /* EqualSign   */ ":=",
       /* MinusEqual  */ "-=",
       /* PlusEqual   */ "+=",
       /* SlashEqual  */ "/=",
       /* StarEqual   */ "*=",
       /* Variable    */ "",
    };

    [DebuggerDisplay("{lexType == LexType.Number ? value.ToString() : lexType.ToString()}")]
    public struct Lexema {
        private LexType lexType;
        private string  value;
        private int     start;

        public Lexema(LexType lexType, int start, string value) {
            this.lexType = lexType;
            this.start = start;
            this.value = value;
        }
        public Lexema(LexType lexType, int start) : this (lexType, start, null) {}

        public LexType LexType { get { return this.lexType; } }
        public string Value {
            get {
                Debug.Assert(
                    LexType == LexType.Number ||
                    LexType == LexType.Identifier || 
                    LexType == LexType.Variable, 
                    "Only numbers have values"
                );
                return value;
            }
        }

        public int Start {
            get {
                return this.start;
            }
        }
    }
}