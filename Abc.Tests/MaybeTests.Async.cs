// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Threading.Tasks;

    using Xunit;

    using Assert = AssertEx;

    // TODO: figure out async tests.
    // - remove AsyncValue.
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
        #region Switch()

        //[Fact]
        //public static void SwitchAsync_None_NullCaseSome() =>
        //    Assert.ThrowsAnexn("caseSome", () =>
        //        Ø.SwitchAsync(
        //            caseSome: Funk<int, AnyResult>.NullAsync,
        //            caseNone: () => AsyncFakes.AsyncValue));

        //[Fact]
        //public static void SwitchAsync_None_NullCaseNone() =>
        //    Assert.ThrowsAnexn("caseNone", () =>
        //        Ø.SwitchAsync(
        //            caseSome: Funk<int, AnyT>.AnyAsync,
        //            caseNone: null!));

        //[Fact]
        //public static void SwitchAsync_Some_NullCaseSome() =>
        //    Assert.ThrowsAnexn("caseSome", () =>
        //        One.SwitchAsync(
        //            caseSome: Funk<int, AnyT>.NullAsync,
        //            caseNone: () => AnyT.AsyncValue));

        //[Fact]
        //public static void SwitchAsync_Some_NullCaseNone() =>
        //    Assert.ThrowsAnexn("caseNone", () =>
        //        One.SwitchAsync(
        //            caseSome: x => AsyncFakes.AsyncValue,
        //            caseNone: null!));

        //[Fact]
        //public static async Task SwitchAsync_None()
        //{
        //    // Arrange
        //    bool onSomeCalled = false;
        //    bool onNoneCalled = false;
        //    // Act
        //    int v = await NoText.SwitchAsync(
        //        caseSome: async x => { await Task.Yield(); onSomeCalled = true; return x.Length; },
        //        caseNone: async () => { await Task.Yield(); onNoneCalled = true; return 0; });
        //    // Assert
        //    Assert.False(onSomeCalled);
        //    Assert.True(onNoneCalled);
        //    Assert.Equal(0, v);
        //}

        //[Fact]
        //public static async Task SwitchAsync_Some()
        //{
        //    // Arrange
        //    bool onSomeCalled = false;
        //    bool onNoneCalled = false;
        //    // Act
        //    int v = await SomeText.SwitchAsync(
        //        caseSome: async x => { await Task.Yield(); onSomeCalled = true; return x.Length; },
        //        caseNone: async () => { await Task.Yield(); onNoneCalled = true; return 0; });
        //    // Assert
        //    Assert.True(onSomeCalled);
        //    Assert.False(onNoneCalled);
        //    Assert.Equal(4, v);
        //}

        [Fact]
        public static async Task Switch_None_Async()
        {
            // Arrange
            bool onSomeCalled = false;
            bool onNoneCalled = false;
            // Act
            int v = await NoText.Switch(
                caseSome: async x => { await Task.Yield(); onSomeCalled = true; return x.Length; },
                caseNone: async () => { await Task.Yield(); onNoneCalled = true; return 0; });
            // Assert
            Assert.False(onSomeCalled);
            Assert.True(onNoneCalled);
            Assert.Equal(0, v);
        }

        [Fact]
        public static async Task Switch_Some_Async()
        {
            // Arrange
            bool onSomeCalled = false;
            bool onNoneCalled = false;
            // Act
            int v = await SomeText.Switch(
                caseSome: async x => { await Task.Yield(); onSomeCalled = true; return x.Length; },
                caseNone: async () => { await Task.Yield(); onNoneCalled = true; return 0; });
            // Assert
            Assert.True(onSomeCalled);
            Assert.False(onNoneCalled);
            Assert.Equal(4, v);
        }

        #endregion
    }

    public partial class MaybeTests
    {
        #region BindAsync()

        [Fact]
        public static void BindAsync_None_NullBinder() =>
            Assert.ThrowsAnexn("binder", () =>
                Ø.BindAsync(AsyncFakes<int, Maybe<AnyT>>.Null));

        [Fact]
        public static void BindAsync_Some_NullBinder() =>
            Assert.ThrowsAnexn("binder", () =>
                One.BindAsync(AsyncFakes<int, Maybe<AnyT>>.Null));

        [Fact]
        public static async Task BindAsync_None()
        {
            await Assert.Async.None(
                Ø.BindAsync(Thunk<int>.ReturnAsync(AnyResult.Some)));

            await Assert.Async.None(
                NoText.BindAsync(Thunk<string>.ReturnAsync(AnyResult.Some)));

            await Assert.Async.None(
                NoUri.BindAsync(Thunk<Uri>.ReturnAsync(AnyResult.Some)));

            await Assert.Async.None(
                AnyT.None.BindAsync(Thunk<AnyT>.ReturnAsync(AnyResult.Some)));
        }

        [Fact]
        public static async Task BindAsync_Some_ReturnsNone()
        {
            await Assert.Async.None(
                One.BindAsync(Thunk<int>.ReturnAsync(AnyResult.None)));

            await Assert.Async.None(
                SomeText.BindAsync(Thunk<string>.ReturnAsync(AnyResult.None)));

            await Assert.Async.None(
                SomeUri.BindAsync(Thunk<Uri>.ReturnAsync(AnyResult.None)));

            await Assert.Async.None(
                AnyT.Some.BindAsync(Thunk<AnyT>.ReturnAsync(AnyResult.None)));
        }

        [Fact]
        public static async Task BindAsync_Some_ReturnsSome()
        {
            await Assert.Async.Some(AnyResult.Value,
                One.BindAsync(Thunk<int>.ReturnAsync(AnyResult.Some)));

            await Assert.Async.Some(AnyResult.Value,
                SomeText.BindAsync(Thunk<string>.ReturnAsync(AnyResult.Some)));

            await Assert.Async.Some(AnyResult.Value,
                SomeUri.BindAsync(Thunk<Uri>.ReturnAsync(AnyResult.Some)));

            await Assert.Async.Some(AnyResult.Value,
                AnyT.Some.BindAsync(Thunk<AnyT>.ReturnAsync(AnyResult.Some)));
        }

        [Fact]
        public static async Task BindAsync_SomeInt32() =>
            await Assert.Async.Some(6L, Two.BindAsync(MultiplyBy3Async_));

        [Fact]
        public static async Task BindAsync_SomeInt64() =>
            await Assert.Async.Some(8L, TwoL.BindAsync(MultiplyBy4Async_));

        [Fact]
        public static async Task BindAsync_SomeUri() =>
            await Assert.Async.Some(MyUri.AbsoluteUri,
                SomeUri.BindAsync(GetAbsoluteUriAsync_));

        #endregion

        #region SelectAsync()

        [Fact]
        public static void SelectAsync_None_NullSelector() =>
            Assert.ThrowsAnexn("selector", () =>
                Ø.SelectAsync(AsyncFakes<int, AnyT>.Null));

        [Fact]
        public static void SelectAsync_Some_NullSelector() =>
            Assert.ThrowsAnexn("selector", () =>
                One.SelectAsync(AsyncFakes<int, AnyT>.Null));

        [Fact]
        public static async Task SelectAsync_None()
        {
            await Assert.Async.None(Ø.SelectAsync(__selector));

            static async Task<long> __selector(int x)
            {
                await Task.Yield();
                return x;
            }
        }

        [Fact]
        public static async Task SelectAsync_SomeInt32() =>
            await Assert.Async.Some(6L, Two.SelectAsync(MultiplyBy3Async));

        [Fact]
        public static async Task SelectAsync_SomeInt64() =>
            await Assert.Async.Some(8L, TwoL.SelectAsync(MultiplyBy4Async));

        [Fact]
        public static async Task SelectAsync_SomeUri() =>
            await Assert.Async.Some(MyUri.AbsoluteUri,
                SomeUri.SelectAsync(GetAbsoluteUriAsync));

        #endregion

        #region OrElseAsync()

        [Fact]
        public static void OrElseAsync_None_NullOther() =>
            Assert.ThrowsAnexn("other", () => Ø.OrElseAsync(null!));

        [Fact]
        public static void OrElseAsync_Some_NullOther() =>
            Assert.ThrowsAnexn("other", () => One.OrElseAsync(null!));

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
                        MaybeEx.BindAsync(Ø, AsyncFakes<int, Maybe<AnyT>>.Null));

            [Fact]
            public static async Task BindAsync_Some_NullBinder() =>
                await Assert.Async
                    .ThrowsAnexn("binder", () =>
                        MaybeEx.BindAsync(One, AsyncFakes<int, Maybe<AnyT>>.Null));

            #endregion

            #region SelectAsync()

            [Fact]
            public static async Task SelectAsync_None_NullSelector() =>
                await Assert.Async
                    .ThrowsAnexn("selector", () =>
                        MaybeEx.SelectAsync(Ø, AsyncFakes<int, AnyT>.Null));

            [Fact]
            public static async Task SelectAsync_Some_NullSelector() =>
                await Assert.Async
                    .ThrowsAnexn("selector", () =>
                        MaybeEx.SelectAsync(One, AsyncFakes<int, AnyT>.Null));

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

            //[Fact]
            //public static async Task SwitchAsync_None_NullCaseSome_DoesNotThrow()
            //{
            //    var r = await MaybeEx.SwitchAsync(
            //        Ø,
            //        caseSome: Funk<int, AnyResult>.NullAsync,
            //        caseNone: () => AsyncFakes.AsyncValue
            //    );

            //    Assert.Same(AnyResult.Value, r); // Sanity check
            //}

            //[Fact]
            //public static async Task SwitchAsync_None_NullCaseNone() =>
            //    await Assert.Async
            //        .ThrowsAnexn("caseNone", () =>
            //            MaybeEx.SwitchAsync(
            //                Ø,
            //                caseSome: Funk<int, AnyT>.AnyAsync,
            //                caseNone: null!));

            //[Fact]
            //public static async Task SwitchAsync_Some_NullCaseSome() =>
            //    await Assert.Async
            //        .ThrowsAnexn("caseSome", () =>
            //            MaybeEx.SwitchAsync(
            //                One,
            //                caseSome: Funk<int, AnyT>.NullAsync,
            //                caseNone: () => AnyT.AsyncValue));

            //[Fact]
            //public static async Task SwitchAsync_Some_NullCaseNone_DoesNotThrow()
            //{
            //    var r = await MaybeEx.SwitchAsync(
            //        One,
            //        caseSome: x => AsyncFakes.AsyncValue,
            //        caseNone: null!
            //    );

            //    Assert.Same(AnyResult.Value, r); // Sanity check
            //}

            #endregion
        }
    }
}
