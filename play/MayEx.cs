// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;

    public abstract class MayEx : May
    {
        protected MayEx() { }

        public static Maybe<bool> ParseBoolean(string? value)
            => ParseBoolean(value, BooleanStyles.Default);

        public static Maybe<bool> ParseBoolean(string? value, BooleanStyles style)
        {
            if (value is null) { return Maybe<bool>.None; }

            string trimmed = value.Trim();

            if (trimmed.Length == 0)
            {
                return style.Contains(BooleanStyles.EmptyOrWhiteSpaceIsFalse)
                    ? Maybe.Some(false) : Maybe<bool>.None;
            }
            else if (style.Contains(BooleanStyles.Literal))
            {
                // NB: this method is case-insensitive.
                return Boolean.TryParse(trimmed, out bool retval)
                    ? Maybe.Some(retval) : Maybe<bool>.None;
            }
            else if (style.Contains(BooleanStyles.ZeroOrOne)
                && (trimmed == "0" || trimmed == "1"))
            {
                return Maybe.Some(trimmed == "1");
            }
            else if (style.Contains(BooleanStyles.HtmlInput) && value == "on")
            {
                return Maybe.Some(true);
            }
            else
            {
                return Maybe<bool>.None;
            }
        }
    }
}
