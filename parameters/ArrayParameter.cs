using System;
using System.Collections.Generic;
using System.IO;
using Kaitai;
using RCP;
using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{
    internal abstract class ArrayParameter<T, E> : ValueParameter<T>, IArrayParameter<T>
    {
        public ArrayDefinition<T, E> ArrayDefinition => TypeDefinition as ArrayDefinition<T, E>;
        private RcpTypes.Datatype FElementType;

        public ArrayParameter(Int16 id, RcpTypes.Datatype elementType, IParameterManager manager, params int[] structure) : 
            base(id, manager)
        {
            FElementType = elementType;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            ValueChanged = Value != null;
        }

        protected override void WriteValue(BinaryWriter writer)
        {
            if (ValueChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Value);
                ArrayDefinition.WriteValue(writer, Value);
                ValueChanged = false;
            }
        }
    }
}
