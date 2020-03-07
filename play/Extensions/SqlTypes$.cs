// See LICENSE.txt in the project root for license information.

namespace Abc.Extensions
{
    using System;
    using System.Data.SqlTypes;

    using static MaybeFactory;

    using MaybeT = Maybe;

    /// <summary>
    /// Provides extension methods to convert native SQL server data types to
    /// CLR types.
    /// </summary>
    public static partial class SqlTypesX { }

    // CLR value types.
    public partial class SqlTypesX
    {
        public static Maybe<bool> Maybe(this SqlBoolean @this)
            => @this.IsNull ? Maybe<bool>.None : Some(@this.Value);

        public static Maybe<byte> Maybe(this SqlByte @this)
            => @this.IsNull ? Maybe<byte>.None : Some(@this.Value);

        public static Maybe<DateTime> Maybe(this SqlDateTime @this)
            => @this.IsNull ? Maybe<DateTime>.None : Some(@this.Value);

        public static Maybe<decimal> Maybe(this SqlDecimal @this)
            => @this.IsNull ? Maybe<decimal>.None : Some(@this.Value);

        public static Maybe<double> Maybe(this SqlDouble @this)
            => @this.IsNull ? Maybe<double>.None : Some(@this.Value);

        public static Maybe<Guid> Maybe(this SqlGuid @this)
            => @this.IsNull ? Maybe<Guid>.None : Some(@this.Value);

        public static Maybe<short> Maybe(this SqlInt16 @this)
            => @this.IsNull ? Maybe<short>.None : Some(@this.Value);

        public static Maybe<int> Maybe(this SqlInt32 @this)
            => @this.IsNull ? Maybe<int>.None : Some(@this.Value);

        public static Maybe<long> Maybe(this SqlInt64 @this)
            => @this.IsNull ? Maybe<long>.None : Some(@this.Value);

        public static Maybe<decimal> Maybe(this SqlMoney @this)
            => @this.IsNull ? Maybe<decimal>.None : Some(@this.Value);

        public static Maybe<float> Maybe(this SqlSingle @this)
            => @this.IsNull ? Maybe<float>.None : Some(@this.Value);
    }

    // CLR reference types.
    public partial class SqlTypesX
    {
        public static Maybe<byte[]> Maybe(this SqlBinary @this)
            => MaybeT.Guard(!@this.IsNull).ReplaceWith(@this.Value);

        public static Maybe<string> Maybe(this SqlString @this)
            => MaybeT.Guard(!@this.IsNull).ReplaceWith(@this.Value);

        /// <remarks>
        /// Beware, this extension method does NOT throw when the object is null
        /// but rather returns an empty maybe.
        /// </remarks>
        public static Maybe<byte[]> Maybe(this SqlBytes? @this)
            => from x in SomeOrNone(@this) where !x.IsNull select x.Value;

        /// <remarks>
        /// Beware, this extension method does NOT throw when the object is null
        /// but rather returns an empty maybe.
        /// </remarks>
        public static Maybe<char[]> Maybe(this SqlChars? @this)
            => from x in SomeOrNone(@this) where !x.IsNull select x.Value;

        /// <remarks>
        /// Beware, this extension method does NOT throw when the object is null
        /// but rather returns an empty maybe.
        /// </remarks>
        public static Maybe<string> Maybe(this SqlXml? @this)
            => from x in SomeOrNone(@this) where !x.IsNull select x.Value;
    }
}
