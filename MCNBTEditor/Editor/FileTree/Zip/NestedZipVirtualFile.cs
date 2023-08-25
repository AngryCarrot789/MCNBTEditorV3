using System;
using System.IO.Compression;

namespace MCNBTEditor.Editor.FileTree.Zip {
    public class NestedZipVirtualFile : ZipEntryVirtualFile, IZipRoot {
        public override bool CanHoldItems => true;

        public ZipArchive Archive { get; set; }

        public NestedZipVirtualFile(TreeFileSystem fileSystem, string fullZipPath) : base(fullZipPath) {
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