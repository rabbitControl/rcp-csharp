using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{
    public abstract class NumberDefinition<T> : DefaultDefinition<T>, INumberDefinition<T> where T: struct
    {
        protected bool FMinimumChanged;
        private T FMinimum;
        public T Minimum { get { return FMinimum; } set { FMinimumChanged = !FMinimum.Equals(value); FMinimum = value; } }

        protected bool FMaximumChanged;
        private T FMaximum;
        public T Maximum { get { return FMaximum; } set { FMaximumChanged = !FMaximum.Equals(value); FMaximum = value; } }

        protected bool FMultipleOfChanged;
        protected T FMultipleOf;
        public T MultipleOf { get { return FMultipleOf; } set { FMultipleOfChanged = !FMultipleOf.Equals(value); FMultipleOf = value; } }

        private bool FScaleChanged;
        private RcpTypes.NumberScale FScale;
        public RcpTypes.NumberScale Scale { get { return FScale; } set { FScaleChanged = !FScale.Equals(value); FScale = value; } }

        private bool FUnitChanged;
        private string FUnit = "";
        public string Unit { get { return FUnit; } set { FUnitChanged = !FUnit.Equals(value); FUnit = value; } }

        public NumberDefinition(RcpTypes.Datatype datatype)
        : base(datatype)
        { }

        public override bool AnyChanged()
        {
            return FMinimumChanged || FMaximumChanged || FMultipleOfChanged || FScaleChanged || FUnitChanged;
        }

        public override void ResetForInitialize()
        {
            FScaleChanged = FScale != RcpTypes.NumberScale.Linear;
            FUnitChanged = FUnit != "";
        }

        protected override void WriteOptions(BinaryWriter writer)
        {
            base.WriteOptions(writer);

            if (FMinimumChanged)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Minimum);
                WriteValue(writer, Minimum);
                FMinimumChanged = false;
            }

            if (FMaximumChanged)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Maximum);
                WriteValue(writer, Maximum);
                FMaximumChanged = false;
            }

            if (FMultipleOfChanged)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Multipleof);
                WriteValue(writer, MultipleOf);
                FMultipleOfChanged = false;
            }

            if (FScaleChanged)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Scale);
                writer.Write((byte)Scale);
                FScaleChanged = false;
            }

            if (FUnitChanged)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Unit);
                writer.Write(Unit);
                FUnitChanged = false;
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

        public override void CopyTo(ITypeDefinition other)
        {
            base.CopyTo(other);

            var otherNumber = other as NumberDefinition<T>;

            if (FMinimumChanged)
                otherNumber.Minimum = FMinimum;

            if (FMaximumChanged)
                otherNumber.Maximum = FMaximum;

            if (FMultipleOfChanged)
                otherNumber.MultipleOf = FMultipleOf;

            if (FScaleChanged)
                otherNumber.Scale = FScale;

            if (FUnitChanged)
                otherNumber.Unit = FUnit;
        }
    }
}