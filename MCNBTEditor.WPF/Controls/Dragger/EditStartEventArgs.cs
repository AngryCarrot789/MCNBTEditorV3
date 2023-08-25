using System.Windows;

namespace MCNBTEditor.WPF.Controls.Dragger {
    public class EditStartEventArgs : RoutedEventArgs {
        public EditStartEventArgs() : base(NumberDragger.EditStartedEvent) {
        }
    }
}