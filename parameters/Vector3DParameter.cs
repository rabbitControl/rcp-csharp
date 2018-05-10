using System;
using System.IO;
using System.Numerics;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    internal class Vector3f32Parameter : NumberParameter<Vector3>
    {
        public Vector3f32Parameter(Int16 id, IParameterManager manager)
        : base(id, RcpTypes.Datatype.Vector3f32, manager)
        {
            FValue = new Vector3(0, 0, 0);
            FDefault = new Vector3(0, 0, 0);
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FValueChanged = Value != new Vector3(0, 0, 0);
            FDefaultChanged = Default != new Vector3(0, 0, 0);

            FMinimumChanged = Minimum != new Vector3(-9999, -9999, -9999);
            FMaximumChanged = Maximum != new Vector3(9999, 9999, 9999);
            FMultipleOfChanged = MultipleOf != new Vector3(0.01f, 0.01f, 0.01f);
        }

        public override Vector3 ReadValue(KaitaiStream input)
        {
            return new Vector3(input.ReadF4be(), input.ReadF4be(), input.ReadF4be());
        }

        public override void WriteValue(BinaryWriter writer, Vector3 value)
        {
            writer.Write(value.X, ByteOrder.BigEndian);
            writer.Write(value.Y, ByteOrder.BigEndian);
            writer.Write(value.Z, ByteOrder.BigEndian);
        }
    }
}