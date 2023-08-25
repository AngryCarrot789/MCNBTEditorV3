using System.Threading.Tasks;
using MCNBTEditor.Shortcuts.Managing;

namespace MCNBTEditor.Shortcuts {
    public interface IShortcutHandler {
        Task<bool> OnShortcutActivated(ShortcutProcessor processor, GroupedShortcut shortcut);
    }
}