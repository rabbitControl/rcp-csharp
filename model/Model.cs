using System;
using System.Linq;

namespace RCP.Model
{
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