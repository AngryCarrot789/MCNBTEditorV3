using System.Collections.Generic;
using MCNBTEditor.Editor.FileTree.Physical;
using MCNBTEditor.Utils;

namespace MCNBTEditor.Editor.FileTree.NBT {
    public class PhysicalNBTDatVirtualFile : PhysicalVirtualFile, INBTDatFile {
        public override bool CanHoldItems => true;

        public PhysicalNBTDatVirtualFile(NBTFileSystem fileSystem) {
            this.FileSystem = fileSystem;
            this.SetData(NBTFileSystem.IsRootDatFileKey, BoolBox.True);
        }

        public void SetContents(IEnumerable<BaseNBTTreeItem> items) {
            this.ClearItemsRecursiveCore();
            this.AddItemsCore(items);
        }
    }
}