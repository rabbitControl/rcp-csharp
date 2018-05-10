using System;

using Kaitai;
using RCP.Protocol;
using System.IO;

namespace RCP.Parameter
{
    internal class BooleanParameter : ValueParameter<bool>, IBooleanParameter
    {
        public BooleanParameter(Int16 id, IParameterManager manager) : 
            base (id, RcpTypes.Datatype.Boolean, manager)
        { }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FValueChanged = Value != false;
            FDefaultChanged = Default != false;
        }

        public override bool ReadValue(KaitaiStream input)
        {
            return input.ReadU1() > 0;
        }

        public override void WriteValue(BinaryWriter writer, bool value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}
