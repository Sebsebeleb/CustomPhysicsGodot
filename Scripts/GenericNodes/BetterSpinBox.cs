using System.Linq;
using Godot;

namespace CustomPhysics.GenericNodes;

public partial class BetterSpinBox : SpinBox
{
    private LineEdit lineEdit;
    public override void _Ready()
    {
        lineEdit = GetLineEdit();
        lineEdit.TextSubmitted += OnTextSubmitted;
        lineEdit.TextChanged += LineEditOnTextChanged;
    }

    private static string GetNumbersAndComma(string input)
    {
        return new string(input.Where(c => char.IsDigit(c) || c== ',' | c=='.').ToArray());
    }
    private void LineEditOnTextChanged(string newtext)
    {
        var column = lineEdit.CaretColumn;
        lineEdit.Text = GetNumbersAndComma(newtext);
        lineEdit.CaretColumn = column;
    }

    private void OnTextSubmitted(string newtext)
    {
        lineEdit.ReleaseFocus();
    }
}