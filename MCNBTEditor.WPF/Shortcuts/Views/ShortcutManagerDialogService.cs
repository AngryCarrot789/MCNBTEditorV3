using MCNBTEditor.Shortcuts.Dialogs;
using MCNBTEditor.Shortcuts.Managing;
using MCNBTEditor.Shortcuts.ViewModels;

namespace MCNBTEditor.WPF.Shortcuts.Views {
    [ServiceImplementation(typeof(IShortcutManagerDialogService))]
    public class ShortcutManagerDialogService : IShortcutManagerDialogService {
        private ShortcutEditorWindow window;

        public bool IsOpen => this.window != null;

        public void ShowEditorDialog() {
            if (this.window != null) {
                return;
            }

            this.window = new ShortcutEditorWindow();
            ShortcutManagerViewModel manager = new ShortcutManagerViewModel(ShortcutManager.Instance);
            this.window.DataContext = manager;
            this.window.Closed += (sender, args) => {
                this.window = null;
            };

            this.window.Show();
        }
    }
}