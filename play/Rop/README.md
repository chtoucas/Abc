A Result type. Too many problems with the current design.

It should
1. Be both a Result type and an Option type.
2. Be a reference type.
3. Be designed for pattern matching.
4. Offer basic support for the Query Expression Syntax.

Solutions?
- Generic `Result<T, TErr>` type using Either. 
  Usability is questionnable and it is only a Result type (not an Option type).
- Base `Result<T>` and generic error type `Error<T, TErr>`.
  Usability is questionnable.
- Only cover the simplest situation, the one where the error is a string?
