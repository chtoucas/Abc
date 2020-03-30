// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1034 // Nested types should not be visible

namespace Abc
{
    using System.Threading.Tasks;

    using Xunit;

    using Assert = AssertEx;

    // TODO: figure out async tests.
    // ConfigureAwait(false) or ConfigureAwait(true)?
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
        public static void BindAsync_None()
        {
            Assert.Async.None(Ø.BindAsync(AsyncFakes.ConstAsync));
            Assert.Async.None(NoText.BindAsync(AsyncFakes.ConstAsync));
            Assert.Async.None(NoUri.BindAsync(AsyncFakes.ConstAsync));
            Assert.Async.None(AnyT.None.BindAsync(AsyncFakes.ConstAsync));
        }

        [Fact]
        public static void BindAsync_Some()
        {
            Assert.Async.Some(AnyResult.Value, One.BindAsync(_ => AnyResult.AsyncSome));
            Assert.Async.Some(AnyResult.Value, SomeText.BindAsync(_ => AnyResult.AsyncSome));
            Assert.Async.Some(AnyResult.Value, SomeUri.BindAsync(_ => AnyResult.AsyncSome));
            Assert.Async.Some(AnyResult.Value, AnyT.Some.BindAsync(_ => AnyResult.AsyncSome));

            Assert.Async.None(One.BindAsync(_ => AnyResult.AsyncNone));
            Assert.Async.None(SomeText.BindAsync(_ => AnyResult.AsyncNone));
            Assert.Async.None(SomeUri.BindAsync(_ => AnyResult.AsyncNone));
            Assert.Async.None(AnyT.Some.BindAsync(_ => AnyResult.AsyncNone));
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
                        MaybeEx.BindAsync(Ø, Kunc<int, AnyT>.NullAsync))
                    .ConfigureAwait(false);

            #endregion
        }
    }
}
