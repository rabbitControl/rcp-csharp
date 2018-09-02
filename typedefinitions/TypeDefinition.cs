using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{
    public abstract class TypeDefinition : ITypeDefinition
    {
        public static bool HasElementType(RcpTypes.Datatype type)
        {
            switch (type)
            {
                case RcpTypes.Datatype.Array:
                case RcpTypes.Datatype.List:
                case RcpTypes.Datatype.Range:
                    return true;
            }
            return false;
        }

        public static TypeDefinition Create(RcpTypes.Datatype type)
        {
            switch (type)
            {
                case RcpTypes.Datatype.Customtype:
                    break;
                case RcpTypes.Datatype.Boolean:
                    return new BooleanDefinition();
                case RcpTypes.Datatype.Int8:
                    return new Integer8Definition();
                case RcpTypes.Datatype.Uint8:
                    return new UInteger8Definition();
                case RcpTypes.Datatype.Int16:
                    return new Integer16Definition();
                case RcpTypes.Datatype.Uint16:
                    return new UInteger16Definition();
                case RcpTypes.Datatype.Int32:
                    return new Integer32Definition();
                case RcpTypes.Datatype.Uint32:
                    return new UInteger32Definition();
                case RcpTypes.Datatype.Int64:
                    return new Integer64Definition();
                case RcpTypes.Datatype.Uint64:
                    return new UInteger64Definition();
                case RcpTypes.Datatype.Float32:
                    return new Float32Definition();
                case RcpTypes.Datatype.Float64:
                    return new Float64Definition();
                case RcpTypes.Datatype.Vector2i32:
                    break;
                case RcpTypes.Datatype.Vector2f32:
                    return new Vector2f32Definition();
                case RcpTypes.Datatype.Vector3i32:
                    break;
                case RcpTypes.Datatype.Vector3f32:
                    return new Vector3f32Definition();
                case RcpTypes.Datatype.Vector4i32:
                    break;
                case RcpTypes.Datatype.Vector4f32:
                    return new Vector4f32Definition();
                case RcpTypes.Datatype.String:
                    return new StringDefinition();
                case RcpTypes.Datatype.Rgb:
                    return new RGBDefinition();
                case RcpTypes.Datatype.Rgba:
                    return new RGBADefinition();
                case RcpTypes.Datatype.Enum:
                    return new EnumDefinition();
                case RcpTypes.Datatype.Array:
                    throw new ArgumentException(nameof(type), "Must not be an array.");
                case RcpTypes.Datatype.List:
                    throw new ArgumentException(nameof(type), "Must not be a list.");
                case RcpTypes.Datatype.Bang:
                    break;
                case RcpTypes.Datatype.Group:
                    return new GroupDefinition();
                case RcpTypes.Datatype.Uri:
                    break;
                case RcpTypes.Datatype.Ipv4:
                    break;
                case RcpTypes.Datatype.Ipv6:
                    break;
                case RcpTypes.Datatype.Range:
                    throw new ArgumentException(nameof(type), "Must not be a range.");
                default:
                    throw new NotImplementedException($"Unkown type {type}.");
            }
            throw new NotSupportedException($"{type} is not supported yet.");
        }

        public TypeDefinition(RcpTypes.Datatype datatype)
        {
            Datatype = datatype;
        }

        public RcpTypes.Datatype Datatype { get; }
        public abstract Type ClrType { get; }

        public abstract TypeDefinition CreateArray(int[] structure);
        public abstract TypeDefinition CreateRange();
        public abstract Parameter CreateParameter(Int16 id, IParameterManager manager);
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

        public DefaultDefinition(RcpTypes.Datatype datatype) : base(datatype)
        {
        }

        public override Type ClrType => typeof(T);

        public T Default
        {
            get { return FDefault; }
            set
            {
                DefaultChanged = !FDefault?.Equals(value) ?? value != null;
                FDefault = value;
            }
        }

        public override sealed TypeDefinition CreateArray(int[] structure) => new ArrayDefinition<T>(this, structure);
        public override TypeDefinition CreateRange() => throw new NotSupportedException();

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