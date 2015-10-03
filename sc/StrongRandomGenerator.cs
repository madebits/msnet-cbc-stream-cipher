using System;
using System.Collections;

namespace sc
{
	/// <summary>
	/// StrongRandomGenerator rnd = new StrongRandomGenerator();
	/// byte[] buff = new byte[...];
	/// rnd.NextByte(buff);
	/// </summary>
	
	public sealed class StrongRandomGenerator
	{
		private byte[] digest = null;
		public static readonly int DIGLEN = 32; //SHA256

		public StrongRandomGenerator()
		{
			SetSeed(null);
		}

		public StrongRandomGenerator(byte[] seed)
		{
			SetSeed(seed);
		}

		public void NextBytes(byte[] buffer)
		{
			if((buffer == null) || (buffer.Length <=0 )) return;
			if(digest == null) return;
			int bufferLen = DIGLEN / 2; // required
			int full = buffer.Length / bufferLen;
			int rem = buffer.Length % bufferLen;
			int i = 0;
			for(; i < full; ++i)
			{
				NextDigest();
				Array.Copy(digest, 0, buffer, i * bufferLen, bufferLen);
			}
			if(rem > 0)
			{
				NextDigest();
				Array.Copy(digest, 0, buffer, i * bufferLen, rem);
			}
		}

		private void NextDigest()
		{
			byte[] temp = null;
			while(true)
			{
				temp = SHA256.MessageSHA256(digest);
				if(!SameDigest(temp, digest))
				{
					digest = temp;
					return;
				}
				else // special case, should not happen, reset
				{
					digest = GetDefaultSeed(); 
				}
			}
		}

		private static bool SameDigest(byte[] d1, byte[] d2)
		{
			for(int i = 0; i < DIGLEN; ++i)
			{
				if(d1[i] != d2[i]) return false;
			}
			return true;
		}

		private void SetSeed(byte[] seed)
		{
			if((seed != null) && (seed.Length > 0))
			{
				digest = seed;
			}
			else
			{
				digest = GetDefaultSeed();
			}
			digest = SHA256.MessageSHA256(digest);
		}

		#region defaultseed

		public static byte[] GetDefaultSeed()
		{
			byte[] seed = new byte[96 + DIGLEN];
			if(seed == null) return null;
			SetFromInt64(seed,  0, System.DateTime.Now.Ticks);
			SetFromInt64(seed,  8, Environment.WorkingSet);
			SetFromInt64(seed, 16, (System.Int64)Environment.TickCount);
			SetFromInt64(seed, 24, (System.Int64)AppDomain.GetCurrentThreadId());
			SetFromInt64(seed, 32, GC.GetTotalMemory(false));
			System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
			SetFromInt64(seed, 40, (System.Int64)(p.Threads.Count + p.Id));
			SetFromInt64(seed, 48, p.UserProcessorTime.Ticks);
			SetFromInt64(seed, 56, p.TotalProcessorTime.Ticks);
			SetFromInt64(seed, 64, p.Handle.ToInt64());
			SetFromInt64(seed, 72, (System.Int64)(p.HandleCount + p.PeakWorkingSet));
			SetFromInt64(seed, 80, (System.Int64)(p.NonpagedSystemMemorySize + p.PagedMemorySize + p.PagedSystemMemorySize));
			SetFromInt64(seed, 88, p.StartTime.Ticks);
			p.Close();
			p = null;
			System.Collections.IDictionary env = Environment.GetEnvironmentVariables();
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach(string s in env.Keys)
			{
				sb.Append(s).Append(env[s]);
			}
			SetFromString(seed, 96, sb.ToString());
			sb = null;
			return SHA256.MessageSHA256(seed);
		}

		private static void SetFromInt64(byte[] buff, int start, System.Int64 data)
		{
			for(int i = 0; i < 8; ++i)
			{
				buff[start + i] = (byte)(data >> i * 8);
			}
		}

		private static void SetFromString(byte[] buff, int start, string s)
		{
			byte[] temp = SHA256.MessageSHA256(System.Text.Encoding.ASCII.GetBytes(s));
			Array.Copy(temp, 0, buff, start, DIGLEN);
		}

		#endregion defaultseed
	}//EOC

}//EON