using Kaitai;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RCP.Model
{
    public class NumberParameter<T> : ValueParameter<T> where T: struct
    {
        new public Nullable<T> Value { get; set; }
        public INumberDefinition<T> NumberDefinition { get; private set; }

        public NumberParameter(uint id, INumberDefinition<T> numberDefinition): base (id, numberDefinition)
        {
            NumberDefinition = numberDefinition;
        }

        protected override void WriteValue(BinaryWriter writer)
        {
            if (Value != null)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Value);
                NumberDefinition.WriteValue(writer, (T)Value);
            }
        }
    }
}
