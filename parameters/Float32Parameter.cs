using System;
using System.IO;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    internal class Float32Parameter : NumberParameter<float>
    {
        public Float32Parameter(Int16 id, IParameterManager manager)
        : base(id, RcpTypes.Datatype.Float32, manager)
        {
            FMultipleOf = 0.01f;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FValueChanged = Value != 0f;
            FDefaultChanged = Default != 0f;

            FMinimumChanged = Minimum != -99999f;
            FMaximumChanged = Maximum != 99999f;
            FMultipleOfChanged = MultipleOf != 0.01f;
        }

        public override float ReadValue(KaitaiStream input)
        {
            return input.ReadF4be();
        }

        public override void WriteValue(BinaryWriter writer, float value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}