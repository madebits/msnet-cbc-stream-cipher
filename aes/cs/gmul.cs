//
using System;

public class GMul
{

	public delegate byte gfmuldel(byte b);
	
	public static void Main()
	{
		Console.WriteLine("#region gfmultby\r\n");
		
		// not needed
		//GenFunction("gfmultby01", new gfmuldel(gfmultby01));
		
		GenFunction("gfmultby02", new gfmuldel(gfmultby02));
		GenFunction("gfmultby03", new gfmuldel(gfmultby03));
		
		GenFunction("gfmultby09", new gfmuldel(gfmultby09));
		GenFunction("gfmultby0b", new gfmuldel(gfmultby0b));
		GenFunction("gfmultby0d", new gfmuldel(gfmultby0d));
		GenFunction("gfmultby0e", new gfmuldel(gfmultby0e));
		
		Console.WriteLine("#endregion gfmultby");
	}

    
    
    private static void GenFunction(string name, gfmuldel f)
    {
    	Console.Write("private static readonly byte[] " + name + " = new byte[256] {\r\n\t");
    	for(int b = 0; b < 256; b++)
    	{
    		byte r = f((byte)b);
    		string rr = r.ToString("x");
    		Console.Write("0x" + (rr.Length < 2 ? "0" : string.Empty) + rr);
    		if(b != 255) Console.Write(", ");
    		if(((b + 1) % 16 == 0)) Console.Write("\r\n\t");
    	}
    	Console.Write("};\r\n\r\n");
    }
    
    private static byte gfmultby01(byte b)
    {
          return b;
    }
    
    private static byte gfmultby02(byte b)
    {
      if (b < 0x80)
        return (byte)(int)(b << 1);
      else
        return (byte)( (int)(b << 1) ^ (int)(0x1b) );
    }

    private static byte gfmultby03(byte b)
    {
      return (byte) ( (int)gfmultby02(b) ^ (int)b );
    }

    private static byte gfmultby09(byte b)
    {
      return (byte)( (int)gfmultby02(gfmultby02(gfmultby02(b))) ^
                     (int)b );
    }

    private static byte gfmultby0b(byte b)
    {
      return (byte)( (int)gfmultby02(gfmultby02(gfmultby02(b))) ^
                     (int)gfmultby02(b) ^
                     (int)b );
    }

    private static byte gfmultby0d(byte b)
    {
      return (byte)( (int)gfmultby02(gfmultby02(gfmultby02(b))) ^
                     (int)gfmultby02(gfmultby02(b)) ^
                     (int)(b) );
    }

    private static byte gfmultby0e(byte b)
    {
      return (byte)( (int)gfmultby02(gfmultby02(gfmultby02(b))) ^
                     (int)gfmultby02(gfmultby02(b)) ^
                     (int)gfmultby02(b) );
    }

}//EOC