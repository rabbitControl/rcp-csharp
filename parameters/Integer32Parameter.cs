using System;
using System.IO;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class Integer32Parameter : NumberParameter<int>
    {
        public Integer32Parameter(Int16 id, IParameterManager manager)
        : base(id, RcpTypes.Datatype.Int32, manager)
        {
            FMultipleOf = 1;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FValueChanged = Value != 0;
            FDefaultChanged = Default != 0;

            FMinimumChanged = Minimum != -99999;
            FMaximumChanged = Maximum != 99999;
            FMultipleOfChanged = MultipleOf != 1;
        }

        public override int ReadValue(KaitaiStream input)
        {
            return input.ReadS4be();
        }

        public override void WriteValue(BinaryWriter writer, int value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}