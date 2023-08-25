using System;
using System.Collections.Generic;
using System.Linq;
using MCNBTEditor.Editor.FileTree;
using MCNBTEditor.NBT;
using MCNBTEditor.Utils;

namespace MCNBTEditor.Editor {
    public abstract class BaseNBTTreeItem : TreeEntry {
        private string name;

        /// <summary>
        /// The name of this tag, used only when this item is stored in a <see cref="TagCompoundTreeItem"/>
        /// </summary>
        public string Name {
            get => this.name;
            set => this.RaisePropertyChanged(ref this.name, value);
        }

        protected BaseNBTTreeItem(TreeFileSystem fileSystem) {
            this.FileSystem = fileSystem;
        }

        /// <summary>
        /// Convert this view model to an NBT object
        /// </summary>
        /// <returns></returns>
        public abstract NBTBase ToTag();

        // public static BaseNBTTreeItem FromTag(NBTBase tag) {
        //     BaseNBTTreeItem item = ForTag(tag);
        //     item.LoadFromTag(tag);
        //     return item;
        // }
        // protected abstract void LoadFromTag(NBTBase tag);

        public virtual BaseNBTTreeItem Clone(TreeFileSystem fs) {
            return CreateAndLoadFrom(fs, this.name, this.ToTag());
        }

        // public static BaseNBTTreeItem ForTag(VirtualFileSystem fs, NBTBase tag) {
        //     return ForTagId(fs, (int) tag.TagType) ?? throw new ArgumentException("Unknown NBT type: " + tag.TagType);
        // }
        // public static BaseNBTTreeItem ForTagId(VirtualFileSystem fs, int id) {
        //     switch (id) {
        //         case 0: return new TagEndTreeItem(fs);
        //         case 1: return new TagByteTreeItem(fs);
        //         case 2: return new TagShortTreeItem(fs);
        //         case 3: return new TagIntTreeItem(fs);
        //         case 4: return new TagLongTreeItem(fs);
        //         case 5: return new TagFloatTreeItem(fs);
        //         case 6: return new TagDoubleTreeItem(fs);
        //         case 7: return new TagByteArrayTreeItem(fs);
        //         case 8: return new TagStringTreeItem(fs);
        //         case 9: return new TagListTreeItem(fs);
        //         case 10: return new TagCompoundTreeItem(fs);
        //         case 11: return new TagIntArrayTreeItem(fs);
        //         case 12: return new TagLongArrayTreeItem(fs);
        //         default: return null;
        //     }
        // }

        public static TagCompoundTreeItem CreateAndLoadFrom(TreeFileSystem fs, string name, NBTTagCompound nbt) {
            TagCompoundTreeItem tag = new TagCompoundTreeItem(fs) {
                Name = name
            };
            tag.AddItem(nbt);
            return tag;
        }

        public static TagListTreeItem CreateAndLoadFrom(TreeFileSystem fs, string name, NBTTagList nbt) {
            TagListTreeItem tag = new TagListTreeItem(fs) {
                Name = name
            };
            tag.AddItem(nbt);
            return tag;
        }

        public static BaseNBTTreeItem CreateAndLoadFrom(TreeFileSystem fs, string name, NBTBase nbt) {
            switch (nbt) {
                case NBTTagCompound  comp: return CreateAndLoadFrom(fs, name, comp);
                case NBTTagList      list: return CreateAndLoadFrom(fs, name, list);
                case NBTTagLongArray la:   return new TagLongArrayTreeItem(fs) {Value = Arrays.CloneArrayUnsafe(la.data)};
                case NBTTagIntArray  ia:   return new TagIntArrayTreeItem(fs) {Value = Arrays.CloneArrayUnsafe(ia.data)};
                case NBTTagByteArray ba:   return new TagByteArrayTreeItem(fs) {Value = Arrays.CloneArrayUnsafe(ba.data)};
                case NBTTagByte      b:    return new TagByteTreeItem(fs) {Value = b.data};
                case NBTTagShort     s:    return new TagShortTreeItem(fs) {Value = s.data};
                case NBTTagInt       i:    return new TagIntTreeItem(fs) {Value = i.data};
                case NBTTagLong      l:    return new TagLongTreeItem(fs) {Value = l.data};
                case NBTTagFloat     f:    return new TagFloatTreeItem(fs) {Value = f.data};
                case NBTTagDouble    d:    return new TagDoubleTreeItem(fs) {Value = d.data};
                case NBTTagString    str:  return new TagStringTreeItem(fs) {Value = str.data};
                case NBTTagEnd       _:    return new TagEndTreeItem(fs);
                case null:                 return null;
                default:                   throw new ArgumentException($"Unexpected NBT instance: {nbt}", nameof(nbt));
            }
        }

