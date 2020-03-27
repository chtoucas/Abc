﻿// See LICENSE.txt in the project root for license information.

using System;

[Flags]
internal enum FlagsEnum
{
    None = 0,
    One = 1 << 0,
    Two = 1 << 1,
    Four = 1 << 2,

    OneTwo = One | Two,
    OneTwoFour = One | Two | Four
}