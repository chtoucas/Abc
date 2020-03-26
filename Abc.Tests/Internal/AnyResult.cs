// See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;

using Abc;

// This one is a singleton to simplify some tests.
internal sealed class AnyResult
{
    /// <summary>
    /// Represents the empty "maybe" for the <see cref="AnyResult"/> class.
    /// </summary>
    public static readonly Maybe<AnyResult> None = Maybe<AnyResult>.None;

    private AnyResult() { }

    /// <summary>
    /// Gets the unique instance of the <see cref="AnyResult"/> class.
    /// </summary>
    public static AnyResult Value => Instance_.Value;

    public static Task<AnyResult> AsyncValue => Task.FromResult(Instance_.Value);

    /// <summary>
    /// Creates a new non-empty "maybe" for the <see cref="AnyResult"/> class.
    /// </summary>
    public static Maybe<AnyResult> Some => Maybe.SomeOrNone(Instance_.Value);

    private static class Instance_
    {
        public static readonly AnyResult Value = new AnyResult();
    }
}