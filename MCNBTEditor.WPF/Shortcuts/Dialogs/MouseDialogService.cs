using MCNBTEditor.Shortcuts.Dialogs;
using MCNBTEditor.Shortcuts.Inputs;

namespace MCNBTEditor.WPF.Shortcuts.Dialogs {
    [ServiceImplementation(typeof(IMouseDialogService))]
    public class MouseDialogService : IMouseDialogService {
        public MouseStroke? ShowGetMouseStrokeDialog() {
            MouseStrokeInputWindow window = new MouseStrokeInputWindow();
            if (window.ShowDialog() != true || window.Stroke.Equals(default)) {
                return null;
            }
            else {
                return window.Stroke;
            }
        }
    }
}