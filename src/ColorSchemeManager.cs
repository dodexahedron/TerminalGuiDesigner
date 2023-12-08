using System.Collections.ObjectModel;
using System.Reflection;
using Terminal.Gui;
using TerminalGuiDesigner;
using TerminalGuiDesigner.Operations;
using System.Linq;

namespace TerminalGuiDesigner;

/// <summary>
/// Tracks usage of <see cref="ColorScheme"/> in designed views.
/// Each <see cref="ColorScheme"/> that the user has created or are
/// supplied by the designer out of the box is modeled by <see cref="NamedColorScheme"/>.
/// This class hosts the collection of all <see cref="NamedColorScheme"/>.
/// </summary>
public class ColorSchemeManager
{
    private readonly List<NamedColorScheme> colorSchemes = new();

    private ColorSchemeManager()
    {
    }

    /// <summary>
    /// Gets the Singleton instance of <see cref="ColorSchemeManager"/>.
    /// </summary>
    public static ColorSchemeManager Instance { get; } = new();

    /// <summary>
    /// Gets all known named color schemes defined in editor.
    /// </summary>
    public ReadOnlyCollection<NamedColorScheme> Schemes => this.colorSchemes.AsReadOnly();

    /// <summary>
    /// Clears all <see cref="NamedColorScheme"/> tracked by manager.
    /// </summary>
    public void Clear()
    {
        this.colorSchemes.Clear();
    }

    /// <summary>
    /// Makes <see cref="ColorSchemeManager"/> forget about <paramref name="toDelete"/>.
    /// Note that this does not remove it from any users (to do that use
    /// <see cref="DeleteColorSchemeOperation"/> instead).
    /// </summary>
    /// <param name="toDelete"><see cref="NamedColorScheme"/> to forget about.</param>
    public void Remove(NamedColorScheme toDelete)
    {
        // match on name as instances may change e.g. due to Undo/Redo etc
        var match = this.colorSchemes.FirstOrDefault(s => s.Name.Equals(toDelete.Name));

        if (match != null)
        {
            this.colorSchemes.Remove(match);
        }
    }

    /// <summary>
    ///   Populates the internal collection of <see cref="NamedColorScheme" />s based on the private <see cref="ColorScheme" /> fields
    ///   declared in the Designer.cs file of the <paramref name="viewBeingEdited" />.<br />
    /// </summary>
    /// <remarks>Does not clear any existing known <see cref="ColorScheme" />s.</remarks>
    /// <param name="viewBeingEdited">
    ///   View to find color schemes in, must be the root design (i.e. <see cref="Design.IsRoot" />).
    /// </param>
    /// <exception cref="ArgumentException">Thrown if passed a non-root <see cref="Design" />.</exception>
    public void FindDeclaredColorSchemes(Design viewBeingEdited)
    {
        if (!viewBeingEdited.IsRoot)
        {
            throw new ArgumentException("Expected to only be passed the root view");
        }

        // Find all private fields of type ColorScheme in the class,
        // filter out any that are null,
        // and add any that don't already exist in the ColorSchemeManager to the collection
        colorSchemes.AddRange( viewBeingEdited.View.GetType( )
                                              .GetFields( BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly )
                                              .Where( FieldIsColorScheme )
                                              .Select( FieldAndSchemeTuples )
                                              .Where( SchemeIsNotNull )
                                              .Where( SameNameDoesNotAlreadyExist )
                                              .Select( NamedColorSchemesToAdd ) );
        return;

        static bool FieldIsColorScheme( FieldInfo fieldInfo )
        {
            return fieldInfo.FieldType == typeof( ColorScheme );
        }

        (FieldInfo Field, ColorScheme? Scheme) FieldAndSchemeTuples( FieldInfo fieldInfo )
        {
            return ( Field: fieldInfo, Scheme: fieldInfo.GetValue( viewBeingEdited.View ) as ColorScheme );
        }

        static bool SchemeIsNotNull( (FieldInfo Field, ColorScheme? Scheme) fieldAndScheme )
        {
            return fieldAndScheme.Scheme is not null;
        }

        bool SameNameDoesNotAlreadyExist( (FieldInfo Field, ColorScheme? Scheme) fieldAndNonNullScheme )
        {
            return !colorSchemes.Any( namedColorScheme => namedColorScheme.Name.Equals( fieldAndNonNullScheme.Field.Name ) );
        }

        static NamedColorScheme NamedColorSchemesToAdd( (FieldInfo Field, ColorScheme? Scheme) fieldAndScheme )
        {
            return new ( fieldAndScheme.Field.Name, fieldAndScheme.Scheme! );
        }
    }

