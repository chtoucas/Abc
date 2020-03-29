﻿// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System.Collections.Generic;
    using System.Linq;

    using Xunit;

    using Assert = AssertEx;

    // Misc methods.
    public partial class MaybeTests
    {
        [Fact]
        public static void ReplaceWith()
        {
            // Arrange
            var some = Maybe.Unit;

            // Act & Assert
            Assert.Some("value", some.ReplaceWith("value"));
            Assert.None(Ø.ReplaceWith("value"));

            Assert.None(some.ReplaceWith(NullString));
            Assert.None(Ø.ReplaceWith(NullString));

#nullable disable
            Assert.Some(2, some.ReplaceWith((int?)2));
            Assert.None(Ø.ReplaceWith((int?)2));

            Assert.None(some.ReplaceWith(NullNullString));
            Assert.None(Ø.ReplaceWith(NullNullString));
#nullable restore
        }
    }

    // Helpers for Maybe<T> where T is a struct.
    public partial class MaybeTests
    {
        [Fact]
        public static void Squash()
        {
            // Arrange
            Maybe<int?> none = Maybe<int?>.None;
            Maybe<int?> one = One.Select(x => (int?)x);
            // Act & Assert
            Assert.Equal(Ø, none.Squash());
            Assert.Equal(One, one.Squash());
        }

        [Fact]
        public static void ToNullable()
        {
            // Arrange
            Maybe<int?> none = Maybe<int?>.None;
            Maybe<int?> one = One.Select(x => (int?)x);
            // Act & Assert
            Assert.Null(Ø.ToNullable());
            Assert.Equal(1, One.ToNullable());
            Assert.Null(none.ToNullable());
            Assert.Equal(1, one.ToNullable());
        }
    }

    // Helpers for Maybe<Unit>.
    public partial class MaybeTests
    {
        [Fact]
        public static void Unit() => Assert.Some(Abc.Unit.Default, Maybe.Unit);

        [Fact]
        public static void Zero() => Assert.None(Maybe.Zero);

        [Fact]
        public static void Guard()
        {
            Assert.Equal(Maybe.Zero, Maybe.Guard(false));
            Assert.Equal(Maybe.Unit, Maybe.Guard(true));
        }
    }

    // Helpers for Maybe<bool>.
    public partial class MaybeTests
    {
        [Fact]
        public static void True() => Assert.Some(Maybe.True);

        [Fact]
        public static void False() => Assert.Some(Maybe.False);

        [Fact]
        public static void Unknown() => Assert.None(Maybe.Unknown);

        [Fact]
        public static void Negate()
        {
            Assert.Some(false, Maybe.True.Negate());
            Assert.Some(true, Maybe.False.Negate());
            Assert.Unknown(Maybe.Unknown.Negate());
        }

        [Fact]
        public static void Or()
        {
            Assert.Some(true, Maybe.True.Or(Maybe.True));
            Assert.Some(true, Maybe.True.Or(Maybe.False));
            Assert.Some(true, Maybe.True.Or(Maybe.Unknown));

            Assert.Some(true, Maybe.False.Or(Maybe.True));
            Assert.Some(false, Maybe.False.Or(Maybe.False));
            Assert.Unknown(Maybe.False.Or(Maybe.Unknown));

            Assert.Some(true, Maybe.Unknown.Or(Maybe.True));
            Assert.Unknown(Maybe.Unknown.Or(Maybe.False));
            Assert.Unknown(Maybe.Unknown.Or(Maybe.Unknown));
        }

        [Fact]
        public static void And()
        {
            Assert.Some(true, Maybe.True.And(Maybe.True));
            Assert.Some(false, Maybe.True.And(Maybe.False));
            Assert.Unknown(Maybe.True.And(Maybe.Unknown));

            Assert.Some(false, Maybe.False.And(Maybe.True));
            Assert.Some(false, Maybe.False.And(Maybe.False));
            Assert.Some(false, Maybe.False.And(Maybe.Unknown));

            Assert.Unknown(Maybe.Unknown.And(Maybe.True));
            Assert.Some(false, Maybe.Unknown.And(Maybe.False));
            Assert.Unknown(Maybe.Unknown.And(Maybe.Unknown));
        }
    }

    // Helpers for Maybe<IEnumerable<T>>.
    public partial class MaybeTests
    {
        [Fact]
        public static void EmptyEnumerable()
        {
            // TODO: a better test whould not check the reference equality
            // but the equality of both sequences.
            Assert.Some(Enumerable.Empty<int>(), Maybe.EmptyEnumerable<int>());
        }

        [Fact]
        public static void CollectAny_NullSource()
        {
            Assert.ThrowsArgNullEx("source", () =>
                Maybe.CollectAny(default(IEnumerable<Maybe<int>>)!));
        }

        [Fact]
        public static void CollectAny_IsDeferred()
        {
            IEnumerable<Maybe<int>> source = new ThrowingEnumerable<Maybe<int>>();

            var q = Maybe.CollectAny(source);
            Assert.ThrowsOnNext(q);
        }

        // TODO: non-empty test.
        [Fact]
        public static void CollectAny()
        {
            Assert.Empty(Maybe.CollectAny(Enumerable.Empty<Maybe<int>>()));
        }
    }

    // Helpers for Maybe<T> where T is disposable.
    public partial class MaybeTests
    {
#pragma warning disable CA2000 // Dispose objects before losing scope

        [Fact]
        public static void Use_NullBinder()
        {
            // Arrange
            var source = Maybe.Of(new AnyDisposable());
            // Act & Assert
            Assert.ThrowsArgNullEx("binder", () =>
                source.Use(Kunc<AnyDisposable, int>.Null));
        }

        [Fact]
        public static void Use_NullSelector()
        {
            // Arrange
            var source = Maybe.Of(new AnyDisposable());
            // Act & Assert
            Assert.ThrowsArgNullEx("selector", () =>
                source.Use(Funk<AnyDisposable, int>.Null));
        }

        [Fact]
        public static void Use_Bind()
        {
            // Arrange
            var obj = new AnyDisposable();
            var source = Maybe.Of(obj);
            // Act
            Maybe<int> result = source.Use(_ => Maybe.Some(1));
            // Assert
            Assert.Some(1, result);
            Assert.True(obj.WasDisposed);
        }

        [Fact]
        public static void Use_Select()
        {
            // Arrange
            var obj = new AnyDisposable();
            var source = Maybe.Of(obj);
            // Act
            Maybe<int> result = source.Use(_ => 1);
            // Assert
            Assert.Some(1, result);
            Assert.True(obj.WasDisposed);
        }

#pragma warning restore CA2000
    }
}
