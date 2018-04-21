using System;
using System.IO;
using System.Drawing;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class RGBAParameter : ValueParameter<Color>
    {
        public new Nullable<Color> Value { get; set; }
        public new IRGBADefinition TypeDefinition 
        {
            get { return base.TypeDefinition as IRGBADefinition;}
        }

		public RGBAParameter(Int16 id): 
            base (id, new RGBADefinition())
        { }

        public RGBAParameter(Int16 id, IRGBADefinition definition):
            base(id, definition)
        { }

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            switch (option)
            {
                case RcpTypes.ParameterOptions.Value:
                    Value = TypeDefinition.ReadValue(input);
                    return true;
            }

            return false;
        }

        //since we're reintroducing the value as nullable we also need to override WriteValue
        protected override void WriteValue(BinaryWriter writer)
        {
            if (Value != null)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Value);
                TypeDefinition.WriteValue(writer, (Color)Value);
            }
        }
    }
}
