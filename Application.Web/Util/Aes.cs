using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Application.Web
{
    public class Aes  // Advanced Encryption Standard
    {
         // State matrix

        /// <summary>
        /// 加密
        /// </summary>
        public string JIAMI(string str)
        {
            string MyEnData = "";
            string MyData = str;
            string MyPassword = "40405690";
            string MyDeData = "";
            //使用对称算法加密数据
            byte[] MyIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            byte[] MyKey = Encoding.UTF8.GetBytes(MyPassword);
            DESCryptoServiceProvider MyProvider = new DESCryptoServiceProvider();
            byte[] MyBytes = Encoding.UTF8.GetBytes(MyData);
            MemoryStream MyMemory = new MemoryStream();
            CryptoStream MyCrypt = new CryptoStream(MyMemory, MyProvider.CreateEncryptor(MyKey, MyIV), CryptoStreamMode.Write);

            MyCrypt.Write(MyBytes, 0, MyBytes.Length);
            MyCrypt.FlushFinalBlock();
            MyEnData = Convert.ToBase64String(MyMemory.ToArray());
            //使用对称算法解密数据
            byte[] MyEnKey = System.Text.Encoding.Default.GetBytes(MyPassword);
            byte[] MyEnBytes = Convert.FromBase64String(MyEnData);
            MemoryStream MyDeMemory = new MemoryStream();
            CryptoStream MyDeCrypt = new CryptoStream(MyDeMemory, MyProvider.CreateDecryptor(MyEnKey, MyIV), CryptoStreamMode.Write);
            MyDeCrypt.Write(MyEnBytes, 0, MyEnBytes.Length);
            MyDeCrypt.FlushFinalBlock();
            System.Text.Encoding MyEncoding = new System.Text.UTF8Encoding();
            MyDeData = MyEncoding.GetString(MyDeMemory.ToArray());
            return MyEnData;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="lname"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string JIEMI(string str)
        {
            string MyPassword = "40405690";
            string MyEnData = str;
            string MyDeData = "";
            byte[] MyIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            byte[] MyKey = Encoding.UTF8.GetBytes(MyPassword);
            //使用对称算法解密数据
            byte[] MyEnKey = System.Text.Encoding.Default.GetBytes(MyPassword);
            byte[] MyEnBytes = Convert.FromBase64String(MyEnData);
            DESCryptoServiceProvider MyProvider = new DESCryptoServiceProvider();
            MemoryStream MyDeMemory = new MemoryStream();
            CryptoStream MyDeCrypt = new CryptoStream(MyDeMemory, MyProvider.CreateDecryptor(MyEnKey, MyIV), CryptoStreamMode.Write);
            MyDeCrypt.Write(MyEnBytes, 0, MyEnBytes.Length);
            MyDeCrypt.FlushFinalBlock();
            System.Text.Encoding MyEncoding = new System.Text.UTF8Encoding();
            MyDeData = MyEncoding.GetString(MyDeMemory.ToArray());
            return MyDeData;
        }
    }
}