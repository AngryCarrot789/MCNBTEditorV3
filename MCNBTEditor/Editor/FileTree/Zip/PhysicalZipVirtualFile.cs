using System;
using System.IO.Compression;
using MCNBTEditor.Editor.FileTree.Physical;

namespace MCNBTEditor.Editor.FileTree.Zip {
    public class PhysicalZipVirtualFile : PhysicalVirtualFile, IZipRoot {
        public override bool CanHoldItems => true;

        public ZipArchive Archive { get; set; }

        public PhysicalZipVirtualFile(TreeFileSystem fileSystem) {
            this.FileSystem = fileSystem;
        }

        protected override void OnRemovedFromParent(TreeEntry parent) {
            base.OnRemovedFromParent(parent);
            if (this.FileSystem is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
}