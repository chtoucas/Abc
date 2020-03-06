// See LICENSE.txt in the project root for license information.

namespace Abc.Extensions
{
    using System;
    using System.Data.SqlTypes;

    /// <summary>
    /// Provides extension methods to convert native SQL server data types to
    /// CLR types.
    /// </summary>
    public static partial class SqlTypesX { }

    // CLR value types.
    public partial class SqlTypesX
    {
        public static Maybe<bool> Maybe(this SqlBoolean @this)
            => @this.IsNull ? Maybe<bool>.None : Abc.Maybe.Some(@this.Value);

        public static Maybe<byte> Maybe(this SqlByte @this)
            => @this.IsNull ? Maybe<byte>.None : Abc.Maybe.Some(@this.Value);

        public static Maybe<DateTime> Maybe(this SqlDateTime @this)
            => @this.IsNull ? Maybe<DateTime>.None : Abc.Maybe.Some(@this.Value);

        public static Maybe<decimal> Maybe(this SqlDecimal @this)
            => @this.IsNull ? Maybe<decimal>.None : Abc.Maybe.Some(@this.Value);

        public static Maybe<double> Maybe(this SqlDouble @this)
            => @this.IsNull ? Maybe<double>.None : Abc.Maybe.Some(@this.Value);

        public static Maybe<Guid> Maybe(this SqlGuid @this)
            => @this.IsNull ? Maybe<Guid>.None : Abc.Maybe.Some(@this.Value);

        public static Maybe<short> Maybe(this SqlInt16 @this)
            => @this.IsNull ? Maybe<short>.None : Abc.Maybe.Some(@this.Value);

        public static Maybe<int> Maybe(this SqlInt32 @this)
            => @this.IsNull ? Maybe<int>.None : Abc.Maybe.Some(@this.Value);

        public static Maybe<long> Maybe(this SqlInt64 @this)
            => @this.IsNull ? Maybe<long>.None : Abc.Maybe.Some(@this.Value);

        public static Maybe<decimal> Maybe(this SqlMoney @this)
            => @this.IsNull ? Maybe<decimal>.None : Abc.Maybe.Some(@this.Value);

        public static Maybe<float> Maybe(this SqlSingle @this)
            => @this.IsNull ? Maybe<float>.None : Abc.Maybe.Some(@this.Value);
    }

    // CLR reference types.
    public partial class SqlTypesX
    {
        public static Maybe<byte[]> Maybe(this SqlBinary @this)
            => @this.IsNull ? Maybe<byte[]>.None : Abc.Maybe.Of(@this.Value);

        public static Maybe<byte[]> Maybe(this SqlBytes @this)
            => @this is null || @this.IsNull ? Maybe<byte[]>.None : Abc.Maybe.Of(@this.Value);

        public static Maybe<char[]> Maybe(this SqlChars @this)
            => @this is null || @this.IsNull ? Maybe<char[]>.None : Abc.Maybe.Of(@this.Value);

        public static Maybe<string> Maybe(this SqlString @this)
            => @this.IsNull ? Maybe<string>.None : Abc.Maybe.Of(@this.Value);

        public static Maybe<string> Maybe(this SqlXml @this)
            => @this is null || @this.IsNull ? Maybe<string>.None : Abc.Maybe.Of(@this.Value);
    }
}
