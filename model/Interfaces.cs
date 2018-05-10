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

    public interface IParameter : IWriteable
    {
        Int16 Id { get; }
        RcpTypes.Datatype Datatype { get; }
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

    public interface IArrayParameter<T> : IValueParameter<T>
    {
        //new List<T> ReadValue(Kaitai.KaitaiStream input);
    }

    public interface IBooleanParameter : IValueParameter<bool>
    {
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

    public interface INumberParameter<T> : IValueParameter<T> where T : struct
    {
        T Minimum { get; set; }
        T Maximum { get; set; }
        T MultipleOf { get; set; }
        RcpTypes.NumberScale Scale { get; set; }
        string Unit { get; set; }
    }

    public interface IGroupParameter: IParameter
    {
    }
}
