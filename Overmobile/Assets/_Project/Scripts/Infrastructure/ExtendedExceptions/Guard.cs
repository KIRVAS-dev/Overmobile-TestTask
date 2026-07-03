using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace ExtendedExceptions
{
    public static class Guard
    {
        public static void AgainstNull(Object value, Func<ExtendedException> exceptionFactory)
        {
            if (!value)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstNull(object value, Func<ExtendedException> exceptionFactory)
        {
            if (value == null)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstNullOrEmpty<T>(IReadOnlyList<T> values, Func<ExtendedException> exceptionFactory)
        {
            if (values == null
             || values.Count == 0)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstNonPositive(float value, Func<ExtendedException> exceptionFactory)
        {
            if (value <= 0f)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstNonPositive(int value, Func<ExtendedException> exceptionFactory)
        {
            if (value <= 0)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstNegative(float value, Func<ExtendedException> exceptionFactory)
        {
            if (value < 0f)
            {
                throw exceptionFactory();
            }
        }
    }
}
