using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GenPrime {
    class Program {
        static void Main() {
            List<long> factors = new List<long>();
            List<long> exp = new List<long>();
            long n = 8669;
            Factor(n, GenPrimes((long)Math.Sqrt(n)), factors, exp);
            PrintFactors(factors, exp);
            Console.WriteLine();
        }

        public static void PrintFactors(List<long> factors, List<long> exp) {
            int index = 0;
            foreach(long l in factors) {
                Console.Write(factors[index] + "^" + exp[index] + " * ");
                index++;
            }
        }
        public static void Print(List<long> primes, int testNumber) {
            Console.WriteLine("Test of {0}", testNumber);
            foreach (long i in primes) {
                Console.WriteLine(i);
            }
            Console.WriteLine("-------------");
        }


        // Generate primes up to desired number
        public static List<long> GenPrimes(long upToNumber) {
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

        public static void Factor(long n, List<long> primes, List<long> factors, List<long> exp) {
            if (n < 0) {
                factors.Add(-1);
                exp.Add(1);
                n *= -1;
            }
            if (IsPrime(n, primes)) {
                factors.Add(n);
                exp.Add(1);
                return;
            } 
            long root = (long)Math.Sqrt(n);
            int index = 0;
            foreach (long l in primes){
                if (n % primes[index] == 0) {
                    n /= primes[index];
                    factors.Add(primes[index]);
                    exp.Add(1);
                    while (n % primes[index] == 0) {
                        n /= primes[index];
                        exp[exp.Count - 1]++;
                    }
                }
                index++;
                root = (long)Math.Sqrt(n);
            }
            if (n!=1) {
                factors.Add(n);
                exp.Add(1);
            }   
            return;
        }

        public static bool IsPrime(long n, List<long> primes) {
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
}