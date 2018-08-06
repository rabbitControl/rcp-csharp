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
        private Dictionary<string, string> FLabels = new Dictionary<string, string>();
        private Dictionary<string, string> FDescriptions = new Dictionary<string, string>();

        protected IParameterManager FManager;

        public event EventHandler Updated;

        public Int16 Id { get; private set; }

        public ITypeDefinition TypeDefinition { get; set; }

        private bool FParentIdChanged;
        private Int16 FParentId;
        public Int16 ParentId { get { return FParentId; } set { FParentId = value; FParentIdChanged = true; SetDirty(); } }

        private bool FLabelChanged;
        public string Label { get { return FLabels.ContainsKey("any") ? FLabels["any"] : ""; } set { FLabels["any"] = value; FLabelChanged = true; SetDirty(); } }

        private bool FDescriptionChanged;
        public string Description { get { return FDescriptions.ContainsKey("any") ? FDescriptions["any"] : ""; } set { FDescriptions["any"] = value; FDescriptionChanged = true; SetDirty(); } }

        private bool FTagsChanged;
        private string FTags = "";
        public string Tags { get { return FTags; } set { FTags = value; FTagsChanged = true; SetDirty(); } }

        private bool FOrderChanged;
        private int FOrder;
        public int Order { get { return FOrder; } set { FOrder = value; FOrderChanged = true; SetDirty(); } }

        private bool FUserdataChanged;
        private byte[] FUserdata = new byte[0];
        public byte[] Userdata { get { return FUserdata; } set { FUserdata = value; FUserdataChanged = true; SetDirty(); } }

        private bool FUserIdChanged;
        private string FUserId = "";
        public string UserId { get { return FUserId; } set { FUserId = value; FUserIdChanged = true; SetDirty(); } }

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
            FLabelChanged = true;
        }

        public void RemoveLanguageLabel(string iso639_3)
        {
            if (FLabels.ContainsKey(iso639_3))
            {
                FLabels.Remove(iso639_3);
                FLabelChanged = true;
            }
        }

        public void SetLanguageDescription(string iso639_3, string description)
        {
            FDescriptions[iso639_3] = description;
            FDescriptionChanged = true;
        }

        public void RemoveLanguageDescription(string iso639_3)
        {
            if (FDescriptions.ContainsKey(iso639_3))
            {
                FDescriptions.Remove(iso639_3);
                FDescriptionChanged = true;
            }
        }

        public void Write(BinaryWriter writer)
        {
            //mandatory
            writer.Write(Id, ByteOrder.BigEndian);

            TypeDefinition.Write(writer);

            //optional
            WriteValue(writer);

            if (FLabelChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Label);
                foreach (var language in FLabels.Keys)
                {
                    writer.Write(Encoding.ASCII.GetBytes(language));
                    RcpTypes.TinyString.Write(FLabels[language], writer);
                }
                writer.Write((byte)0);
                FLabelChanged = false;
            }

            if (FDescriptionChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Description);
                foreach (var language in FDescriptions.Keys)
                {
                    writer.Write(Encoding.ASCII.GetBytes(language));
                    RcpTypes.ShortString.Write(FDescriptions[language], writer);
                }
                writer.Write((byte)0);
                FDescriptionChanged = false;
            }

            if (FTagsChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Tags);
                RcpTypes.TinyString.Write(Tags, writer);
                FTagsChanged = false;
            }

            if (FOrderChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Order);
                writer.Write(Order, ByteOrder.BigEndian);
                FOrderChanged = false;
            }

            if (FParentIdChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Parentid);
                writer.Write(ParentId, ByteOrder.BigEndian);
                FParentIdChanged = false;
            }

            if (Widget != null)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Widget);
                Widget.Write(writer);
            }

            if (FUserdataChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Userdata);
                writer.Write(Userdata.Length, ByteOrder.BigEndian);
                writer.Write(Userdata);
                FUserdataChanged = false;
            }

            if (FUserIdChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Userid);
                RcpTypes.TinyString.Write(UserId, writer);
                FUserIdChanged = false;
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
                        }
                        input.ReadByte(); //0 terminator
                        break;

                    case RcpTypes.ParameterOptions.Description:
                        while (input.PeekChar() > 0)
                        {
                            var language = new string(input.ReadChars(3));
                            FDescriptions.Add(language, new RcpTypes.ShortString(input).Data);
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

        public virtual void CopyTo(IParameter other)
        {
            TypeDefinition.CopyTo(other.TypeDefinition);

            //if (FParentIdChanged)
            //    other.ParentId = FParentId;

            //TODO: language specific copy
            //if (FLabelChanged)
            //    other.Label = FLabel;

            //if (FDescriptionChanged)
            //    other.Description = FDescription;

            if (FTagsChanged)
                other.Tags = FTags;

            if (FOrderChanged)
                other.Order = FOrder;

            if (FUserdataChanged)
                other.Userdata = FUserdata;

            if (FUserIdChanged)
                other.UserId = FUserId;

            if (AnyChanged())
                (other as Parameter).Updated?.Invoke(other, null);
        }

        protected virtual bool AnyChanged()
        {
            return TypeDefinition.AnyChanged() || FParentIdChanged || FLabelChanged || FDescriptionChanged || FTagsChanged || FOrderChanged || FUserdataChanged || FUserIdChanged;
        }

        public virtual void ResetForInitialize()
        {
            FParentIdChanged = FParentId != 0;
            FLabelChanged = FLabels.Count > 0;
            FDescriptionChanged = FDescriptions.Count > 0;
            FTagsChanged = FTags != "";
            FOrderChanged = FOrder != 0;
            FUserdataChanged = FUserdata.Length != 0;
            FUserIdChanged = FUserId != "";

            TypeDefinition.ResetForInitialize();
        }
    }

    internal abstract class ValueParameter<T> : Parameter, IValueParameter<T>
    {
        public IDefaultDefinition<T> DefaultDefinition => TypeDefinition as IDefaultDefinition<T>;
        public T Default { get { return DefaultDefinition.Default; } set { DefaultDefinition.Default = value; SetDirty(); } }

        public event EventHandler<T> ValueUpdated;
        protected bool FValueChanged;
        protected T FValue;
        public T Value { get { return FValue; } set { FValue = value; FValueChanged = true; SetDirty(); } }

        public ValueParameter(Int16 id, IParameterManager manager) : 
            base (id, manager)
        { }

        protected override bool AnyChanged()
        {
            return base.AnyChanged() || FValueChanged;
        }

        public override void CopyTo(IParameter other)
        {
            if (FValueChanged)
            {
                var valueParameter = other as ValueParameter<T>;
                valueParameter.Value = FValue;
                valueParameter.ValueUpdated?.Invoke(other, FValue);
            }

            //last, because this also fires the Update event
            base.CopyTo(other);
        }
    }
}
