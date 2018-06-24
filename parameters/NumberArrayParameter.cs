using System;
using System.Collections.Generic;
using System.IO;
using Kaitai;
using RCP;
using RCP.Protocol;
using RCP.Exceptions;
using System.Numerics;

namespace RCP.Parameter
{
    internal class NumberArrayParameter<T, E> : ArrayParameter<T, E>, INumberArrayParameter<T, E> where E: struct
    {
        //public NumberDefinition<E> NumberDefinition => ArrayDefinition.ElementDefinition as NumberDefinition<E>;

        public NumberArrayParameter(Int16 id, IParameterManager manager, params int[] structure) : 
            base(id, RcpTypes.Datatype.String, manager, structure)
        {
            if (typeof(E) == typeof(float))
                TypeDefinition = new ArrayDefinition<T, E>(new Float32Definition() as NumberDefinition<E>, structure);
            else if (typeof(E) == typeof(int))
                TypeDefinition = new ArrayDefinition<T, E>(new Integer32Definition() as NumberDefinition<E>, structure);
            else if (typeof(E) == typeof(Vector2))
                TypeDefinition = new ArrayDefinition<T, E>(new Vector2f32Definition() as NumberDefinition<E>, structure);
            else if (typeof(E) == typeof(Vector3))
                TypeDefinition = new ArrayDefinition<T, E>(new Vector3f32Definition() as NumberDefinition<E>, structure);
            else if (typeof(E) == typeof(Vector4))
                TypeDefinition = new ArrayDefinition<T, E>(new Vector4f32Definition() as NumberDefinition<E>, structure);
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            //FValueChanged = FValue != "";
        }

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            switch (option)
            {
                case RcpTypes.ParameterOptions.Value:
                    Value = ArrayDefinition.ReadValue(input);
                    return true;
            }

            return false;
        }
    }
}
