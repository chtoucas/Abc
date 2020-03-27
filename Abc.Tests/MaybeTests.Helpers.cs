﻿// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Xunit;

    using Assert = AssertEx;

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

    // Helpers for functions in the Kleisli category.
    public partial class MaybeTests
    {
        [Fact]
        public static void Invoke_NullBinder()
        {
            Assert.ThrowsArgNullEx("binder", () =>
                Kunc<AnyT, AnyResult>.Null
                    .Invoke(AnyT.Some));
        }

        [Fact]
        public static void Invoke_WithNone()
        {
            // Arrange
            var f = Thunk<AnyT>.Const(AnyResult.Some);
            // Act & Assert
            Assert.None(f.Invoke(AnyT.None));
        }

        [Fact]
        public static void Invoke_WithSome()
        {
            // Arrange
            var f = Thunk<AnyT1>.Const(AnyResult.Some);
            var g = Thunk<AnyT1>.Const(AnyResult.None);
            // Act & Assert
            Assert.Some(AnyResult.Value, f.Invoke(AnyT1.Some));
            Assert.None(g.Invoke(AnyT1.Some));
        }

        [Fact]
        public static void Compose_NullObject()
        {
            Assert.ThrowsArgNullEx("this", () =>
                Kunc<AnyT1, AnyT2>.Null
                    .Compose(Kunc<AnyT2, AnyResult>.Any));
        }

        [Fact]
        public static void Compose()
        {
            // Arrange
            var f = Thunk<AnyT1>.Const(AnyT2.Some);
            var g = Thunk<AnyT2>.Const(AnyResult.Some);
            // Act & Assert
            Assert.Some(AnyResult.Value, f.Compose(g)(AnyT1.Value));
        }

        [Fact]
        public static void ComposeBack_NullOther()
        {
            Assert.ThrowsArgNullEx("other", () =>
                Kunc<AnyT2, AnyResult>.Any
                    .ComposeBack(Kunc<AnyT1, AnyT2>.Null));
        }

        [Fact]
        public static void ComposeBack()
        {
            // Arrange
            var f = Thunk<AnyT1>.Const(AnyT2.Some);
            var g = Thunk<AnyT2>.Const(AnyResult.Some);
            // Act & Assert
            Assert.Some(AnyResult.Value, g.ComposeBack(f)(AnyT1.Value));
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

    // Lift.
    public partial class MaybeTests
    {
        [Fact]
        public static void Lift_NullSelector()
        {
            Assert.ThrowsArgNullEx("selector", () =>
                Funk<AnyT, AnyResult>.Null
                    .Lift(AnyT.Some));
        }

        [Fact]
        public static void Lift_WithNone()
        {
            // Arrange
            var source = Thunk<AnyT>.Const(AnyResult.Value);
            // Act & Assert
            Assert.None(source.Lift(AnyT.None));
        }

        [Fact]
        public static void Lift_WithSome()
        {
            // Arrange
            var source = Thunk<AnyT>.Const(AnyResult.Value);
            // Act & Assert
            Assert.Some(AnyResult.Value, source.Lift(AnyT.Some));
        }

        [Fact]
        public static void Lift2_NullObject()
        {
            Assert.ThrowsArgNullEx("this", () =>
                Funk<AnyT1, AnyT2, AnyResult>.Null
                    .Lift(AnyT1.Some, AnyT2.Some));
        }

        [Fact]
        public static void Lift2_WithNone()
        {
            // Arrange
            var source = Thunk<AnyT1, AnyT2>.Const(AnyResult.Value);
            // Act & Assert
            Assert.None(source.Lift(AnyT1.None, AnyT2.Some));
            Assert.None(source.Lift(AnyT1.Some, AnyT2.None));
        }

        [Fact]
        public static void Lift2_WithSome()
        {
            // Arrange
            var source = Thunk<AnyT1, AnyT2>.Const(AnyResult.Value);
            // Act & Assert
            Assert.Some(AnyResult.Value, source.Lift(AnyT1.Some, AnyT2.Some));
        }

        [Fact]
        public static void Lift3_NullObject()
        {
            Assert.ThrowsArgNullEx("this", () =>
                Funk<AnyT1, AnyT2, AnyT3, AnyResult>.Null
                    .Lift(AnyT1.Some, AnyT2.Some, AnyT3.Some));
        }

        [Fact]
        public static void Lift3_WithNone()
        {
            // Arrange
            var source = Thunk<AnyT1, AnyT2, AnyT3>.Const(AnyResult.Value);
            // Act & Assert
            Assert.None(source.Lift(AnyT1.None, AnyT2.Some, AnyT3.Some));
            Assert.None(source.Lift(AnyT1.Some, AnyT2.None, AnyT3.Some));
            Assert.None(source.Lift(AnyT1.Some, AnyT2.Some, AnyT3.None));
        }

        [Fact]
        public static void Lift3_WithSome()
        {
            // Arrange
            var source = Thunk<AnyT1, AnyT2, AnyT3>.Const(AnyResult.Value);
            // Act & Assert
            Assert.Some(AnyResult.Value, source.Lift(AnyT1.Some, AnyT2.Some, AnyT3.Some));
        }

        [Fact]
        public static void Lift4_NullObject()
        {
            Assert.ThrowsArgNullEx("this", () =>
                Funk<AnyT1, AnyT2, AnyT3, AnyT4, AnyResult>.Null
                    .Lift(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.Some));
        }

        [Fact]
        public static void Lift4_WithNone()
        {
            // Arrange
            var source = Thunk<AnyT1, AnyT2, AnyT3, AnyT4>.Const(AnyResult.Value);
            // Act & Assert
            Assert.None(source.Lift(AnyT1.None, AnyT2.Some, AnyT3.Some, AnyT4.Some));
            Assert.None(source.Lift(AnyT1.Some, AnyT2.None, AnyT3.Some, AnyT4.Some));
            Assert.None(source.Lift(AnyT1.Some, AnyT2.Some, AnyT3.None, AnyT4.Some));
            Assert.None(source.Lift(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.None));
        }

        [Fact]
        public static void Lift4_WithSome()
        {
            // Arrange
            var source = Thunk<AnyT1, AnyT2, AnyT3, AnyT4>.Const(AnyResult.Value);
            // Act & Assert
            Assert.Some(AnyResult.Value, source.Lift(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.Some));
        }

        [Fact]
        public static void Lift5_NullObject()
        {
            Assert.ThrowsArgNullEx("this", () =>
                Funk<AnyT1, AnyT2, AnyT3, AnyT4, AnyT5, AnyResult>.Null
                    .Lift(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.Some, AnyT5.Some));
        }

        [Fact]
        public static void Lift5_WithNone()
        {
            // Arrange
            var source = Thunk<AnyT1, AnyT2, AnyT3, AnyT4, AnyT5>.Const(AnyResult.Value);
            // Act & Assert
            Assert.None(source.Lift(AnyT1.None, AnyT2.Some, AnyT3.Some, AnyT4.Some, AnyT5.Some));
            Assert.None(source.Lift(AnyT1.Some, AnyT2.None, AnyT3.Some, AnyT4.Some, AnyT5.Some));
            Assert.None(source.Lift(AnyT1.Some, AnyT2.Some, AnyT3.None, AnyT4.Some, AnyT5.Some));
            Assert.None(source.Lift(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.None, AnyT5.Some));
            Assert.None(source.Lift(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.Some, AnyT5.None));
        }

        [Fact]
        public static void Lift5_WithSome()
        {
            // Arrange
            var source = Thunk<AnyT1, AnyT2, AnyT3, AnyT4, AnyT5>.Const(AnyResult.Value);
            // Act & Assert
            Assert.Some(AnyResult.Value,
                source.Lift(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.Some, AnyT5.Some));
        }
    }

    // Helpers for Maybe<T> where T is a function.
    public partial class MaybeTests
    {
        #region Invoke()

        [Fact]
        public static void Invoke_None()
        {
            // Arrange
            var source = Maybe<Func<AnyT, AnyResult>>.None;
            // Act & Assert
            Assert.None(source.Invoke(AnyT.Value));
        }

        [Fact]
        public static void Invoke_Some()
        {
            // Arrange
            var f = Thunk<AnyT>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.Some(AnyResult.Value, source.Invoke(AnyT.Value));
        }

        [Fact]
        public static void Invoke2_None()
        {
            // Arrange
            var source = Maybe<Func<AnyT1, AnyT2, AnyResult>>.None;
            // Act & Assert
            Assert.None(source.Invoke(AnyT1.Value, AnyT2.Value));
        }

        [Fact]
        public static void Invoke2_Some()
        {
            // Arrange
            var f = Thunk<AnyT1, AnyT2>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.Some(AnyResult.Value, source.Invoke(AnyT1.Value, AnyT2.Value));
        }

        [Fact]
        public static void Invoke3_None()
        {
            // Arrange
            var source = Maybe<Func<AnyT1, AnyT2, AnyT3, AnyResult>>.None;
            // Act & Assert
            Assert.None(source.Invoke(AnyT1.Value, AnyT2.Value, AnyT3.Value));
        }

        [Fact]
        public static void Invoke3_Some()
        {
            // Arrange
            var f = Thunk<AnyT1, AnyT2, AnyT3>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.Some(AnyResult.Value,
                    source.Invoke(AnyT1.Value, AnyT2.Value, AnyT3.Value));
        }

        [Fact]
        public static void Invoke4_None()
        {
            // Arrange
            var source = Maybe<Func<AnyT1, AnyT2, AnyT3, AnyT4, AnyResult>>.None;
            // Act & Assert
            Assert.None(source.Invoke(AnyT1.Value, AnyT2.Value, AnyT3.Value, AnyT4.Value));
        }

        [Fact]
        public static void Invoke4_Some()
        {
            // Arrange
            var f = Thunk<AnyT1, AnyT2, AnyT3, AnyT4>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.Some(AnyResult.Value,
                source.Invoke(AnyT1.Value, AnyT2.Value, AnyT3.Value, AnyT4.Value));
        }

        [Fact]
        public static void Invoke5_None()
        {
            // Arrange
            var source = Maybe<Func<AnyT1, AnyT2, AnyT3, AnyT4, AnyT5, AnyResult>>.None;
            // Act & Assert
            Assert.None(source.Invoke(
                AnyT1.Value, AnyT2.Value, AnyT3.Value, AnyT4.Value, AnyT5.Value));
        }

        [Fact]
        public static void Invoke5_Some()
        {
            // Arrange
            var f = Thunk<AnyT1, AnyT2, AnyT3, AnyT4, AnyT5>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.Some(AnyResult.Value,
                source.Invoke(AnyT1.Value, AnyT2.Value, AnyT3.Value, AnyT4.Value, AnyT5.Value));
        }

        #endregion

        #region Apply()

        [Fact]
        public static void Apply_None()
        {
            // Arrange
            var source = Maybe<Func<AnyT, AnyResult>>.None;
            // Act & Assert
            Assert.None(source.Apply(AnyT.None));
            Assert.None(source.Apply(AnyT.Some));
        }

        [Fact]
        public static void Apply_Some_WithNone()
        {
            // Arrange
            var f = Thunk<AnyT>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.None(source.Apply(AnyT.None));
        }

        [Fact]
        public static void Apply_Some_WithSome()
        {
            // Arrange
            var f = Thunk<AnyT>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.Some(AnyResult.Value, source.Apply(AnyT.Some));
        }

        [Fact]
        public static void Apply2_None()
        {
            // Arrange
            var source = Maybe<Func<AnyT1, AnyT2, AnyResult>>.None;
            // Act & Assert
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some));
            Assert.None(source.Apply(AnyT1.None, AnyT2.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.None));
        }

        [Fact]
        public static void Apply2_Some_WithNone()
        {
            // Arrange
            var f = Thunk<AnyT1, AnyT2>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.None(source.Apply(AnyT1.None, AnyT2.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.None));
        }

        [Fact]
        public static void Apply2_Some_WithSome()
        {
            // Arrange
            var f = Thunk<AnyT1, AnyT2>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.Some(AnyResult.Value, source.Apply(AnyT1.Some, AnyT2.Some));
        }

        [Fact]
        public static void Apply3_None()
        {
            // Arrange
            var source = Maybe<Func<AnyT1, AnyT2, AnyT3, AnyResult>>.None;
            // Act & Assert
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.Some));
            Assert.None(source.Apply(AnyT1.None, AnyT2.Some, AnyT3.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.None, AnyT3.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.None));
        }

        [Fact]
        public static void Apply3_Some_WithNone()
        {
            // Arrange
            var f = Thunk<AnyT1, AnyT2, AnyT3>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.None(source.Apply(AnyT1.None, AnyT2.Some, AnyT3.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.None, AnyT3.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.None));
        }

        [Fact]
        public static void Apply3_Some_WithSome()
        {
            // Arrange
            var f = Thunk<AnyT1, AnyT2, AnyT3>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.Some(AnyResult.Value, source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.Some));
        }

        [Fact]
        public static void Apply4_None()
        {
            // Arrange
            var source = Maybe<Func<AnyT1, AnyT2, AnyT3, AnyT4, AnyResult>>.None;
            // Act & Assert
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.Some));
            Assert.None(source.Apply(AnyT1.None, AnyT2.Some, AnyT3.Some, AnyT4.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.None, AnyT3.Some, AnyT4.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.None, AnyT4.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.None));
        }

        [Fact]
        public static void Apply4_Some_WithNone()
        {
            // Arrange
            var f = Thunk<AnyT1, AnyT2, AnyT3, AnyT4>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.None(source.Apply(AnyT1.None, AnyT2.Some, AnyT3.Some, AnyT4.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.None, AnyT3.Some, AnyT4.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.None, AnyT4.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.None));
        }

        [Fact]
        public static void Apply4_Some_WithSome()
        {
            // Arrange
            var f = Thunk<AnyT1, AnyT2, AnyT3, AnyT4>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.Some(AnyResult.Value,
                source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.Some));
        }

        [Fact]
        public static void Apply5_None()
        {
            // Arrange
            var source = Maybe<Func<AnyT1, AnyT2, AnyT3, AnyT4, AnyT5, AnyResult>>.None;
            // Act & Assert
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.Some, AnyT5.Some));
            Assert.None(source.Apply(AnyT1.None, AnyT2.Some, AnyT3.Some, AnyT4.Some, AnyT5.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.None, AnyT3.Some, AnyT4.Some, AnyT5.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.None, AnyT4.Some, AnyT5.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.None, AnyT5.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.Some, AnyT5.None));
        }

        [Fact]
        public static void Apply5_Some_WithNone()
        {
            // Arrange
            var f = Thunk<AnyT1, AnyT2, AnyT3, AnyT4, AnyT5>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.None(source.Apply(AnyT1.None, AnyT2.Some, AnyT3.Some, AnyT4.Some, AnyT5.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.None, AnyT3.Some, AnyT4.Some, AnyT5.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.None, AnyT4.Some, AnyT5.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.None, AnyT5.Some));
            Assert.None(source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.Some, AnyT5.None));
        }

        [Fact]
        public static void Apply5_Some_WithSome()
        {
            // Arrange
            var f = Thunk<AnyT1, AnyT2, AnyT3, AnyT4, AnyT5>.Const(AnyResult.Value);
            var source = Maybe.Of(f);
            // Act & Assert
            Assert.Some(AnyResult.Value,
                source.Apply(AnyT1.Some, AnyT2.Some, AnyT3.Some, AnyT4.Some, AnyT5.Some));
        }

        #endregion
    }
}
