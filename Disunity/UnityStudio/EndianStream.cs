namespace Unity_Studio
{
    using System;
    using System.IO;
    using System.Text;

    public class EndianStream : BinaryReader
    {
        private byte[] a16;
        private byte[] a32;
        private byte[] a64;
        public EndianType endian;

        public EndianStream(Stream stream, EndianType endian) : base(stream)
        {
            this.a16 = new byte[2];
            this.a32 = new byte[4];
            this.a64 = new byte[8];
        }

        public void AlignStream(int alignment)
        {
            long position = base.BaseStream.Position;
            if ((position % ((long) alignment)) > 0L)
            {
                Stream baseStream = base.BaseStream;
                baseStream.Position += alignment - (position % ((long) alignment));
            }
        }

        public void Dispose()
        {
            base.Dispose();
        }

        ~EndianStream()
        {
            this.Dispose();
        }

        public string ReadAlignedString(int length)
        {
            if ((length > 0) && (length < (base.BaseStream.Length - base.BaseStream.Position)))
            {
                byte[] buffer = new byte[length];
                base.Read(buffer, 0, length);
                string str = Encoding.UTF8.GetString(buffer);
                this.AlignStream(4);
                return str;
            }
            return "";
        }

        public string ReadASCII(int length)
        {
            return Encoding.ASCII.GetString(base.ReadBytes(length));
        }

        public override bool ReadBoolean()
        {
            return base.ReadBoolean();
        }

        public override byte ReadByte()
        {
            try
            {
                return base.ReadByte();
            }
            catch
            {
                return 0;
            }
        }

        public override char ReadChar()
        {
            return base.ReadChar();
        }

        public override double ReadDouble()
        {
            if (this.endian == EndianType.BigEndian)
            {
                this.a64 = base.ReadBytes(8);
                Array.Reverse(this.a64);
                return (double) BitConverter.ToUInt64(this.a64, 0);
            }
            return base.ReadDouble();
        }

        public override short ReadInt16()
        {
            if (this.endian == EndianType.BigEndian)
            {
                this.a16 = base.ReadBytes(2);
                Array.Reverse(this.a16);
                return BitConverter.ToInt16(this.a16, 0);
            }
            return base.ReadInt16();
        }

        public override int ReadInt32()
        {
            if (this.endian == EndianType.BigEndian)
            {
                this.a32 = base.ReadBytes(4);
                Array.Reverse(this.a32);
                return BitConverter.ToInt32(this.a32, 0);
            }
            return base.ReadInt32();
        }

        public override long ReadInt64()
        {
            if (this.endian == EndianType.BigEndian)
            {
                this.a64 = base.ReadBytes(8);
                Array.Reverse(this.a64);
                return BitConverter.ToInt64(this.a64, 0);
            }
            return base.ReadInt64();
        }

        public override float ReadSingle()
        {
            if (this.endian == EndianType.BigEndian)
            {
                this.a32 = base.ReadBytes(4);
                Array.Reverse(this.a32);
                return BitConverter.ToSingle(this.a32, 0);
            }
            return base.ReadSingle();
        }

        public override string ReadString()
        {
            return base.ReadString();
        }

        public string ReadStringToNull()
        {
            string str = "";
            for (int i = 0; i < base.BaseStream.Length; i++)
            {
                char ch = (char) base.ReadByte();
                if (ch == '\0')
                {
                    return str;
                }
                str = str + ch.ToString();
            }
            return str;
        }

        public override ushort ReadUInt16()
        {
            if (this.endian == EndianType.BigEndian)
            {
                this.a16 = base.ReadBytes(2);
                Array.Reverse(this.a16);
                return BitConverter.ToUInt16(this.a16, 0);
            }
            return base.ReadUInt16();
        }

        public override uint ReadUInt32()
        {
            if (this.endian == EndianType.BigEndian)
            {
                this.a32 = base.ReadBytes(4);
                Array.Reverse(this.a32);
                return BitConverter.ToUInt32(this.a32, 0);
            }
            return base.ReadUInt32();
        }

        public override ulong ReadUInt64()
        {
            if (this.endian == EndianType.BigEndian)
            {
                this.a64 = base.ReadBytes(8);
                Array.Reverse(this.a64);
                return BitConverter.ToUInt64(this.a64, 0);
            }
            return base.ReadUInt64();
        }

        public long Position
        {
            get
            {
                return base.BaseStream.Position;
            }
            set
            {
                base.BaseStream.Position = value;
            }
        }
    }
}

