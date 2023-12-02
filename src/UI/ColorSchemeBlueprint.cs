﻿using Terminal.Gui;
using YamlDotNet.Serialization;
using Attribute = Terminal.Gui.Attribute;

namespace TerminalGuiDesigner.UI;

/// <summary>
/// Serialize-able version of <see cref="ColorScheme"/>.
/// </summary>
public class ColorSchemeBlueprint
{
    /// <summary>
    /// Gets or Sets the <see cref="Color"/> to use for <see cref="ColorScheme.Normal"/> <see cref="Attribute.Foreground"/>.
    /// </summary>
    public ColorName NormalForeground { get; set; }

    /// <summary>
    /// Gets or Sets the <see cref="Color"/> to use for <see cref="ColorScheme.Normal"/> <see cref="Attribute.Background"/>.
    /// </summary>
    public ColorName NormalBackground { get; set; }

    /// <summary>
    /// Gets or Sets the <see cref="Color"/> to use for <see cref="ColorScheme.HotNormal"/> <see cref="Attribute.Foreground"/>.
    /// </summary>
    public ColorName HotNormalForeground { get; set; }

    /// <summary>
    /// Gets or Sets the <see cref="Color"/> to use for <see cref="ColorScheme.HotNormal"/> <see cref="Attribute.Background"/>.
    /// </summary>
    public ColorName HotNormalBackground { get; set; }

    /// <summary>
    /// Gets or Sets the <see cref="Color"/> to use for <see cref="ColorScheme.Focus"/> <see cref="Attribute.Foreground"/>.
    /// </summary>
    public ColorName FocusForeground { get; set; }

    /// <summary>
    /// Gets or Sets the <see cref="Color"/> to use for <see cref="ColorScheme.Focus"/> <see cref="Attribute.Background"/>.
    /// </summary>
    public ColorName FocusBackground { get; set; }

    /// <summary>
    /// Gets or Sets the <see cref="Color"/> to use for <see cref="ColorScheme.HotFocus"/> <see cref="Attribute.Foreground"/>.
    /// </summary>
    public ColorName HotFocusForeground { get; set; }

    /// <summary>
    /// Gets or Sets the <see cref="Color"/> to use for <see cref="ColorScheme.HotFocus"/> <see cref="Attribute.Background"/>.
    /// </summary>
    public ColorName HotFocusBackground { get; set; }

    /// <summary>
    /// Gets or Sets the <see cref="Color"/> to use for <see cref="ColorScheme.Disabled"/> <see cref="Attribute.Foreground"/>.
    /// </summary>
    public ColorName DisabledForeground { get; set; }

    /// <summary>
    /// Gets or Sets the <see cref="Color"/> to use for <see cref="ColorScheme.Disabled"/> <see cref="Attribute.Background"/>.
    /// </summary>
    public ColorName DisabledBackground { get; set; }

    /// <summary>
    /// Gets a new <see cref="ColorScheme"/> from the blueprint.
    /// </summary>
    [YamlIgnore]
    public ColorScheme Scheme => new ColorScheme
    {
        Normal = new Attribute(this.NormalForeground, this.NormalBackground),
        HotNormal = new Attribute(this.HotNormalForeground, this.HotNormalBackground),
        Focus = new Attribute(this.FocusForeground, this.FocusBackground),
        HotFocus = new Attribute(this.HotFocusForeground, this.HotFocusBackground),
        Disabled = new Attribute(this.DisabledForeground, this.DisabledBackground),
    };
}