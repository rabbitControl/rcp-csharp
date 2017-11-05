using Kaitai;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RCP.Model
{
    public class StringParameter : ValueParameter<string>
    {
        public new IStringDefinition TypeDefinition 
    	{ 
    		get 
    		{
    			return base.TypeDefinition as IStringDefinition;
    		}
   		}

        public StringParameter(uint id): 
            base (id, new StringDefinition())
        { }

        public StringParameter(uint id, IStringDefinition definition):
            base(id, definition)
        { }

        //protected override void WriteValue(BinaryWriter writer)
        //{
        //    if (Value != null)
        //    {
        //        writer.Write((byte)RcpTypes.ParameterOptions.Value);
        //        NumberDefinition.WriteValue(writer, (T)Value);
        //    }
        //}
    }
}
