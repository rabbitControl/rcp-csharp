using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using Kaitai;

namespace RCP.Model
{
    public class ArrayDefinition<T> : DefaultDefinition<List<T>>
    {
        public DefaultDefinition<T> Subtype { get; private set; }
        public uint Length { get; private set; }
        //        public T Default { get; set; }

        public ArrayDefinition(dynamic subtype, uint length)
        : base(RcpTypes.Datatype.FixedArray)
        {
            Subtype = subtype;
            Length = length;
        }

        public static ITypeDefinition Parse(KaitaiStream input)
        {
            // parse mandatory subtype
            var subtypeDefinition = DefaultDefinition<T>.Parse(input);

            // read mandatory length
            var length = input.ReadU4be();

            // create ArrayDefinition
            var arrayDefinition = Create(subtypeDefinition, length);

            if (arrayDefinition != null)
            {
                arrayDefinition.ParseOptions(input);
            }

            return arrayDefinition;
        }

        private static ITypeDefinition Create(ITypeDefinition subtypeDefinition, uint length)
        {
            switch (subtypeDefinition.Datatype)
            {
                case RcpTypes.Datatype.Boolean:
                    return new ArrayDefinition<Boolean>(subtypeDefinition, length);

                //case INT8:
                //    return new ArrayDefinition<Byte>((DefaultDefinition<Byte>)_sub_type, length);
                case RcpTypes.Datatype.Int32:
                    return new ArrayDefinition<int>(subtypeDefinition, length);
                case RcpTypes.Datatype.Float32:
                    return new ArrayDefinition<float>(subtypeDefinition, length);

                case RcpTypes.Datatype.String:
                    return new ArrayDefinition<String>(subtypeDefinition, length);

                //case RcpTypes.Datatype.FixedArray:
                //    return new ArrayDefinition<ArrayDefinition<?>>(
                //            (DefaultDefinition < ArrayDefinition <?>>)_sub_type,
                //                                                   length);

                default:
                    break;
            }

            return null;
        }

        public override List<T> ReadValue(KaitaiStream input)
        {
            var value = new List<T>();
            for (int i = 0; i < Length; i++)
                value.Add(Subtype.ReadValue(input));

            return value;
        }

        public override void WriteValue(BinaryWriter writer, List<T> value)
        {
            foreach (var v in value)
                Subtype.WriteValue(writer, v);
        }

        protected override void WriteOptions(BinaryWriter writer)
        {
            base.WriteOptions(writer);

            Subtype.Write(writer);

            //writer.Write((byte)RcpTypes.FixedArrayOptions.Length);
            //writer.Write(Length);

            //if (Default != null)
            //{
            //    writer.Write((byte)RcpTypes.NumberOptions.Minimum);
            //    WriteValue(writer, (T)Minimum);
            //}
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write((byte)Datatype);

            // write subtype
            Subtype.Write(writer);

            // write length (4byte)
            writer.Write(Length, ByteOrder.BigEndian);

            // write options
            if (Default != null)
            {
                writer.Write((byte)RcpTypes.FixedArrayOptions.Default);
                WriteValue(writer, Default);
            }

            //terminate
            writer.Write((byte)0);
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var option = (RcpTypes.FixedArrayOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.FixedArrayOptions), option))
                throw new RCPDataErrorException();

            switch (option)
            {
                case RcpTypes.FixedArrayOptions.Default:
                    Default = ReadValue(input);
                    return true;
            }

            return false;
        }
    }
}

