using System;
using System.Collections.Generic;
using System.Diagnostics;

class Factorization {
    public static void Factor(double result) {
        long number = Convert.ToInt64(result);
        if (number != result) {
            throw new Exception(string.Format("Factorization is not defined for {0} (non-whole)", result));
            //throw new Exception("Factorization is not defined for non-whole numbers");
        }
        List<long> factors = new List<long>();
        List<long> exp = new List<long>();
        if (number < 0) {
            factors.Add(-1);
            exp.Add(1);
            number *= -1;
        }
        Factor(number, GenPrimes((long)Math.Sqrt(number)), factors, exp);
        PrintFactors(factors, exp);
    }

    private static void PrintFactors(List<long> factors, List<long> exp) {
        int index = 0;
        foreach (long l in factors) {
            if (factors.Count == 1) {
                Console.WriteLine(factors[index]);
                //Console.WriteLine(factors[index] + "^" + exp[index]);
            } else if (index == factors.Count - 1) {
                if (exp[index] == 1) {
                    Console.WriteLine(factors[index]);
                } else {
                    Console.WriteLine(factors[index] + "^" + exp[index]);
                }
            } else if (exp[index] == 1) {
                Console.Write(factors[index] + " * ");
            } else {
                Console.Write(factors[index] + "^" + exp[index] + " * ");
            }
            index++;
        }
    }
    private static void Print(List<long> primes, int testNumber) {
        Console.WriteLine("Test of {0}", testNumber);
        foreach (long i in primes) {
            Console.WriteLine(i);
        }
        Console.WriteLine("-------------");
    }


    // Generate primes up to desired number
    private static List<long> GenPrimes(long upToNumber) {
        //negatives are not needed for GenPrimes, only for factoring
        if (upToNumber < 0) {
            throw new Exception("Dimension must be positive");
        } else if (upToNumber <= 1) {
            return new List<long>();
        } else if (upToNumber == 2) {
            return new List<long> { 2 };
        } else if (upToNumber == 3) {
            return new List<long> { 2, 3 };
        } else {
            List<long> primes = new List<long>();
            primes.Add(2);
            primes.Add(3);
            // Any number n can be express n = 6*k + a with a being from -1 to 4
            long n = 5;
            bool a = false; // false +=2; true +=4 
            while (n <= upToNumber) {
                long root = (long)Math.Sqrt(n);
                bool prime = true;
                for (int j = 0; j < primes.Count; j++) {
                    if (n % primes[j] == 0) {
                        // not prime
                        prime = false;
                        break;
                    }
                }

                if (prime) {
                    // prime number
                    primes.Add(n);
                }
                n += a ? 4 : 2;
                a = !a;
            }
            return primes;
        }
    }

    private static void Factor(long number, List<long> primes, List<long> factors, List<long> exp) {
        if (number < 0) {
            factors.Add(-1);
            exp.Add(1);
            number *= -1;
        }
        if (IsPrime(number, primes)) {
            factors.Add(number);
            exp.Add(1);
            return;
        }
        long root = (long)Math.Sqrt(number);
        int index = 0;
        foreach (long l in primes) {
            if (number % primes[index] == 0) {
                number /= primes[index];
                factors.Add(primes[index]);
                exp.Add(1);
                while (number % primes[index] == 0) {
                    number /= primes[index];
                    exp[exp.Count - 1]++;
                }
            }
            index++;
            root = (long)Math.Sqrt(number);
        }
        if (number != 1) {
            factors.Add(number);
            exp.Add(1);
        }
        return;
    }

    private static bool IsPrime(long n, List<long> primes) {
        if (n < 2) {
            return IsPrime(n * -1, primes);
        }
        foreach (long l in primes) {
            if (n % l == 0) {
                return false;
            }
        }
        return true;
    }
}
