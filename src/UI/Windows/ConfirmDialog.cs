﻿
//------------------------------------------------------------------------------

//  <auto-generated>
//      This code was generated by:
//        TerminalGuiDesigner v1.0.18.0
//      You can make changes to this file and they will not be overwritten when saving.
//  </auto-generated>
// -----------------------------------------------------------------------------
namespace TerminalGuiDesigner.UI.Windows {
    using NStack;
    using Terminal.Gui;
    
    
    public partial class ConfirmDialog {
        
        public bool Result { get; private set; }

        private string _title;

        public ConfirmDialog(string title, string message, string okText = "Yes", string cancelText = "No") {
            
            const int defaultWidth = 50;

            InitializeComponent();

            // add space for right hand side shadow
            btnOk.Text = okText + " ";
            btnOk.Width = okText.Length + 4;
            btnOk.Clicked += () => {
                Result = true;
                Application.RequestStop();
                };
            btnOk.ColorScheme = Colors.Dialog;
            btnOk.DrawContentComplete += (r)=>PaintShadow(btnOk);


            btnCancel.Text = cancelText + " ";
            btnCancel.Width = okText.Length + 2;
            btnCancel.Clicked += () => {
                Result = false;
                Application.RequestStop();
            };
            btnCancel.ColorScheme = Colors.Dialog;
            btnCancel.DrawContentComplete += (r) => PaintShadow(btnCancel);

            _title = title;
                
            label1.Text = message;

            int buttonWidth;

            // align buttons bottom of dialog 
            buttonPanel.Width = buttonWidth = btnOk.Frame.Width + btnCancel.Frame.Width + 1;
            
            int maxWidthLine = TextFormatter.MaxWidthLine(message);
            if (maxWidthLine > Application.Driver.Cols)
            {
                maxWidthLine = Application.Driver.Cols;
            }
            
            maxWidthLine = Math.Max(maxWidthLine, defaultWidth);
               

            int textWidth = Math.Min(TextFormatter.MaxWidth(message, maxWidthLine), Application.Driver.Cols);
            int textHeight = TextFormatter.MaxLines(message, textWidth) + 1; // message.Count (ustring.Make ('\n')) + 1;
            int msgboxHeight = Math.Min(Math.Max(1, textHeight) + 4, Application.Driver.Rows); // textHeight + (top + top padding + buttons + bottom)

            Width = Math.Min(Math.Max(maxWidthLine, Math.Max(Title.ConsoleWidth, Math.Max(textWidth + 2, buttonWidth))), Application.Driver.Cols);
            Height = msgboxHeight;
        }

        private void PaintShadow(Button btn)
        {
            var bounds = btn.Bounds;

            // draw the 'end' button symbol one in
            btn.AddRune(bounds.Width - 2, 0, ']');

            var backgroundColor = ColorScheme.Normal.Background;

            // shadow color
            Driver.SetAttribute(new Terminal.Gui.Attribute(Color.Black, backgroundColor));

            // end shadow (right)
            btn.AddRune(bounds.Width - 1, 0, '▄');

            // leave whitespace in lower left in parent/default background color
            Driver.SetAttribute(new Terminal.Gui.Attribute(Color.Black, backgroundColor));
            btn.AddRune(0, 1, ' ');

            // The color for rendering shadow is 'black' + parent/default background color
            Driver.SetAttribute(new Terminal.Gui.Attribute(backgroundColor, Color.Black));

            // underline shadow                
            for (int x = 1; x < bounds.Width; x++)
            {
                btn.AddRune(x, 1, '▄');
            }
        }
        public override void Redraw(Rect bounds)
        {
            base.Redraw(bounds);

            Move(1, 0, false);

            var padding = ((bounds.Width - _title.Sum(Rune.ColumnWidth)) / 2) - 1;

            Driver.SetAttribute(
                new Attribute(ColorScheme.Normal.Foreground, ColorScheme.Normal.Background));
            
            Driver.AddStr(ustring.Make(Enumerable.Repeat(Driver.HDLine,padding)));

            Driver.SetAttribute(
                new Attribute(ColorScheme.Normal.Background, ColorScheme.Normal.Foreground));
            Driver.AddStr(_title);

            Driver.SetAttribute(
                new Attribute(ColorScheme.Normal.Foreground, ColorScheme.Normal.Background));
            Driver.AddStr(ustring.Make(Enumerable.Repeat(Driver.HDLine, padding)));
        }

        internal static bool Show(string title, string message, string okText = "Yes", string cancelText = "No")
        {
            var dlg = new ConfirmDialog(title, message, okText, cancelText);
            Application.Run(dlg);
            return dlg.Result;
        }
    }
}
