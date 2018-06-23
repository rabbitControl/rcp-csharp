using System;
using System.IO;

using Kaitai;
using RCP.Protocol;
using RCP;
using RCP.Exceptions;

namespace RCP.Parameter
{
    internal class NumberParameter<T> : ValueParameter<T>, INumberParameter<T> where T : struct
    {
        public NumberDefinition<T> NumberDefinition => TypeDefinition as NumberDefinition<T>;
        
        public T Minimum { get { return NumberDefinition.Minimum; } set { NumberDefinition.Minimum = value; SetDirty(); } }
        public T Maximum { get { return NumberDefinition.Maximum; } set { NumberDefinition.Maximum = value; SetDirty(); } }
        public T MultipleOf { get { return NumberDefinition.MultipleOf; } set { NumberDefinition.MultipleOf = value; SetDirty(); } }
        public RcpTypes.NumberScale Scale { get { return NumberDefinition.Scale; } set { NumberDefinition.Scale = value; SetDirty(); } }
        public string Unit { get { return NumberDefinition.Unit; } set { NumberDefinition.Unit = value; SetDirty(); } }

        public NumberParameter(Int16 id, IParameterManager manager) : 
            base (id, manager)
        {
            if (typeof(T) == typeof(float))
                TypeDefinition = new Float32Definition() as NumberDefinition<T>;
            else if (typeof(T) == typeof(int))
                TypeDefinition = new Integer32Definition() as NumberDefinition<T>;
            //else if (typeof(T) == typeof(Vector2))
            //    param = new Vector2f32Parameter(FIdCounter++, this);
            //else if (typeof(T) == typeof(Vector3))
            //    param = new Vector3f32Parameter(FIdCounter++, this);

            //TypeDefinition = new Integer32Definition() as NumberDefinition<T>;
        }

        protected override bool AnyChanged()
        {
            return base.AnyChanged() || TypeDefinition.AnyChanged();
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FValueChanged = !FValue.Equals(default(T));
        }

        protected override void WriteValue(BinaryWriter writer)
        {
            if (FValueChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Value);
                NumberDefinition.WriteValue(writer, Value);
                FValueChanged = false;
            }
        }

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            switch (option)
            {
                case RcpTypes.ParameterOptions.Value:
                    Value = NumberDefinition.ReadValue(input);
                    return true;
            }

            return false;
        }

        public override void CopyTo(IParameter other)
        {
            var param = other as NumberParameter<T>;

            TypeDefinition.CopyTo(param.TypeDefinition);

            base.CopyTo(other);
        }
    }
}
