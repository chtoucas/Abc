// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Linq;

    using Xunit;

    using static global::My;

    using Assert = AssertEx;

    public static partial class MaybeTests
    {
        private static readonly Maybe<int> ZERO = Maybe<int>.None;

        private static readonly Maybe<int> Ø = Maybe<int>.None;
        private static readonly Maybe<int> One = Maybe.Some(1);
        private static readonly Maybe<int> Two = Maybe.Some(2);
        private static readonly Maybe<long> ØL = Maybe<long>.None;
        private static readonly Maybe<long> TwoL = Maybe.Some(2L);
    }

    // Construction, properties.
    public partial class MaybeTests
    {
        [Fact]
        public static void Default()
        {
            Assert.None(default(Maybe<Unit>));
            Assert.None(default(Maybe<int>));
            Assert.None(default(Maybe<string>));
        }

        [Fact]
        public static void Unit()
        {
            Assert.Some(Maybe.Unit);
        }

        [Fact]
        public static void Zero()
        {
            Assert.None(Maybe.Zero);
        }

        [Fact]
        public static void IsNone()
        {
            Assert.True(Maybe<int>.None.IsNone);
            Assert.True(Maybe<int?>.None.IsNone);
            Assert.True(Maybe<string>.None.IsNone);
            Assert.True(Maybe<string?>.None.IsNone);
        }

        [Fact]
        public static void Of_Reference()
        {
            Assert.None(Maybe.Of((string?)null));
            Assert.None(Maybe.Of(NullString));

            Assert.Some(Maybe.Of("value"));
            Assert.Some(Maybe.Of((string?)"value"));
        }

        [Fact]
        public static void Of_Value()
        {
            Assert.Some(Maybe.Of(1));

#pragma warning disable CS0618 // Type or member is obsolete
            Assert.None(Maybe.Of((int?)null));
            Assert.Some(Maybe.Of((int?)1));
#pragma warning restore CS0618
        }

        [Fact]
        public static void Some()
        {
            Assert.Some(Maybe.Some(1));
        }
    }

    // Operators.
    public partial class MaybeTests
    {
        [Fact]
        public static void ImplicitToMaybe()
        {
            // Arrange
            Maybe<string> maybe = NullString; // implicit cast of a null-string

            // Act & Assert
            Assert.Some(1, 1);      // the second 1 is implicit casted to Maybe<int>
            Assert.None(maybe);

            Assert.True(1 == One);
            Assert.True(One == 1);
        }

        [Fact]
        public static void ExplicitFromMaybe()
        {
            Assert.Equal(1, (int)One);

            Assert.Throws<InvalidCastException>(() => (string)Maybe<string>.None);
        }

        [Fact]
        public static void BitwiseOr()
        {
            // Some Some -> Some
            Assert.Equal(One, One | Two);
            Assert.Equal(Two, Two | One);
            // Some None -> Some
            Assert.Equal(One, One | Ø);
            // None Some -> Some
            Assert.Equal(Two, Ø | Two);
            // None None -> None
            Assert.Equal(Ø, Ø | Ø);
        }

        [Fact]
        public static void BitwiseAnd()
        {
            // Some Some -> Some
            Assert.Equal(Two, One & Two);
            Assert.Equal(One, Two & One);
            // Some None -> None
            Assert.Equal(Ø, One & Ø);
            // None Some -> None
            Assert.Equal(Ø, Ø & Two);
            // None None -> None
            Assert.Equal(Ø, Ø & Ø);
        }

        [Fact]
        public static void ExclusiveOr()
        {
            // Some Some -> None
            Assert.Equal(Ø, One ^ Two);
            // Some None -> Some
            Assert.Equal(One, One ^ Ø);
            // None Some -> Some
            Assert.Equal(Two, Ø ^ Two);
            // None None -> None
            Assert.Equal(Ø, Ø ^ Ø);

            Assert.False(One ^ Two);
            Assert.True(One ^ Ø);
            Assert.True(Ø ^ Two);
            Assert.False(Ø ^ Ø);
        }

        [Fact]
        public static void LogicalOr()
        {
            // Some Some -> Some
            Assert.Equal(One, One || Two);
            Assert.Equal(Two, Two || One);      // non-abelian!
            // Some None -> Some
            Assert.Equal(One, One || Ø);
            // None Some -> Some
            Assert.Equal(Two, Ø || Two);
            // None None -> None
            Assert.Equal(Ø, Ø || Ø);

            Assert.True(One || Two);
            Assert.True(One || Ø);
            Assert.True(Ø || Two);
            Assert.False(Ø || Ø);
        }

        [Fact]
        public static void LogicalAnd()
        {
            // Some Some -> Some
            Assert.Equal(Two, One && Two);
            Assert.Equal(One, Two && One);      // non-abelian!
            // Some None -> None
            Assert.Equal(Ø, One && Ø);
            // None Some -> None
            Assert.Equal(Ø, Ø && Two);
            // None None -> None
            Assert.Equal(Ø, Ø && Ø);

            Assert.True(One && Two);
            Assert.False(One && Ø);
            Assert.False(Ø && Two);
            Assert.False(Ø && Ø);
        }
    }

    // Core methods.
    public partial class MaybeTests
    {
        [Fact]
        public static void Bind_InvalidBinder()
        {
            Assert.ThrowsArgNullEx("binder", () => Ø.Bind((Func<int, Maybe<string>>)null!));
            Assert.ThrowsArgNullEx("binder", () => One.Bind((Func<int, Maybe<string>>)null!));
        }

        [Fact]
        public static void OrElse()
        {
            // Some Some -> Some
            Assert.Equal(One, One.OrElse(Two));
            // Some None -> Some
            Assert.Equal(One, One.OrElse(Ø));
            // None Some -> Some
            Assert.Equal(Two, Ø.OrElse(Two));
            // None None -> None
            Assert.Equal(Ø, Ø.OrElse(Ø));
        }
    }

    // Rules, sanity checks. A bit limited, maybe we could use fuzz testing.
    public partial class MaybeTests
    {
        //
        // Functor rules.
        //

        // First Functor Law: the identity map is a fixed point for Select.
        //   fmap id  ==  id
        [Fact]
        public static void Functor_FirstLaw()
        {
            Assert.Equal(Ø, Ø.Select(x => x));
            Assert.Equal(One, One.Select(x => x));
        }

        // Second Functor Law: Select preserves the composition operator.
        //   fmap (f . g)  ==  fmap f . fmap g
        [Fact]
        public static void Functor_SecondLaw()
        {
            Func<int, long> g = x => 2L * x;
            Func<long, string> f = x => $"{x}";

            Assert.Equal(
                Ø.Select(x => f(g(x))),
                Ø.Select(g).Select(f));

            Assert.Equal(
                One.Select(x => f(g(x))),
                One.Select(g).Select(f));
        }

        //
        // Monoid rules.
        // We use additive notations: + is OrElse(), zero is None.
        // 1) zero + x = x
        // 2) x + zero = x
        // 3) x + (y + z) = (x + y) + z
        // TODO: fourth law
        // mconcat = foldr '(<>)' mempty
        //

        // First Monoid Law: None is a left identity for OrElse().
        [Fact]
        public static void OrElse_LeftIdentity()
        {
            Assert.Equal(Ø, ZERO.OrElse(Ø));
            Assert.Equal(One, ZERO.OrElse(One));
        }

        // Second Monoid Law: None is a right identity for OrElse().
        [Fact]
        public static void OrElse_RightIdentity()
        {
            Assert.Equal(Ø, Ø.OrElse(ZERO));
            Assert.Equal(One, One.OrElse(ZERO));
        }

        // Third Monoid Law: OrElse() is associative.
        [Fact]
        public static void OrElse_Associativity()
        {
            Assert.Equal(
                Ø.OrElse(One.OrElse(Two)),
                Ø.OrElse(One).OrElse(Two));

            Assert.Equal(
                One.OrElse(Two.OrElse(Ø)),
                One.OrElse(Two).OrElse(Ø));

            Assert.Equal(
                Two.OrElse(Ø.OrElse(One)),
                Two.OrElse(Ø).OrElse(One));
        }

        //
        // MonadZero rules.
        //

        // MonadZero: None is a left zero for Bind().
        //   mzero >>= f = mzero
        [Fact]
        public static void Bind_LeftZero()
        {
            Func<int, Maybe<int>> f = x => Maybe.Some(2 * x);

            Assert.Equal(ZERO, ZERO.Bind(f));
        }

        // MonadMore: None is a right zero for Bind() or equivalently None is a
        // right zero for AndThen().

        // MonadMore: None is a right zero for AndThen(), implied by the
        // definition of AndThen() and the MonadMore rule.

        // MonadPlus: Bind() is right distributive over OrElse().

        // MonadOr: Unit is a left zero for OrElse().

        // Unit is a right zero for OrElse().

        // AndThen() is associative, implied by the definition of AndThen() and
        // the third monad law.
        //   (m >> n) >> o = m >> (n >> o)
        [Fact]
        public static void AndThen_Associativity()
        {
            Assert.Equal(
                Ø.AndThen(One.AndThen(Two)),
                Ø.AndThen(One).AndThen(Two));

            Assert.Equal(
                One.AndThen(Two.AndThen(Ø)),
                One.AndThen(Two).AndThen(Ø));

            Assert.Equal(
                Two.AndThen(Ø.AndThen(One)),
                Two.AndThen(Ø).AndThen(One));
        }
    }

    // Query Expression Pattern.
    public partial class MaybeTests
    {
        [Fact]
        public static void Select_InvalidSelector()
        {
            Assert.ThrowsArgNullEx("selector", () => Ø.Select((Func<int, string>)null!));
            Assert.ThrowsArgNullEx("selector", () => One.Select((Func<int, string>)null!));
        }

        [Fact]
        public static void Select()
        {
            Assert.None(Ø.Select(x => x));
            Assert.None(from x in Ø select x);

            Assert.Some(2, One.Select(x => 2 * x));
            Assert.Some(2, from x in One select 2 * x);
        }

        [Fact]
        public static void Where_InvalidPredicate()
        {
            Assert.ThrowsArgNullEx("predicate", () => Ø.Where(null!));
            Assert.ThrowsArgNullEx("predicate", () => One.Where(null!));
        }

        [Fact]
        public static void Where()
        {
            // None.Where(false) -> None
            Assert.None(Ø.Where(_ => true));
            Assert.None(from x in Ø where true select x);
            // None.Where(true) -> None
            Assert.None(Ø.Where(_ => false));
            Assert.None(from x in Ø where false select x);

            // Some.Where(false) -> None
            Assert.None(One.Where(x => x == 2));
            Assert.None(from x in One where x == 2 select x);
            // Some.Where(true) -> Some
            Assert.Some(1, One.Where(x => x == 1));
            Assert.Some(1, from x in One where x == 1 select x);
        }

        [Fact]
        public static void SelectMany()
        {
            // None.SelectMany(None) -> None
            Assert.None(Ø.SelectMany(_ => Ø, (i, j) => i + j));
            Assert.None(from i in Ø from j in Ø select i + j);
            // None.SelectMany(Some) -> None
            Assert.None(Ø.SelectMany(i => Maybe.Some(2 * i), (i, j) => i + j));
            Assert.None(from i in Ø from j in Maybe.Some(2 * i) select i + j);
            // Some.SelectMany(None) -> None
            Assert.None(One.SelectMany(_ => Ø, (i, j) => i + j));
            Assert.None(from i in One from j in Ø select i + j);

            // Some.SelectMany(Some) -> Some
            Assert.Some(3, One.SelectMany(i => Maybe.Some(2 * i), (i, j) => i + j));
            Assert.Some(3, from i in One from j in Maybe.Some(2 * i) select i + j);
        }

        [Fact]
        public static void Join()
        {
            Assert.None(One.Join(Two, i => i, i => i, (i, j) => i + j));
            Assert.None(from i in One join j in Two on i equals j select i + j);
        }
    }

    // Misc methods.
    public partial class MaybeTests
    {
        [Fact]
        public static void ZipWith()
        {
            // Some Some -> Some
            Assert.Some(3L, One.ZipWith(TwoL, (i, j) => i + j));
            // Some None -> None
            Assert.None(One.ZipWith(ØL, (i, j) => i + j));
            // None Some -> None
            Assert.None(Ø.ZipWith(TwoL, (i, j) => i + j));
            // None None -> None
            Assert.None(Ø.ZipWith(ØL, (i, j) => i + j));
        }

        [Fact]
        public static void AndThen()
        {
            // Some Some -> Some
            Assert.Equal(TwoL, One.AndThen(TwoL));
            // Some None -> None
            Assert.Equal(ØL, One.AndThen(ØL));
            // None Some -> None
            Assert.Equal(ØL, Ø.AndThen(TwoL));
            // None None -> None
            Assert.Equal(ØL, Ø.AndThen(ØL));
        }

        [Fact]
        public static void ZeroOutWhen()
        {
            // Some Some -> None
            Assert.Equal(Ø, One.ZeroOutWhen(TwoL));
            // Some None -> Some
            Assert.Equal(One, One.ZeroOutWhen(ØL));
            // None Some -> None
            Assert.Equal(Ø, Ø.ZeroOutWhen(TwoL));
            // None None -> None
            Assert.Equal(Ø, Ø.ZeroOutWhen(ØL));
        }

        [Fact]
        public static void XorElse()
        {
            // Some Some -> None
            Assert.Equal(Ø, One.XorElse(Two));
            // Some None -> Some
            Assert.Equal(One, One.XorElse(Ø));
            // None Some -> Some
            Assert.Equal(Two, Ø.XorElse(Two));
            // None None -> None
            Assert.Equal(Ø, Ø.XorElse(Ø));

            // XorElse() flips to itself.
            Assert.Equal(Ø, Two.XorElse(One));
            Assert.Equal(One, Ø.XorElse(One));
            Assert.Equal(Two, Two.XorElse(Ø));
            Assert.Equal(Ø, Ø.XorElse(Ø));
        }

        [Fact]
        public static void Skip()
        {
            Assert.Equal(Maybe.Zero, Ø.Skip());
            Assert.Equal(Maybe.Unit, One.Skip());
        }
    }

    // Iterable.
    public partial class MaybeTests
    {
        [Fact]
        public static void ToEnumerable()
        {
            // Arrange
            var some = Maybe.Of("value");
            var none = Maybe<string>.None;
            // Act & Assert
            Assert.Equal(Enumerable.Repeat("value", 1), some.ToEnumerable());
            Assert.Empty(none.ToEnumerable());
        }

        [Fact]
        public static void GetEnumerator_Some()
        {
            var some = Maybe.Of("value");
            int count = 0;

            foreach (string x in some) { count++; Assert.Equal("value", x); }

            Assert.Equal(1, count);
        }

        [Fact]
        public static void GetEnumerator_None()
        {
            var none = Maybe<string>.None;
            int count = 0;

            foreach (string x in none) { count++; }

            Assert.Equal(0, count);
        }
    }

    // Comparison
    public partial class MaybeTests
    {
        [Fact]
        public static void Comparison_WithNone_ReturnFalse()
        {
            Assert.False(One < Ø);
            Assert.False(One > Ø);
            Assert.False(One <= Ø);
            Assert.False(One >= Ø);

            // The other way around.
            Assert.False(Ø < One);
            Assert.False(Ø > One);
            Assert.False(Ø <= One);
            Assert.False(Ø >= One);

#pragma warning disable CS1718 // Comparison made to same variable
            Assert.False(Ø < Ø);
            Assert.False(Ø > Ø);
            Assert.False(Ø <= Ø);
            Assert.False(Ø >= Ø);
#pragma warning restore CS1718
        }

        [Fact]
        public static void CompareTo_WithNone()
        {
            Assert.Equal(1, One.CompareTo(Ø));
            Assert.Equal(-1, Ø.CompareTo(One));
            Assert.Equal(0, Ø.CompareTo(Ø));
        }
    }
}
