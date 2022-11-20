//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TerminalGuiDesigner.UI.Windows;

using Terminal.Gui;
using TerminalGuiDesigner;
using TerminalGuiDesigner.ToCode;

/// <summary>
/// Editor for the <see cref="Dim"/> class.
/// </summary>
public partial class DimEditor : Dialog
{
    private Design design;
    private Property property { get; }

    /// <summary>
    /// The final <see cref="Dim"/> value user has configured based on
    /// radio buttons and text box values.
    /// </summary>
    public Dim Result { get; private set; }

    /// <summary>
    /// True if dialog was canceled.
    /// </summary>
    public bool Cancelled { get; private set; }

    /// <summary>
    /// Creates a new instance of the <see cref="DimEditor"/> class.
    /// </summary>
    /// <param name="design"></param>
    /// <param name="property"></param>
    public DimEditor(Design design, Property property) {
        InitializeComponent();
        
        this.design = design;
        this.property = property;


        Title = "Dim Designer";
        Border.BorderStyle = BorderStyle.Double;

        btnOk.Clicked += BtnOk_Clicked;
        btnCancel.Clicked += BtnCancel_Clicked;
        Cancelled = true;
        Modal = true;
        rgDimType.KeyPress += RgDimType_KeyPress;
        

        var val = (Dim)property.GetValue();
        if(val.GetDimType(out var type,out var value, out var offset))
        {
            switch(type)
            {
                case DimType.Absolute:
                    rgDimType.SelectedItem = 0;
                    break;
                case DimType.Percent:
                    rgDimType.SelectedItem = 1;
                    break;
                case DimType.Fill:
                    rgDimType.SelectedItem = 2;
                    break;
            }

            tbValue.Text = value.ToString("G5");
            tbOffset.Text = offset.ToString();
        }

        SetupForCurrentDimType();

        rgDimType.SelectedItemChanged += DdType_SelectedItemChanged;
    }

    private void RgDimType_KeyPress(KeyEventEventArgs obj)
    {
        var c = (char)obj.KeyEvent.KeyValue;

        // if user types in some text change the focus to the text box to enable entering digits
        if ((obj.KeyEvent.Key == Key.Backspace || char.IsDigit(c)) && tbValue.Visible)
        {
            tbValue?.FocusFirst();
        }
    }

    private void DdType_SelectedItemChanged(SelectedItemChangedArgs obj)
    {
        SetupForCurrentDimType();
    }
    private DimType GetDimType()
    {
        return Enum.Parse<DimType>(rgDimType.RadioLabels[rgDimType.SelectedItem].ToString());
    }
    private void SetupForCurrentDimType()
    {
        switch(GetDimType())
        {
            case DimType.Absolute:
                lblValue.Text = "Value";
                lblOffset.Visible = false;
                tbOffset.Visible = false;
                SetNeedsDisplay();
                break;
            case DimType.Fill:
                lblOffset.Visible = false;
                tbOffset.Visible = false;
                lblValue.Text = "Margin";
                SetNeedsDisplay();
                break;
            case DimType.Percent:
                lblValue.Text = "Factor";
                lblOffset.Visible = true;
                tbOffset.Visible = true;
                SetNeedsDisplay();
                break;

            default: throw new ArgumentOutOfRangeException();
        }
    }

    private void BtnCancel_Clicked()
    {
        Cancelled = true;
        Application.RequestStop();
    }

    private void BtnOk_Clicked()
    {
        Cancelled = false;
        Result = BuildResult();
        Application.RequestStop();
    }

    private Dim BuildResult()
    {
        // pick what type of Pos they want
        var type = GetDimType();
        var val = string.IsNullOrWhiteSpace(tbValue.Text.ToString()) ? 0 : float.Parse(tbValue.Text.ToString());
        var offset = string.IsNullOrWhiteSpace(tbOffset.Text.ToString()) ? 0 : int.Parse(tbOffset.Text.ToString());

        switch (type)
        {
            case DimType.Absolute:
                return Dim.Sized((int)val);
            case DimType.Percent:
                return offset == 0? Dim.Percent(val) : Dim.Percent(val) + offset;
            case DimType.Fill:
                return Dim.Fill((int)val);

            default: throw new ArgumentOutOfRangeException(nameof(type));

        }
    }
}
