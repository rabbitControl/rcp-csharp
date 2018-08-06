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
        void CopyTo(ITypeDefinition other);
    }

    public interface IDefaultDefinition<T> : ITypeDefinition
    {
        T Default { get; set; }
        T ReadValue(Kaitai.KaitaiStream input);
        void WriteValue(BinaryWriter writer, T value);
    }

    public interface IBoolDefinition : IDefaultDefinition<bool>
    {
    }

    public interface INumberDefinition<T> : IDefaultDefinition<T> where T : struct
    {
        T Minimum { get; set; }
        T Maximum { get; set; }
        T MultipleOf { get; set; }
        RcpTypes.NumberScale Scale { get; set; }
        string Unit { get; set; }
    }

    public interface IStringDefinition : IDefaultDefinition<string>
    {
    }

    public interface IRGBADefinition : IDefaultDefinition<Color>
    {
    }

    public interface IUriDefinition : IDefaultDefinition<string>
    {
        string Schema { get; set; }
        string Filter { get; set; }
    }

    public interface IEnumDefinition : IDefaultDefinition<string>
    {
        string[] Entries { get; set; }
    }

    public interface IArrayDefinition : ITypeDefinition
    {
        RcpTypes.Datatype ElementType { get; }
        int[] Structure { get; set; }
    }

    public interface IParameter : IWriteable
    {
        Int16 Id { get; }
        ITypeDefinition TypeDefinition { get; }
        string Label { get; set; }
        string Description { get; set; }
        string Tags { get; set; }
        int Order { get; set; }
        Int16 ParentId { get; }
        Widget Widget { get; set; }
        byte[] Userdata { get; set; }
        string UserId { get; set; }

        event EventHandler Updated;
    }

    public interface IValueParameter<T>: IParameter
    {
        T Value { get; set; }
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
