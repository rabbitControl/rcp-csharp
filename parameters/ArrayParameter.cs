using System;
using System.Collections.Generic;
using System.IO;
using Kaitai;
using RCP;
using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{
    internal abstract class ArrayParameter<T> : ValueParameter<T>, IArrayParameter<T>
    {
        private RcpTypes.Datatype FElementType;

        private int[] FStructure;
        public int[] Structure { get { return FStructure; } set { FStructure = value; } }

        public ArrayParameter(Int16 id, RcpTypes.Datatype elementType, IParameterManager manager, params int[] structure) : 
            base(id, RcpTypes.Datatype.Array, manager)
        {
            FElementType = elementType;
            FStructure = structure;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FValueChanged = Value != null;
            FDefaultChanged = Default != null;
        }

        //protected override void ParseTypeDefinitionOptions(KaitaiStream input)
        //{
        //    get options from the stream
        //    while (true)
        //    {
        //        var code = input.ReadU1();
        //        if (code == 0)
        //            break;

        //        var option = (RcpTypes.NumberOptions)code;
        //        if (!Enum.IsDefined(typeof(RcpTypes.NumberOptions), option))
        //            throw new RCPDataErrorException("Parameter parsing: Unknown option: " + option.ToString());

        //        switch (option)
        //        {
        //            case RcpTypes.NumberOptions.Default:
        //                Default = ReadValue(input); break;

        //            default:
        //                if (!HandleTypeDefinitionOption(input, code))
        //                {
        //                    throw new RCPUnsupportedFeatureException();
        //                }
        //                break;
        //        }
        //    }
        //}

        public override void WriteValue(BinaryWriter writer, T value)
        {
            WriteStructure(writer);
        }

        protected override void WriteTypeDefinitionOptions(BinaryWriter writer)
        {
            writer.Write((byte)FElementType);
            //write elementtype options
            writer.Write((byte)0);

            base.WriteTypeDefinitionOptions(writer);

            // write structure
            writer.Write((byte)RcpTypes.ArrayOptions.Structure);
            WriteStructure(writer);
        }

        private void WriteStructure(BinaryWriter writer)
        {
            var rank = FStructure.Length;

            //number of dimensions
            writer.Write(rank, ByteOrder.BigEndian);
            //length per dimension
            for (int i = 0; i < rank; i++)
                writer.Write(FStructure[i], ByteOrder.BigEndian);
        }

        private int[] ReadStructure(KaitaiStream input)
        {
            var rank = input.ReadS4be();
            var dimensions = new int[rank];

            for (int i = 0; i < rank; i++)
                dimensions[i] = input.ReadS4be();

            return dimensions;
        }

        protected override bool HandleTypeDefinitionOption(KaitaiStream input, byte code)
        {
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
    }
}
