using System;
using System.Collections.Generic;
using MCNBTEditor.Streams;

namespace MCNBTEditor.NBT {
    public class NBTTagCompound : NBTBase {
        public readonly Dictionary<string, NBTBase> map;

        public override NBTType TagType => NBTType.Compound;

        public NBTTagCompound() {
            this.map = new Dictionary<string, NBTBase>();
        }

        public NBTTagCompound(IDictionary<string, NBTBase> map) {
            this.map = new Dictionary<string, NBTBase>(map);
        }

        public override void Read(IDataInput input, int deep) {
            if (deep <= 512) {
                this.map.Clear();
                while (ReadTag(input, deep + 1, out string name, out NBTBase nbt)) {
                    this.map[name ?? ""] = nbt;
                }
            }
            else {
                throw new Exception("Tried to read NBT tag with too high complexity, depth > 512");
            }
        }

        public override void Write(IDataOutput output) {
            foreach (KeyValuePair<string, NBTBase> pair in this.map) {
                WriteTag(output, pair.Key, pair.Value);
            }
            output.WriteByte(0);
        }

        public void Set(string key, NBTBase nbt) {
            this.map[key ?? ""] = nbt;
        }

        public NBTBase Get(string key) {
            return this.map.TryGetValue(key ?? "", out NBTBase tag) ? tag : null;
        }

        public bool TryGet(string key, out NBTBase nbt) {
            return this.map.TryGetValue(key ?? "", out nbt);
        }

        public void SetByte(string name, byte value)        => this.Set(name, new NBTTagByte(value));
        public void SetShort(string name, short value)      => this.Set(name, new NBTTagShort(value));
        public void SetInt(string name, int value)          => this.Set(name, new NBTTagInt(value));
        public void SetLong(string name, long value)        => this.Set(name, new NBTTagLong(value));
        public void SetFloat(string name, float value)      => this.Set(name, new NBTTagFloat(value));
        public void SetDouble(string name, double value)    => this.Set(name, new NBTTagDouble(value));
        public void SetString(string name, string value)    => this.Set(name, new NBTTagString(value));
        public void SetByteArray(string name, byte[] value) => this.Set(name, new NBTTagByteArray(value));
        public void SetIntArray(string name, int[] value)   => this.Set(name, new NBTTagIntArray(value));
        public void SetLongArray(string name, long[] value) => this.Set(name, new NBTTagLongArray(value));
        public void SetList(string name, IEnumerable<NBTBase> tags) {
            this.Set(name, new NBTTagList(tags));
        }

        public void SetCompound(string name, IDictionary<string, NBTBase> map) {
            this.Set(name, new NBTTagCompound(map));
        }

        public byte GetByte(string name, byte def = default)       => (this.Get(name) as NBTTagByte)?.data ?? def;
        public short GetShort(string name, short def = default)    => (this.Get(name) as NBTTagShort)?.data ?? def;
        public int GetInt(string name, int def = default)          => (this.Get(name) as NBTTagInt)?.data ?? def;
        public long GetLong(string name, long def = default)       => (this.Get(name) as NBTTagLong)?.data ?? def;
        public float GetFloat(string name, float def = default)    => (this.Get(name) as NBTTagFloat)?.data ?? def;
        public double GetDouble(string name, double def = default) => (this.Get(name) as NBTTagDouble)?.data ?? def;
        public string GetString(string name, string def = null)    => (this.Get(name) as NBTTagString)?.data ?? def;
        public byte[] GetByteArray(string name)                    => (this.Get(name) as NBTTagByteArray)?.data;
        public int[] GetIntArray(string name)                      => (this.Get(name) as NBTTagIntArray)?.data;
        public long[] GetLongArray(string name)                    => (this.Get(name) as NBTTagLongArray)?.data;
        public IEnumerable<NBTBase> GetList(string name)           => (this.Get(name) as NBTTagList)?.tags;

        public override NBTBase CloneTag() {
            NBTTagCompound nbt = new NBTTagCompound();
            foreach (KeyValuePair<string, NBTBase> pair in this.map) {
                nbt.map[pair.Key] = Clone(pair.Value);
            }
            return nbt;
        }
    }
}