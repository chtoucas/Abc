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

    // Instance methods (eager validation).
    public partial class MaybeTests
    {
        #region BindAsync()

        [Fact]
        public static void BindAsync_None_NullBinder() =>
            Assert.ThrowsAnexn("binder", () =>
                Ø.BindAsync(Kunc<int, AnyT>.NullAsync));

        [Fact]
        public static void BindAsync_Some_NullBinder() =>
            Assert.ThrowsAnexn("binder", () =>
                One.BindAsync(Kunc<int, AnyT>.NullAsync));

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

        [Fact]
        public static async Task OrElseAsync_None()
        {
            await Assert.Async.Some(3,
                Ø.OrElseAsync(Thunk.ReturnAsync_(3)));

            await Assert.Async.Some("other",
                NoText.OrElseAsync(Thunk.ReturnAsync_("other")));

            var otherUri = new Uri("https://source.dot.net/");
            await Assert.Async.Some(otherUri,
                NoUri.OrElseAsync(Thunk.ReturnAsync_(otherUri)));

            var otherAnyT = AnyT.Value;
            await Assert.Async.Some(otherAnyT,
                AnyT.None.OrElseAsync(Thunk.ReturnAsync_(otherAnyT)));
        }

        [Fact]
        public static async Task OrElseAsync_Some()
        {
            var anyT = AnyT.New();
            await Assert.Async.Some(anyT.Value,
                anyT.Some.OrElseAsync(Thunk.ReturnAsync_(AnyT.Value)));
        }

        [Fact]
        public static async Task OrElseAsync_SomeInt32() =>
            await Assert.Async.Some(1,
                One.OrElseAsync(Thunk.ReturnAsync_(3)));

        [Fact]
        public static async Task OrElseAsync_SomeText() =>
            await Assert.Async.Some(MyText,
                SomeText.OrElseAsync(Thunk.ReturnAsync_("other")));

        [Fact]
        public static async Task OrElseAsync_SomeUri()
        {
            var otherUri = new Uri("https://source.dot.net/");
            await Assert.Async.Some(MyUri,
                SomeUri.OrElseAsync(Thunk.ReturnAsync_(otherUri)));
        }

        #endregion
    }

    // Extension methods (delayed validation).
    public partial class MaybeTests
    {
        public static class HelperClass
        {
            #region BindAsync()

            [Fact]
            public static async Task BindAsync_None_NullBinder()
            {
                await Assert.Async
                    .ThrowsAnexn("binder", () =>
                        MaybeEx.BindAsync(Ø, Kunc<int, AnyResult>.NullAsync));
                await Assert.Async
                    .ThrowsAnexn("binder", () =>
                        MaybeEx.BindAsync(NoText, Kunc<string, AnyResult>.NullAsync));
                await Assert.Async
                    .ThrowsAnexn("binder", () =>
                        MaybeEx.BindAsync(NoUri, Kunc<Uri, AnyResult>.NullAsync));
                await Assert.Async
                    .ThrowsAnexn("binder", () =>
                        MaybeEx.BindAsync(AnyT.None, Kunc<AnyT, AnyResult>.NullAsync));
            }

            [Fact]
            public static async Task BindAsync_Some_NullBinder()
            {
                await Assert.Async
                    .ThrowsAnexn("binder", () =>
                        MaybeEx.BindAsync(One, Kunc<int, AnyResult>.NullAsync));
                await Assert.Async
                    .ThrowsAnexn("binder", () =>
                        MaybeEx.BindAsync(SomeText, Kunc<string, AnyResult>.NullAsync));
                await Assert.Async
                    .ThrowsAnexn("binder", () =>
                        MaybeEx.BindAsync(SomeUri, Kunc<Uri, AnyResult>.NullAsync));
                await Assert.Async
                    .ThrowsAnexn("binder", () =>
                        MaybeEx.BindAsync(AnyT.Some, Kunc<AnyT, AnyResult>.NullAsync));
            }

            #endregion

            #region SelectAsync()

            [Fact]
            public static async Task SelectAsync_None_NullSelector()
            {
                await Assert.Async
                    .ThrowsAnexn("selector", () =>
                        MaybeEx.SelectAsync(Ø, Funk<int, AnyResult>.NullAsync));
                await Assert.Async
                    .ThrowsAnexn("selector", () =>
                        MaybeEx.SelectAsync(NoText, Funk<string, AnyResult>.NullAsync));
                await Assert.Async
                    .ThrowsAnexn("selector", () =>
                        MaybeEx.SelectAsync(NoUri, Funk<Uri, AnyResult>.NullAsync));
                await Assert.Async
                    .ThrowsAnexn("selector", () =>
                        MaybeEx.SelectAsync(AnyT.None, Funk<AnyT, AnyResult>.NullAsync));
            }

            [Fact]
            public static async Task SelectAsync_Some_NullSelector()
            {
                await Assert.Async
                    .ThrowsAnexn("selector", () =>
                        MaybeEx.SelectAsync(One, Funk<int, AnyResult>.NullAsync));
                await Assert.Async
                    .ThrowsAnexn("selector", () =>
                        MaybeEx.SelectAsync(SomeText, Funk<string, AnyResult>.NullAsync));
                await Assert.Async
                    .ThrowsAnexn("selector", () =>
                        MaybeEx.SelectAsync(SomeUri, Funk<Uri, AnyResult>.NullAsync));
                await Assert.Async
                    .ThrowsAnexn("selector", () =>
                        MaybeEx.SelectAsync(AnyT.Some, Funk<AnyT, AnyResult>.NullAsync));
            }

            #endregion

            #region OrElseAsync()

            [Fact]
            public static async Task OrElseAsync_None_NullOther()
            {
                await Assert.Async
                    .ThrowsAnexn("other", () => MaybeEx.OrElseAsync(Ø, null!));
                await Assert.Async
                    .ThrowsAnexn("other", () => MaybeEx.OrElseAsync(NoText, null!));
                await Assert.Async
                    .ThrowsAnexn("other", () => MaybeEx.OrElseAsync(NoUri, null!));
                await Assert.Async
                    .ThrowsAnexn("other", () => MaybeEx.OrElseAsync(AnyT.None, null!));
            }

            [Fact]
            public static async Task OrElseAsync_Some_NullOther()
            {
                await Assert.Async
                    .ThrowsAnexn("other", () => MaybeEx.OrElseAsync(One, null!));
                await Assert.Async
                    .ThrowsAnexn("other", () => MaybeEx.OrElseAsync(SomeText, null!));
                await Assert.Async
                    .ThrowsAnexn("other", () => MaybeEx.OrElseAsync(SomeUri, null!));
                await Assert.Async
                    .ThrowsAnexn("other", () => MaybeEx.OrElseAsync(AnyT.Some, null!));
            }

            #endregion
        }
    }
}
