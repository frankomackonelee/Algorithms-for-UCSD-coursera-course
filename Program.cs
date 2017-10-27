using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox_Week5
{
    class Program
    {
        static void Main(string[] args)
        {
            //doPrimativeCalculator(Utils.getInt());
            //doKnapsack(Utils.getArray(), Utils.getArray());
            //doEditDistance(Utils.getString(), Utils.getString());
            //doArithMeticExpression();
            doLongestCommonSubsequence(Utils.getInt(), Utils.getArray(), Utils.getInt(), Utils.getArray(), Utils.getInt(), Utils.getArray());
        }

        private static void doLongestCommonSubsequence(int n, int[] A, int m, int[] B, int l, int[] C)
        {
            CommonSubsequences cs = new CommonSubsequences(n, A, m, B, l, C);
            cs.doSubsequenceLength();
            Console.WriteLine(cs.getMaxSubsequence());
        }

        private static void doArithMeticExpression()
        {
            ArithmeticExpression ae = new ArithmeticExpression(Utils.getString());
            ae.doMaxAndMin();
            Console.WriteLine(ae.getMax());
        }

        private static void doEditDistance(String line1, String line2)
        {
            EditDistance ed = new EditDistance(line1, line2);
            Console.WriteLine(ed.getEditDistance());
        }

        private static void doPrimativeCalculator(int input)
        {
            PrimitiveCalculator pc = new PrimitiveCalculator(input);
            Console.WriteLine(pc.minNumberOfOperations());
            Console.WriteLine(pc.numberPath(input));
        }

        private static void doKnapsack(int[] firstLine, int[] secondLine)
        {
            Knapsack kNs = new Knapsack(firstLine[0], firstLine[1], secondLine);
            Console.WriteLine(kNs.getOptimal());
        }
    }

    class CommonSubsequences
    {
        private int n, m, l;
        private int[] A, B, C;
        private int[,,] SubsequenceLength;

        public CommonSubsequences(int n, int[] A, int m, int[] B, int l, int[] C)
        {
            this.n = n + 1;
            this.A = makeArrayWithLeadZero(A);
            this.m = m + 1;
            this.B = makeArrayWithLeadZero(B);
            this.l = l + 1;
            this.C = makeArrayWithLeadZero(C);
            SubsequenceLength = new int[this.n, this.m, this.l];
        }

        private int[] makeArrayWithLeadZero(int[] original)
        {
            int[] answer = new int[original.Length + 1];
            Array.Copy(original, 0, answer, 1, original.Length);
            return answer;
        }

        public int getMaxSubsequence()
        {
            return SubsequenceLength[n - 1, m - 1, l - 1];
        }

        public void doSubsequenceLength()
        {
            int longestArray = Utils.Max(n, m, l);
            for(int diagonal = 1; diagonal < longestArray; diagonal++)
            {
                for(int edge = diagonal; edge < longestArray; edge++)
                {
                    for(int fill = diagonal; fill < longestArray; fill++)
                    {
                        fillBottom(fill, edge, diagonal);
                        fillSide(fill, edge, diagonal);
                        fillBack(fill, edge, diagonal);
                    }
                }
            }
        }

        /*This one is going to be used with z fixed fill strips of (x - inner, y - outer, z - fixed):
         *  for z = diagonal
         *      increase y stepwise starting from diagonal
         *          increase x stepwise starting from diagonal 
         */
        private void fillBottom(int inner, int outer, int outermost)
        {
            applyIterativeFormula(inner, outer, outermost);
        }

        /*This one is going to be used with x fixed fill strips of (y - inner, z - outer, x - fixed):
        */
        private void fillSide(int inner, int outer, int outermost)
        {
            applyIterativeFormula(outermost, inner, outer);
        }

        private void fillBack(int inner, int outer, int outermost)
        {
            applyIterativeFormula(outer, outermost, inner);
        }

        private void applyIterativeFormula(int row, int col, int height)
        {
            if (row < n && col < m && height < l)
            { 
                int longestSequence = Utils.Max(SubsequenceLength[row - 1, col, height], 
                                                SubsequenceLength[row, col - 1, height],
                                                SubsequenceLength[row, col, height - 1]);
                if (A[row] == B[col] && B[col] == C[height])
                {
                    longestSequence = Math.Max(longestSequence, SubsequenceLength[row - 1, col - 1, height - 1] + 1);
                }
                else
                {
                    longestSequence = Math.Max(longestSequence, SubsequenceLength[row - 1, col - 1, height - 1]);
                }
                SubsequenceLength[row, col, height] = longestSequence;
            }
        }
    }

    class ArithmeticExpression
    {
        private int[] numbers;
        private String operations;
        private long[,] maximums;
        private long[,] minimums;

        public ArithmeticExpression(String expression)
        {
            numbers = new int[(expression.Length + 1)/2];
            StringBuilder o = new StringBuilder();
            for(int l = 0; l < expression.Length; l++)
            {
                if(l % 2 == 1)
                {
                    o.Append(expression[l]);
                }
                else
                {
                    numbers[l / 2] = int.Parse(expression.Substring(l,1));
                }
            }
            operations = o.ToString();
            maximums = new long[numbers.Length,numbers.Length];
            minimums = new long[numbers.Length, numbers.Length];
        }

        public long getMax()
        {
            return maximums[0, numbers.Length - 1];
        }

        public void doMaxAndMin()
        {
            for(int distFromDiabonal = 0; distFromDiabonal<numbers.Length; distFromDiabonal++)
            {
                for(int row = 0; row < numbers.Length - distFromDiabonal; row++)
                {
                    setMaxAndMin(row, distFromDiabonal + row);
                }
            }
        }

        private void setMaxAndMin(int i, int j)
        {
            if(j-i == 0)
            {
                maximums[i, j] = numbers[i];
                minimums[i, j] = numbers[i];
                //Console.WriteLine("Row is : " + i + " Col is : " + j + " Max and Min are " + numbers[i]);
            }
            else
            {
                int numberOfIts = j - i;
                long max = long.MinValue;
                long min = long.MaxValue;
                for(int k = 0; k<numberOfIts; k++)
                {
                    long highhigh = doOperation(maximums[i, i + k], operations.Substring(i + k, 1), maximums[i + k + 1, j]);
                    long highlow = doOperation(maximums[i, i + k], operations.Substring(i + k, 1), minimums[i + k + 1, j]);
                    long lowhigh = doOperation(minimums[i, i + k], operations.Substring(i + k, 1), maximums[i + k + 1, j]);
                    long lowlow = doOperation(minimums[i, i + k], operations.Substring(i + k, 1), minimums[i + k + 1, j]);
                    max = Math.Max(Utils.Max(highhigh, highlow, lowhigh, lowlow), max);
                    min = Math.Min(Utils.Min(highhigh, highlow, lowhigh, lowlow), min);
                }
                maximums[i, j] = max;
                minimums[i, j] = min;
                //Console.WriteLine("Row is : " + i + " Col is : " + j + " Max is : " + maximums[i, j] + " Min is " + minimums[i, j]);
            }
        }

        private long doOperation(long firstTerm, String operation, long secondTerm)
        {
            switch (operation)
            {
                case "+":
                    return firstTerm + secondTerm;
                case "-":
                    return firstTerm - secondTerm;
                default:
                    return firstTerm * secondTerm;
            }
        }
    }

    class EditDistance
    {
        private String stringj = "";
        private String stringi = "";
        private int[,] distances;

        public EditDistance(String line1, String line2)
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.Append("0");
            sb1.Append(line1);
            stringj = sb1.ToString();

            StringBuilder sb2 = new StringBuilder();
            sb2.Append("0");
            sb2.Append(line2);
            stringi = sb2.ToString();

            distances = new int[stringi.Length, stringj.Length];
            doDistances();
        }

        private void doDistances()
        {
            for (int j = 0; j < stringj.Length; j++)
            {
                distances[0, j] = j;
            }
            for (int i = 0; i < stringi.Length; i++)
            {
                distances[i, 0] = i;
            }
            int insertion = 0;
            int deletion = 0;
            int match = 0;
            int mismatch = 0;
            for(int i = 1; i < stringi.Length; i++)
            {
                for (int j = 1; j < stringj.Length; j++)
                {
                    insertion = distances[i, j - 1] + 1;
                    deletion = distances[i - 1, j] + 1;
                    match = distances[i - 1, j - 1];
                    mismatch = match + 1;
                    if (stringj[j] == stringi[i])
                    {
                        distances[i,j] = Utils.Min(insertion, deletion, match);
                    }
                    else
                    {
                        distances[i, j] = Utils.Min(insertion, deletion, mismatch);
                    }
                }
            }
        }

        public int getEditDistance()
        {
            return distances[stringi.Length - 1, stringj.Length - 1];
        }
    }

    class Knapsack
    {
        private int W;
        private int N;
        private int[] weights;
        private int[] values;
        private int[,] optimumValues;
        private bool[,] optimumValuesCalculated;

        public Knapsack(int W, int n, int[] weights)
        {
            this.W = W;
            this.N = n;
            this.weights = new int[n + 1];
            this.values = new int[n + 1];
            this.weights[0] = 0;
            Array.Copy(weights, 0, this.weights, 1, weights.Length);
            Array.Copy(this.weights, this.values, this.weights.Length);
            optimumValues = new int[n + 1,W + 1];
            optimumValuesCalculated = new bool[n + 1, W + 1];
            for (int x = 0; x<W+1; x++)
            {
                setOptimal(0, x, 0);
            }
            for (int y = 0; y < n + 1 ; y++)
            {
                setOptimal(y, 0, 0);
            }
        }

        public int getOptimal()
        {
            return optimal(N, W);
        }

        private int optimal(int n, int w)
        {
            int answer = 0;
            if (optimumValuesCalculated[n,w])
            {
                answer = optimumValues[n, w];
            }
            else
            {
                if(w - weights[n] < 0)
                {
    
                    answer = optimal(n - 1, w);
                }
                else
                {
                    answer = Math.Max(optimal(n - 1, w - weights[n]) + values[n], optimal(n - 1, w));
                }
                setOptimal(n, w, answer);
            }
            return answer;
        }

        private void setOptimal(int n, int w, int value)
        {
            optimumValues[n, w] = value;
            optimumValuesCalculated[n, w] = true;
        }
    }

    class PrimitiveCalculator
    {
        private int[] minNumber;
        private int upperBound;

        public PrimitiveCalculator(int N)
        {
            upperBound = N + 1;
            minNumber = new int[upperBound];
            minNumber[0] = 0;
            minNumber[1] = 0;
            for(int n = 2; n<upperBound; n++)
            {
                minNumber[n] = C(n);
            }
        }

        private int C(int n)
        {
            return Utils.Min(Best(n) + 1, middle(n) + 1, minNumber[n - 1] + 1);
        }

        private int Best(int n)
        {
            if (n % 3 == 0)
            {
                return minNumber[n / 3];
            }
            else
            {
                return upperBound;
            }
        }

        private int middle(int n)
        {
            if (n % 2 == 0)
            {
                return minNumber[n / 2];
            }
            else
            {
                return upperBound;
            }
        }

        private bool divides3(int n)
        {
            return n % 3 == 0;
        }

        private bool divides2(int n)
        {
            return n % 2 == 0;
        }

        public int minNumberOfOperations()
        {
            return minNumber[upperBound - 1];
        }

        public String numberPath(int n)
        {
            int currentN = n;
            StringBuilder sb = new StringBuilder();
            sb.Append(currentN);
            while (currentN > 1)
            {
                if (divides3(currentN) && minNumber[currentN] - minNumber[currentN / 3] == 1)
                {
                    currentN = currentN / 3;
                }
                else if (divides2(currentN) && minNumber[currentN] - minNumber[currentN / 2] == 1)
                {
                    currentN = currentN / 2;
                }
                else
                {
                    currentN--;
                }
                sb.Insert(0, " ");
                sb.Insert(0, currentN);
            }    
            return sb.ToString();
        }
    }


    class Utils
    {
        public static String getString()
        {
            String input = "";
            try
            {
                var inputs = System.Console.ReadLine();
                input = (String)inputs;
            }
            catch
            {
                throw new System.FormatException("There was a problem with the inputted String");
            }
            return input;
        }

        public static long[] getArrayOfLongs()
        {
            long[] longArray;
            try
            {
                var inputs = System.Console.ReadLine();
                var tokens = inputs.Split(' ');
                longArray = new long[tokens.Length];
                for (int i = 0; i < tokens.Length; i++)
                {
                    longArray[i] = long.Parse(tokens[i]);
                }
            }
            catch (System.FormatException)
            {
                throw new System.FormatException("There was a problem with the inputted long array");
            }
            return longArray;
        }
        public static int[] getArray()
        {
            int[] intArray;
            try
            {
                var inputs = System.Console.ReadLine();
                var tokens = inputs.Split(' ');
                intArray = new int[tokens.Length];
                for (int i = 0; i < tokens.Length; i++)
                {
                    intArray[i] = int.Parse(tokens[i]);
                }
            }
            catch (System.FormatException)
            {
                throw new System.FormatException("There was a problem with the inputted int array");
            }
            return intArray;
        }

        public static int getInt()
        {
            int intInput;
            try
            {
                var input = System.Console.ReadLine();
                intInput = int.Parse(input);
            }
            catch (System.FormatException)
            {
                throw new System.FormatException("There was a problem with the inputted int");
            }
            return intInput;
        }

        public static void printInt(int i)
        {
            Console.WriteLine(i);
        }

        public static void printIntArray(int[] myArray)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int i in myArray)
            {
                sb.Append(i + " ");
            }
            sb.Remove(sb.Length - 1, 1);
            Console.WriteLine(sb.ToString());
        }

        public static int Min(int first, int second, int third)
        {
            return Math.Min(first, Math.Min(second, third));
        }

        public static int Max(int first, int second, int third)
        {
            return Math.Max(first, Math.Max(second, third));
        }

        public static long Min(long first, long second, long third, long fourth)
        {
            return Math.Min(Math.Min(first, second), Math.Min(third, fourth));
        }

        public static long Max(long first, long second, long third, long fourth)
        {
            return Math.Max(Math.Max(first, second), Math.Max(third, fourth));
        }
    }
}
