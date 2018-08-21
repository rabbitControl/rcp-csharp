using System;
using System.IO;

using Kaitai;
using RCP.Exceptions;
using RCP.Protocol;
using System.Collections.Generic;
using System.Text;

namespace RCP.Parameter
{
    public enum Status { Update, Remove };

    internal abstract class Parameter : IParameter, IWriteable
    {
        protected IParameterManager FManager;

        public event EventHandler Updated;

        public Int16 Id { get; private set; }

        public ITypeDefinition TypeDefinition { get; set; }

        public bool ParentIdChanged { get; private set; }
        private Int16 FParentId;
        public Int16 ParentId { get { return FParentId; } set { FParentId = value; ParentIdChanged = true; SetDirty(); } }

        public bool LabelChanged { get; private set; }
        private Dictionary<string, string> FLabels = new Dictionary<string, string>();
        public string Label { get { return FLabels.ContainsKey("any") ? FLabels["any"] : ""; } set { FLabels["any"] = value; LabelChanged = true; SetDirty(); } }

        public bool DescriptionChanged { get; private set; }
        private Dictionary<string, string> FDescriptions = new Dictionary<string, string>();
        public string Description { get { return FDescriptions.ContainsKey("any") ? FDescriptions["any"] : ""; } set { FDescriptions["any"] = value; DescriptionChanged = true; SetDirty(); } }

        public bool TagsChanged { get; private set; }
        private string FTags = "";
        public string Tags { get { return FTags; } set { FTags = value; TagsChanged = true; SetDirty(); } }

        public bool OrderChanged { get; private set; }
        private int FOrder;
        public int Order { get { return FOrder; } set { FOrder = value; OrderChanged = true; SetDirty(); } }

        public bool UserdataChanged { get; private set; }
        private byte[] FUserdata = new byte[0];
        public byte[] Userdata { get { return FUserdata; } set { FUserdata = value; UserdataChanged = true; SetDirty(); } }

        public bool UserIdChanged { get; private set; }
        private string FUserId = "";
        public string UserId { get { return FUserId; } set { FUserId = value; UserIdChanged = true; SetDirty(); } }

        public bool WidgetChanged { get; private set; }
        public Widget Widget { get; set; }

        public Parameter(Int16 id, IParameterManager manager)
        {
            Id = id;
            FManager = manager;

            SetDirty();
        }

        public void SetParent(IParameter param)
        {
            ParentId = param.Id;
        }

        protected void SetDirty()
        {
            if (FManager != null) //not assigned for temp-parameters (ie. those just used for parsing on clients)
                FManager.SetParameterDirty(this);
        }
        
        public void SetLanguageLabel(string iso639_3, string label)
        {
            FLabels[iso639_3] = label;
            LabelChanged = true;
        }

        public void RemoveLanguageLabel(string iso639_3)
        {
            if (FLabels.ContainsKey(iso639_3))
            {
                FLabels.Remove(iso639_3);
                LabelChanged = true;
            }
        }

        public void SetLanguageDescription(string iso639_3, string description)
        {
            FDescriptions[iso639_3] = description;
            DescriptionChanged = true;
        }

        public void RemoveLanguageDescription(string iso639_3)
        {
            if (FDescriptions.ContainsKey(iso639_3))
            {
                FDescriptions.Remove(iso639_3);
                DescriptionChanged = true;
            }
        }

        public void Write(BinaryWriter writer)
        {
            //mandatory
            writer.Write(Id, ByteOrder.BigEndian);

            TypeDefinition.Write(writer);

            //optional
            WriteValue(writer);

            if (LabelChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Label);
                foreach (var language in FLabels.Keys)
                {
                    writer.Write(Encoding.ASCII.GetBytes(language));
                    RcpTypes.TinyString.Write(FLabels[language], writer);
                }
                writer.Write((byte)0);
                LabelChanged = false;
            }

            if (DescriptionChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Description);
                foreach (var language in FDescriptions.Keys)
                {
                    writer.Write(Encoding.ASCII.GetBytes(language));
                    RcpTypes.ShortString.Write(FDescriptions[language], writer);
                }
                writer.Write((byte)0);
                DescriptionChanged = false;
            }

