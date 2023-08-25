using System;
using System.IO;
using System.IO.Compression;
using MCNBTEditor.Streams;

namespace MCNBTEditor.NBT {
    public static class CompressedStreamTools {
        public static NBTBase Read(string filePath, out string tagName, bool compressed = true, bool useBigEndianness = true) {
            using (FileStream stream = File.OpenRead(filePath)) {
                return Read(stream, out tagName, compressed, useBigEndianness);
            }
        }

        public static NBTBase Read(Stream stream, out string tagName, bool compressed = true, bool useBigEndianness = true) {
            using (BufferedStream buffered = new BufferedStream(compressed ? new GZipStream(stream, CompressionMode.Decompress, true) : stream)) {
                if (NBTBase.ReadTag(CreateInput(buffered, useBigEndianness), 0, out tagName, out NBTBase nbt)) {
                    return nbt;
                }
                else {
                    throw new Exception("Failed to read NBTTagCompound from stream");
                }
            }
        }

        public static void Write(NBTBase nbt, string filePath, bool compressed = true, bool useBigEndianness = true) {
            using (FileStream stream = File.OpenWrite(filePath)) {
                Write(nbt, stream, compressed, useBigEndianness);
            }
        }

        public static void Write(NBTBase nbt, Stream stream, bool compressed = true, bool useBigEndianness = true) {
            using (Stream output = compressed ? new GZipStream(stream, CompressionMode.Compress, true) : stream) {
                NBTBase.WriteTag(CreateOutput(output, useBigEndianness), null, nbt);
            }
        }

        public static IDataInput CreateInput(Stream stream, bool useBigEndianness = true) {
            IDataInput output;
            if (useBigEndianness) {
                output = new DataInputStream(stream);
            }
            else {
                output = new DataInputStreamLE(stream);
            }

            return output;
        }

        public static IDataOutput CreateOutput(Stream stream, bool useBigEndianness = true) {
            IDataOutput output;
            if (useBigEndianness) {
                output = new DataOutputStream(stream);
            }
            else {
                output = new DataOutputStreamLE(stream);
            }
            return output;
        }
    }
}