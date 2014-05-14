using System;
using System.Security.Cryptography;
using System.Text;

namespace uWebshop.Domain.Helpers
{
	/// <summary>
	///     Helper class with encryption related functions
	/// </summary>
	public static class CryptographyHelper
	{
		private static string key = "QNHMKh4HTJnTxzDsorGvL5IZxfPgvagA";
		private static string IV = "21Z8CmgIDQEB9Khm7fs8aw==";

		/// <summary>
		///     Encrypts a string with Rijndael
		/// </summary>
		/// <param name="plainText">String to encrypt</param>
		/// <returns>Encrypted string</returns>
		public static string Encrypt(string plainText)
		{
			var rijndael = new RijndaelManaged();
			rijndael.Key = Convert.FromBase64String(key);
			rijndael.IV = Convert.FromBase64String(IV);

			byte[] buffer = Encoding.UTF8.GetBytes(plainText);

			return Convert.ToBase64String(rijndael.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));
		}

		/// <summary>
		///     Decrypts a Rijndael encrypted string
		/// </summary>
		/// <returns>Decrypted string</returns>
		public static string Decrypt(string encryptedText)
		{
			var rijndael = new RijndaelManaged();
			rijndael.Key = Convert.FromBase64String(key);
			rijndael.IV = Convert.FromBase64String(IV);

			byte[] buffer = Convert.FromBase64String(encryptedText);

			return Encoding.UTF8.GetString(rijndael.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
		}
	}
}