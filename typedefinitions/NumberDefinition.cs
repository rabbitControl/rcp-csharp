using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{
    public abstract class NumberDefinition<T> : DefaultDefinition<T>, INumberDefinition<T> where T: struct
    {
        public bool MinimumChanged { get; protected set; }
        private T FMinimum;
        public T Minimum { get { return FMinimum; } set { MinimumChanged = !FMinimum.Equals(value); FMinimum = value; } }

        public bool MaximumChanged { get; protected set; }
        private T FMaximum;
        public T Maximum { get { return FMaximum; } set { MaximumChanged = !FMaximum.Equals(value); FMaximum = value; } }

        public bool MultipleOfChanged { get; protected set; }
        protected T FMultipleOf;
        public T MultipleOf { get { return FMultipleOf; } set { MultipleOfChanged = !FMultipleOf.Equals(value); FMultipleOf = value; } }

        public bool ScaleChanged { get; protected set; }
        private RcpTypes.NumberScale FScale;
        public RcpTypes.NumberScale Scale { get { return FScale; } set { ScaleChanged = !FScale.Equals(value); FScale = value; } }

        public bool UnitChanged { get; protected set; }
        private string FUnit = "";
        public string Unit { get { return FUnit; } set { UnitChanged = !FUnit.Equals(value); FUnit = value; } }

        public NumberDefinition(RcpTypes.Datatype datatype)
        : base(datatype)
        { }

        public override bool AnyChanged()
        {
            return base.AnyChanged() || MinimumChanged || MaximumChanged || MultipleOfChanged || ScaleChanged || UnitChanged;
        }

        public override void ResetForInitialize()
        {
            ScaleChanged = FScale != RcpTypes.NumberScale.Linear;
            UnitChanged = FUnit != "";
        }

        protected override void WriteOptions(BinaryWriter writer)
        {
            base.WriteOptions(writer);

            if (MinimumChanged)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Minimum);
                WriteValue(writer, Minimum);
                MinimumChanged = false;
            }

            if (MaximumChanged)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Maximum);
                WriteValue(writer, Maximum);
                MaximumChanged = false;
            }

            if (MultipleOfChanged)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Multipleof);
                WriteValue(writer, MultipleOf);
                MultipleOfChanged = false;
            }

            if (ScaleChanged)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Scale);
                writer.Write((byte)Scale);
                ScaleChanged = false;
            }

            if (UnitChanged)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Unit);
                writer.Write(Unit);
                UnitChanged = false;
            }
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var result = base.HandleOption(input, code);
            if (result)
                return result;

            var option = (RcpTypes.NumberOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.NumberOptions), option))
                throw new RCPDataErrorException("NumberDefinition parsing: Unknown option: " + option.ToString());

            switch (option)
            {
                case RcpTypes.NumberOptions.Default:
                    Default = ReadValue(input);
                    return true;

                case RcpTypes.NumberOptions.Minimum:
                    Minimum = ReadValue(input);
                    return true;

                case RcpTypes.NumberOptions.Maximum:
                    Maximum = ReadValue(input);
                    return true;

                case RcpTypes.NumberOptions.Multipleof:
                    MultipleOf = ReadValue(input);
                    return true;

                case RcpTypes.NumberOptions.Scale:
                    Scale = (RcpTypes.NumberScale)input.ReadU1();
                    return true;

                case RcpTypes.NumberOptions.Unit:
                    Unit = new RcpTypes.TinyString(input).Data;
                    return true;
            }

            return false;
        }

        public override void CopyFrom(ITypeDefinition other)
        {
            base.CopyFrom(other);

            var otherNumber = other as INumberDefinition<T>;

            if (otherNumber.MinimumChanged)
                FMinimum = otherNumber.Minimum;

            if (otherNumber.MaximumChanged)
                FMaximum = otherNumber.Maximum;

            if (otherNumber.MultipleOfChanged)
                FMultipleOf = otherNumber.MultipleOf;

            if (otherNumber.ScaleChanged)
                FScale = otherNumber.Scale;

            if (otherNumber.UnitChanged)
                FUnit = otherNumber.Unit;
        }
    }
}