// (c) Vasian Cepa 2004
// http://madebits.com
// compile as:
// csc aes-example.cs /r:aes.dll
using System;
using System.IO;

using sc;
using aes;

public class AesExample
{
	public static void Main(string[] args)
	{
		try
		{
		
		// sorry for the politically biased example
		string input = "Long live George Bush Junior!";
		string password = "transparency";
		
		// context
		int keySize = 32; // = bits256, 24 = bits192, 16 = bits128
				
		IBlockCipher ibc = AesFactory.GetAes();
				
		// set key paramters, fixed in this example
		byte[] iv = new byte[ibc.BlockSizeInBytes()]; // 16 for Aes
		for(int i = 0; i < iv.Length; ++i) iv[i] = 0;
		byte[] salt = new byte[]{0, 0, 0, 0, 0, 0, 0, 0};
		int iterationCount = 1024;
		
		long start = DateTime.Now.Ticks;
		byte[] key = KeyGen.DeriveKey(password, keySize, salt, iterationCount);
		start = DateTime.Now.Ticks - start;
		TimeSpan ts1 = new TimeSpan(start);
		Console.WriteLine("Key generation took: " + start + " = " + ts1.TotalSeconds + "s = " + ts1.TotalMilliseconds + "ms");
		StreamCtx _aes = StreamCipher.MakeStreamCtx(ibc, key, iv);


		//#1 byte (string) encode
		
		Log("Original:         " + input);
		
		byte[] inbuff = System.Text.Encoding.UTF8.GetBytes(input);
		
		// if ascii is all that is required then no need for System.Text at all
		// byte[] inbuff = AesProvider.ASCIIEncoder(input);
		
		byte[] outbuff = StreamCipher.Encode(_aes, inbuff, StreamCipher.ENCRYPT);
		
		Log("Coded:            " + System.Text.Encoding.UTF8.GetString(outbuff));
		
		inbuff = StreamCipher.Encode(_aes, outbuff, StreamCipher.DECRYPT);
		
		Log("Back to original: " + System.Text.Encoding.UTF8.GetString(inbuff));
		
		//#2 file encode
		
		//create a test file
		string inFileName = "original.test";
		CreateTestFile(inFileName, input);
		
		// & now action
		FileStream instr = new FileStream(inFileName, FileMode.Open);
		FileStream outstr = new FileStream(inFileName + ".coded", FileMode.Create);
		
		StreamCipher.Encrypt(_aes, instr, outstr);
		
		instr.Close();
		
		FileStream outstr2 = new FileStream("copy of " + inFileName, FileMode.Create);
		outstr.Seek(0, SeekOrigin.Begin);
		StreamCipher.Decrypt(_aes, outstr, outstr2);
		outstr2.Close();
		outstr.Close();
		
		// speed test
		Console.WriteLine("Speed Test!");
		inbuff = new byte[16] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff };
		int MAX = 10000;
		start = DateTime.Now.Ticks;
		for(int i = 0; i < MAX; ++i)
		{
			outbuff = StreamCipher.Encode(_aes, inbuff, StreamCipher.ENCRYPT);
			inbuff = StreamCipher.Encode(_aes, outbuff, StreamCipher.DECRYPT);
		}
		start = DateTime.Now.Ticks - start;
		TimeSpan ts = new TimeSpan(start);
		Console.WriteLine("Avg Time / Block " + (start / MAX) + " = " + ts.TotalSeconds + "s = " + ts.TotalMilliseconds + "ms");
		}catch(Exception ex)
		{
			System.Console.WriteLine(ex.Message + "\n"  + ex.StackTrace);
		}
	}
	
	private static void Log(string s)
	{
		if(s != null) Console.WriteLine(s);
	}
	
	private static void CreateTestFile(string name, string data)
	{
		StreamWriter sw = new StreamWriter(name);
		sw.WriteLine(data);
		sw.Close();
		Log("Test file \"" + name + "\" created!");
	}
	
}