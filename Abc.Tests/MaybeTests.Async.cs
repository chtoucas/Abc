// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System.Threading.Tasks;

    using Xunit;

    using Assert = AssertEx;

    // TODO: figure out async tests.
    // ConfigureAwait(false) or ConfigureAwait(true)?
    // - It seems that the answer is no: https://github.com/xunit/xunit/issues/1215
    // - AssertEx.
    // - https://github.com/xunit/xunit/issues/507
    // - https://github.com/xunit/xunit/issues/1880
    // https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/november/async-programming-unit-testing-asynchronous-code
    // https://bradwilson.typepad.com/blog/2012/01/xunit19.html

    // Async methods.
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
            await Assert.Async.None(Ø.BindAsync(AsyncFakes.ReturnSomeAsync));
            await Assert.Async.None(NoText.BindAsync(AsyncFakes.ReturnSomeAsync));
            await Assert.Async.None(NoUri.BindAsync(AsyncFakes.ReturnSomeAsync));
            await Assert.Async.None(AnyT.None.BindAsync(AsyncFakes.ReturnSomeAsync));
        }

        [Fact]
        public static async Task BindAsync_Some()
        {
            await Assert.Async
                .Some(AnyResult.Value, One.BindAsync(AsyncFakes.ReturnSomeAsync));
            await Assert.Async
                .Some(AnyResult.Value, SomeText.BindAsync(AsyncFakes.ReturnSomeAsync));
            await Assert.Async
                .Some(AnyResult.Value, SomeUri.BindAsync(AsyncFakes.ReturnSomeAsync));
            await Assert.Async
                .Some(AnyResult.Value, AnyT.Some.BindAsync(AsyncFakes.ReturnSomeAsync));

            await Assert.Async.None(One.BindAsync(AsyncFakes.ReturnNoneAsync));
            await Assert.Async.None(SomeText.BindAsync(AsyncFakes.ReturnNoneAsync));
            await Assert.Async.None(SomeUri.BindAsync(AsyncFakes.ReturnNoneAsync));
            await Assert.Async.None(AnyT.Some.BindAsync(AsyncFakes.ReturnNoneAsync));
        }

        #endregion

        #region SelectAsync()

        [Fact]
        public static void SelectAsync_None_NullSelector() =>
            Assert.ThrowsAnexn("selector", () =>
                Ø.SelectAsync(Funk<int, AnyT>.NullAsync));

        [Fact]
        public static void SelectAsync_Some_NullSelector() =>
            Assert.ThrowsAnexn("selector", () =>
                One.SelectAsync(Funk<int, AnyT>.NullAsync));

        #endregion

        #region OrElseAsync()

        [Fact]
        public static void OrElseAsync_None_NullOther() =>
            Assert.ThrowsAnexn("other", () => Ø.OrElseAsync(null!));

        [Fact]
        public static void OrElseAsync_Some_NullOther() =>
            Assert.ThrowsAnexn("other", () => One.OrElseAsync(null!));

        #endregion

        #region SwitchAsync()

        [Fact]
        public static void SwitchAsync_None_NullCaseSome() =>
            Assert.ThrowsAnexn("caseSome", () =>
                Ø.SwitchAsync(
                    caseSome: Funk<int, AnyResult>.NullAsync,
                    caseNone: AnyResult.AsyncValue));

        [Fact]
        public static void SwitchAsync_None_NullCaseNone() =>
            Assert.ThrowsAnexn("caseNone", () =>
                Ø.SwitchAsync(
                    caseSome: Funk<int, AnyT>.AnyAsync,
                    caseNone: null!));

        [Fact]
        public static void SwitchAsync_Some_NullCaseSome() =>
            Assert.ThrowsAnexn("caseSome", () =>
                One.SwitchAsync(
                    caseSome: Funk<int, AnyT>.NullAsync,
                    caseNone: AnyT.AsyncValue));

        [Fact]
        public static void SwitchAsync_Some_NullCaseNone() =>
            Assert.ThrowsAnexn("caseNone", () =>
                One.SwitchAsync(
                    caseSome: x => AnyResult.AsyncValue,
                    caseNone: null!));

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

            #region SwitchAsync()

            [Fact]
            public static async Task SwitchAsync_None_NullCaseSome_DoesNotThrow()
            {
                var r = await MaybeEx.SwitchAsync(
                    Ø,
                    caseSome: Funk<int, AnyResult>.NullAsync,
                    caseNone: AnyResult.AsyncValue
                );

                Assert.Same(AnyResult.Value, r); // Sanity check
            }

            [Fact]
            public static async Task SwitchAsync_None_NullCaseNone() =>
                await Assert.Async
                    .ThrowsAnexn("caseNone", () =>
                        MaybeEx.SwitchAsync(
                            Ø,
                            caseSome: Funk<int, AnyT>.AnyAsync,
                            caseNone: null!));

            [Fact]
            public static async Task SwitchAsync_Some_NullCaseSome() =>
                await Assert.Async
                    .ThrowsAnexn("caseSome", () =>
                        MaybeEx.SwitchAsync(
                            One,
                            caseSome: Funk<int, AnyT>.NullAsync,
                            caseNone: AnyT.AsyncValue));

            [Fact]
            public static async Task SwitchAsync_Some_NullCaseNone_DoesNotThrow()
            {
                var r = await MaybeEx.SwitchAsync(
                    One,
                    caseSome: x => AnyResult.AsyncValue,
                    caseNone: null!
                );

                Assert.Same(AnyResult.Value, r); // Sanity check
            }

            #endregion
        }
    }
}
