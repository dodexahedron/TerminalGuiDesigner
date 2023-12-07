using System;
using Terminal.Gui;
using TerminalGuiDesigner;

namespace UnitTests;

[TestFixture]
[TestOf( typeof( ViewFactory ) )]
[Category( "Core" )]
internal class ViewFactoryTests : Tests
{
    private static readonly MenuBarItem[] DefaultMenuBarItems_ExpectedCollection =
    {
        new( "_File (F9)",
             new[]
             {
                 new MenuItem( ViewFactory.DefaultMenuItemText,
                               string.Empty,
                               ( ) => { } )
             } )
    };

    private static readonly Type[] KnownUnsupportedTypes_ExpectedTypes =
    {
        typeof( Toplevel ),
        typeof( Dialog ),
        typeof( FileDialog ),
        typeof( SaveDialog ),
        typeof( OpenDialog ),
        typeof( ScrollBarView ),
        typeof( TreeView<> ),
        typeof( Slider<> ),
        typeof( Frame ),
        typeof( Wizard ),
        typeof( WizardStep )
    };

    private static MenuBarItem[] ViewFactory_DefaultMenuBarItems => ViewFactory.DefaultMenuBarItems;

    private static Type[] ViewFactory_KnownUnsupportedTypes => ViewFactory.KnownUnsupportedTypes;

    [Test]
    [Category( "Change Control" )]
    public void DefaultMenuBarItems_IsExactlyAsExpected( )
    {
        Assert.Multiple( ( ) =>
        {
            Assert.That( ViewFactory_DefaultMenuBarItems, Has.Length.EqualTo( 1 ) );
            Assert.That( ViewFactory_DefaultMenuBarItems[ 0 ].Title, Is.EqualTo( "_File (F9)" ) );
        } );

        Assert.Multiple( ( ) =>
        {
            Assert.That( ViewFactory_DefaultMenuBarItems[ 0 ].Children, Has.Length.EqualTo( 1 ) );
            Assert.That( ViewFactory_DefaultMenuBarItems[ 0 ].Children[ 0 ].Title, Is.EqualTo( ViewFactory.DefaultMenuItemText ) );
            Assert.That( ViewFactory_DefaultMenuBarItems[ 0 ].Children[ 0 ].Help, Is.Empty );
        } );
    }

    [Test]
    [Description( "Checks that all tested types exist in the collection in ViewFactory" )]
    [Category( "Change Control" )]
    public void KnownUnsupportedTypes_ContainsExpectedItems( [ValueSource( nameof( KnownUnsupportedTypes_ExpectedTypes ) )] Type expectedType )
    {
        Assert.That( ViewFactory.KnownUnsupportedTypes, Contains.Item( expectedType ) );
    }

    [Test]
    [Description( "Checks that no new types have been added that aren't tested" )]
    [Category( "Change Control" )]
    public void KnownUnsupportedTypes_DoesNotContainUnexpectedItems( [ValueSource( nameof( ViewFactory_KnownUnsupportedTypes ) )] Type typeDeclaredUnsupportedInViewFactory )
    {
        Assert.That( KnownUnsupportedTypes_ExpectedTypes, Contains.Item( typeDeclaredUnsupportedInViewFactory ) );
    }

    [Test]
    [Category( "Change Control" )]
    public void ViewType_IsTerminalGuiView( )
    {
        Assert.That( ViewFactory.ViewType, Is.EqualTo( typeof( View ) ) );
    }
}
