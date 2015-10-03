using System;
using sc;

namespace aes
{
	public sealed class AesFactory
	{
		private AesFactory()
		{}

		public static IBlockCipher GetAes()
		{
			return new CAes();
		}
	}
}
