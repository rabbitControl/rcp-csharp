using Kaitai;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RCP.Model
{
    public class StringParameter : ValueParameter<string>
    {
        public IStringDefinition StringDefinition { get; private set; }

        public StringParameter(uint id, IStringDefinition stringDefinition): base (id, stringDefinition)
        {
            StringDefinition = stringDefinition;
        }

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
