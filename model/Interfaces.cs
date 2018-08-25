using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using RCP.Protocol;

namespace RCP
{
    public interface IWriteable
    {
        void Write(BinaryWriter writer);
    }

    public interface ITypeDefinition : IWriteable
    {
        RcpTypes.Datatype Datatype { get; }
        void ParseOptions(Kaitai.KaitaiStream input);
        void ResetForInitialize();
        bool AnyChanged();
        void CopyFrom(ITypeDefinition other);
    }

    public interface IDefaultDefinition<T> : ITypeDefinition
    {
        T Default { get; set; }
        bool DefaultChanged { get; }
        T ReadValue(Kaitai.KaitaiStream input);
        void WriteValue(BinaryWriter writer, T value);
    }

    public interface IBoolDefinition : IDefaultDefinition<bool>
    {
    }

    public interface INumberDefinition<T> : IDefaultDefinition<T> where T : struct
    {
        T Minimum { get; set; }
        bool MinimumChanged { get; }
        T Maximum { get; set; }
        bool MaximumChanged { get; }
        T MultipleOf { get; set; }
        bool MultipleOfChanged { get; }
        RcpTypes.NumberScale Scale { get; set; }
        bool ScaleChanged { get; }
        string Unit { get; set; }
        bool UnitChanged { get; }
    }

    public interface IStringDefinition : IDefaultDefinition<string>
    {
        string RegularExpression { get; set; }
        bool RegularExpressionChanged { get; }
    }

    public interface IRGBADefinition : IDefaultDefinition<Color>
    {
    }

    public interface IUriDefinition : IDefaultDefinition<string>
    {
        string Schema { get; set; }
        bool SchemaChanged { get; }
        string Filter { get; set; }
        bool FilterChanged { get; }
    }

    public interface IEnumDefinition : IDefaultDefinition<string>
    {
        string[] Entries { get; set; }
        bool EntriesChanged { get; }
        bool MultiSelect { get; set; }
        bool MultiSelectChanged { get; }
    }

    public interface IArrayDefinition : ITypeDefinition
    {
        ITypeDefinition ElementDefinition { get; }
        RcpTypes.Datatype ElementType { get; }
        int[] Structure { get; set; }
    }

    public interface IParameter : IWriteable
    {
        Int16 Id { get; }
        ITypeDefinition TypeDefinition { get; }
        string Label { get; set; }
        bool LabelChanged { get; }
        string Description { get; set; }
        bool DescriptionChanged { get; }
        string Tags { get; set; }
        bool TagsChanged { get; }
        int Order { get; set; }
        bool OrderChanged { get; }
        Int16 ParentId { get; }
        bool ParentIdChanged { get; }
        Widget Widget { get; set; }
        bool WidgetChanged { get; }
        byte[] Userdata { get; set; }
        bool UserdataChanged { get; }
        string UserId { get; set; }
        bool UserIdChanged { get; }

        bool AnyChanged { get; }
        event EventHandler Updated;
    }

    public interface IValueParameter<T>: IParameter
    {
        T Value { get; set; }
        bool ValueChanged { get; }
        T Default { get; set; }
        event EventHandler<T> ValueUpdated;
    }

    public interface IBooleanParameter : IValueParameter<bool>
    {
    }

    public interface INumberParameter<T> : IValueParameter<T> where T : struct
    {
        T Minimum { get; set; }
        T Maximum { get; set; }
        T MultipleOf { get; set; }
        RcpTypes.NumberScale Scale { get; set; }
        string Unit { get; set; }
    }

    public interface IEnumParameter : IValueParameter<string>
    {
    	string[] Entries { get; set; }
    }

    public interface IStringParameter : IValueParameter<string>
    {
        string RegularExpression { get; set; }
    }

    public interface IUriParameter : IValueParameter<string>
    {
        string Schema { get; set; }
        string Filter { get; set; }
    }

    public interface IRGBAParameter : IValueParameter<Color>
    {
    }

    public interface IArrayParameter<T> : IValueParameter<T>
    {
    }

    public interface IBooleanArrayParameter : IArrayParameter<bool[]>
    {
    }

    public interface INumberArrayParameter<T, E> : IArrayParameter<T>
    {
    }

    public interface IStringArrayParameter : IArrayParameter<string[]> 
    {
    }

    public interface IEnumArrayParameter : IArrayParameter<string[]>
    {
    }

    public interface IRGBAArrayParameter : IArrayParameter<Color[]>
    {
    }

    public interface IUriArrayParameter : IArrayParameter<string[]>
    {
    }

    public interface IGroupParameter: IParameter
    {
    }
}
