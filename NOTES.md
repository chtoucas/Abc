# Developer Notes

Guidelines
----------

- Methods that return something should have the attribure `Pure`. It is not
  mandatory but it clearly states that the result should not be ignored.
  It might seem superfluous, but "ça ne mange pas de pain".
- Add _nullable annotations_ whenever necessary.
- _Seal classes_ unless they are designed with extensibility in mind.
- Tag any use of the null-forgiving operator (!) with `BONSANG!`.

TODOs
-----

See `FIXME`, `TODO` and `REVIEW` within the code and:
- Tests & code coverage (coverlet). Currently, there are many cases where we
  don't go beyond smoke tests.
- XML comments with complete examples.
- Perf tool (LINQ, nulls).
- Optimize LINQ ops.

### Issues

- [MSB3277](https://github.com/microsoft/msbuild/issues/608)

Future plans?
-------------

[Narvalo.NET](https://github.com/chtoucas/Narvalo.NET)
- utilities to write code in the ROP style (Railway Oriented Programming).
- a fast Gregorian date type
- country codes
- currency codes
- simple money type
- BIC and IBAN
- roman numerals
- string builder & math extensions

Numerics:
- port of hypercalc to .NET (WinUI)
- big decimal
- [Integer](https://en.wikipedia.org/wiki/Integer_(computer_science))
- [Minifloat](https://en.wikipedia.org/wiki/Minifloat)
- [Signed number representations](https://en.wikipedia.org/wiki/Signed_number_representations)
- [Binary-coded decimal](https://en.wikipedia.org/wiki/Binary-coded_decimal)
