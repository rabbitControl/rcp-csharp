using System;
using System.IO;
using Kaitai;

namespace RCP.Model
{
    public class StringDefinition : DefaultDefinition<string>, IStringDefinition
    {
        //public string Format { get; set; }
        //public string Filemask { get; set; }
        //public string MaxChars { get; set; }

        public StringDefinition()
        : base(RcpTypes.Datatype.String)
        { }

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
        	if (!String.IsNullOrWhiteSpace(Default))
        	{
        		writer.Write((byte)RcpTypes.StringOptions.Default);
        		RcpTypes.LongString.Write(Default, writer);
        	}
        }
    	
        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var option = (RcpTypes.StringOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.StringOptions), option))
                throw new RCPDataErrorException();

            switch (option)
            {
                case RcpTypes.StringOptions.Default:
                    Default = ReadValue(input);
                    return true;
            }

            return false;
        }
    }
}
