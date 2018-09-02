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
        Type ClrType { get; }
        void ParseOptions(Kaitai.KaitaiStream input);
        void ResetForInitialize();
        bool AnyChanged();
        void CopyFrom(ITypeDefinition other);
    }

    public interface INumberDefinition : ITypeDefinition
    {
        object Minimum { get; }
        object Maximum { get; }
        object MultipleOf { get; }
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

    public interface INumberDefinition<T> : IDefaultDefinition<T>, INumberDefinition where T : struct
    {
        new T Minimum { get; set; }
        bool MinimumChanged { get; }
        new T Maximum { get; set; }
        bool MaximumChanged { get; }
        new T MultipleOf { get; set; }
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

    public interface IRangeDefinition : ITypeDefinition
    {
        INumberDefinition ElementType { get; }
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

    public interface IValueParameter : IParameter
    {
        bool ValueChanged { get; }
        object Value { get; set; }
        object Default { get; set; }
        event EventHandler ValueUpdated;
    }

    public interface IValueParameter<T>: IValueParameter
    {
        new T Value { get; set; }
        new T Default { get; set; }
    }

    public interface IGroupParameter: IParameter
    {
    }

    public interface IRangeParameter : IValueParameter
    {
        new IRangeDefinition TypeDefinition { get; }
        object Lower { get; set; }
        object Upper { get; set; }
    }
}
