﻿using Terminal.Gui;
using TerminalGuiDesigner.Operations.Generics;
using TerminalGuiDesigner.UI.Windows;

namespace TerminalGuiDesigner.Operations.StatusBarOperations
{
    /// <summary>
    /// Changes the <see cref="StatusItem.Shortcut"/> of a <see cref="StatusItem"/> on
    /// a <see cref="StatusBar"/>.
    /// </summary>
    public class SetShortcutOperation : GenericArrayElementOperation<StatusBar, StatusItem>
    {
        private Key originalShortcut;
        private Key? shortcut;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetShortcutOperation"/> class.
        /// </summary>
        /// <param name="design">Wrapper for a <see cref="StatusBar"/>.</param>
        /// <param name="statusItem">The <see cref="StatusItem"/> whose shortcut you want to change.</param>
        /// <param name="shortcut">The new shortcut or null to prompt user at runtime.</param>
        public SetShortcutOperation(Design design, StatusItem statusItem, Key? shortcut)
            : base(
                  (v) => v.Items,
                  (v, a) => v.Items = a,
                  (e) => e.Title?.ToString() ?? Unnamed,
                  design,
                  statusItem)
        {
            this.shortcut = shortcut;
            this.originalShortcut = statusItem.Shortcut;
        }

        /// <inheritdoc/>
        public override void Redo()
        {
            if (this.shortcut == null)
            {
                return;
            }

            this.OperateOn.SetShortcut(this.shortcut.Value);
        }

        /// <inheritdoc/>
        public override void Undo()
        {
            this.OperateOn.SetShortcut(this.originalShortcut);
        }

        /// <inheritdoc/>
        protected override bool DoImpl()
        {
            if (this.shortcut == null)
            {
                this.shortcut = Modals.GetShortcut();
            }

            this.OperateOn.SetShortcut(this.shortcut.Value);
            return true;
        }
    }
}
