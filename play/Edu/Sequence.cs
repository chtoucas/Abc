// See LICENSE.txt in the project root for license information.

namespace Play.Edu
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides static methods for producing sequences.
    /// </summary>
    public static class Sequence
    {
        /// <summary>
        /// Generates a sequence that contains exactly one value.
        /// </summary>
        public static IEnumerable<T> Return<T>(T value)
        {
            // Enumerable.Repeat(value, 1) works too.
            yield return value;
        }

        /// <summary>
        /// Generates an infinite sequence of one repeated value.
        /// </summary>
        public static IEnumerable<T> Repeat<T>(T value)
        {
            while (true)
            {
                yield return value;
            }
        }

        public static IEnumerable<T> Generate<T>(T seed, Func<T, T> generator)
        {
            Require.NotNull(generator, nameof(generator));

            return __iterator();

            IEnumerable<T> __iterator()
            {
                T current = seed;

                while (true)
                {
                    yield return current;

                    current = generator(current);
                }
            }
        }

        public static IEnumerable<T> Generate<T>(
            T seed,
            Func<T, T> generator,
            Func<T, bool> predicate)
        {
            Require.NotNull(generator, nameof(generator));
            Require.NotNull(predicate, nameof(predicate));

            return __iterator();

            IEnumerable<T> __iterator()
            {
                T current = seed;

                while (predicate(current))
                {
                    yield return current;

                    current = generator(current);
                }
            }
        }

        public static IEnumerable<TResult> Unfold<TState, TResult>(
            TState seed,
            Func<TState, (TState, TResult)> generator)
        {
            Require.NotNull(generator, nameof(generator));

            return __iterator();

            IEnumerable<TResult> __iterator()
            {
                TState state = seed;
                TResult result;

                while (true)
                {
                    (state, result) = generator(state);

                    yield return result;
                }
            }
        }

        public static IEnumerable<TResult> Unfold<TState, TResult>(
            TState seed,
            Func<TState, (TState, TResult)> generator,
            Func<TState, bool> predicate)
        {
            Require.NotNull(generator, nameof(generator));
            Require.NotNull(predicate, nameof(predicate));

            return __iterator();

            IEnumerable<TResult> __iterator()
            {
                TState state = seed;
                TResult result;

                while (predicate(state))
                {
                    (state, result) = generator(state);

                    yield return result;
                }
            }
        }

        /// <remarks>
        /// This method can be derived from:
        /// <code>
        /// Sequence.Unfold(seed, state => (generator(state), resultSelector(state)));
        /// </code>
        /// </remarks>
        public static IEnumerable<TResult> Unfold<TState, TResult>(
            TState seed,
            Func<TState, TState> generator,
            Func<TState, TResult> resultSelector)
        {
            Require.NotNull(generator, nameof(generator));
            Require.NotNull(resultSelector, nameof(resultSelector));

            return __iterator();

            IEnumerable<TResult> __iterator()
            {
                TState state = seed;

                while (true)
                {
                    yield return resultSelector(state);

                    state = generator(state);
                }
            }
        }

        /// <remarks>
        /// This method can be derived from:
        /// <code>
        /// Sequence.Unfold(seed, state => (generator(state), resultSelector(state)), predicate);
        /// </code>
        /// </remarks>
        public static IEnumerable<TResult> Unfold<TState, TResult>(
            TState seed,
            Func<TState, TState> generator,
            Func<TState, TResult> resultSelector,
            Func<TState, bool> predicate)
        {
            Require.NotNull(generator, nameof(generator));
            Require.NotNull(resultSelector, nameof(resultSelector));
            Require.NotNull(predicate, nameof(predicate));

            return __iterator();

            IEnumerable<TResult> __iterator()
            {
                TState state = seed;

                while (predicate(state))
                {
                    yield return resultSelector(state);

                    state = generator(state);
                }
            }
        }
    }
}
