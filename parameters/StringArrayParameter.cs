using System;
using System.Collections.Generic;
using System.IO;
using Kaitai;
using RCP;
using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{
    internal class StringArrayParameter<T> : ArrayParameter<T>, IStringArrayParameter<T>
    {
        public StringArrayParameter(Int16 id, IParameterManager manager, params int[] structure) : 
            base(id, RcpTypes.Datatype.String, manager, structure)
        {
        }

        public override T ReadValue(KaitaiStream input)
        {
            var dimCount = input.ReadS4be();
            var elementCount = 0;
            var dimensions = new int[dimCount];
            for (int i = 0; i < dimCount; i++)
            {
                dimensions[i] = input.ReadS4be();
                elementCount += dimensions[i];
            }

            var a = Array.CreateInstance(typeof(string), dimensions);

            //TODO: support multiple dimensions
            for (int i=0; i<elementCount; i++)
            {
                a.SetValue(new RcpTypes.LongString(input).Data, i);
            }

            return (T)(object)a;
        }

        public override void WriteValue(BinaryWriter writer, T value)
        {
            base.WriteValue(writer, value);            

            var a = value as Array;
            var rank = 1;//a.Rank;

            //TODO: support multiple dimensions
            for (int i=0; i<rank; i++)
            {
                var l = a.GetLength(i);
                for (int j=0; j<l; j++)
                    RcpTypes.LongString.Write((string)a.GetValue(j), writer);
            }
        }

        //protected override void WriteTypeDefinitionOptions(BinaryWriter writer)
        //{
        //    writer.Write((byte)FElementType);
        //    base.WriteTypeDefinitionOptions(writer);
        //    writer.Write((byte)0);

        //    // write length (4byte)
        //    writer.Write(FValue.Length, ByteOrder.BigEndian);
        //}

        //protected override bool HandleTypeDefinitionOption(KaitaiStream input, byte code)
        //{
        //    var option = (RcpTypes.ArrayOptions)code;
        //    if (!Enum.IsDefined(typeof(RcpTypes.ArrayOptions), option))
        //        throw new RCPDataErrorException("Arraydefinition parsing: Unknown option: " + option.ToString());

        //    switch (option)
        //    {
        //        case RcpTypes.ArrayOptions.Default:
        //            Default = ReadValue(input);
        //            return true;
        //    }

        //    return false;
        //}
    }
}