            if (TagsChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Tags);
                RcpTypes.TinyString.Write(Tags, writer);
                TagsChanged = false;
            }

            if (OrderChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Order);
                writer.Write(Order, ByteOrder.BigEndian);
                OrderChanged = false;
            }

            if (ParentIdChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Parentid);
                writer.Write(ParentId, ByteOrder.BigEndian);
                ParentIdChanged = false;
            }

            if (Widget != null)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Widget);
                Widget.Write(writer);
            }

            if (UserdataChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Userdata);
                writer.Write(Userdata.Length, ByteOrder.BigEndian);
                writer.Write(Userdata);
                UserdataChanged = false;
            }

            if (UserIdChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Userid);
                RcpTypes.TinyString.Write(UserId, writer);
                UserIdChanged = false;
            }

            //terminate
            writer.Write((byte)0);
        }

        protected virtual void WriteTypeDefinitionOptions(BinaryWriter writer) { }
        protected virtual void ParseTypeDefinitionOptions(KaitaiStream input) { }
        protected virtual void WriteValue(BinaryWriter writer) { }

        public static Parameter Parse(KaitaiStream input, IParameterManager manager)
        {
            // get mandatory id
            var id = input.ReadS2be();

            var datatype = (RcpTypes.Datatype)input.ReadU1();
            if (!Enum.IsDefined(typeof(RcpTypes.Datatype), datatype))
                throw new RCPDataErrorException("Parameter parsing: Unknown datatype!");

            Parameter parameter = null;

            switch (datatype)
            {
                case RcpTypes.Datatype.Array:
                    {
                        //parse element type definition
                        //create array param
                        //set element type definition on array param
                        //create array definition
                        //parse array definition options
                        //set array definition on array param

                        var elementType = (RcpTypes.Datatype)input.ReadU1();
                        parameter = (Parameter)RCPClient.CreateArrayParameter(id, elementType, manager);
                        parameter.TypeDefinition.ParseOptions(input);
                        break;
                    }

                default:
                    {
                        parameter = (Parameter)RCPClient.CreateParameter(id, datatype, manager);
                        parameter.TypeDefinition.ParseOptions(input);
                        break;
                    }
            }

            parameter.ParseOptions(input);
            return parameter;
        }

        protected virtual bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions code)
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
                        while (input.PeekChar() > 0)
                        {
                            var language = new string(input.ReadChars(3));
                            FLabels.Add(language, new RcpTypes.TinyString(input).Data);
                            LabelChanged = true;
                            SetDirty();
                        }
                        input.ReadByte(); //0 terminator
                        break;

                    case RcpTypes.ParameterOptions.Description:
                        while (input.PeekChar() > 0)
                        {
                            var language = new string(input.ReadChars(3));
                            FDescriptions.Add(language, new RcpTypes.ShortString(input).Data);
                            DescriptionChanged = true;
                            SetDirty();
                        }
                        input.ReadByte(); //0 terminator
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

                    default:
                        if (!HandleOption(input, option))
                        {
                            throw new RCPUnsupportedFeatureException();
                        }
                        break;
                }
            }
        }

        public virtual void CopyFrom(IParameter other)
        {
            TypeDefinition.CopyFrom(other.TypeDefinition);

            if (other.ParentIdChanged)
                FParentId = other.ParentId;

            //TODO: language specific copy
            if (other.LabelChanged)
                FLabels["any"] = other.Label;

            if (other.DescriptionChanged)
                FDescriptions["any"] = other.Description;

            if (other.TagsChanged)
                FTags = other.Tags;

            if (other.OrderChanged)
                FOrder = other.Order;

            if (other.UserdataChanged)
                FUserdata = other.Userdata;

            if (other.UserIdChanged)
                FUserId = other.UserId;

            if (other.AnyChanged)
                Updated?.Invoke(this, null);
        }

        public virtual bool AnyChanged => TypeDefinition.AnyChanged() || ParentIdChanged || LabelChanged || DescriptionChanged || TagsChanged || OrderChanged || UserdataChanged || UserIdChanged;

        public virtual void ResetForInitialize()
        {
            ParentIdChanged = FParentId != 0;
            LabelChanged = FLabels.Count > 0;
            DescriptionChanged = FDescriptions.Count > 0;
            TagsChanged = FTags != "";
            OrderChanged = FOrder != 0;
            UserdataChanged = FUserdata.Length != 0;
            UserIdChanged = FUserId != "";

            TypeDefinition.ResetForInitialize();
        }
    }

    internal abstract class ValueParameter<T> : Parameter, IValueParameter<T>
    {
        public IDefaultDefinition<T> DefaultDefinition => TypeDefinition as IDefaultDefinition<T>;
        public T Default { get { return DefaultDefinition.Default; } set { DefaultDefinition.Default = value; SetDirty(); } }

        public event EventHandler<T> ValueUpdated;
        public bool ValueChanged { get; protected set; }
        protected T FValue;
        public T Value { get { return FValue; } set { FValue = value; ValueChanged = true; SetDirty(); } }

        public ValueParameter(Int16 id, IParameterManager manager) : 
            base (id, manager)
        { }

        public override bool AnyChanged => base.AnyChanged || ValueChanged;

        public override void CopyFrom(IParameter other)
        {
            var otherValue = other as ValueParameter<T>;
            if (otherValue.ValueChanged)
            {
                FValue = otherValue.Value;
                ValueUpdated?.Invoke(other, FValue);
            }

            //last, because this also fires the Update event
            base.CopyFrom(other);
        }
    }
}
