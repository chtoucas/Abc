// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;

    using static MaybeFactory;

    public abstract class MayEx : May
    {
        private static readonly Maybe<bool> s_True = Some(true);
        private static readonly Maybe<bool> s_False = Some(false);

        protected MayEx() { }

        public static Maybe<bool> ParseBoolean(string? value)
            => ParseBoolean(value, BooleanStyles.Default);

        public static Maybe<bool> ParseBoolean(string? value, BooleanStyles style)
        {
            if (value is null) { return Maybe<bool>.None; }

            string val = value.Trim();

            if (val.Length == 0)
            {
                return style.Contains(BooleanStyles.EmptyOrWhiteSpaceIsFalse)
                    ? s_False : Maybe<bool>.None;
            }
            else if (style.Contains(BooleanStyles.Literal))
            {
                // NB: Cette méthode n'est pas sensible à la casse de "value".
                return Boolean.TryParse(val, out bool retval)
                    ? retval ? s_True : s_False
                    : Maybe<bool>.None;
            }
            else if (style.Contains(BooleanStyles.ZeroOrOne) && (val == "0" || val == "1"))
            {
                return val == "1" ? s_True : s_False;
            }
            else if (style.Contains(BooleanStyles.HtmlInput) && value == "on")
            {
                return s_True;
            }
            else
            {
                return Maybe<bool>.None;
            }
        }
    }
}
