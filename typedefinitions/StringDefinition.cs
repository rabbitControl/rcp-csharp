using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{                           
    public class StringDefinition : DefaultDefinition<string>, IStringDefinition
    {
        public bool RegularExpressionChanged { get; private set; }
        private string FRegEx;
        public string RegularExpression { get { return FRegEx; } set { RegularExpressionChanged = FRegEx != value;  FRegEx = value; } }

        public StringDefinition()
        : base(RcpTypes.Datatype.String)
        {
            FDefault = "";
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            DefaultChanged = Default != "";
            RegularExpressionChanged = RegularExpression != "";
        }

        public override bool AnyChanged()
        {
            return base.AnyChanged() || RegularExpressionChanged;
        }

        public override string ReadValue(KaitaiStream input)
        {
            return new RcpTypes.LongString(input).Data;
        }

        public override void WriteValue(BinaryWriter writer, string value)
        {
            RcpTypes.LongString.Write(value, writer);
        }

        protected override void WriteOptions(BinaryWriter writer)
        {
            base.WriteOptions(writer);

            if (RegularExpressionChanged)
            {
                writer.Write((byte)RcpTypes.StringOptions.RegularExpression);
                RcpTypes.TinyString.Write(FRegEx, writer);
                writer.Write((byte)0);

                RegularExpressionChanged = false;
            }
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var option = (RcpTypes.StringOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.StringOptions), option))
                throw new RCPDataErrorException("StringDefinition parsing: Unknown option: " + option.ToString());

            switch (option)
            {
                case RcpTypes.StringOptions.Default:
                    Default = ReadValue(input);
                    return true;

                case RcpTypes.StringOptions.RegularExpression:
                    RegularExpression = new RcpTypes.TinyString(input).Data;
                    return true;
            }

            return false;
        }

        public override void CopyFrom(ITypeDefinition other)
        {
            base.CopyFrom(other);

            var otherString = other as IStringDefinition;

            if (otherString.RegularExpressionChanged)
                FRegEx = otherString.RegularExpression;
        }
    }
}