        public static BaseNBTTreeItem CreateFrom(TreeFileSystem fs, string name, NBTType type) {
            BaseNBTTreeItem tag;
            switch (type) {
                case NBTType.Compound:  tag = new TagCompoundTreeItem(fs); break;
                case NBTType.List:      tag = new TagListTreeItem(fs); break;
                case NBTType.LongArray: tag = new TagLongArrayTreeItem(fs); break;
                case NBTType.IntArray:  tag = new TagIntArrayTreeItem(fs); break;
                case NBTType.ByteArray: tag = new TagByteArrayTreeItem(fs); break;
                case NBTType.Byte:      tag = new TagByteTreeItem(fs); break;
                case NBTType.Short:     tag = new TagShortTreeItem(fs); break;
                case NBTType.Int:       tag = new TagIntTreeItem(fs); break;
                case NBTType.Long:      tag = new TagLongTreeItem(fs); break;
                case NBTType.Float:     tag = new TagFloatTreeItem(fs); break;
                case NBTType.Double:    tag = new TagDoubleTreeItem(fs); break;
                case NBTType.String:    tag = new TagStringTreeItem(fs); break;
                case NBTType.End:       tag = new TagEndTreeItem(fs); break;
                default:                throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            tag.Name = name;
            return tag;
        }
    }

    public abstract class BaseNbtPrimitiveTreeItem : BaseNBTTreeItem {
        protected BaseNbtPrimitiveTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }
    }

