using System;
using System.Collections.Generic;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{
    public class EnumDefinition : DefaultDefinition<ushort>, IEnumDefinition
    {
    	public string[] Entries { get; set; }
    	
        public EnumDefinition()
        : base(RcpTypes.Datatype.Enum)
        { }

        public override ushort ReadValue(KaitaiStream input)
        {
            return input.ReadU2be();
        }

        public override void WriteValue(BinaryWriter writer, ushort value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    	
    	protected override void WriteOptions(BinaryWriter writer)
        {
            base.WriteOptions(writer);

            if (Entries != null)
            {
                writer.Write((byte)RcpTypes.EnumOptions.Entries);
                ushort entryCount = (ushort) Entries.Length;
	            writer.Write(entryCount, ByteOrder.BigEndian);
	
	            foreach (var entry in Entries)
	                RcpTypes.TinyString.Write(entry, writer);
            }
        }
    	
    	protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var result = base.HandleOption(input, code);
            if (result)
                return result;

            var option = (RcpTypes.EnumOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.EnumOptions), option))
                throw new RCPDataErrorException("EnumDefinition parsing: Unknown option: " + option.ToString());

            switch (option)
            {
                case RcpTypes.EnumOptions.Default:
                    Default = ReadValue(input);
                    return true;
            	
            	case RcpTypes.EnumOptions.Entries:
            		var entries = new List<string>();
            		var entryCount = input.ReadU2be();
		            for (int i=0; i<entryCount; i++)
		                entries.Add(new RcpTypes.TinyString(input).Data);
		            Entries = entries.ToArray();
                    return true;
            }
        	
        	return false;
        }
    }
}