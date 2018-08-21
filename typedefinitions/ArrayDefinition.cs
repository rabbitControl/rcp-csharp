using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{
    public class ArrayDefinition<T, E> : DefaultDefinition<T>, IArrayDefinition
    {
        protected bool FStructureChanged;
        private int[] FStructure;
        public int[] Structure { get { return FStructure; } set { FStructure = value; FStructureChanged = true; } }

        public RcpTypes.Datatype ElementType => FElementDefinition.Datatype;

        private IDefaultDefinition<E> FElementDefinition;
        public ITypeDefinition ElementDefinition => FElementDefinition; 

        public ArrayDefinition(DefaultDefinition<E> elementDefinition, int[] structure) : base(RcpTypes.Datatype.Array)
        {
            FElementDefinition = elementDefinition;
            Structure = structure;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            DefaultChanged = Default != null;
        }

        protected override void WriteOptions(BinaryWriter writer)
        {
            FElementDefinition.Write(writer);

            base.WriteOptions(writer);

            if (FStructureChanged)
            {
                writer.Write((byte)RcpTypes.ArrayOptions.Structure);
                WriteStructure(writer);
            }
        }

        public void WriteStructure(BinaryWriter writer)
        {
            var rank = FStructure.Length;

            //number of dimensions
            writer.Write(rank, ByteOrder.BigEndian);
            //length per dimension
            for (int i = 0; i < rank; i++)
                writer.Write(FStructure[i], ByteOrder.BigEndian);
        }

        public override void ParseOptions(KaitaiStream input)
        {
            FElementDefinition.ParseOptions(input);

            while (true)
            {
                var code = input.ReadU1();
                if (code == 0) // terminator
                    break;

                // handle option in specific implementation
                if (!HandleOption(input, code))
                {
                    throw new RCPUnsupportedFeatureException();
                }
            }
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var option = (RcpTypes.ArrayOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.ArrayOptions), option))
                throw new RCPDataErrorException("Arraydefinition parsing: Unknown option: " + option.ToString());

            switch (option)
            {
                case RcpTypes.ArrayOptions.Default:
                    FDefault = ReadValue(input);
                    return true;

                case RcpTypes.ArrayOptions.Structure:
                    Structure = ReadStructure(input);
                    return true;
            }

            return false;
        }

        private int[] ReadStructure(KaitaiStream input)
        {
            var rank = input.ReadS4be();
            var dimensions = new int[rank];

            for (int i = 0; i < rank; i++)
                dimensions[i] = input.ReadS4be();

            return dimensions;
        }

        public override T ReadValue(KaitaiStream input)
        {
            FStructure = ReadStructure(input);

            var a = Array.CreateInstance(typeof(E), FStructure[0]);

            //TODO: support multiple dimensions
            for (int i = 0; i < FStructure[0]; i++)
            {
                a.SetValue((E)FElementDefinition.ReadValue(input), i);
            }

            return (T)(object)a;
        }

        public override void WriteValue(BinaryWriter writer, T value)
        {
            WriteStructure(writer);

            var a = value as Array;
            var rank = 1;//a.Rank;

            //TODO: support multiple dimensions
            for (int i = 0; i < rank; i++)
            {
                var l = a.GetLength(i);
                for (int j = 0; j < l; j++)
                    FElementDefinition.WriteValue(writer, (E)a.GetValue(j));
            }
        }
    }
}

