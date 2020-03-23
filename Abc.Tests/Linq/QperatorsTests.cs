// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Tests;

    using Xunit;

    using Assert = AssertEx;

    public partial class QperatorsTests : EnumerableTests
    {
        private static readonly IEnumerable<int> Null = null!;
        private static readonly IEnumerable<int> NotNull = Enumerable.Empty<int>();
    }

    // ElementAtOrNone.
    // Largely inspired by
    // https://github.com/dotnet/corefx/blob/master/src/System.Linq/tests/ElementAtOrDefaultTests.cs
    public partial class QperatorsTests
    {
        [Fact]
        public static void ElementAtOrNone_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.ElementAtOrNone(Null, 1));
        }

        //[t("ElementAtOrNone() for int's returns the same result when called repeatedly.")]
        [Fact]
        public static void ElementAtOrNone1()
        {
            var q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                    where x > Int32.MinValue
                    select x;

            Assert.Equal(q.ElementAtOrNone(3), q.ElementAtOrNone(3));
        }

        //[t("ElementAtOrNone() for string's returns the same result when called repeatedly.")]
        [Fact]
        public static void ElementAtOrNone2()
        {
            var q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", String.Empty }
                    where !String.IsNullOrEmpty(x)
                    select x;

            Assert.Equal(q.ElementAtOrNone(4), q.ElementAtOrNone(4));
        }

        //[T("ElementAtOrDefault")]
        [Theory]
        [MemberData(nameof(ElementAtOrNoneData))]
        public static void ElementAtOrNone3(IEnumerable<int> source, int index, Maybe<int> expected)
        {
            Assert.Equal(expected, source.ElementAtOrNone(index));
        }

        //[T("ElementAtOrDefaultRunOnce")]
        [Theory]
        [MemberData(nameof(ElementAtOrNoneData))]
        public static void ElementAtOrNone4(IEnumerable<int> source, int index, Maybe<int> expected)
        {
            Assert.Equal(expected, source.RunOnce().ElementAtOrNone(index));
        }

        //[t("NullableArray_NegativeIndex_ReturnsNull")]
        [Fact]
        public static void ElementAtOrNone5()
        {
            string[] source = { "a", "b" };
            Assert.Equal(Maybe<string>.None, source.ElementAtOrNone(-1));
        }

        //[t("NullableArray_ValidIndex_ReturnsCorrectObjecvt")]
        [Fact]
        public static void ElementAtOrNone6()
        {
            string[] source = { "a", "b", null!, "d", "e" };

            Assert.Equal(Maybe<string>.None, source.ElementAtOrNone(2));
            Assert.Equal(Maybe.Of("d"), source.ElementAtOrNone(3));
        }

        public static IEnumerable<object[]> ElementAtOrNoneData
        {
            get
            {
                yield return new object[] { NumberRangeGuaranteedNotCollectionType(9, 1), 0, Maybe.Of(9) };
                yield return new object[] { NumberRangeGuaranteedNotCollectionType(9, 10), 9, Maybe.Of(18) };
                yield return new object[] { NumberRangeGuaranteedNotCollectionType(-4, 10), 3, Maybe.Of(-1) };

                yield return new object[] { new int[] { 1, 2, 3, 4 }, 4, Maybe<int>.None };
                yield return new object[] { Array.Empty<int>(), 0, Maybe<int>.None };
                yield return new object[] { new int[] { -4 }, 0, Maybe.Of(-4) };
                yield return new object[] { new int[] { 9, 8, 0, -5, 10 }, 4, Maybe.Of(10) };

                yield return new object[] { NumberRangeGuaranteedNotCollectionType(-4, 5), -1, Maybe<int>.None };
                yield return new object[] { NumberRangeGuaranteedNotCollectionType(5, 5), 5, Maybe<int>.None };
                yield return new object[] { NumberRangeGuaranteedNotCollectionType(0, 0), 0, Maybe<int>.None };
            }
        }
    }

    // FirstOrNone.
    // Largely inspired by
    // https://github.com/dotnet/corefx/blob/master/src/System.Linq/tests/FirstOrDefaultTests.cs
    public partial class QperatorsTests
    {
        [Fact]
        public static void FirstOrNone_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.FirstOrNone(Null));

            Assert.ThrowsArgNullEx("source", () => Qperators.FirstOrNone(Null, Funk<int, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => Qperators.FirstOrNone(NotNull, Funk<int, bool>.Null));
        }

        [Fact]
        //[t("FirstOrNone() for int's returns the same result when called repeatedly.")]
        public static void FirstOrNone1()
        {
            var source = Enumerable.Range(0, 0);

            var q = from x in source select x;

            Assert.Equal(q.FirstOrNone(), q.FirstOrNone());
        }

        [Fact]
        //[t("FirstOrNone() for string's returns the same result when called repeatedly.")]
        public static void FirstOrNone2()
        {
            var q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", String.Empty }
                    where !String.IsNullOrEmpty(x)
                    select x;

            Assert.Equal(q.FirstOrNone(), q.FirstOrNone());
        }

        private static void FirstOrNone3Impl<T>()
        {
            T[] source = Array.Empty<T>();
            var expected = Maybe<T>.None;

            Assert.IsAssignableFrom<IList<T>>(source);
            Assert.Equal(expected, source.RunOnce().FirstOrNone());
        }

        [Fact]
        //[t("FirstOrNone() for an empty IList<T>.")]
        public static void FirstOrNone3()
        {
            FirstOrNone3Impl<int>();
            FirstOrNone3Impl<string>();
            FirstOrNone3Impl<DateTime>();
            FirstOrNone3Impl<QperatorsTests>();
        }

        [Fact]
        //[t("FirstOrNone() for an IList<T> of one element.")]
        public static void FirstOrNone4()
        {
            int[] source = { 5 };
            var expected = Maybe.Of(5);

            Assert.IsAssignableFrom<IList<int>>(source);
            Assert.Equal(expected, source.FirstOrNone());
        }

        [Fact]
        //[t("FirstOrNone() for an IList<T> of many elements whose first is none.")]
        public static void FirstOrNone5()
        {
            string[] source = { null!, "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS" };
            var expected = Maybe<string>.None;

            Assert.IsAssignableFrom<IList<string>>(source);
            Assert.Equal(expected, source.FirstOrNone());
        }

        [Fact]
        //[t("FirstOrNone() for an IList<T> of many elements whose first is some.")]
        public static void FirstOrNone6()
        {
            string[] source = { "!@#$%^", null!, "C", "AAA", "", "Calling Twice", "SoS" };
            var expected = Maybe.Of("!@#$%^");

            Assert.IsAssignableFrom<IList<string>>(source);
            Assert.Equal(expected, source.FirstOrNone());
        }

        private static void FirstOrNone7Impl<T>()
        {
            var source = EmptySource<T>();
            var expected = Maybe<T>.None;

            Assert.Null(source as IList<T>);
            Assert.Equal(expected, source.RunOnce().FirstOrNone());
        }

        [Fact]
        //[t("EmptyNotIListT")]
        public static void FirstOrNone7()
        {
            FirstOrNone7Impl<int>();
            FirstOrNone7Impl<string>();
            FirstOrNone7Impl<DateTime>();
            FirstOrNone7Impl<QperatorsTests>();
        }

        [Fact]
        //[t("OneElementNotIListT")]
        public static void FirstOrNone8()
        {
            IEnumerable<int> source = NumberRangeGuaranteedNotCollectionType(-5, 1);
            var expected = Maybe.Of(-5);

            Assert.Null(source as IList<int>);
            Assert.Equal(expected, source.FirstOrNone());
        }

        [Fact]
        //[t("ManyElementsNotIListT")]
        public static void FirstOrNone9()
        {
            IEnumerable<int> source = NumberRangeGuaranteedNotCollectionType(3, 10);
            var expected = Maybe.Of(3);

            Assert.Null(source as IList<int>);
            Assert.Equal(expected, source.FirstOrNone());
        }

        [Fact]
        //[t("EmptySource")]
        public static void FirstOrNone10()
        {
            string[] source = Array.Empty<string>();
            var expected = Maybe<string>.None;

            Assert.Equal(expected, source.FirstOrNone(x => true));
            Assert.Equal(expected, source.FirstOrNone(x => false));
        }

        [Fact]
        //[t("FirstOrNone() on a list of one element returns some.")]
        public static void FirstOrNone11()
        {
            int[] source = { 4 };
            Func<int, bool> predicate = IsEven;
            var expected = Maybe.Of(4);

            Assert.Equal(expected, source.FirstOrNone(predicate));
        }

        [Fact]
        //[t("FirstOrNone() w/ predicate always false returns none.")]
        public static void FirstOrNone12()
        {
            int[] source = { 9, 5, 1, 3, 17, 21 };
            Func<int, bool> predicate = IsEven;
            var expected = Maybe<int>.None;

            Assert.Equal(expected, source.FirstOrNone(predicate));
        }

        [Fact]
        //[t("FirstOrNone() w/ predicate returns last.")]
        public static void FirstOrNone13()
        {
            int[] source = { 9, 5, 1, 3, 17, 21, 50 };
            Func<int, bool> predicate = IsEven;
            var expected = Maybe.Of(50);

            Assert.Equal(expected, source.FirstOrNone(predicate));
        }

        [Fact]
        //[t("FirstOrNone() w/ predicate returns some (1).")]
        public static void FirstOrNone14()
        {
            int[] source = { 3, 7, 10, 7, 9, 2, 11, 17, 13, 8 };
            Func<int, bool> predicate = IsEven;
            var expected = Maybe.Of(10);

            Assert.Equal(expected, source.FirstOrNone(predicate));
        }

        [Fact]
        //[t("FirstOrNone() w/ predicate returns some (2).")]
        public static void FirstOrNone15()
        {
            int[] source = { 3, 7, 10, 7, 9, 2, 11, 17, 13, 8 };
            Func<int, bool> predicate = IsEven;
            var expected = Maybe.Of(10);

            Assert.Equal(expected, source.RunOnce().FirstOrNone(predicate));
        }
    }

    // FoldAny.
    public partial class QperatorsTests
    {
        [Fact]
        public static void FoldAny_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.FoldAny(Null, 1, Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("accumulator", () => Qperators.FoldAny(NotNull, 1, Kunc<int, int, int>.Null));

            Assert.ThrowsArgNullEx("source", () => Qperators.FoldAny(Null, 1, Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Any));
            Assert.ThrowsArgNullEx("accumulator", () => Qperators.FoldAny(NotNull, 1, Kunc<int, int, int>.Null, Funk<Maybe<int>, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => Qperators.FoldAny(NotNull, 1, Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Null));
        }
    }

    // LastOrNone.
    // Largely inspired by
    // https://github.com/dotnet/corefx/blob/master/src/System.Linq/tests/LastOrDefaultTests.cs
    public partial class QperatorsTests
    {
        [Fact]
        public static void LastOrNone_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.LastOrNone(Null));

            Assert.ThrowsArgNullEx("source", () => Qperators.LastOrNone(Null, Funk<int, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => Qperators.LastOrNone(NotNull, Funk<int, bool>.Null));
        }

        //[t("LastOrNone() for int's returns the same result when called repeatedly.")]
        [Fact]
        public static void LastOrNone1()
        {
            var q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                    where x > Int32.MinValue
                    select x;

            Assert.Equal(q.LastOrNone(), q.LastOrNone());
        }

        [Fact]
        //[t("LastOrNone() for string's returns the same result when called repeatedly.")]
        public static void LastOrNone2()
        {
            var q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", String.Empty }
                    where !String.IsNullOrEmpty(x)
                    select x;

            Assert.Equal(q.LastOrNone(), q.LastOrNone());
        }

        private static void LastOrNone3Impl<T>()
        {
            T[] source = Array.Empty<T>();
            var expected = Maybe<T>.None;

            Assert.IsAssignableFrom<IList<T>>(source);
            Assert.Equal(expected, source.RunOnce().LastOrNone());
        }

        [Fact]
        //[t("EmptyIListT")]
        public static void LastOrNone3()
        {
            LastOrNone3Impl<int>();
            LastOrNone3Impl<string>();
            LastOrNone3Impl<DateTime>();
            LastOrNone3Impl<QperatorsTests>();
        }

        [Fact]
        //[t("IListTOneElement")]
        public static void LastOrNone4()
        {
            int[] source = { 5 };
            var expected = Maybe.Of(5);

            Assert.IsAssignableFrom<IList<int>>(source);
            Assert.Equal(expected, source.LastOrNone());
        }

        [Fact]
        //[t("IListTManyElementsLastIsDefault")]
        public static void LastOrNone5()
        {
            string[] source = { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", null! };
            var expected = Maybe<string>.None;

            Assert.IsAssignableFrom<IList<string>>(source);
            Assert.Equal(expected, source.LastOrNone());
        }

        [Fact]
        //[t("IListTManyElementsLastIsNotDefault")]
        public static void LastOrNone6()
        {
            string[] source = { "!@#$%^", "C", "AAA", "", "Calling Twice", null!, "SoS" };
            var expected = Maybe.Of("SoS");

            Assert.IsAssignableFrom<IList<string>>(source);
            Assert.Equal(expected, source.LastOrNone());
        }

        private static void LastOrNone7Impl<T>()
        {
            var source = EmptySource<T>();
            var expected = Maybe<T>.None;

            Assert.Null(source as IList<T>);
            Assert.Equal(expected, source.RunOnce().LastOrNone());
        }

        [Fact]
        //[t("EmptyNotIListT")]
        public static void LastOrNone7()
        {
            LastOrNone7Impl<int>();
            LastOrNone7Impl<string>();
            LastOrNone7Impl<DateTime>();
            LastOrNone7Impl<QperatorsTests>();
        }

        [Fact]
        //[t("OneElementNotIListT")]
        public static void LastOrNone8()
        {
            IEnumerable<int> source = NumberRangeGuaranteedNotCollectionType(-5, 1);
            var expected = Maybe.Of(-5);

            Assert.Null(source as IList<int>);
            Assert.Equal(expected, source.LastOrNone());
        }

        [Fact]
        //[t("ManyElementsNotIListT")]
        public static void LastOrNone9()
        {
            IEnumerable<int> source = NumberRangeGuaranteedNotCollectionType(3, 10);
            var expected = Maybe.Of(12);

            Assert.Null(source as IList<int>);
            Assert.Equal(expected, source.LastOrNone());
        }

        [Fact]
        //[t("EmptyIListSource")]
        public static void LastOrNone10()
        {
            string[] source = Array.Empty<string>();

            Assert.Equal(Maybe<string>.None, source.LastOrNone(x => true));
            Assert.Equal(Maybe<string>.None, source.LastOrNone(x => false));
        }

        [Fact]
        //[t("OneElementIListTruePredicate")]
        public static void LastOrNone11()
        {
            int[] source = { 4 };
            Func<int, bool> predicate = IsEven;
            var expected = Maybe.Of(4);

            Assert.Equal(expected, source.LastOrNone(predicate));
        }

        [Fact]
        //[t("ManyElementsIListPredicateFalseForAll")]
        public static void LastOrNone12()
        {
            int[] source = { 9, 5, 1, 3, 17, 21 };
            Func<int, bool> predicate = IsEven;
            var expected = Maybe<int>.None;

            Assert.Equal(expected, source.LastOrNone(predicate));
        }

        [Fact]
        //[t("IListPredicateTrueOnlyForLast")]
        public static void LastOrNone13()
        {
            int[] source = { 9, 5, 1, 3, 17, 21, 50 };
            Func<int, bool> predicate = IsEven;
            var expected = Maybe.Of(50);

            Assert.Equal(expected, source.LastOrNone(predicate));
        }

        [Fact]
        //[t("IListPredicateTrueForSome")]
        public static void LastOrNone14()
        {
            int[] source = { 3, 7, 10, 7, 9, 2, 11, 18, 13, 9 };
            Func<int, bool> predicate = IsEven;
            var expected = Maybe.Of(18);

            Assert.Equal(expected, source.LastOrNone(predicate));
        }

        [Fact]
        //[t("IListPredicateTrueForSomeRunOnce")]
        public static void LastOrNone15()
        {
            int[] source = { 3, 7, 10, 7, 9, 2, 11, 18, 13, 9 };
            Func<int, bool> predicate = IsEven;
            var expected = Maybe.Of(18);

            Assert.Equal(expected, source.RunOnce().LastOrNone(predicate));
        }

        [Fact]
        //[t("EmptyNotIListSource")]
        public static void LastOrNone16()
        {
            IEnumerable<string> source = Enumerable.Repeat("value", 0);

            Assert.Equal(Maybe<string>.None, source.LastOrNone(x => true));
            Assert.Equal(Maybe<string>.None, source.LastOrNone(x => false));
        }

        [Fact]
        //[t("OneElementNotIListTruePredicate")]
        public static void LastOrNone17()
        {
            IEnumerable<int> source = ForceNotCollection(new[] { 4 });
            Func<int, bool> predicate = IsEven;
            var expected = Maybe.Of(4);

            Assert.Equal(expected, source.LastOrNone(predicate));
        }

        [Fact]
        //[t("ManyElementsNotIListPredicateFalseForAll")]
        public static void LastOrNone18()
        {
            IEnumerable<int> source = ForceNotCollection(new int[] { 9, 5, 1, 3, 17, 21 });
            Func<int, bool> predicate = IsEven;
            var expected = Maybe<int>.None;

            Assert.Equal(expected, source.LastOrNone(predicate));
        }

        [Fact]
        //[t("NotIListPredicateTrueOnlyForLast")]
        public static void LastOrNone19()
        {
            IEnumerable<int> source = ForceNotCollection(new int[] { 9, 5, 1, 3, 17, 21, 50 });
            Func<int, bool> predicate = IsEven;
            var expected = Maybe.Of(50);

            Assert.Equal(expected, source.LastOrNone(predicate));
        }

        [Fact]
        //[t("NotIListPredicateTrueForSome")]
        public static void LastOrNone20()
        {
            IEnumerable<int> source = ForceNotCollection(new int[] { 3, 7, 10, 7, 9, 2, 11, 18, 13, 9 });
            Func<int, bool> predicate = IsEven;
            var expected = Maybe.Of(18);

            Assert.Equal(expected, source.LastOrNone(predicate));
        }

        [Fact]
        //[t("NotIListPredicateTrueForSomeRunOnce")]
        public static void LastOrNone21()
        {
            IEnumerable<int> source = ForceNotCollection(new int[] { 3, 7, 10, 7, 9, 2, 11, 18, 13, 9 });
            Func<int, bool> predicate = IsEven;
            var expected = Maybe.Of(18);

            Assert.Equal(expected, source.RunOnce().LastOrNone(predicate));
        }
    }

    // ReduceAny.
    public partial class QperatorsTests
    {
        [Fact]
        public static void ReduceAny_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.ReduceAny(Null, Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("accumulator", () => Qperators.ReduceAny(NotNull, Kunc<int, int, int>.Null));

            Assert.ThrowsArgNullEx("source", () => Qperators.ReduceAny(Null, Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Any));
            Assert.ThrowsArgNullEx("accumulator", () => Qperators.ReduceAny(NotNull, Kunc<int, int, int>.Null, Funk<Maybe<int>, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => Qperators.ReduceAny(NotNull, Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Null));
        }
    }

    // SelectAny .
    public partial class QperatorsTests
    {
        [Fact]
        public static void SelectAny_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.SelectAny(Null, Kunc<int, int>.Any));
            Assert.ThrowsArgNullEx("selector", () => Qperators.SelectAny(NotNull, Kunc<int, int>.Null));
        }

        [Fact]
        public static void SelectAny_Deferred()
        {
            bool notCalled = true;
            Func<Maybe<int>> fun = () => { notCalled = false; return Maybe.Of(1); };
            var q = Enumerable.Repeat(fun, 1).SelectAny(f => f());

            Assert.True(notCalled);
            Assert.CalledOnNext(q, ref notCalled);
        }
    }

    // SingleOrNone.
    // Largely inspired by
    // https://github.com/dotnet/corefx/blob/master/src/System.Linq/tests/SingleOrDefaultTests.cs
    public partial class QperatorsTests
    {
        [Fact]
        public static void SingleOrNone_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.SingleOrNone(Null));

            Assert.ThrowsArgNullEx("source", () => Qperators.SingleOrNone(Null, Funk<int, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => Qperators.SingleOrNone(NotNull, Funk<int, bool>.Null));
        }

        [Fact]
        //[t("SingleOrNone() for int's returns the same result when called repeatedly.")]
        public static void SingleOrNone1()
        {
            var q = from x in new[] { 0.12335f }
                    select x;

            Assert.Equal(q.SingleOrNone(), q.SingleOrNone());
        }

        [Fact]
        //[t("SingleOrNone() for string's returns the same result when called repeatedly.")]
        public static void SingleOrNone2()
        {
            var q = from x in new[] { "" }
                    select x;

            Assert.Equal(q.SingleOrNone(String.IsNullOrEmpty), q.SingleOrNone(String.IsNullOrEmpty));
        }

        [Fact]
        //[t("EmptyIList")]
        public static void SingleOrNone3()
        {
            string[] source = Array.Empty<string>();
            var expected = Maybe<string>.None;

            Assert.Equal(expected, source.SingleOrNone());
        }

        [Fact]
        //[t("SingleElementIList")]
        public static void SingleOrNone4()
        {
            int[] source = { 4 };
            var expected = Maybe.Of(4);

            Assert.Equal(expected, source.SingleOrNone());
        }

        [Fact]
        //[t("ManyElementIList")]
        public static void SingleOrNone5()
        {
            int[] source = { 4, 4, 4, 4, 4 };
            var expected = Maybe<int>.None;

            // NB: SingleOrDefault() throws InvalidOperationException.
            Assert.Equal(expected, source.SingleOrNone());
        }

        [Fact]
        //[t("EmptyNotIList")]
        public static void SingleOrNone6()
        {
            IEnumerable<int> source = RepeatedNumberGuaranteedNotCollectionType(0, 0);
            var expected = Maybe<int>.None;

            Assert.Equal(expected, source.SingleOrNone());
        }

        [Fact]
        //[t("SingleElementNotIList")]
        public static void SingleOrNone7()
        {
            IEnumerable<int> source = RepeatedNumberGuaranteedNotCollectionType(-5, 1);
            var expected = Maybe.Of(-5);

            Assert.Equal(expected, source.SingleOrNone());
        }

        [Fact]
        //[t("ManyElementNotIList")]
        public static void SingleOrNone8()
        {
            IEnumerable<int> source = RepeatedNumberGuaranteedNotCollectionType(3, 5);
            var expected = Maybe<int>.None;

            // NB: SingleOrDefault() throws InvalidOperationException.
            Assert.Equal(expected, source.SingleOrNone());
        }

        [Fact]
        //[t("EmptySourceWithPredicate")]
        public static void SingleOrNone9()
        {
            int[] source = Array.Empty<int>();
            var expected = Maybe<int>.None;

            Assert.Equal(expected, source.SingleOrNone(i => i % 2 == 0));
        }

        [Fact]
        //[t("SingleElementPredicateTrue")]
        public static void SingleOrNone10()
        {
            int[] source = { 4 };
            var expected = Maybe.Of(4);

            Assert.Equal(expected, source.SingleOrNone(i => i % 2 == 0));
        }

        [Fact]
        //[t("SingleElementPredicateFalse")]
        public static void SingleOrNone11()
        {
            int[] source = { 3 };
            var expected = Maybe<int>.None;

            Assert.Equal(expected, source.SingleOrNone(i => i % 2 == 0));
        }

        [Fact]
        //[t("ManyElementsPredicateFalseForAll")]
        public static void SingleOrNone12()
        {
            int[] source = { 3, 1, 7, 9, 13, 19 };
            var expected = Maybe<int>.None;

            Assert.Equal(expected, source.SingleOrNone(i => i % 2 == 0));
        }

        [Fact]
        //[t("ManyElementsPredicateTrueForLast")]
        public static void SingleOrNone13()
        {
            int[] source = { 3, 1, 7, 9, 13, 19, 20 };
            var expected = Maybe.Of(20);

            Assert.Equal(expected, source.SingleOrNone(i => i % 2 == 0));
        }

        [Fact]
        //[t("ManyElementsPredicateTrueForFirstAndFifth")]
        public static void SingleOrNone14()
        {
            int[] source = { 2, 3, 1, 7, 10, 13, 19, 9 };
            var expected = Maybe<int>.None;

            // NB: SingleOrDefault() throws InvalidOperationException.
            Assert.Equal(expected, source.SingleOrNone(i => i % 2 == 0));
        }

        [Theory]
        //[T("FindSingleMatch")]
        [InlineData(1, 100)]
        [InlineData(42, 100)]
        public static void SingleOrNone15(int target, int range)
        {
            var expected = Maybe.Of(target);
            Assert.Equal(expected, Enumerable.Range(0, range).SingleOrNone(i => i == target));
        }

        [Theory]
        //[T("RunOnce")]
        [InlineData(1, 100)]
        [InlineData(42, 100)]
        public static void SingleOrNone16(int target, int range)
        {
            var expected = Maybe.Of(target);
            Assert.Equal(expected, Enumerable.Range(0, range).RunOnce().SingleOrNone(i => i == target));
        }
    }

    // WhereAny.
    public partial class QperatorsTests
    {
        [Fact]
        public static void WhereAny_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.WhereAny(Null, Kunc<int, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => Qperators.WhereAny(NotNull, Kunc<int, bool>.Null));
        }

        [Fact]
        public static void WhereAny_Deferred()
        {
            bool notCalled = true;
            Func<Maybe<bool>> fun = () => { notCalled = false; return Maybe.Of(true); };
            var q = Enumerable.Repeat(fun, 1).WhereAny(f => f());

            Assert.True(notCalled);
            Assert.CalledOnNext(q, ref notCalled);
        }
    }

    // ZipAny.
    public partial class QperatorsTests
    {
        [Fact]
        public static void ZipAny_InvalidArg()
        {
            Assert.ThrowsArgNullEx("first", () => Qperators.ZipAny(Null, NotNull, Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("second", () => Qperators.ZipAny(NotNull, Null, Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("resultSelector", () => Qperators.ZipAny(NotNull, NotNull, Kunc<int, int, int>.Null));
        }
    }
}
