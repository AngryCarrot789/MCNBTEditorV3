using System.Collections.Generic;
using MCNBTEditor.Editor.FileTree.Zip;
using MCNBTEditor.Utils;

namespace MCNBTEditor.Editor.FileTree.NBT {
    public class ZipNBTDatVirtualFile : ZipEntryVirtualFile, INBTDatFile {
        public override bool CanHoldItems => true;

        public ZipNBTDatVirtualFile(NBTFileSystem fileSystem, string fullZipPath) : base(fullZipPath) {
            this.SetData(NBTFileSystem.IsRootDatFileKey, BoolBox.True);
            this.FileSystem = fileSystem;
        }

        public void SetContents(IEnumerable<BaseNBTTreeItem> items) {
            this.ClearItemsRecursiveCore();
            this.AddItemsCore(items);
        }
    }
}