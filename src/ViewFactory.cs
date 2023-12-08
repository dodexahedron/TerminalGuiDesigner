using System.Data;
using System.Reflection;
using Terminal.Gui;
using Terminal.Gui.TextValidateProviders;
using TerminalGuiDesigner.Operations.MenuOperations;
using Attribute = Terminal.Gui.Attribute;

namespace TerminalGuiDesigner;

/// <summary>
///   Creates new <see cref="View" /> instances configured to have sensible dimensions
///   and content for dragging/configuring in the designer.
/// </summary>
public static class ViewFactory
{
    /// <summary>
    ///   A constant defining the default text for a new menu item added via the <see cref="ViewFactory" />
    /// </summary>
    /// <remarks>
    ///   <see cref="AddMenuOperation" /> adds a new top level menu (e.g. File, Edit etc.).<br />
    ///   In the designer, all menus must have at least 1 <see cref="MenuItem" /> under them, so it will be
    ///   created with a single <see cref="MenuItem" /> in it, already.<br />
    ///   That item will bear this text.<br /><br />
    ///   This string should be used by any other areas of code that want to create new <see cref="MenuItem" /> under
    ///   a top/sub menu (e.g. <see cref="ViewFactory" />).
    /// </remarks>
    /// <value>The string "Edit Me"</value>
    /// <seealso cref="DefaultMenuBarItems" />
    internal const string DefaultMenuItemText = "Edit Me";

    internal static readonly Type[] KnownUnsupportedTypes = 
    {
        typeof( Toplevel ),
        typeof( Dialog ),
        typeof( FileDialog ),
        typeof( SaveDialog ),
        typeof( OpenDialog ),
        typeof( ScrollBarView ),
        typeof( TreeView<> ),
        typeof( Slider<> ),

        // Theses are special types of view and shouldn't be added manually by user
        typeof( Frame ),
        // BUG These seem to cause stack overflows in CreateSubControlDesigns (see TestAddView_RoundTrip)
        typeof( Wizard ),
        typeof( WizardStep ),
    };

    /// <summary>
    ///   A static reference to <see langword="typeof" />(<see cref="View" />)
    /// </summary>
    internal static readonly Type ViewType = typeof(View);

    /// <summary>
    ///   Gets a new instance of a default <see cref="MenuBarItem" />[], to include as the default initial
    ///   <see cref="MenuBar.Menus" />
    ///   collection of a new <see cref="MenuBar" />
    /// </summary>
    /// <value>
    ///   A new single-element array of <see cref="MenuBarItem" />, with default text, an empty
    ///   <see cref="MenuItem.Action" />, and empty <see cref="MenuItem.Help" /> string.
    /// </value>
    internal static MenuBarItem[] DefaultMenuBarItems
    {
        get
        {
            return new[]
            {
                new MenuBarItem(
                    "_File (F9)",
                    new[] { new MenuItem( DefaultMenuItemText, string.Empty, ( ) => { } ) } )
            };
        }
    }

    /// <summary>
    ///   Gets all <see cref="View" /> Types that are supported by <see cref="ViewFactory" />.
    /// </summary>
    /// <value>An <see cref="IEnumerable{T}" /> of <see cref="Type" />s supported by <see cref="ViewFactory" />.</value>
    public static IEnumerable<Type> SupportedViewTypes { get; } =
        ViewType.Assembly.DefinedTypes
                .Where( unfilteredType => unfilteredType is
                {
                    IsInterface: false,
                    IsAbstract: false,
                    IsPublic: true,
                    IsValueType: false
                } )
                .Where( filteredType => filteredType.IsSubclassOf( ViewType ) )
                .Where( viewDescendantType => !KnownUnsupportedTypes.Any( viewDescendantType.IsAssignableTo )
                                              || viewDescendantType == typeof( Window ) );

    private static bool IsSupportedType( this Type t )
    {
        return t == typeof( Window ) || ( !KnownUnsupportedTypes.Any( t.IsSubclassOf ) & !KnownUnsupportedTypes.Contains( t ) );
    }

