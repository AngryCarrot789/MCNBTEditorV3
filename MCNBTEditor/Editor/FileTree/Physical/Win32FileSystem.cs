using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MCNBTEditor.Editor.FileTree.NBT;
using MCNBTEditor.Editor.FileTree.Zip;
using MCNBTEditor.Utils;

namespace MCNBTEditor.Editor.FileTree.Physical {
    public class Win32FileSystem : TreeFileSystem {
        public static Win32FileSystem Instance { get; } = new Win32FileSystem();

        public const string FilePathKey = "PhysicalFilePath";

        private Win32FileSystem() {
        }

        public override async Task<bool> QueryContent(TreeEntry target) {
            if (!target.CanHoldItems)
                throw new Exception("File is not a directory");
            if (!target.TryGetData(FilePathKey, out string path))
                throw new Exception("File does not have a file path associated with it");

            DirectoryInfo info = new DirectoryInfo(path);
            IEnumerable<FileSystemInfo> enumerable;
            try {
                enumerable = info.EnumerateFileSystemInfos();
            }
            catch (DirectoryNotFoundException) {
                await IoC.MessageDialogs.ShowMessageAsync("Directory not found", $"'{path}' no longer exists");
                return false;
            }
            catch (UnauthorizedAccessException e) {
                await IoC.MessageDialogs.ShowMessageExAsync("Unauthorized Access", $"Cannot access the folder '{path}'", e.GetToString());
                return false;
            }
            catch (Exception e) {
                await IoC.MessageDialogs.ShowMessageExAsync("Error", $"An error occurred while getting files at '{path}'", e.GetToString());
                return false;
            }

            foreach (FileSystemInfo item in enumerable) {
                target.AddItemCore(this.ForFileSystemInfo(item));
            }

            return true;
        }

        public TreeEntry ForFileSystemInfo(FileSystemInfo item) {
            if (item is DirectoryInfo) {
                return ForDirectory(item.FullName);
            }
            else {
                return ForFile(item.FullName);
            }
        }

        /// <summary>
        /// Returns a new physical virtual folder, whose file system is set to the current instance, and file path is set to the given path
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <returns>A virtual folder</returns>
        public PhysicalVirtualFolder ForDirectory(string path) {
            PhysicalVirtualFolder entry = new PhysicalVirtualFolder { FileSystem = this };
            entry.SetData(FilePathKey, path);
            return entry;
        }

        /// <summary>
        /// Returns a new physical file, whose file system is set to the current instance, and file path is set to the given path.
        /// <para>
        /// The type of the returned file is determined by the file path (e.g. '.zip' returns <see cref="PhysicalZipVirtualFile"/>)
        /// </para>
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>A virtual file</returns>
        public PhysicalVirtualFile ForFile(string path) {
            PhysicalVirtualFile entry;
            string extension = Path.GetExtension(path);
            if (extension == ".jar" || extension == ".zip") {
                entry = new PhysicalZipVirtualFile(new ZipFileSystem(() => new BufferedStream(File.OpenRead(path))));
            }
            else if (extension == ".dat") {
                entry = new PhysicalNBTDatVirtualFile(new NBTFileSystem(() => new BufferedStream(File.OpenRead(path))));
            }
            else {
                entry = new PhysicalVirtualFile() { FileSystem = this };
            }

            entry.SetData(FilePathKey, path);
            return entry;
        }

        public TreeEntry ForFilePath(string path) {
            if (Directory.Exists(path)) {
                return ForDirectory(path);
            }
            else {
                return ForFile(path);
            }
        }
    }
}