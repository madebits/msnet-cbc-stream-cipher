using System;
using sc;
using aes;

// AES TEST
// csc aes-test.cs /r:aes.dll
public class AesTest
{
	public static void Main(string[] args)
	{
		byte[] key = new byte[32];
		byte[] inb = new byte[16];
		byte[] outb = new byte[16];
		
		for(int i = 0; i < 16; i++)
		{
			inb[i] = key[i] = key[i * 2] = 0; 
		}
		PB("Key", key);
		
		IBlockCipher _aes = AesFactory.GetAes();
		_aes.InitCipher(key);
		
		PB("Original", inb);
		_aes.Cipher(inb, outb);
		PB("Encrypted", outb);
		_aes.InvCipher(outb, inb);
		PB("Decrypted", inb);
		
		
		// FIPS-197
		// 256 bit
		// PLAINTEXT: 00112233445566778899aabbccddeeff
		// KEY: 000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f
		// ENCRYPTED: 8ea2b7ca516745bfeafc49904b496089
		
		inb = new byte[16] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff };
		key = new byte[32] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f,
				       0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f };
				       
		_aes = AesFactory.GetAes();
		_aes.InitCipher(key);
		_aes.Cipher(inb, outb);
		PB("FIPS-197 Encrypted [8ea2b7ca516745bfeafc49904b496089]\n", outb);
		
		
		// speed test
		int MAX = 10000;
		long start = DateTime.Now.Ticks;
		for(int i = 0; i < MAX; ++i)
		{
			_aes.Cipher(inb, outb);
			_aes.InvCipher(outb, inb);
		}
		start = DateTime.Now.Ticks - start;
		TimeSpan ts = new TimeSpan(start);
		Console.WriteLine("Avg Time / Block " + (start / MAX) + " = " + ts.TotalSeconds + "s = " + ts.TotalMilliseconds + "ms");
	}
	
	public static void PB(string s, byte[] b)
	{
		Console.Write(s + " (" + b.Length + ") -> ");
		for(int i = 0; i < b.Length; ++i) Console.Write(b[i].ToString("x") + ", ");
		Console.WriteLine("");
	}
	

}