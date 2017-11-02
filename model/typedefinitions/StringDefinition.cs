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
        : base(RcpTypes.Datatype.String){ }
    	
    	public static new StringDefinition Parse(KaitaiStream input)
        {
            var stringDefinition = new StringDefinition();

            while (true)
            {
                var code = input.ReadU1();
                if (code == 0)
                    break;

                var option = (RcpTypes.StringOptions)code;
				if (!Enum.IsDefined(typeof(RcpTypes.StringOptions), option)) 
                	throw new RCPDataErrorException();

                switch (option)
                {
                    case RcpTypes.StringOptions.Default:
                        stringDefinition.Default = new RcpTypes.LongString(input).Data;
                        break;
                }
            }

            return stringDefinition;
        }

    	public override void WriteValue(BinaryWriter writer, string value)
        {
            RcpTypes.LongString.Write(value, writer);
        }
    	
        protected override void WriteProperties(BinaryWriter writer)
        {
        	if (!String.IsNullOrWhiteSpace(Default))
        	{
        		writer.Write((byte)RcpTypes.StringOptions.Default);
        		RcpTypes.LongString.Write(Default, writer);
        	}
        }
    }
}
