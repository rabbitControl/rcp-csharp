using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Kaitai;

namespace RCP.Model
{
    public class RCPString : TypeDefinition<string>
    {
        public string Format { get; set; }
        public string Filemask { get; set; }
        public string MaxChars { get; set; }

        public RCPString()
        : base(RcpTypes.Datatype.String){ }
    	
    	public static new RCPString Parse(KaitaiStream input)
        {
            var stringDefinition = new RCPString();

            while (true)
            {
                var code = input.ReadU1();
                if (code == 0)
                    break;

                var property = (RcpTypes.StringProperty)code;
				if (!Enum.IsDefined(typeof(RcpTypes.StringProperty), property)) 
                	throw new RCPDataErrorException();

                switch (property)
                {
                    case RcpTypes.StringProperty.Default:
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
        		writer.Write((byte)RcpTypes.StringProperty.Default);
        		RcpTypes.LongString.Write(Default, writer);
        	}
        }
    }
}
