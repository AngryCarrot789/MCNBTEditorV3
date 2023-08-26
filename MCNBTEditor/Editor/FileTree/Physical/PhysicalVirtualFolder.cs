using MCNBTEditor.Drop;
using System.Threading.Tasks;

namespace MCNBTEditor.Editor.FileTree.Physical {
    /// <summary>
    /// A class for a physical virtual folder
    /// </summary>
    public class PhysicalVirtualFolder : BasePhysicalVirtualFile, IDropHandler {
        public sealed override bool CanHoldItems => true;

        public PhysicalVirtualFolder() {
        }

        public DropType OnDropEnter(string[] paths) {
            return DropType.Link;
        }

        public void OnDropLeave(bool isCancelled) {
        }

        public async Task OnFilesDropped(string[] paths) {
            if (this.FileSystem != Win32FileSystem.Instance) {
                return;
            }

            foreach (string path in paths) {
                this.AddItemCore(Win32FileSystem.Instance.ForFilePath(path));
            }
        }
    }
}