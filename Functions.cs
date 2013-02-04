using System;
using System.Collections.Generic;

class Functions {
    public static double Average(List<double> args) {
        if (args.Count == 0) {
            throw new InvalidOperationException("Average requires 1 or more arguments.");
        }
        double average = args[0];
        for (int i = 1; i < args.Count; i++) {
            average += args[i];
        }
        //double average = ;
        //foreach (double d in args) {
        //    average += d;
        //}
        return average / args.Count;
    }

    public static void CheckArgs(List<double> args, int excpectedArgCount, string name) {
        if (args.Count != excpectedArgCount) {
            throw new Exception(name + " requires " + excpectedArgCount + " arguments.");
        }
        return;
    }

    public static double Factorial(double number) {
        int i = (int)number;
        if (number != i || i < 0) {
            throw new Exception(string.Format("The factorial is not defined for {0}", number));
        }
        return Factorial(i);
    }

    private static double Factorial(int number) {
        double dnumber = 1;
        while (number > 1) {
            dnumber *= number;
            number--;
            if (double.IsInfinity(dnumber)) {
                break;
            }
        }
        return dnumber;
    }

    public static double GCF(double num1, double num2, bool checkWhole) {
        if (checkWhole) {
            long l1 = (long)num1;
            long l2 = (long)num2;
            if (l1 != num1 || l2 != num2) {
                throw new Exception("Arguments of GCF must be whole numbers");
            }
        }
        while (num2 != 0) {
            double temp = num2;
            num2 = num1 % num2;
            num1 = temp;
        }
        return num1;
    }

    public static bool IsPrime(double n) {
        if (n == 2 || n == 3) {
            return true;
        }
        if (n % 2 == 0 || n % 3 == 0) {
            return false;
        }
        if (n < 2) {
            return false;
        }
        //1 is added because of rounding
        //Prime numbers are of form 6*k+1 and 6*k-1
        for (long k = 1; k <= ((long)Math.Sqrt(n)) / 6 + 1; k++) {
            if (n % (6 * k + 1) == 0 || n % (6 * k - 1) == 0) {
                return false;
            }
        }
        return true;
    }

    public static double LCM(double num1, double num2) {
        long l1 = (long)num1;
        long l2 = (long)num2;
        if (l1 != num1 || l2 != num2) {
            throw new Exception("Arguments of LCM must be whole numbers");
        }
        return (num1 * num2) / GCF(num1, num2, false);
    }

    public static double Max(List<double> args) {
        if (args.Count == 0) {
            throw new InvalidOperationException("Max requires 1 or more arguments.");
        }
        double max = args[0];
        for (int i = 1; i < args.Count; i++) {
            if (args[i] > max) {
                max = args[i];
            }
        }
        return max;
    }

    public static double Min(List<double> args) {
        if (args.Count == 0) {
            throw new InvalidOperationException("Min requires 1 or more arguments.");
        }
        double min = args[0];
        for (int i = 1; i < args.Count; i++) {
            if (args[i] < min) {
                min = args[i];
            }
        }
        return min;
    }

    public static double ToRadian(double angle) {
        return Math.PI * angle / 180;
    }

    public static double ToDegree(double angle) {
        return angle * (180.0 / Math.PI);
    }
}