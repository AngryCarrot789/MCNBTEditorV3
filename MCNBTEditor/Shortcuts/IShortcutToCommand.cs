using System.Windows.Input;

namespace MCNBTEditor.Shortcuts {
    public interface IShortcutToCommand {
        ICommand GetCommandForShortcut(string shortcutId);
    }
}