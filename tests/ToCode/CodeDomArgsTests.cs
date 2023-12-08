using TerminalGuiDesigner.ToCode;

namespace UnitTests.ToCode;

[TestFixture]
[TestOf( typeof( CodeDomArgs ) )]
[Category( "Code Generation" )]
internal class CodeDomArgsTests
{
    private static IEnumerable<TestCaseData> GetUniqueFieldName_GivesExpectedResultString_Cases( ) => MakeValidFieldName_GivesExpectedResultString_Cases( );

    private static IEnumerable<TestCaseData> MakeValidFieldName_GivesExpectedResultString_Cases( )
    {
        yield return new( "fff" ) { HasExpectedResult = true, ExpectedResult = "fff" };
        yield return new( "33Dalmatians" ) { HasExpectedResult = true, ExpectedResult = "dalmatians" };
        yield return new( "Dalmatians33" ) { HasExpectedResult = true, ExpectedResult = "dalmatians33" };
        yield return new( "" ) { HasExpectedResult = true, ExpectedResult = "blank" };
        yield return new( "bob is great" ) { HasExpectedResult = true, ExpectedResult = "bobIsGreat" };
        yield return new( "\t" ) { HasExpectedResult = true, ExpectedResult = "blank" };
        yield return new( null ) { HasExpectedResult = true, ExpectedResult = "blank" };
        yield return new( "test\r\nffish\r\n" ) { HasExpectedResult = true, ExpectedResult = "testFfish" };
    }

    private static IEnumerable<TestCaseData> MakeValidFieldName_CommonCases( )
    {
        yield return new( "fff" ) { HasExpectedResult = true, ExpectedResult = true };
        yield return new( "33Damatians" ) { HasExpectedResult = true, ExpectedResult = true };
        yield return new( "Dalmatians33" ) { HasExpectedResult = true, ExpectedResult = true };
        yield return new( string.Empty ) { HasExpectedResult = true, ExpectedResult = true };
        yield return new( "bob is great" ) { HasExpectedResult = true, ExpectedResult = true };
        yield return new( "\t" ) { HasExpectedResult = true, ExpectedResult = true };
        yield return new( Environment.NewLine ) { HasExpectedResult = true, ExpectedResult = true };
        yield return new( null ) { HasExpectedResult = true, ExpectedResult = true };
        yield return new( $"test{Environment.NewLine}ffish{Environment.NewLine}" ) { HasExpectedResult = true, ExpectedResult = true };
    }

    [Test]
    [TestCaseSource( nameof( MakeValidFieldName_GivesExpectedResultString_Cases ) )]
    public string MakeValidFieldName_GivesExpectedResultString( string? input )
    {
        return CodeDomArgs.MakeValidFieldName( input );
    }

    [Test]
    [TestCaseSource( nameof( MakeValidFieldName_CommonCases ) )]
    public bool MakeValidFieldName_PublicStartsWithCaps( string? input )
    {
        return char.IsUpper( CodeDomArgs.MakeValidFieldName( input, true )[ 0 ] );
    }

    [Test]
    [TestCaseSource( nameof( MakeValidFieldName_CommonCases ) )]
    public bool MakeValidFieldName_NonPublicStartsWithLowerCase( string? input )
    {
        return char.IsLower( CodeDomArgs.MakeValidFieldName( name: input, isPublic: false ), 0 );
    }

    [Test]
    [TestCaseSource( nameof( MakeValidFieldName_CommonCases ) )]
    public bool MakeValidFieldName_PublicAndPrivateSameAfterFirstChar( string? input )
    {
        string publicFieldName = CodeDomArgs.MakeValidFieldName( input, isPublic: true );
        string privateFieldName = CodeDomArgs.MakeValidFieldName( input, isPublic: false );

        return publicFieldName[ 1.. ] == privateFieldName[ 1.. ];
    }

    [Test]
    [TestCaseSource( nameof( GetUniqueFieldName_GivesExpectedResultString_Cases ) )]
    public string GetUniqueFieldName_GivesExpectedResultString( string? input )
    {
        return new CodeDomArgs( new( ), new( ) ).GetUniqueFieldName( input );
    }

    [Test]
    [TestCase( "bob", "bob", "bob2" )]
    [TestCase( "blank", "", "blank2" )]
    public void Test_GetUniqueFieldName_AfterAdding( string firstAdd, string? thenInput, string expectOutput )
    {
        var args = new CodeDomArgs( new( ), new( ) );
        args.FieldNamesUsed.Add( firstAdd );
        ClassicAssert.AreEqual( expectOutput, args.GetUniqueFieldName( thenInput ) );
    }

    private static readonly string[] ReservedKeywords =
    {
        "abstract",
        "as",
        "base",
        "bool",
        "break",
        "byte",
        "case",
        "catch",
        "char",
        "checked",
        "class",
        "const",
        "continue",
        "decimal",
        "default",
        "delegate",
        "do",
        "double",
        "event",
        "explicit",
        "extern",
        "false",
        "finally",
        "fixed",
        "float",
        "for",
        "foreach",
        "goto",
        "if",
        "implicit",
        "in",
        "int",
        "interface",
        "internal",
        "is",
        "lock",
        "new",
        "null",
        "object",
        "operator",
        "out",
        "override",
        "params",
        "private",
        "protected",
        "public",
        "readonly",
        "ref",
        "return",
        "sbyte",
        "sealed",
        "short",
        "sizeof",
        "stackalloc",
        "struct",
        "switch",
        "this",
        "throw",
        "true",
        "try",
        "typeof",
        "uint",
        "ulong",
        "unchecked",
        "unsafe",
        "ushort",
        "using",
        "virtual",
        "void",
        "volatile",
        "while"
    };

    [Test]
    public void MakeValidFieldName_ShouldPrependUnderscoreToReservedKeywords( [ValueSource( nameof( ReservedKeywords ) )] string input )
    {
        Assert.That( CodeDomArgs.MakeValidFieldName( input ), Is.EqualTo( $"_{input}" ) );
    }
}