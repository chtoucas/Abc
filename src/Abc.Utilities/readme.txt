

Instead of static classes, we use abstract partial classes:

    [DebuggerNonUserCode]
    internal abstract partial class MyClass
    {
        [ExcludeFromCodeCoverage]
        protected MyClass() { }
    }

Code coverage:
- filter out "System" classes: [Abc.Utilities]System.*