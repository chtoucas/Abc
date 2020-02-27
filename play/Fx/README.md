A word of caution
-----------------
The classes found here exist for the sole purpose of my own education.
API and documentation are **adapted** (copied) from the Haskell sources.
The port from Haskell is quite loose and the result is NOT meant for efficiency.

Overview
--------
- Functor
- Functor > Applicative
- Functor > Applicative > Alternative
- Functor > Applicative > Monad
- Functor > Applicative > Alternative + Monad > MonadPlus

Compiler switches
-----------------
- `MONADS_VIA_MAP_MULTIPLY`.
  The default behaviour is to define monads via Bind.
- `COMONADS_VIA_MAP_COMULTIPLY`.
  The default behaviour is to define comonads via cobind.
