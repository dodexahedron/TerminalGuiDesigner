
//------------------------------------------------------------------------------

//  <auto-generated>
//      This code was generated by:
//        TerminalGuiDesigner v1.0.18.0
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// -----------------------------------------------------------------------------
namespace TerminalGuiDesigner.UI.Windows; 
using System;
using Terminal.Gui;


public partial class ColorPicker : Terminal.Gui.Dialog {
    
    private Terminal.Gui.Label lblForeground;
    
    private Terminal.Gui.Label lblBackground;
    
    private Terminal.Gui.Label lblResult;
    
    private Terminal.Gui.RadioGroup radiogroup1;
    
    private Terminal.Gui.RadioGroup radiogroup2;
    
    private Terminal.Gui.Label lblPreview;
    
    private Terminal.Gui.Button btnOk;
    
    private Terminal.Gui.Button btnCancel;
    
    private void InitializeComponent() {
        this.btnCancel = new Terminal.Gui.Button();
        this.btnOk = new Terminal.Gui.Button();
        this.lblPreview = new Terminal.Gui.Label();
        this.radiogroup2 = new Terminal.Gui.RadioGroup();
        this.radiogroup1 = new Terminal.Gui.RadioGroup();
        this.lblResult = new Terminal.Gui.Label();
        this.lblBackground = new Terminal.Gui.Label();
        this.lblForeground = new Terminal.Gui.Label();
        this.Width = 52;
        this.Height = 20;
        this.X = Pos.Center();
        this.Y = Pos.Center();
        this.Modal = true;
        this.Text = "";
        this.Border.BorderStyle = Terminal.Gui.LineStyle.Single;
        this.TextAlignment = Terminal.Gui.TextAlignment.Left;
        this.Title = "Color Picker";
        this.lblForeground.Width = 11;
        this.lblForeground.Height = 1;
        this.lblForeground.X = 1;
        this.lblForeground.Y = 0;
        this.lblForeground.Data = "lblForeground";
        this.lblForeground.Text = "Foreground:";
        this.lblForeground.TextAlignment = Terminal.Gui.TextAlignment.Left;
        this.Add(this.lblForeground);
        this.lblBackground.Width = 11;
        this.lblBackground.Height = 1;
        this.lblBackground.X = 17;
        this.lblBackground.Y = 0;
        this.lblBackground.Data = "lblBackground";
        this.lblBackground.Text = "Background:";
        this.lblBackground.TextAlignment = Terminal.Gui.TextAlignment.Left;
        this.Add(this.lblBackground);
        this.lblResult.Width = 7;
        this.lblResult.Height = 1;
        this.lblResult.X = 38;
        this.lblResult.Y = 0;
        this.lblResult.Data = "lblResult";
        this.lblResult.Text = "Result:";
        this.lblResult.TextAlignment = Terminal.Gui.TextAlignment.Left;
        this.Add(this.lblResult);
        this.radiogroup1.Width = 17;
        this.radiogroup1.Height = 16;
        this.radiogroup1.X = 3;
        this.radiogroup1.Y = 1;
        this.radiogroup1.Data = "radiogroup1";
        this.radiogroup1.Text = "";
        this.radiogroup1.TextAlignment = Terminal.Gui.TextAlignment.Left;
        this.radiogroup1.RadioLabels = new string[] {
                "Black",
                "Blue",
                "Green",
                "Cyan",
                "Red",
                "Magenta",
                "Yellow",
                "Gray",
                "DarkGray",
                "BrightBlue",
                "BrightGreen",
                "BrightCyan",
                "BrightRed",
                "BrightMagenta",
                "BrightYellow",
                "White"};
        this.Add(this.radiogroup1);
        this.radiogroup2.Width = 17;
        this.radiogroup2.Height = 16;
        this.radiogroup2.X = 20;
        this.radiogroup2.Y = 1;
        this.radiogroup2.Data = "radiogroup2";
        this.radiogroup2.Text = "";
        this.radiogroup2.TextAlignment = Terminal.Gui.TextAlignment.Left;
        this.radiogroup2.RadioLabels = new string[] {
                "Black",
                "Blue",
                "Green",
                "Cyan",
                "Red",
                "Magenta",
                "Yellow",
                "Gray",
                "DarkGray",
                "BrightBlue",
                "BrightGreen",
                "BrightCyan",
                "BrightRed",
                "BrightMagenta",
                "BrightYellow",
                "White"};
        this.Add(this.radiogroup2);
        this.lblPreview.Width = 13;
        this.lblPreview.Height = 1;
        this.lblPreview.X = 37;
        this.lblPreview.Y = 1;
        this.lblPreview.Data = "lblPreview";
        this.lblPreview.Text = "\"Sample Text\"";
        this.lblPreview.TextAlignment = Terminal.Gui.TextAlignment.Left;
        this.Add(this.lblPreview);
        this.btnOk.Width = 8;
        this.btnOk.X = 10;
        this.btnOk.Y = 17;
        this.btnOk.Data = "btnOk";
        this.btnOk.Text = "Ok";
        this.btnOk.TextAlignment = Terminal.Gui.TextAlignment.Centered;
        this.btnOk.IsDefault = true;
        this.Add(this.btnOk);
        this.btnCancel.Width = 10;
        this.btnCancel.X = 28;
        this.btnCancel.Y = 17;
        this.btnCancel.Data = "btnCancel";
        this.btnCancel.Text = "Cancel";
        this.btnCancel.TextAlignment = Terminal.Gui.TextAlignment.Centered;
        this.btnCancel.IsDefault = false;
        this.Add(this.btnCancel);
    }
}
