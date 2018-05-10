using Kaitai;

using RCP.Protocol;
using System;
using System.IO;

namespace RCP.Parameter
{
    public class StringParameter : ValueParameter<string>
    {
        public StringParameter(Int16 id, IParameterManager manager) : 
            base (id, RcpTypes.Datatype.String, manager)
        {
            FValue = "";
            FDefault = "";
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FValueChanged = Value != "";
            FDefaultChanged = Default != "";
        }

        public override string ReadValue(KaitaiStream input)
        {
            return new RcpTypes.LongString(input).Data;
        }

        public override void WriteValue(BinaryWriter writer, string value)
        {
            RcpTypes.LongString.Write(value, writer);
        }
    }
}
