namespace MCNBTEditor.Editor.FileTree.Physical {
    /// <summary>
    /// A class for a physical virtual folder
    /// </summary>
    public class PhysicalVirtualFolder : BasePhysicalVirtualFile {
        public sealed override bool CanHoldItems => true;

        public PhysicalVirtualFolder() {
        }
    }
}