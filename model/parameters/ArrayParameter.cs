using Kaitai;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RCP.Model
{
    public class ArrayParameter<T> : ValueParameter<List<T>>
    {
        //public new IArrayDefinition<T> TypeDefinition
        //{
        //    get { return FTypeDefinition; }
        //}
        ArrayDefinition<T> FTypeDefinition;

        public ArrayParameter(uint id, ArrayDefinition<T> typeDefinition): base (id, typeDefinition)
        {
            FTypeDefinition = typeDefinition;
        }

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            switch (option)
            {
                case RcpTypes.ParameterOptions.Value:
                    Value = FTypeDefinition.ReadValue(input);
                    return true;
            }

            return false;
        }
    }
}
