using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terminal.Gui;
using TerminalGuiDesigner.UI.Windows;

namespace TerminalGuiDesigner.Operations;

/// <summary>
///   <see cref="Operation" /> for adding a new <see cref="View" /> to a <see cref="Design" />.
///   Supports adding to the root or any container view (e.g. <see cref="TabView" />).
/// </summary>
/// <typeparam name="T">
///   The type of <see cref="View" /> for this operation.<br />
///   Must inherit from <see cref="View" /> and be a supported type.<br />
///   Explicitly using <see cref="View" />, as a fallback, is acceptable.<br />
///   Must be a concrete type understood by <see cref="ViewFactory" />
/// </typeparam>
public class AddViewOperation : Operation
{
    private readonly Design to;
    private View? add;
    private string? fieldName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddViewOperation"/> class.
    /// When/If run this operation will add <paramref name="add"/> to the <see cref="View"/>
    /// wrapped by <paramref name="to"/> with the provided <paramref name="fieldName"/>.
    /// </summary>
    /// <param name="add">An instance of <typeparamref name="T"/>, which must inherit from <see cref="View"/> to add.<br />
    /// If you want to pick at runtime then
    /// use <see cref="AddViewOperation(Design)"/> overload instead.</param>
    /// <param name="to">A <see cref="Design"/> (which should be <see cref="Design.IsContainerView"/>)
    /// to add the <paramref name="add"/> to.</param>
    /// <param name="fieldName">Field name to assign to <paramref name="add"/> when wrapping it as a
    /// <see cref="Design"/>.  This determines the private field name that it will have in the .Designer.cs
    /// file.</param>
    public AddViewOperation(View add, Design to, string? fieldName)
    {
        this.add = add;
        this.fieldName = fieldName == null
            ? to.GetUniqueFieldName(add.GetType())
            : to.GetUniqueFieldName(fieldName);
        this.to = to;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddViewOperation"/> class.
    /// This overload asks users what view type they want at runtime (See <see cref="Operation.Do"/>).
    /// </summary>
    /// <param name="design">A <see cref="Design"/> (which should be <see cref="Design.IsContainerView"/>)
    /// to add any newly created <see cref="View"/> to.</param>
    public AddViewOperation(Design design)
    {
        this.to = design;
    }

    /// <inheritdoc/>
    public override void Redo()
    {
        if (this.add == null)
        {
            return;
        }

        var v = this.GetViewToAddTo();
        v.Add(this.add);
        v.SetNeedsDisplay();
    }

    /// <inheritdoc/>
    public override void Undo()
    {
        if (this.add == null)
        {
            return;
        }

        var v = this.GetViewToAddTo();
        v.Remove(this.add);
        v.SetNeedsDisplay();
    }

    /// <inheritdoc/>
    protected override bool DoImpl()
    {
        if (this.add == null)
        {
            var selectable = ViewFactory.SupportedViewTypes.ToArray();

            if (Modals.Get("Type of Control", "Add", true, selectable, static t => t?.Name ?? "Null", false, null, out var selected))
            {
                this.add = ViewFactory.Create( add.GetType() );
                this.fieldName = this.to.GetUniqueFieldName(selected);
            }
        }

        // user canceled picking a type
        if (this.add == null || string.IsNullOrWhiteSpace(this.fieldName))
        {
            return false;
        }

        Design design;
        this.add.Data = design = this.to.CreateSubControlDesign(this.fieldName, this.add);

        var v = this.GetViewToAddTo();
        v.Add(this.add);

        if (Application.Driver != null)
        {
            this.add.SetFocus();
        }

        SelectionManager.Instance.ForceSetSelection(design);

        v.SetNeedsDisplay();
        return true;
    }

    private View GetViewToAddTo()
    {
        if (this.to.View is TabView tabView)
        {
            return tabView.SelectedTab.View;
        }

        return this.to.View;
    }
}