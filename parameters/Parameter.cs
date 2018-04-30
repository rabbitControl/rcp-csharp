using System;
using System.IO;

using Kaitai;
using RCP.Exceptions;
using RCP.Protocol;

namespace RCP.Parameter
{
    public enum Status { Update, Remove };

    public abstract class Parameter : IParameter, IWriteable
    {
        protected IManager FManager;
        private Status FStatus;
        public Status Status => FStatus;

        public Int16 Id { get; private set; }
        public ITypeDefinition TypeDefinition { get; private set; }

        public string Label { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public int? Order { get; set; }

        public Int16? ParentId { get; private set; }
        public Widget Widget { get; set; }
        public byte[] Userdata { get; set; }
        public string UserId { get; set; }

        public Parameter(Int16 id, ITypeDefinition typeDefinition, IManager manager)
        {
            Id = id;
            TypeDefinition = typeDefinition;
            FManager = manager;

            SetDirty();
        }

        public void Destroy()
        {
            FStatus = Status.Remove;
            SetDirty();
        }

        public void SetParent(IParameter param)
        {
            ParentId = param.Id;
            SetDirty();
        }

        protected void SetDirty()
        {
            if (FManager != null) //not assigned for temp-parameters (ie. those just used for parsing on clients)
                FManager.SetParameterDirty(this);
        }

        public void Write(BinaryWriter writer)
        {
            //mandatory
            writer.Write(Id, ByteOrder.BigEndian);
            TypeDefinition.Write(writer);

            //optional
            WriteValue(writer);

            if (!string.IsNullOrWhiteSpace(Label))
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Label);
                RcpTypes.TinyString.Write(Label, writer);
            }

            if (!string.IsNullOrWhiteSpace(Description))
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Description);
                RcpTypes.ShortString.Write(Description, writer);
            }

            if (!string.IsNullOrWhiteSpace(Tags))
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Tags);
                RcpTypes.TinyString.Write(Tags, writer);
            }

            if (Order.HasValue)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Order);
                writer.Write(Order.Value, ByteOrder.BigEndian);
            }

            if (ParentId.HasValue)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Parentid);
                writer.Write(ParentId.Value, ByteOrder.BigEndian);
            }

            if (Widget != null)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Widget);
                Widget.Write(writer);
            }

            if (Userdata != null)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Userdata);
                writer.Write(Userdata.Length, ByteOrder.BigEndian);
                writer.Write(Userdata);
            }

            if (!string.IsNullOrWhiteSpace(UserId))
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Userid);
                RcpTypes.TinyString.Write(UserId, writer);
            }

            //terminate
            writer.Write((byte)0);
        }

        protected abstract void WriteValue(BinaryWriter writer);

        public static Parameter Parse(KaitaiStream input)
        {
            // get mandatory id
            var id = input.ReadS2be();

            var datatype = (RcpTypes.Datatype)input.ReadU1();
            if (!Enum.IsDefined(typeof(RcpTypes.Datatype), datatype))
                throw new RCPDataErrorException("Parameter parsing: Unknown datatype!");

            Parameter parameter = null;

            switch (datatype)
            {
                //case RcpTypes.Datatype.FixedArray:
                //    {
                //        dynamic arrayDefinition = ArrayDefinition<dynamic>.Parse(input);
                //        parameter = (Parameter)ParameterFactory.CreateArrayParameter(id, arrayDefinition.Subtype.Datatype, arrayDefinition.Length);
                //        break;
                //    }

                default:
                    {
                        parameter = (Parameter)ParameterFactory.CreateParameter(id, datatype, null);
                        parameter.TypeDefinition.ParseOptions(input);
                        break;
                    }
            }

            parameter.ParseOptions(input);
            return parameter;
        }

        protected virtual bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            return false;
        }

        private void ParseOptions(KaitaiStream input)
        {
            // get options from the stream
            while (true)
            {
                var code = input.ReadU1();
                if (code == 0)
                    break;

                var option = (RcpTypes.ParameterOptions)code;
                if (!Enum.IsDefined(typeof(RcpTypes.ParameterOptions), option))
                    throw new RCPDataErrorException("Parameter parsing: Unknown option: " + option.ToString());

                switch (option)
                {
                    case RcpTypes.ParameterOptions.Label:
                        Label = new RcpTypes.TinyString(input).Data;
                        break;

                    case RcpTypes.ParameterOptions.Description:
                        Description = new RcpTypes.ShortString(input).Data;
                        break;

                    case RcpTypes.ParameterOptions.Tags:
                        Tags = new RcpTypes.TinyString(input).Data;
                        break;

                    case RcpTypes.ParameterOptions.Order:
                        Order = input.ReadS4be();
                        break;

                    case RcpTypes.ParameterOptions.Parentid:
                        ParentId = input.ReadS2be();
                        break;

                    case RcpTypes.ParameterOptions.Widget:
                        Widget = Widget.Parse(input);
                        break;

                    case RcpTypes.ParameterOptions.Userdata:
                        Userdata = new RcpTypes.Userdata(input).Data;
                        break;

                    case RcpTypes.ParameterOptions.Userid:
                        UserId = new RcpTypes.TinyString(input).Data;
                        break;

                    case RcpTypes.ParameterOptions.Value:
                    default:
                        if (!HandleOption(input, option))
                        {
                            throw new RCPUnsupportedFeatureException();
                        }
                        break;
                }
            }
        }
    }

    public abstract class ValueParameter<T> : Parameter, IValueParameter<T>
    {
        public T Value { get; set; }

        public ValueParameter(Int16 id, IDefaultDefinition<T> typeDefinition, IManager manager) : 
            base (id, typeDefinition, manager)
        { }

        protected override void WriteValue(BinaryWriter writer)
        {
            if (Value != null)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Value);
                ((IDefaultDefinition<T>)TypeDefinition).WriteValue(writer, Value);
            }
        }
    }
}
