Abc.Utilities
=============

Les codes C# contenus dans le paquet Abc.Utilities.Sources sont compatibles
avec netstandard1.1+, netcoreapp (versions LTS et postérieures dans chaque
branche majeure, p.ex. netcoreapp2.x où x >= 1) et .NET Framework 4.5.2+ ---
il est fort probable que tout marche correctement avec netstandard1.0+,
netcoreapp1.0+ et .NET 4.5+, mais cela n'a pas été vérifié explicitement.

Propriétés MSBuild optionnelles :
`PkgAbc_Utilities_Sources__CompileItem` :
  Par défaut, tous les codes sources sont inclus dans le projet mais il est
  préférable de ne sélectionner que le strict nécessaire. Par exemple,
  ```xml
  <ItemGroup>
    <PkgAbc_Utilities_Sources__CompileItem Include="MathOperations.g.cs">
      <Link>Utilities\%(Identity)</Link>
    </PkgAbc_Utilities_Sources__CompileItem>

    <PkgAbc_Utilities_Sources__CompileItem Include="system\NullableAttributes.g.cs">
      <Visible>false</Visible>
    </PkgAbc_Utilities_Sources__CompileItem>
  </ItemGroup>
  ```
  Les chemins sont relatifs au dossier `PkgAbc_Utilities_Sources__CompileRoot`.
  Notons au passage, qu'on peut attacher des métadonnées à un élément, ces
  dernières étant alors automatiquement transposées dans le fichier projet.
  Attention, certains fichiers ne fonctionnent pas de manière isolée. Par
  exemple, pour pouvoir utiliser "InternalForTestingAttribute.g.cs", il ne faut
  pas oublier d'inclure aussi "AccessibilityLevel.g.cs".

`PkgAbc_Utilities_Sources__ImportTfr` :
  Il s'agit d'une propriété booléenne dont la valeur par défaut est `true`.
  À moins que cette propriété ne soit égale à `false`, on définit les
  symboles de compilation suivants :
  - `NETSTANDARD1_x` quand le "framework" cible est netstandard1.x ;
  - `NETSTANDARD2_x` quand le "framework" cible est netstandard2.x ;
  - `NETCOREAPP1_x` quand le "framework" cible est netcoreapp1.x ;
  - `NETCOREAPP2_x` quand le "framework" cible est netcoreapp2.x ;
  - `NETCOREAPP3_x` quand le "framework" cible est netcoreapp3.x ;
  - `NETCOREAPP5_x` quand le "framework" cible est net5.x.

Toutes les classes non-statiques sont fermées (`sealed`), quant aux classes
statiques elles sont systématiquement partielles (`partial`), donc extensibles
localement. Par exemple,
```csharp
// Extrait de MathOperations.g.cs.
namespace Abc.Utilities
{
    internal static partial class MathOperations
    {
        protected MathOperations() { }
    }
}

// Localement on peut compléter MathOperations comme suit.
namespace Abc.Utilities
{
    internal partial class MathOperations
    {
        public static MyMethod() { }
    }
}
```

Remarques :
- compatible avec les NRTs ;
- compatible avec la compilation déterministe ;
- pour exclure le code provenant de ce paquet lorsqu'on mesure la couverture
  de code, avec coverlet.msbuild on peut p.ex. utiliser
  ```
  /p:Exclude=[MyAssembly]System.*
  /p:ExcludeByAttribute=DebuggerNonUserCode
  ```
- les méthodes retournant quelque chose ont l'attribut `Pure`. Pour en bénéficier
  il est nécessaire de définir le symbole de compilation `CONTRACTS_FULL`, chose
  que l'on ne fait pas automatiquement lors de l'installation du paquet.
