using TerminalGuiDesigner.ToCode;

namespace UnitTests.ToCode;

[TestFixture]
[TestOf( typeof( ViewToCode ) )]
[Category("Code Generation")]
public class ViewToCodeTests
{
    private ViewToCode _viewToCode;

    [SetUp]
    public void Setup( )
    {
        _viewToCode = new ( );
    }

    [Test]
    public void GetGenerateNewViewCode_ValidParameters_ReturnsExpectedCode( )
    {
        string className = "TestView";
        string namespaceName = "TestNamespace";

        string result = ViewToCode.GetGenerateNewViewCode( className, namespaceName );

        Assert.That( result, Is.Not.Null );
        Assert.That( result, Does.Contain( className ) );
        Assert.That( result, Does.Contain( namespaceName ) );
    }

    [Test]
    public void GetGenerateNewViewCode_NullParameters_ThrowsArgumentNullException( )
    {
        Assert.That( ( ) => ViewToCode.GetGenerateNewViewCode( null!, null! ), Throws.ArgumentException );
    }

    [Test]
    public void GenerateNewView_ValidParameters_ReturnsExpectedDesign([ValueSource(typeof(ViewFactory), nameof(ViewFactory.SupportedViewTypes))] Type viewType )
    {
        FileInfo csFilePath = new ( $"GenerateNewView_ValidParameters_ReturnsExpectedDesign_{viewType.Name}.cs" );
        const string namespaceName = "TestNamespace";

        Design result = _viewToCode.GenerateNewView( csFilePath, namespaceName, viewType );

        Assert.That( result,Is.Not.Null );
        Assert.That( result.View, Is.InstanceOf( viewType ) );
    }

    [Test]
    public void GenerateDesignerCs_ValidParameters_NoExceptionThrown( )
    {
        //Design rootDesign = new Design( );
        //Type viewType = typeof( View );

        //Assert.DoesNotThrow( ( ) => _viewToCode.GenerateDesignerCs( rootDesign, viewType ) );
    }

    [Test]
    public void AddSubViewsToDesignerCs_ValidParameters_NoExceptionThrown( )
    {
        //View forView = new View( );
        //CodeDomArgs args = new CodeDomArgs( );
        //CodeExpression parentViewExpression = null;

        //Assert.DoesNotThrow( ( ) => _viewToCode.AddSubViewsToDesignerCs( forView, args, parentViewExpression ) );
    }
}