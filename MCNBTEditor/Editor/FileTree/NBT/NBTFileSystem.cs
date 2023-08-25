using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MCNBTEditor.NBT;
using MCNBTEditor.Utils;

namespace MCNBTEditor.Editor.FileTree.NBT {
    public class NBTFileSystem : TreeFileSystem {
        public const string IsRootDatFileKey = "IsNBTDatRoot";

        public Func<Stream> StreamProvider { get; }

        public NBTBase RootTag { get; private set; }

        public NBTFileSystem(Func<Stream> streamProvider) {
            this.StreamProvider = streamProvider;
        }

        public override async Task<bool> QueryContent(TreeEntry target) {
            if (!(target is INBTDatFile datFile))
                return true;

            if (this.RootTag == null) {
                Stream stream;
                try {
                    stream = this.StreamProvider();
                }
                catch (Exception e) {
                    await IoC.MessageDialogs.ShowMessageExAsync("Zip Failure", "Failed to open DAT file stream", e.GetToString());
                    return false;
                }

                try {
                    this.RootTag = CompressedStreamTools.Read(stream, out _);
                }
                catch (Exception e) {
                    await IoC.MessageDialogs.ShowMessageExAsync("DAT Failure", "Failed to read DAT file contents", e.GetToString());
                    return false;
                }
                finally {
                    stream.Dispose();
                }

                if (!(this.RootTag is NBTTagCompound compound)) {
                    await IoC.MessageDialogs.ShowMessageAsync("Invalid .dat file", "DAT file's root content was not a tag compound");
                    return false;
                }

                this.LoadRootContents(datFile, compound);
            }

            return true;
        }

        public void LoadRootContents(INBTDatFile datFile, NBTTagCompound tag) {
            if (tag.map.Count < 1) {
                return;
            }

            List<KeyValuePair<string, NBTBase>> list = new List<KeyValuePair<string, NBTBase>>(tag.map);
            list.Sort((a, b) => {
                int cmp = a.Value.TagType.Compare4(b.Value.TagType);
                return cmp != 0 ? cmp : string.Compare(a.Key ?? "", b.Key ?? "", StringComparison.CurrentCulture);
            });

            datFile.SetContents(list.Select(x => BaseNBTTreeItem.CreateAndLoadFrom(this, x.Key, x.Value)));
        }
    }
}