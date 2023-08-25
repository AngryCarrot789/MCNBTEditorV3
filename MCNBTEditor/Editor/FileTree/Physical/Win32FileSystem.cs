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
            catch (DirectoryNotFoundException e) {
                await IoC.MessageDialogs.ShowMessageExAsync("Unauthorized Access", $"Cannot access the folder '{path}'", e.GetToString());
                return false;
            }
            catch (UnauthorizedAccessException e) {
                await IoC.MessageDialogs.ShowMessageExAsync("Unauthorized Access", $"Cannot access the folder '{path}'", e.GetToString());
                return false;
            }

            foreach (FileSystemInfo item in enumerable) {
                target.AddItemCore(this.ForEntry(item));
            }

            return true;
        }

        public TreeEntry ForEntry(FileSystemInfo item) {
            TreeEntry vfile;
            if (item is DirectoryInfo directory) {
                vfile = new PhysicalVirtualFolder {
                    FilePath = directory.FullName,
                    FileSystem = this
                };
            }
            else {
                FileInfo file = (FileInfo) item;
                string extension = file.Extension;
                if (extension == ".jar" || extension == ".zip") {
                    vfile = new PhysicalZipVirtualFile(new ZipFileSystem(() => new BufferedStream(File.OpenRead(file.FullName))));
                }
                else if (extension == ".dat") {
                    vfile = new PhysicalNBTDatVirtualFile(new NBTFileSystem(() => new BufferedStream(File.OpenRead(file.FullName))));
                }
                else {
                    vfile = new PhysicalVirtualFile() {
                        FileSystem = this
                    };
                }

                vfile.SetData(FilePathKey, file.FullName);
            }

            return vfile;
        }
    }
}