using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{                           
    public class StringDefinition : DefaultDefinition<string>, IStringDefinition
    {
        private bool FRegexChanged;
        private string FRegEx;
        public string RegEx { get { return FRegEx; } set { FRegexChanged = FRegEx != value;  FRegEx = value; } }

        public StringDefinition()
        : base(RcpTypes.Datatype.String)
        {
            FDefault = "";
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FDefaultChanged = Default != "";
            FRegexChanged = RegEx != "";
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

            if (FRegexChanged)
            {
                writer.Write((byte)RcpTypes.StringOptions.RegularExpression);
                RcpTypes.TinyString.Write(FRegEx, writer);
                writer.Write((byte)0);

                FRegexChanged = false;
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
                    RegEx = new RcpTypes.TinyString(input).Data;
                    return true;
            }

            return false;
        }

        public override void CopyTo(ITypeDefinition other)
        {
            base.CopyTo(other);

            var otherString = other as StringDefinition;

            if (FRegexChanged)
                otherString.RegEx = FRegEx;
        }
    }
}
