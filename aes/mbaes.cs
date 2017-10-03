// SAMPLE
// (c) Vasian Cepa - GPL
using System;
using System.IO;
using sc;
using aes;

public class CA
{
	public static int Main(string[] args)
	{
		string pass         = null;
		int    icount       = 1024;
		int    keySizeBytes = 16;
		string file         = null;
		bool   encrypt      = true;

		try
		{
			Log("Get Cr!pAES at ");
			if((args == null) || args.Length <= 0)
			{
				ShowHelp();
				return 1;
			}
			for(int i = 0; i < args.Length; ++i)
			{
				string arg = args[i].ToUpper();
				switch(arg)
				{
					case "-P": pass = args[++i].Trim(); break;
					case "-I": icount = Convert.ToInt32(args[++i]);
										 break;
					case "-A": switch(args[++i])
										 {
										   case "196": keySizeBytes = 24; break;
										   case "256": keySizeBytes = 32; break;
										   default: keySizeBytes = 16; break;
										 }
										 break;
					default: file = args[i]; break;
				}
			}
			if((pass == null) || (pass.Length <= 0)) throw new Exception("password must not be empty");
			if(icount < 1) throw new Exception("iteration count munst be > 0");
			if((file == null) || !File.Exists(file)) throw new Exception("input file missing or not found");
			file = Path.GetFullPath(file);
			if(file.ToUpper().EndsWith(".CR!PT")) encrypt = false;
			string outFile = (encrypt? file + ".cr!pt" : Path.GetFileNameWithoutExtension(file));
			if(File.Exists(outFile)) throw new Exception("output file exists: " + outFile);
			Log("| Mode            : " + (encrypt ? "encrypt" : "decrypt"));
			Log("| AES Key size    : " + keySizeBytes * 8);
			Log("| Iteration count : " + icount);
			Log("| Input           : " + file);
			Log("| Output          : " + outFile);
			Log("| Please wait ...");
			ProcessFile(file, outFile, pass, icount, keySizeBytes, encrypt);
			Log("| Done!");
		}
		catch(Exception ex)
		{
			Log("#Error: " + ex.Message);
			return 1;
		}
		return 0;
	}

	private static void Log(string s)
	{
		Console.WriteLine(s);
	}

	private static void ShowHelp()
	{
		Log("Usage: mbaes -p password [-i iterationCount] [-a keySize] file");
	}

	private static void ProcessFile(string file, string outFile, string password, int icount, int keySizeBytes, bool encrypt)
	{
			FileStream fin = null;
			FileStream fout = null;
			try
			{
				fin = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 0x1000);
				if(fin == null) throw new Exception("cannot read");
				if(fin.Length <= 0) throw new Exception("empty file");
				fout = new FileStream(outFile, FileMode.Create, FileAccess.Write, FileShare.Read, 0x1000);
				if(fout == null) throw new Exception("cannot write");
				byte[] iv = new byte[16];
				byte[] salt = new byte[16];
				StrongRandomGenerator rnd = new StrongRandomGenerator();
				if(encrypt)
				{
					rnd.NextBytes(iv);
					rnd.NextBytes(salt);
					fout.Write(iv, 0, iv.Length);
					fout.Write(salt, 0, salt.Length);
				}
				else
				{
					if(fin.Length <= (iv.Length + salt.Length))
					{
						throw new Exception("no data");
					}
					if(fin.Read(iv, 0, iv.Length) != iv.Length)
					{
						throw new Exception("no data");
					}
					if(fin.Read(salt, 0, salt.Length) != salt.Length)
					{
						throw new Exception("no data");
					}
				}
				byte[] key = KeyGen.DeriveKey(password, keySizeBytes, salt, icount);
				StreamCtx ctx = StreamCipher.MakeStreamCtx(aes.AesFactory.GetAes(), key, iv);
				if(encrypt)
				{
					StreamCipher.Encrypt(ctx, fin, fout);
				}
				else
				{
					StreamCipher.Decrypt(ctx, fin, fout);
				}
			}
			finally
			{
				if(fin != null) fin.Close();
				if(fout != null) fout.Close();
			}
	}
}