    public abstract class BaseNbtArrayTreeItem : BaseNBTTreeItem {
        protected BaseNbtArrayTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }
    }

    public abstract class BaseNBTCollectionTreeItem : BaseNBTTreeItem {
        public override bool CanHoldItems => true;

        protected BaseNBTCollectionTreeItem(TreeFileSystem fileSystem) : base(fileSystem) {
        }

        public void AddChild(BaseNBTTreeItem nbt) {
            this.AddItemCore(nbt);
        }

        public void AddItems(IEnumerable<BaseNBTTreeItem> nbt) {
            foreach (BaseNBTTreeItem item in nbt) {
                this.AddItemCore(item);
            }
        }

        public void AddItem(NBTTagCompound nbt, bool attemptAutoSort = true) {
            if (nbt.map.Count < 1) {
                return;
            }

            if (attemptAutoSort) {
                List<KeyValuePair<string, NBTBase>> list = new List<KeyValuePair<string, NBTBase>>(nbt.map);
                list.Sort((a, b) => {
                    int cmp = a.Value.TagType.Compare4(b.Value.TagType);
                    return cmp != 0 ? cmp : string.Compare(a.Key ?? "", b.Key ?? "", StringComparison.CurrentCulture);
                });

                this.AddItems(list.Select(x => CreateAndLoadFrom(this.FileSystem, x.Key, x.Value)));
            }
            else {
                this.AddItems(nbt.map.Select(x => CreateAndLoadFrom(this.FileSystem, x.Key, x.Value)));
            }
        }

        public void AddItem(NBTTagList nbt) {
            this.AddItems(nbt.tags.Select(x => CreateAndLoadFrom(this.FileSystem, null, x)));
        }

        protected static NBTTagCompound ToCompound(BaseNBTCollectionTreeItem collection) {
            NBTTagCompound tag = new NBTTagCompound();
            foreach (TreeEntry item in collection.Items) {
                if (!string.IsNullOrEmpty(((BaseNBTTreeItem) item).Name))
                    tag.map[((BaseNBTTreeItem) item).Name] = ((BaseNBTTreeItem) item).ToTag();
            }

            return tag;
        }

        protected static NBTTagList ToList(BaseNBTCollectionTreeItem collection) {
            NBTTagList tag = new NBTTagList();
            foreach (TreeEntry item in collection.Items)
                tag.tags.Add(((BaseNBTTreeItem) item).ToTag());
            return tag;
        }
    }

    public class TagEndTreeItem : BaseNbtPrimitiveTreeItem {
        public TagEndTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }

        public NBTTagEnd ToTagCore() => new NBTTagEnd();
        public override NBTBase ToTag() => this.ToTagCore();
    }

    public class TagByteTreeItem : BaseNbtPrimitiveTreeItem {
        private byte value;
        public byte Value { get => this.value; set => this.RaisePropertyChanged(ref this.value, value); }

        public TagByteTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }

        public NBTTagByte ToTagCore() => new NBTTagByte(this.value);
        public override NBTBase ToTag() => this.ToTagCore();
    }

    public class TagShortTreeItem : BaseNbtPrimitiveTreeItem {
        private short value;
        public short Value {
            get => this.value;
            set => this.RaisePropertyChanged(ref this.value, value);
        }

        public TagShortTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }

        public NBTTagShort ToTagCore() => new NBTTagShort(this.value);
        public override NBTBase ToTag() => this.ToTagCore();
    }

    public class TagIntTreeItem : BaseNbtPrimitiveTreeItem {
        private int value;
        public int Value {
            get => this.value;
            set => this.RaisePropertyChanged(ref this.value, value);
        }

        public TagIntTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }

        public NBTTagInt ToTagCore() => new NBTTagInt(this.value);
        public override NBTBase ToTag() => this.ToTagCore();
    }

    public class TagLongTreeItem : BaseNbtPrimitiveTreeItem {
        private long value;
        public long Value {
            get => this.value;
            set => this.RaisePropertyChanged(ref this.value, value);
        }

        public TagLongTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }

        public NBTTagLong ToTagCore() => new NBTTagLong(this.value);
        public override NBTBase ToTag() => this.ToTagCore();
    }

    public class TagFloatTreeItem : BaseNbtPrimitiveTreeItem {
        private float value;
        public float Value {
            get => this.value;
            set => this.RaisePropertyChanged(ref this.value, value);
        }

        public TagFloatTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }

        public NBTTagFloat ToTagCore() => new NBTTagFloat(this.value);
        public override NBTBase ToTag() => this.ToTagCore();
    }

    public class TagDoubleTreeItem : BaseNbtPrimitiveTreeItem {
        private double value;
        public double Value {
            get => this.value;
            set => this.RaisePropertyChanged(ref this.value, value);
        }

        public TagDoubleTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }

        public NBTTagDouble ToTagCore() => new NBTTagDouble(this.value);
        public override NBTBase ToTag() => this.ToTagCore();
    }

    public class TagStringTreeItem : BaseNbtPrimitiveTreeItem {
        private string value;
        public string Value {
            get => this.value;
            set => this.RaisePropertyChanged(ref this.value, value);
        }

        public TagStringTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }

        public NBTTagString ToTagCore() => new NBTTagString(this.value);
        public override NBTBase ToTag() => this.ToTagCore();
    }

    public class TagByteArrayTreeItem : BaseNbtArrayTreeItem {
        private byte[] value;
        public byte[] Value {
            get => this.value;
            set => this.RaisePropertyChanged(ref this.value, value);
        }

        public TagByteArrayTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }

        public NBTTagByteArray ToTagCore() => new NBTTagByteArray(Arrays.CloneArrayUnsafe(this.value));
        public override NBTBase ToTag() => this.ToTagCore();
    }

    public class TagIntArrayTreeItem : BaseNbtArrayTreeItem {
        private int[] value;
        public int[] Value {
            get => this.value;
            set => this.RaisePropertyChanged(ref this.value, value);
        }

        public TagIntArrayTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }

        public NBTTagIntArray ToTagCore() => new NBTTagIntArray(Arrays.CloneArrayUnsafe(this.value));
        public override NBTBase ToTag() => this.ToTagCore();
    }

    public class TagLongArrayTreeItem : BaseNbtArrayTreeItem {
        private long[] value;
        public long[] Value {
            get => this.value;
            set => this.RaisePropertyChanged(ref this.value, value);
        }

        public TagLongArrayTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }

        public NBTTagLongArray ToTagCore() => new NBTTagLongArray(Arrays.CloneArrayUnsafe(this.value));
        public override NBTBase ToTag() => this.ToTagCore();
    }

    public class TagListTreeItem : BaseNBTCollectionTreeItem {
        public TagListTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }

        public NBTTagList ToTagCore() => ToList(this);
        public override NBTBase ToTag() => this.ToTagCore();
    }

    public class TagCompoundTreeItem : BaseNBTCollectionTreeItem {
        public TagCompoundTreeItem(TreeFileSystem fileSystem) : base(fileSystem) { }

        public NBTTagCompound ToTagCore() => ToCompound(this);
        public override NBTBase ToTag() => this.ToTagCore();
    }
}