    /// <summary>
    ///   Creates a new instance of a <see cref="View" /> of Type <typeparamref name="T" /> with
    ///   size/placeholder values that make it easy to see and design in the editor.
    /// </summary>
    /// <typeparam name="T">
    ///   A concrete descendant type of <see cref="View" /> that does not exist in the
    ///   <see cref="KnownUnsupportedTypes" /> collection and which has a public constructor.
    /// </typeparam>
    /// <param name="width">
    ///   An optional width of the requested view. Default values are dependent on the requested
    ///   type, if not supplied.
    /// </param>
    /// <param name="height">
    ///   An optional height of the requested view. Default values are dependent on the requested
    ///   type, if not supplied.
    /// </param>
    /// <exception cref="NotSupportedException">If an unsupported type is requested</exception>
    /// <returns>
    ///   A new instance of <paramref name="{T}" /> with the specified dimensions or defaults, if not provided.
    /// </returns>
    /// <remarks>
    ///   <typeparamref name="T" /> must inherit from <see cref="View" />, must have a public constructor, and must
    ///   not exist in the <see cref="KnownUnsupportedTypes" /> collection, at run-time.
    /// </remarks>
    public static T Create<T>(int? width = null, int? height = null )
        where T : View, new( )
    {
        if ( !IsSupportedType( typeof( T ) ) )
        {
            throw new NotSupportedException( $"Requested type {typeof( T ).Name} is not supported" );
        }

        T newView = new( );

        switch ( newView )
        {
            case TableView tv:
                var dt = new DataTable( );
                dt.Columns.Add( "Column 0" );
                dt.Columns.Add( "Column 1" );
                dt.Columns.Add( "Column 2" );
                dt.Columns.Add( "Column 3" );
                SetDefaultDimensions( newView, width ?? 50, height ?? 5 );
                tv.Table = new DataTableSource( dt );
                break;
            case TabView tv:
                tv.AddEmptyTab( "Tab1" );
                tv.AddEmptyTab( "Tab2" );
                SetDefaultDimensions( newView, width ?? 50, height ?? 5 );
                break;
            case TextValidateField tvf:
                tvf.Provider = new TextRegexProvider( ".*" );
                tvf.Text = "Heya";
                SetDefaultDimensions( newView, width ?? 5, height ?? 1 );
                break;
            case DateField df:
                df.Date = DateTime.Now;
                SetDefaultDimensions( newView, width ?? 20, height ?? 1 );
                break;
            case TextField tf:
                tf.Text = "Heya";
                SetDefaultDimensions( newView, width ?? 5, height ?? 1 );
                break;
            case ProgressBar pb:
                pb.Fraction = 1f;
                SetDefaultDimensions( newView, width ?? 10, height ?? 1 );
                break;
            case MenuBar mb:
                mb.Menus = DefaultMenuBarItems;
                break;
            case StatusBar sb:
                sb.Items = new[] { new StatusItem( Key.F1, "F1 - Edit Me", null ) };
                break;
            case RadioGroup rg:
                rg.RadioLabels = new string[] { "Option 1", "Option 2" };
                SetDefaultDimensions( newView, width ?? 10, height ?? 2 );
                break;
            case GraphView gv:
                gv.GraphColor = new Attribute( Color.White, Color.Black );
                SetDefaultDimensions( newView, width ?? 20, height ?? 5 );
                break;
            case ListView lv:
                lv.SetSource( new List<string> { "Item1", "Item2", "Item3" } );
                SetDefaultDimensions( newView, width ?? 20, height ?? 3 );
                break;
            case FrameView:
            case HexView:
                newView.SetActualText( "Heya" );
                SetDefaultDimensions( newView, width ?? 10, height ?? 5 );
                break;
            case Window:
                SetDefaultDimensions( newView, width ?? 10, height ?? 5 );
                break;
            case LineView:
                SetDefaultDimensions( newView, width ?? 8, height ?? 1 );
                break;
            case TreeView:
                SetDefaultDimensions( newView, width ?? 16, height ?? 5 );
                break;
            case ScrollView sv:
                sv.ContentSize = new Size( 20, 10 );
                SetDefaultDimensions(newView, width ?? 10, height ?? 5 );
                break;
            case Label l:
                l.SetActualText( "Heya" );
                SetDefaultDimensions( newView, width ?? 4, height ?? 1 );
                break;
            case SpinnerView sv:
                sv.AutoSpin = true;
                if ( width is not null )
                {
                    sv.Width = width;
                }

                if ( height is not null )
                {
                    sv.Height = height;
                }

                break;
            default:
                SetDefaultDimensions( newView, width ?? 5, height ?? 1 );
                break;
        }

        return newView;

        static void SetDefaultDimensions( T v, int width = 5, int height = 1 )
        {
            v.Width = Math.Max( v.Bounds.Width, width );
            v.Height = Math.Max( v.Bounds.Height, height );
        }
    }

