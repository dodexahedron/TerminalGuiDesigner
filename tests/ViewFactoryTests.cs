using System;
using Terminal.Gui;
using TerminalGuiDesigner;

namespace UnitTests;

[TestFixture]
[TestOf( typeof( ViewFactory ) )]
[Category( "Core" )]
internal class ViewFactoryTests : Tests
{
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

    private static Type[] ViewFactory_KnownUnsupportdTypes => ViewFactory.KnownUnsupportedTypes;

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
    public void KnownUnsupportedTypes_DoesNotContainUnexpectedItems( [ValueSource( nameof( ViewFactory_KnownUnsupportdTypes ) )] Type typeDeclaredUnsupportedInViewFactory )
    {
        Assert.That( KnownUnsupportedTypes_ExpectedTypes, Contains.Item( typeDeclaredUnsupportedInViewFactory ) );
    }
}
