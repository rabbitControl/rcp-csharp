using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{
    public abstract class TypeDefinition : ITypeDefinition
    {
        public RcpTypes.Datatype Datatype { get; private set; }

        public TypeDefinition(RcpTypes.Datatype datatype)
        {
            Datatype = datatype;
        }
        
        public abstract void CopyFrom(ITypeDefinition other);

        public virtual void ResetForInitialize()
        { }

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write((byte)Datatype);

            //write type specific stuff
            WriteOptions(writer);

            //terminate
            writer.Write((byte)0);
        }

        public virtual bool AnyChanged()
        {
            return false;
        }

        protected virtual void WriteOptions(BinaryWriter writer)
        { }

        protected virtual bool HandleOption(KaitaiStream input, byte option)
        {
            return false;
        }

        public virtual void ParseOptions(KaitaiStream input)
        {
            while (true)
            {
                var code = input.ReadU1();
                if (code == 0) // terminator
                    break;

                // handle option in specific implementation
                if (!HandleOption(input, code))
                {
                    throw new RCPUnsupportedFeatureException();
                }
            }
        }
    }

    public abstract class DefaultDefinition<T>: TypeDefinition, IDefaultDefinition<T>
    {
        public bool DefaultChanged { get; protected set; }
        protected T FDefault;
        public T Default { get { return FDefault; }
            set {
                DefaultChanged = !FDefault?.Equals(value) ?? value != null;
                FDefault = value;
            } }

        public DefaultDefinition(RcpTypes.Datatype datatype) : base(datatype)
        { }

        public abstract void WriteValue(BinaryWriter writer, T value);
        public abstract T ReadValue(KaitaiStream input);

        public override bool AnyChanged()
        {
            return DefaultChanged;
        }

        protected override void WriteOptions(BinaryWriter writer)
        {
            if (DefaultChanged)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Default);
                WriteValue(writer, Default);
                DefaultChanged = false;
            }
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var option = (RcpTypes.NumberOptions)code;

            switch (option)
            {
                case RcpTypes.NumberOptions.Default:
                    Default = ReadValue(input);
                    return true;
            }

            return false;
        }

        public override void CopyFrom(ITypeDefinition other)
        {
            var otherDefinition = other as IDefaultDefinition<T>;
            if (otherDefinition.DefaultChanged)
                FDefault = otherDefinition.Default;
        }

        public static ITypeDefinition Parse(KaitaiStream input)
        {
            var code = input.ReadByte();

            var datatype = (RcpTypes.Datatype)code;
            if (!Enum.IsDefined(typeof(RcpTypes.Datatype), datatype))
                throw new RCPDataErrorException("TypeDefinition parsing: Unknown datatype: " + datatype.ToString());

            //        	if (datatype != null)
            //	            MessageBox.Show(datatype.ToString() + " : ");
            ITypeDefinition definition = null;
            switch (datatype)
            {
                //case RcpTypes.Datatype.Boolean: definition = new BooleanDefinition(); break;
                //case RcpTypes.Datatype.Enum: definition = new EnumDefinition(); break;
                case RcpTypes.Datatype.Int32: definition = new Integer32Definition(); break;
                //				case RcpTypes.Datatype.Uint64: definition = RCPUInt64.Parse(input) as TypeDefinition<T>; break;
                //				case RcpTypes.Datatype.Int64: definition = RCPInt64.Parse(input) as TypeDefinition<T>; break;
                //case RcpTypes.Datatype.Float32: definition = new Float32Definition(); break;
                //				case RcpTypes.Datatype.Float64: definition = RCPFloat64.Parse(input) as TypeDefinition<T>; break;
                //case RcpTypes.Datatype.String: definition = new StringDefinition(); break;
                //case RcpTypes.Datatype.Rgba: definition = new RGBADefinition(); break;
                //case RcpTypes.Datatype.Vector2f32: definition = new Vector2f32Definition(); break;
                //case RcpTypes.Datatype.Vector3f32: definition = new Vector3f32Definition(); break;
            }

            if (definition != null)
            {
                definition.ParseOptions(input);
                return definition;
            }

            throw new RCPDataErrorException("TypeDefinition parsing failed!");
        }
    }
}