    /// <summary>
    /// Returns the <see cref="NamedColorScheme.Name"/> for <paramref name="s"/>
    /// if it is in the collection of known <see cref="Schemes"/>.
    /// </summary>
    /// <param name="s">A <see cref="ColorScheme"/> to look up.</param>
    /// <returns>The name of the scheme or null if it is not known.</returns>
    public string? GetNameForColorScheme(ColorScheme s)
    {
        var match = this.colorSchemes.Where(kvp => s.Equals(kvp.Scheme)).ToArray();

        if (match.Length > 0)
        {
            return match[0].Name;
        }

        // no match
        return null;
    }

    /// <summary>
    /// Updates the named scheme to use the new colors in <paramref name="scheme"/>.  This
    /// will also update all Views in <paramref name="rootDesign"/> which currently use the
    /// named scheme.
    /// </summary>
    /// <param name="name">The user generated name for the <see cref="ColorScheme"/>.
    /// Will become <see cref="NamedColorScheme.Name"/>.</param>
    /// <param name="scheme">The new <see cref="ColorScheme"/> color values to use.</param>
    /// <param name="rootDesign">The topmost <see cref="Design"/> the user is editing (see <see cref="Design.GetRootDesign"/>).</param>
    public void AddOrUpdateScheme(string name, ColorScheme scheme, Design rootDesign)
    {
        var oldScheme = this.colorSchemes.FirstOrDefault(c => c.Name.Equals(name));

        // if we don't currently know about this scheme
        if (oldScheme == null)
        {
            // simply record that we now know about it and exit
            this.colorSchemes.Add(new NamedColorScheme(name, scheme));
            return;
        }

        // we know about this color already and people may be using it!
        foreach (var old in rootDesign.GetAllDesigns())
        {
            // if view uses the scheme that is being replaced (value not reference equality)
            if (old.UsesColorScheme(oldScheme.Scheme))
            {
                // use the new one instead (for the presented View in the GUI and the known state)
                old.View.ColorScheme = old.State.OriginalScheme = scheme;
            }
        }

        oldScheme.Scheme = scheme;
    }

    /// <summary>
    /// Renames the known <see cref="Schemes"/> that is called <paramref name="oldName"/> to
    /// <paramref name="newName"/> if the name exists in <see cref="Schemes"/>.
    /// </summary>
    /// <param name="oldName">The name to change.</param>
    /// <param name="newName">The value to change it to.</param>
    public void RenameScheme(string oldName, string newName)
    {
        var match = this.colorSchemes.FirstOrDefault(c => c.Name.Equals(oldName));

        if (match != null)
        {
            match.Name = newName;
        }
    }

    /// <summary>
    /// Returns the <see cref="NamedColorScheme"/> from <see cref="Schemes"/> where
    /// <see cref="NamedColorScheme.Name"/> matches <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name to look up.</param>
    /// <returns>The scheme if found or null.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the <paramref name="name"/> is not present in <see cref="Schemes"/>.</exception>
    public NamedColorScheme GetNamedColorScheme(string name)
    {
        return this.colorSchemes.FirstOrDefault(c => c.Name.Equals(name))
            ?? throw new KeyNotFoundException($"Could not find a named ColorScheme called {name}");
    }
}