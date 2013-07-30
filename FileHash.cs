using System;
using System.Collections.Generic;
using System.Text;
//
//Author : JuLius Lucero Ramos, ECE, QA
//Email  : juliusLramos@hotmail.com
//
namespace QXplorer
{
    class FileHash
    {
      private static byte[] ConvertStringToByteArray(string data)
		{
			return(new System.Text.UnicodeEncoding()).GetBytes(data);
		}

		private static System.IO.FileStream GetFileStream(string pathName)
		{
			return(new System.IO.FileStream(pathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite));
		}

		public string GetSHA1Hash(string pathName)
		{
			string strResult = "";
			string strHashData = "";

			byte[] arrbytHashValue;
			System.IO.FileStream oFileStream = null;

			System.Security.Cryptography.SHA1CryptoServiceProvider oSHA1Hasher = new System.Security.Cryptography.SHA1CryptoServiceProvider();
         
			try
			{
				oFileStream = GetFileStream(pathName);
				arrbytHashValue = oSHA1Hasher.ComputeHash(oFileStream);
				oFileStream.Close();

				strHashData = System.BitConverter.ToString(arrbytHashValue);
				strHashData = strHashData.Replace("-", "");
				strResult = strHashData;
			}
			catch(System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message, "Error!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxDefaultButton.Button1);
			}

			return(strResult);
		}

		public string GetMD5Hash(string pathName)
		{
			string strResult = "";
			string strHashData = "";

			byte[] arrbytHashValue;
			System.IO.FileStream oFileStream = null;

			System.Security.Cryptography.MD5CryptoServiceProvider oMD5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();

			try
			{
				oFileStream = GetFileStream(pathName);
				arrbytHashValue = oMD5Hasher.ComputeHash(oFileStream);
                
				oFileStream.Close();

				strHashData = System.BitConverter.ToString(arrbytHashValue);
				strHashData = strHashData.Replace("-", "");
				strResult = strHashData;
			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message, "Error!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxDefaultButton.Button1);
			}

			return(strResult);
		}

        public string GetSHA256Hash(string pathName)
        {
            string strResult = "";
            string strHashData = "";

            byte[] arrbytHashValue;
            System.IO.FileStream oFileStream = null;

            System.Security.Cryptography.SHA256 oSHA256Hasher = new System.Security.Cryptography.SHA256Managed();

            try
            {
                oFileStream = GetFileStream(pathName);
                arrbytHashValue = oSHA256Hasher.ComputeHash(oFileStream);
                oFileStream.Close();

                strHashData = System.BitConverter.ToString(arrbytHashValue);
                strHashData = strHashData.Replace("-", "");
                strResult = strHashData;
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxDefaultButton.Button1);
            }

            return (strResult);
        }

        public string GetSHA384Hash(string pathName)
        {
            string strResult = "";
            string strHashData = "";

            byte[] arrbytHashValue;
            System.IO.FileStream oFileStream = null;

            System.Security.Cryptography.SHA384 oSHA384Hasher = new System.Security.Cryptography.SHA384Managed();

            try
            {
                oFileStream = GetFileStream(pathName);
                arrbytHashValue = oSHA384Hasher.ComputeHash(oFileStream);
                oFileStream.Close();

                strHashData = System.BitConverter.ToString(arrbytHashValue);
                strHashData = strHashData.Replace("-", "");
                strResult = strHashData;
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxDefaultButton.Button1);
            }

            return (strResult);
        }

        public string GetSHA512Hash(string pathName)
        {
            string strResult = "";
            string strHashData = "";

            byte[] arrbytHashValue;
            System.IO.FileStream oFileStream = null;

            System.Security.Cryptography.SHA512 oSHA512Hasher = new System.Security.Cryptography.SHA512Managed();

            try
            {
                oFileStream = GetFileStream(pathName);
                arrbytHashValue = oSHA512Hasher.ComputeHash(oFileStream);
                oFileStream.Close();

                strHashData = System.BitConverter.ToString(arrbytHashValue);
                strHashData = strHashData.Replace("-", "");
                strResult = strHashData;
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxDefaultButton.Button1);
            }

            return (strResult);
        }

        public string GetRIPEMD160Hash(string pathName)
        {
            string strResult = "";
            string strHashData = "";

            byte[] arrbytHashValue;
            System.IO.FileStream oFileStream = null;

            System.Security.Cryptography.RIPEMD160 oRIPEMD160Hasher = new System.Security.Cryptography.RIPEMD160Managed();

            try
            {
                oFileStream = GetFileStream(pathName);
                arrbytHashValue = oRIPEMD160Hasher.ComputeHash(oFileStream);
                oFileStream.Close();

                strHashData = System.BitConverter.ToString(arrbytHashValue);
                strHashData = strHashData.Replace("-", "");
                strResult = strHashData;
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxDefaultButton.Button1);
            }

            return (strResult);
        }

       
    }
}
