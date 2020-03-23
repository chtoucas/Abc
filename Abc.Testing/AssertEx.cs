// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Xunit;

    public sealed partial class AssertEx : Assert
    {
        private AssertEx() { }
    }

    public partial class AssertEx
    {
        // Threw ArgumentException.
        public static void ThrowsArgEx(string argName, Action testCode)
        {
            ArgumentException ex = Throws<ArgumentException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Threw ArgumentException.
        public static void ThrowsArgEx(string argName, Func<object> testCode)
        {
            ArgumentException ex = Throws<ArgumentException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Threw ArgumentNullException.
        public static void ThrowsArgNullEx(string argName, Action testCode)
        {
            ArgumentNullException ex = Throws<ArgumentNullException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Threw ArgumentNullException.
        public static void ThrowsArgNullEx(string argName, Func<object> testCode)
        {
            ArgumentNullException ex = Throws<ArgumentNullException>(testCode);
            Equal(argName, ex.ParamName);
        }

#pragma warning disable CA1034 // Nested types should not be visible
        public static class Async
#pragma warning restore CA1034
        {
            // Threw ArgumentNullException.
            public static void ThrowsArgNullEx(string argName, Func<Task> testCode)
            {
                Task<ArgumentNullException> ex = ThrowsAsync<ArgumentNullException>(testCode);
                Equal(argName, ex.Result.ParamName);
            }
        }

        public static void ThrowsOnNext<T>(IEnumerable<T> seq)
        {
            if (seq is null) { throw new ArgumentNullException(nameof(seq)); }

            using var iter = seq.GetEnumerator();
            Throws<InvalidOperationException>(() => iter.MoveNext());
        }

        public static void ThrowsAfter<T>(IEnumerable<T> seq, int count)
        {
            if (seq is null) { throw new ArgumentNullException(nameof(seq)); }

            int i = 0;
            using var iter = seq.GetEnumerator();
            while (i < count) { True(iter.MoveNext()); i++; }
            Throws<InvalidOperationException>(() => iter.MoveNext());
        }

        public static void CalledOnNext<T>(IEnumerable<T> seq, ref bool notCalled)
        {
            if (seq is null) { throw new ArgumentNullException(nameof(seq)); }

            using var iter = seq.GetEnumerator();
            iter.MoveNext();
            False(notCalled);
        }

        public static void CalledAfter<T>(IEnumerable<T> seq, int count, ref bool notCalled)
        {
            if (seq is null) { throw new ArgumentNullException(nameof(seq)); }

            int i = 0;
            using var iter = seq.GetEnumerator();
            while (i < count) { True(iter.MoveNext()); i++; }
            iter.MoveNext();
            False(notCalled);
        }
    }
}
