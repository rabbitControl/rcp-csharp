using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;
using RCP.Parameters;
using System.Linq;

namespace RCP.Types
{
    public sealed class ArrayDefinition<T> : DefaultDefinition<T[]>, IArrayDefinition
    {
        readonly DefaultDefinition<T> FElementType;
        int[] FStructure = Array.Empty<int>();

        public ArrayDefinition(DefaultDefinition<T> elementType, int[] structure) 
            : base(RcpTypes.Datatype.Array, Array.Empty<T>())
        {
            FElementType = elementType;
            Structure = structure;
        }

        public ITypeDefinition ElementDefinition => FElementType;

        public RcpTypes.Datatype ElementType => ElementDefinition.Datatype;

        public int[] Structure
        {
            get => FStructure;
            set
            {
                if (SetProperty(ref FStructure, value))
                    SetChanged(TypeChangedFlags.ArrayStructure);
            }
        }

        public override Parameter CreateParameter(short id, IParameterManager manager) => new ArrayParameter<T>(id, manager, this);

        protected override void WriteOptions(BinaryWriter writer)
        {
            FElementType.Write(writer);

            base.WriteOptions(writer);

            if (IsChanged(TypeChangedFlags.ArrayStructure))
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
            FElementType.ParseOptions(input);
            base.ParseOptions(input);
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            if (base.HandleOption(input, code))
                return true;

            var option = (RcpTypes.ArrayOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.ArrayOptions), option))
                throw new RCPDataErrorException("Arraydefinition parsing: Unknown option: " + option.ToString());

            switch (option)
            {
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

        public override T[] ReadValue(KaitaiStream input)
        {
            FStructure = ReadStructure(input);

            var a = new T[FStructure[0]];;

            //TODO: support multiple dimensions
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = FElementType.ReadValue(input);
            }

            return a;
        }

        public override void WriteValue(BinaryWriter writer, T[] value)
        {
            WriteStructure(writer);

            var a = value as Array;
            var rank = 1;//a.Rank;

            //TODO: support multiple dimensions
            foreach (var e in value)
                FElementType.WriteValue(writer, e);
        }
    }
}

