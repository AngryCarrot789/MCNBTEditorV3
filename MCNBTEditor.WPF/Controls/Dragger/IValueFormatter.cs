namespace MCNBTEditor.WPF.Controls.Dragger {
    public interface IValueFormatter {
        string ToString(double value, int? roundedPlaces);
    }
}