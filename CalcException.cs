using System;
// Mercchange

class CalcException : Exception {
    int errorPosition;
    public CalcException(string message, int errorPosition, Exception inner) : base (message, inner) {
        this.errorPosition = errorPosition;
    }

    public int ErrorPosition {
        get {
            return errorPosition;
        }
    }
}