    /// <summary>
    ///   Creates a new instance of <see cref="View" /> of Type <paramref name="t" /> with
    ///   size/placeholder values that make it easy to see and design in the editor.
    /// </summary>
    /// <param name="t">
    ///   A Type of <see cref="View" />.<br />
    ///   See <see cref="SupportedViewTypes" /> for the full list of allowed Types.
    /// </param>
    /// <returns>A new instance of Type <paramref name="t" />.</returns>
    /// <exception cref="Exception">Thrown if Type is not a subclass of <see cref="View" />.</exception>
    /// <remarks>Delegates to <see cref="Create{T}" />, for types supported by that method.</remarks>
    public static View Create(Type t)
    {
        if (typeof(TableView).IsAssignableFrom(t))
        {
            return Create<TableView>( );
        }

        if (typeof(TabView).IsAssignableFrom(t))
        {
            return Create<TabView>( );
        }

        if (typeof(RadioGroup).IsAssignableFrom(t))
        {
            return Create<RadioGroup>( );
        }

        if (typeof(MenuBar).IsAssignableFrom(t))
        {
            return Create<MenuBar>( );
        }

        if (typeof(StatusBar).IsAssignableFrom(t))
        {
            return Create<StatusBar>( );
        }

        if (t == typeof(TextValidateField))
        {
            return Create<TextValidateField>( );
        }

        if (t == typeof(ProgressBar))
        {
            return Create<ProgressBar>( );
        }

        if (t == typeof(View))
        {
            return Create<View>( );
        }

        if (t == typeof(Window))
        {
            return Create<Window>( );
        }

        if (t == typeof(TextField))
        {
            return Create<TextField>( );
        }

        if (typeof(GraphView).IsAssignableFrom(t))
        {
            return Create<GraphView>( );
        }

        if (typeof(ListView).IsAssignableFrom(t))
        {
            return Create<ListView>( );
        }

        if (t == typeof(LineView))
        {
            return Create<LineView>( );
        }

        if (t == typeof(TreeView))
        {
            return Create<TreeView>( );
        }

        if (t == typeof(ScrollView))
        {
            return Create<ScrollView>( );
        }

        if (typeof(SpinnerView).IsAssignableFrom(t))
        {
            return Create<SpinnerView>( );
        }

        if ( typeof( FrameView ).IsAssignableFrom( t ) )
        {
            return Create<FrameView>( );
        }

        if ( typeof( HexView ).IsAssignableFrom( t ) )
        {
            return Create<HexView>( );
        }

        var instance = Activator.CreateInstance(t) as View ?? throw new Exception($"CreateInstance returned null for Type '{t}'");
        instance.SetActualText("Heya");

        instance.Width = Math.Max(instance.Bounds.Width, 4);
        instance.Height = Math.Max(instance.Bounds.Height, 1);

        return instance;
    }
}
