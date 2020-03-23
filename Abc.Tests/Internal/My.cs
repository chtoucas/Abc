// See LICENSE.txt in the project root for license information.

using System;

internal static class My
{
    public const string NullString = null;
    public const string? NullNullString = null;

    public static readonly Action Noop = () => { };

    public enum Enum012
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Alias1 = One,
    }

    [Flags]
    public enum EnumBits
    {
        None = 0,
        One = 1 << 0,
        Two = 1 << 1,
        Four = 1 << 2,
        OneTwo = One | Two,
        OneTwoFour = One | Two | Four
    }
}
