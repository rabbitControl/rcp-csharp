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
    public sealed class ArrayDefinition<T> : DefaultDefinition<T[]>, IArrayDefinition
    {
        protected bool FStructureChanged;
        private int[] FStructure;
        public int[] Structure
        {
            get { return FStructure; }
            set
            {
                FStructure = value;
                FStructureChanged = true;
            }
        }

        public RcpTypes.Datatype ElementType => FElementDefinition.Datatype;

        private readonly DefaultDefinition<T> FElementDefinition;
        public ITypeDefinition ElementDefinition => FElementDefinition; 

        public ArrayDefinition(DefaultDefinition<T> elementDefinition, int[] structure) : base(RcpTypes.Datatype.Array)
        {
            FElementDefinition = elementDefinition;
            Structure = structure;
        }

        public override Parameter CreateParameter(short id, IParameterManager manager) => new ArrayParameter<T>(id, manager, this);

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
            base.ParseOptions(input);
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

        public override T[] ReadValue(KaitaiStream input)
        {
            FStructure = ReadStructure(input);

            var a = new T[FStructure[0]];;

            //TODO: support multiple dimensions
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = FElementDefinition.ReadValue(input);
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
                FElementDefinition.WriteValue(writer, e);
        }
    }
}

