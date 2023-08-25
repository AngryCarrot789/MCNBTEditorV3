namespace MCNBTEditor.Editor.FileTree.Zip {
    /// <summary>
    /// Base class for files in an archive (zip) file
    /// </summary>
    public abstract class BaseZipVirtualFile : TreeEntry {
        public string FullZipPath => this.GetData<string>(ZipFileSystem.ZipFullPathKey);

        public string ZipFileName => this.GetData<string>(ZipFileSystem.ZipFileNameKey);

        protected BaseZipVirtualFile(string fullZipPath) {
            this.SetData(ZipFileSystem.ZipFullPathKey, fullZipPath);
            this.SetData(ZipFileSystem.ZipFileNameKey, ZipFileSystem.GetFileName(fullZipPath, out _));
        }
    }
}