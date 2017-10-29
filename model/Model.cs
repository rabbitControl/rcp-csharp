using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using Kaitai;

namespace RCP.Model
{
	
//    public class RCPEnum : TypeDefinition
//    {
//        public uint Default { get; set; }
//        public string[] Entries { get; set; }
//
//        public RCPEnum(string[] entries)
//        : base("enum")
//        {
//            Entries = entries;
//        }
//    }
//
//    public class RCPArray : TypeDefinition
//    {
//        public TypeDefinition Subtype { get; set; }
//
//        public RCPArray(TypeDefinition subtype)
//        : base("array")
//        {
//        	Subtype = subtype;
//        }
//    }
	
    public class Widget
    {
        public string Type { get; set; }

        public Widget(string type)
        {
            Type = type;
        }
    }

    public enum ButtonBehavior 
	{ 
		Toggle, 
		Bang, 
		Press
	}
	
    public class RCPButton: Widget
    {
        public ButtonBehavior Behavior { get; set; }

        public RCPButton()
        : base("button")
        {
        }
    }
	
	public class RCPNumberBox<T>: Widget
    {
        public uint Precision { get; set; }
    	
        public T Stepsize { get; set; }
    	
        public bool Cyclic { get; set; }

        public RCPNumberBox()
        : base("numberbox")
        {
//        	if ((typeof(T) == typeof(float)) || typeof(T) == typeof(double))
//                Stepsize = (T) (object) 0.01;
//        	else
//        		Stepsize = (T) (object) 1;
        }
    }

	public static class Converter
	{
	    public static byte[] GetBytes(bool value)
	    {
	        return ReverseAsNeeded(BitConverter.GetBytes(value));
	    }
	    public static byte[] GetBytes(char value)
	    {
	        return ReverseAsNeeded(BitConverter.GetBytes(value));
	    }
	    public static byte[] GetBytes(double value)
	    {
	        return ReverseAsNeeded(BitConverter.GetBytes(value));
	    }
	    public static byte[] GetBytes(float value)
	    {
	        return ReverseAsNeeded(BitConverter.GetBytes(value));
	    }
	    public static byte[] GetBytes(int value)
	    {
	        return ReverseAsNeeded(BitConverter.GetBytes(value));
	    }
	    public static byte[] GetBytes(long value)
	    {
	        return ReverseAsNeeded(BitConverter.GetBytes(value));
	    }
	    public static byte[] GetBytes(short value)
	    {
	        return ReverseAsNeeded(BitConverter.GetBytes(value));
	    }
	    public static byte[] GetBytes(uint value)
	    {
	        return ReverseAsNeeded(BitConverter.GetBytes(value));
	    }
	    public static byte[] GetBytes(ulong value)
	    {
	        return ReverseAsNeeded(BitConverter.GetBytes(value));
	    }
	    public static byte[] GetBytes(ushort value)
	    {
	        return ReverseAsNeeded(BitConverter.GetBytes(value));
	    }
	
	    private static byte[] ReverseAsNeeded(byte[] bytes)
	    {
	        if (BitConverter.IsLittleEndian)
	        	return (byte[])bytes.Reverse().ToArray();    
	    	else
	    		return bytes;
	    }
	}
}