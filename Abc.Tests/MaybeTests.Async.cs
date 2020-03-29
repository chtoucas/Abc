// See LICENSE.txt in the project root for license information.

// TODO: ConfigureAwait(false) or ConfigureAwait(true)?
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task

namespace Abc
{
    using System.Threading.Tasks;

    using Xunit;

    using Assert = AssertEx;

    // Async methods.
    // https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/november/async-programming-unit-testing-asynchronous-code
    public partial class MaybeTests
    {
        // TODO: improve async tests when a test returns immediately, which is
        // almost always the case.

        #region BindAsync()

        [Fact]
        public static async Task BindAsync_None_NullBinder()
        {
            await Assert.Async.ThrowsArgNullEx("binder", () =>
                Ø.BindAsync(Kunc<int, AnyT>.NullAsync));
        }

        [Fact]
        public static async Task BindAsync_Some_NullBinder()
        {
            await Assert.Async.ThrowsArgNullEx("binder", () =>
                One.BindAsync(Kunc<int, AnyT>.NullAsync));
        }

        [Fact]
        public static async Task BindAsync_None()
        {
            var tasks = new Task[4];

            tasks[0] = Assert.Async.None(Ø.BindAsync(AsyncFakes.ConstAsync));
            tasks[1] = Assert.Async.None(NoText.BindAsync(AsyncFakes.ConstAsync));
            tasks[2] = Assert.Async.None(NoUri.BindAsync(AsyncFakes.ConstAsync));
            tasks[3] = Assert.Async.None(AnyT.None.BindAsync(AsyncFakes.ConstAsync));

            await Task.WhenAll(tasks);
        }

        [Fact]
        public static async Task BindAsync_Some()
        {
            var tasks = new Task[4];

            Assert.Async.Some(AnyResult.Value, One.BindAsync(_ => AnyResult.AsyncSome));
            Assert.Async.Some(AnyResult.Value, SomeText.BindAsync(_ => AnyResult.AsyncSome));
            Assert.Async.Some(AnyResult.Value, SomeUri.BindAsync(_ => AnyResult.AsyncSome));
            Assert.Async.Some(AnyResult.Value, AnyT.Some.BindAsync(_ => AnyResult.AsyncSome));

            tasks[0] = Assert.Async.None(One.BindAsync(_ => AnyResult.AsyncNone));
            tasks[1] = Assert.Async.None(SomeText.BindAsync(_ => AnyResult.AsyncNone));
            tasks[2] = Assert.Async.None(SomeUri.BindAsync(_ => AnyResult.AsyncNone));
            tasks[3] = Assert.Async.None(AnyT.Some.BindAsync(_ => AnyResult.AsyncNone));

            await Task.WhenAll(tasks);
        }

        #endregion

        #region SelectAsync()

        [Fact]
        public static async Task SelectAsync_None_NullSelector()
        {
            await Assert.Async.ThrowsArgNullEx("selector", () =>
                Ø.SelectAsync(Funk<int, AnyT>.NullAsync));
        }

        [Fact]
        public static async Task SelectAsync_Some_NullSelector()
        {
            await Assert.Async.ThrowsArgNullEx("selector", () =>
                One.SelectAsync(Funk<int, AnyT>.NullAsync));
        }

        [Fact]
        public static void SelectAsync_None()
        {
            Assert.None(Ø.Select(x => x));
            Assert.None(from x in Ø select x);
        }

        [Fact]
        public static void SelectAsync_Some()
        {
            Assert.Some(2L, One.Select(x => 2L * x));
            Assert.Some(2L, from x in One select 2L * x);

            Assert.Some(6L, Two.Select(x => 3L * x));
            Assert.Some(6L, from x in Two select 3L * x);

            Assert.Some(MyUri.AbsoluteUri, SomeUri.Select(x => x.AbsoluteUri));
            Assert.Some(MyUri.AbsoluteUri, from x in SomeUri select x.AbsoluteUri);
        }

        #endregion

        #region OrElseAsync()

        [Fact]
        public static async Task OrElseAsync_None_NullOther()
        {
            await Assert.Async.ThrowsArgNullEx("other", () => Ø.OrElseAsync(null!));
        }

        [Fact]
        public static async Task OrElseAsync_Some_NullOther()
        {
            await Assert.Async.ThrowsArgNullEx("other", () => One.OrElseAsync(null!));
        }

        #endregion

        #region SwitchAsync()

        [Fact]
        public static async Task SwitchAsync_None_NullCaseNone_Throws()
        {
            await Assert.Async.ThrowsArgNullEx("caseNone", () =>
                Ø.SwitchAsync(
                    caseSome: Funk<int, AnyT>.AnyAsync,
                    caseNone: null!));
        }

        [Fact]
        public static void SwitchAsync_None_NullCaseSome_DoesNotThrow()
        {
            // Act & Assert
            Task<AnyResult> v = Ø.SwitchAsync(
                caseSome: Funk<int, AnyResult>.NullAsync,
                caseNone: AnyResult.AsyncValue);
            Assert.Same(AnyResult.Value, v.Result); // Sanity check
        }

        [Fact]
        public static async Task SwitchAsync_Some_NullCaseSome_Throws()
        {
            await Assert.Async.ThrowsArgNullEx("caseSome", () =>
                One.SwitchAsync(
                    caseSome: Funk<int, AnyT>.NullAsync,
                    caseNone: AnyT.AsyncValue));
        }

        [Fact]
        public static void SwitchAsync_Some_NullCaseNone_DoesNotThrow()
        {
            // Act & Assert
            Task<AnyResult> v = One.SwitchAsync(
                caseSome: x => AnyResult.AsyncValue,
                caseNone: null!);
            Assert.Same(AnyResult.Value, v.Result); // Sanity check
        }

        #endregion
    }
}
