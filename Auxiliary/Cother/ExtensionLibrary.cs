﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Cother
{
    /// <summary>
    /// This static class contains useful extension methods to standard types.
    /// </summary>
    public static class ExtensionLibrary
    {
        private static readonly ThreadLocal<Random> RandomThreadLocal =
       new ThreadLocal<Random>(() => new Random());
        public static void Shuffle<T>(this IList<T> list, int seed)
        {
            list.Shuffle(new Random(seed));
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            list.Shuffle(null);
        }

        public static bool ContainsOneOf<T>(this IList<T> list, IEnumerable<T> ofWhat)
        {
            foreach(T of in ofWhat)
            {
                if (list.Contains(of))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a randomly selected element of the list. If the list is empty, it returns null.
        /// </summary>
        public static T GetRandom<T>(this IReadOnlyList<T> list)
        {
            if (list.Count == 0) return default(T);
            return list[RandomThreadLocal.Value.Next(0, list.Count)];
        }

        public static void Shuffle<T>(this IList<T> list, Random rand)
        {
            var r = rand ?? RandomThreadLocal.Value;

            var len = list.Count;
            for (var i = len - 1; i >= 1; --i)
            {
                var j = r.Next(i);
                var tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }
        /// <summary>
        /// Performs integer exponentiation.
        /// </summary>
        /// <param name="basis">The base.</param>
        /// <param name="power">The exponent.</param>
        /// <remarks>Code by Vlix - http://stackoverflow.com/questions/383587/how-do-you-do-integer-exponentiation-in-c
        /// </remarks>
        public static int Exponentiate(this int basis, uint power)
        {
            int returnedValue = 1;
            while (power != 0)
            {
                if ((power & 1) == 1)
                    returnedValue *= basis;
                basis *= basis;
                power >>= 1;
            }
            return returnedValue;
        }
        /// <summary>
        /// Compares two floats and returns true if they are equal. Takes into account rounding errors when manipulating floating-point numbers.
        /// </summary>
        /// <param name="me">The primary number.</param>
        /// <param name="compareToThis">The second number.</param>
        /// <returns>true, if the two numbers are approximately equal (taking into account rounding errors). False otherwise.</returns>
        public static bool ApproximatelyEqual(this float me, float compareToThis)
        {
            return Math.Abs(me - compareToThis) < 0.001f;
        }
        /// <summary>
        /// Compares two double-precision numbers and returns true if they are equal. Takes into account rounding errors when manipulating floating-point numbers.
        /// </summary>
        /// <param name="me">The primary number.</param>
        /// <param name="compareToThis">The second number.</param>
        /// <returns>true, if the two numbers are approximately equal (taking into account rounding errors). False otherwise.</returns>
        public static bool ApproximatelyEqual(this double me, double compareToThis)
        {
            return Math.Abs(me - compareToThis) < 0.00001;
        }
        /// <summary>
        /// Returns the object, or the minimum or maximum if the object exceeds the range.
        /// </summary>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        /// <summary>
        /// Adds a "+" before non-negative integers. For example, "43" becomes "+43", "0" becomes "+0", "-7" stays "-7".
        /// </summary>
        /// <param name="integer">The integer to convert to string.</param>
        public static string WithPlus(this int integer)
        {
            int absValue = Math.Abs(integer);
            if (integer >= 0)
            {
                return "+" + absValue;
            }
            else
            {
                return "-" + absValue;
            }
        }
    }
}
