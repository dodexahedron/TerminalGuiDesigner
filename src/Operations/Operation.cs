﻿using Terminal.Gui;

namespace TerminalGuiDesigner.Operations;

/// <summary>
/// Abstract base class for <see cref="IOperation{TView}"/>.
/// </summary>
public abstract class Operation : IOperation
{
    /// <summary>
    /// The name to give to all objects which do not have a title/text etc.
    /// </summary>
    public const string Unnamed = "unnamed";

    /// <inheritdoc/>
    public bool IsImpossible { get; protected set; }

    /// <inheritdoc/>
    /// <remarks>Defaults to true.</remarks>
    public bool SupportsUndo { get; protected set; } = true;

    /// <inheritdoc/>
    /// <remarks>Defaults to <see cref="Guid.NewGuid"/>.</remarks>
    public Guid UniqueIdentifier { get; } = Guid.NewGuid();

    /// <inheritdoc/>
    public string Category { get; protected set; } = string.Empty;

    /// <inheritdoc/>
    /// <remarks>Defaults to <see cref="GetOperationName"/>.</remarks>
    public override string ToString()
    {
        return this.GetOperationName();
    }

    /// <inheritdoc/>
    /// <remarks>Returns false if <see cref="IsImpossible"/>.</remarks>
    public bool Do()
    {
        if (this.IsImpossible)
        {
            return false;
        }

        return this.DoImpl();
    }

    /// <inheritdoc/>
    public abstract void Undo();

    /// <inheritdoc/>
    public abstract void Redo();

    /// <inheritdoc cref="IOperation.Do"/>
    protected abstract bool DoImpl();

    /// <summary>
    /// Returns human-readable name for the instance based on it's
    /// C# class name.
    /// </summary>
    /// <returns>Human-readable name.</returns>
    protected string GetOperationName()
    {
        string name = this.GetType().Name;

        return name.EndsWith("Operation") ?
            name.Substring(0, name.Length - "Operation".Length) :
            name;
    }

    /// <summary>
    /// Removes from <paramref name="collection"/> any element which is a child of any other element.
    /// This prevents a copy of a container + 1 or more of its content items resulting in duplicate
    /// pasting.
    /// </summary>
    /// <param name="collection">A collection to modify such that none contains any other.</param>
    /// <returns>Collection without any elements that are children of other elements.</returns>
    protected Design[]? PruneChildViews(Design[]? collection)
    {
        if (collection == null || collection.Length == 0)
        {
            return null;
        }

        var toReturn = collection.ToList().Cast<Design>().ToList();

        foreach (var e in collection)
        {
            var children = e.GetAllChildDesigns(e.View).ToArray();
            toReturn.RemoveAll(children.Contains);
        }

        return toReturn.ToArray();
    }
}
