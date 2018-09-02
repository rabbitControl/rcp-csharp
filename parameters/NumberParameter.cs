using System;
using System.IO;

using Kaitai;
using RCP.Protocol;
using RCP;
using RCP.Exceptions;
using System.Numerics;

namespace RCP.Parameter
{
    public class NumberParameter<T> : ValueParameter<T>
        where T : struct
    {
        public new NumberDefinition<T> TypeDefinition => base.TypeDefinition as NumberDefinition<T>;

        public NumberParameter(Int16 id, IParameterManager manager, NumberDefinition<T> typeDefinition) 
            : base(id, manager, typeDefinition)
        {
        }

        public T Minimum
        {
            get { return TypeDefinition.Minimum; }
            set
            {
                TypeDefinition.Minimum = value;
                if (TypeDefinition.MinimumChanged)
                    SetDirty();
            }
        }

        public T Maximum
        {
            get { return TypeDefinition.Maximum; }
            set
            {
                TypeDefinition.Maximum = value;
                if (TypeDefinition.MaximumChanged)
                    SetDirty();
            }
        }

        public T MultipleOf
        {
            get { return TypeDefinition.MultipleOf; }
            set
            {
                TypeDefinition.MultipleOf = value;
                if (TypeDefinition.MultipleOfChanged)
                    SetDirty();
            }
        }

        public RcpTypes.NumberScale Scale
        {
            get { return TypeDefinition.Scale; }
            set
            {
                TypeDefinition.Scale = value;
                if (TypeDefinition.ScaleChanged)
                    SetDirty();
            }
        }

        public string Unit
        {
            get { return TypeDefinition.Unit; }
            set
            {
                TypeDefinition.Unit = value;
                if (TypeDefinition.UnitChanged)
                    SetDirty();
            }
        }
    }
}
