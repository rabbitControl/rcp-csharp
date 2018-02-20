using System.Collections.Generic;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class ArrayParameter<T> : ValueParameter<List<T>>
    {
        //public new IArrayDefinition<T> TypeDefinition
        //{
        //    get { return FTypeDefinition; }
        //}
        ArrayDefinition<T> FTypeDefinition;

        public ArrayParameter(byte[] id, ArrayDefinition<T> typeDefinition): base (id, typeDefinition)
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
