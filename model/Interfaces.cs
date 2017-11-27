using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace RCP.Model
{
    public interface IRCPServer
    {
        void UpdateValue();
        void UpdateParameter();
    }

    public interface IWriteable
    {
        void Write(BinaryWriter writer);
    }

    public interface IParameter: IWriteable
    {
        uint Id { get; }
        ITypeDefinition TypeDefinition { get; }
        string Label { get; set; }
        string Description { get; set; }
        int? Order { get; set; }
        uint? Parent { get; set; }
        //IWidget Widget { get; set; }
        byte[] Userdata { get; set; }
    }

    public interface IValueParameter<T>: IParameter
    {
        T Value { get; set; }
    }

    public interface ITypeDefinition: IWriteable
    {
        RcpTypes.Datatype Datatype { get; }
        void ParseOptions(Kaitai.KaitaiStream input);
    }

    public interface IDefaultDefinition<T>: ITypeDefinition
    {
        T Default { get; set; }
        T ReadValue(Kaitai.KaitaiStream input);
        void WriteValue(BinaryWriter writer, T value);
    }

    public interface IArrayDefinition<T> : IDefaultDefinition<T>
    {
        new List<T> ReadValue(Kaitai.KaitaiStream input);
    }

    public interface IBooleanDefinition : IDefaultDefinition<bool>
    {
    }

    public interface IStringDefinition : IDefaultDefinition<string>
    {
    }

    public interface IRGBADefinition : IDefaultDefinition<Color>
    {
    }

    public interface INumberDefinition<T>: IDefaultDefinition<T> where T: struct
    {
        Nullable<T> Minimum { get; set; }
        Nullable<T> Maximum { get; set; }
        Nullable<T> MultipleOf { get; set; }
        //scale
        //unit
    }

    public interface INumberParameter<T>: IValueParameter<T> where T : struct
    {
        INumberDefinition<T> NumberDefinition { get; }
    }

    public interface IStringParameter : IValueParameter<string>
    {
        IStringDefinition StringDefinition { get; }
    }

    public interface IGroupParameter: IParameter
    {

    }
}