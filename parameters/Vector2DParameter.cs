using System;
using System.IO;
using System.Numerics;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class Vector2f32Parameter : NumberParameter<Vector2>
    {
        public Vector2f32Parameter(Int16 id, IParameterManager manager)
        : base(id, RcpTypes.Datatype.Vector2f32, manager)
        {
            FValue = new Vector2(0, 0);
            FDefault = new Vector2(0, 0); 
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FValueChanged = Value != new Vector2(0, 0);
            FDefaultChanged = Default != new Vector2(0, 0);

            FMinimumChanged = Minimum != new Vector2(-9999, -9999);
            FMaximumChanged = Maximum != new Vector2(9999, 9999);
            FMultipleOfChanged = MultipleOf != new Vector2(0.01f, 0.01f);
        }

        public override Vector2 ReadValue(KaitaiStream input)
        {
            return new Vector2(input.ReadF4be(), input.ReadF4be());
        }

        public override void WriteValue(BinaryWriter writer, Vector2 value)
        {
            writer.Write(value.X, ByteOrder.BigEndian);
            writer.Write(value.Y, ByteOrder.BigEndian);
        }
    }
}