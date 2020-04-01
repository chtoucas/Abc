// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Threading.Tasks;

    using Xunit;

    using Assert = AssertEx;

    // TODO: figure out async tests.
    // https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/november/async-programming-unit-testing-asynchronous-code
    // https://bradwilson.typepad.com/blog/2012/01/xunit19.html

    // Async methods.
    // Notes:
    // - do not append ConfigureAwait(false), this is not necessary; see
    //   https://github.com/xunit/xunit/issues/1215
    // - use Task.Yield() to force asynchronicity.
    public partial class MaybeTests { }

    public partial class MaybeTests
    {
        #region BindAsync()

        [Fact]
        public static void BindAsync_None_NullBinder() =>
            Assert.ThrowsAnexn("binder", () =>
                Ø.BindAsync(Funk<int, Maybe<AnyT>>.NullAsync));

        [Fact]
        public static void BindAsync_Some_NullBinder() =>
            Assert.ThrowsAnexn("binder", () =>
                One.BindAsync(Funk<int, Maybe<AnyT>>.NullAsync));

        [Fact]
        public static async Task BindAsync_None()
        {
            await Assert.Async.None(Ø.BindAsync(ReturnSomeAsync));
            await Assert.Async.None(NoText.BindAsync(ReturnSomeAsync));
            await Assert.Async.None(NoUri.BindAsync(ReturnSomeAsync));
            await Assert.Async.None(AnyT.None.BindAsync(ReturnSomeAsync));
        }

        [Fact]
        public static async Task BindAsync_Some_ReturnsNone()
        {
            await Assert.Async.None(One.BindAsync(ReturnNoneAsync));
            await Assert.Async.None(SomeText.BindAsync(ReturnNoneAsync));
            await Assert.Async.None(SomeUri.BindAsync(ReturnNoneAsync));
            await Assert.Async.None(AnyT.Some.BindAsync(ReturnNoneAsync));
        }

        [Fact]
        public static async Task BindAsync_Some_ReturnsSome()
        {
            await Assert.Async.Some(AnyResult.Value, One.BindAsync(ReturnSomeAsync));
            await Assert.Async.Some(AnyResult.Value, SomeText.BindAsync(ReturnSomeAsync));
            await Assert.Async.Some(AnyResult.Value, SomeUri.BindAsync(ReturnSomeAsync));
            await Assert.Async.Some(AnyResult.Value, AnyT.Some.BindAsync(ReturnSomeAsync));
        }

        [Fact]
        public static async Task BindAsync_SomeInt32() =>
            await Assert.Async.Some(6, Two.BindAsync(Times3Async_));

        [Fact]
        public static async Task BindAsync_SomeInt64() =>
            await Assert.Async.Some(8L, TwoL.BindAsync(Times4Async_));

        [Fact]
        public static async Task BindAsync_SomeUri() =>
            await Assert.Async.Some(MyUri.AbsoluteUri, SomeUri.BindAsync(GetAbsoluteUriAsync_));

        #endregion

        #region SelectAsync()

        [Fact]
        public static void SelectAsync_None_NullSelector()
        {
            Assert.ThrowsAnexn("selector", () =>
                Ø.SelectAsync(Funk<int, AnyResult>.NullAsync));
            Assert.ThrowsAnexn("selector", () =>
                NoText.SelectAsync(Funk<string, AnyResult>.NullAsync));
            Assert.ThrowsAnexn("selector", () =>
                NoUri.SelectAsync(Funk<Uri, AnyResult>.NullAsync));
            Assert.ThrowsAnexn("selector", () =>
                AnyT.None.SelectAsync(Funk<AnyT, AnyResult>.NullAsync));
        }

        [Fact]
        public static void SelectAsync_Some_NullSelector()
        {
            Assert.ThrowsAnexn("selector", () =>
                One.SelectAsync(Funk<int, AnyResult>.NullAsync));
            Assert.ThrowsAnexn("selector", () =>
                SomeText.SelectAsync(Funk<string, AnyResult>.NullAsync));
            Assert.ThrowsAnexn("selector", () =>
                SomeUri.SelectAsync(Funk<Uri, AnyResult>.NullAsync));
            Assert.ThrowsAnexn("selector", () =>
                AnyT.Some.SelectAsync(Funk<AnyT, AnyResult>.NullAsync));
        }

        [Fact]
        public static async Task SelectAsync_None()
        {
            await Assert.Async.None(Ø.SelectAsync(IdentAsync));
            await Assert.Async.None(NoText.SelectAsync(IdentAsync));
            await Assert.Async.None(NoUri.SelectAsync(IdentAsync));
            await Assert.Async.None(AnyT.None.SelectAsync(IdentAsync));
        }

        [Fact]
        public static async Task SelectAsync_SomeInt32() =>
            await Assert.Async.Some(6, Two.SelectAsync(Times3Async));

        [Fact]
        public static async Task SelectAsync_SomeInt64() =>
            await Assert.Async.Some(8L, TwoL.SelectAsync(Times4Async));

        [Fact]
        public static async Task SelectAsync_SomeUri() =>
            await Assert.Async.Some(MyUri.AbsoluteUri, SomeUri.SelectAsync(GetAbsoluteUriAsync));

        #endregion

        #region OrElseAsync()

        [Fact]
        public static void OrElseAsync_None_NullOther()
        {
            Assert.ThrowsAnexn("other", () => Ø.OrElseAsync(null!));
            Assert.ThrowsAnexn("other", () => NoText.OrElseAsync(null!));
            Assert.ThrowsAnexn("other", () => NoUri.OrElseAsync(null!));
            Assert.ThrowsAnexn("other", () => AnyT.None.OrElseAsync(null!));
        }

        [Fact]
        public static void OrElseAsync_Some_NullOther()
        {
            Assert.ThrowsAnexn("other", () => One.OrElseAsync(null!));
            Assert.ThrowsAnexn("other", () => SomeText.OrElseAsync(null!));
            Assert.ThrowsAnexn("other", () => SomeUri.OrElseAsync(null!));
            Assert.ThrowsAnexn("other", () => AnyT.Some.OrElseAsync(null!));
        }

        #endregion
    }

    public partial class MaybeTests
    {
        public static class HelperClass
        {
            #region BindAsync()

            [Fact]
            public static async Task BindAsync_None_NullBinder() =>
                await Assert.Async
                    .ThrowsAnexn("binder", () =>
                        MaybeEx.BindAsync(Ø, Kunc<int, AnyT>.NullAsync));

            [Fact]
            public static async Task BindAsync_Some_NullBinder() =>
                await Assert.Async
                    .ThrowsAnexn("binder", () =>
                        MaybeEx.BindAsync(One, Kunc<int, AnyT>.NullAsync));

            #endregion

            #region SelectAsync()

            [Fact]
            public static async Task SelectAsync_None_NullSelector() =>
                await Assert.Async
                    .ThrowsAnexn("selector", () =>
                        MaybeEx.SelectAsync(Ø, Funk<int, AnyT>.NullAsync));

            [Fact]
            public static async Task SelectAsync_Some_NullSelector() =>
                await Assert.Async
                    .ThrowsAnexn("selector", () =>
                        MaybeEx.SelectAsync(One, Funk<int, AnyT>.NullAsync));

            #endregion

            #region OrElseAsync()

            [Fact]
            public static async Task OrElseAsync_None_NullOther() =>
                await Assert.Async
                    .ThrowsAnexn("other", () => MaybeEx.OrElseAsync(Ø, null!));

            [Fact]
            public static async Task OrElseAsync_Some_NullOther() =>
                await Assert.Async
                    .ThrowsAnexn("other", () => MaybeEx.OrElseAsync(One, null!));

            #endregion
        }
    }
}
