using System;
using System.IO;

using Kaitai;
using RCP.Protocol;
using RCP;
using RCP.Exceptions;

namespace RCP.Parameter
{
    public abstract class NumberParameter<T> : ValueParameter<T> where T : struct
    {
        private bool FMinimumChanged;
        private T FMinimum;
        public T Minimum { get { return FMinimum; } set { FMinimum = value; FMinimumChanged = true; } }

        private bool FMaximumChanged;
        private T FMaximum;
        public T Maximum { get { return FMaximum; } set { FMaximum = value; FMaximumChanged = true; } }

        private bool FMultipleOfChanged;
        private T FMultipleOf;
        public T MultipleOf { get { return FMultipleOf; } set { FMultipleOf = value; FMultipleOfChanged = true; } }

        private bool FScaleChanged;
        private RcpTypes.NumberScale FScale;
        public RcpTypes.NumberScale Scale { get { return FScale; } set { FScale = value; FScaleChanged = true; } }

        private bool FUnitChanged;
        private string FUnit;
        public string Unit { get { return FUnit; } set { FUnit = value; FUnitChanged = true; } }

        public NumberParameter(Int16 id, RcpTypes.Datatype datatype, IManager manager) : 
            base (id, datatype, manager)
        { }

        protected override void WriteTypeDefinitionOptions(BinaryWriter writer)
        {
            base.WriteTypeDefinitionOptions(writer);

            if (FMinimumChanged)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Minimum);
                WriteValue(writer, (T)Minimum);
                FMinimumChanged = false;
            }

            if (FMaximumChanged)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Maximum);
                WriteValue(writer, (T)Maximum);
                FMaximumChanged = false;
            }

            if (FMultipleOfChanged)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Multipleof);
                WriteValue(writer, (T)MultipleOf);
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

        protected override bool HandleTypeDefinitionOption(KaitaiStream input, byte code)
        {
            var option = (RcpTypes.NumberOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.NumberOptions), option))
                throw new RCPDataErrorException("NumberDefinition parsing: Unknown option: " + option.ToString());

            switch (option)
            {
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
    }
}
