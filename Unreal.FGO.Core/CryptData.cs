
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.BZip2;

public class CryptData
{
    public static Dictionary<string, string> getQueryDic(string query, Dictionary<string, string> dic = null)
    {
        Regex queryRegex = new Regex("([^&=?]+)=([^&=?]*)");
        if (dic == null)
            dic = new Dictionary<string, string>();
        var matchs = queryRegex.Matches(query);
        if (matchs.Count > 0)
        {
            foreach (Match item in matchs)
            {
                dic[item.Groups[1].Value] = System.Web.HttpUtility.UrlDecode((item.Groups[2].Value));
            }
        }
        return dic;
    }

    public static string Sign(Dictionary<string, string> dic, bool ios)
    {
        var keys = dic.Keys.ToList();
        keys.Sort();
        var str = "";
        foreach (var key in keys)
        {
            if (key == "sign")
                continue;
            str += dic[key];
        }
        str += ios ? "2a7ee43463114270bf2620ae5d6d59c4" : "a4e39619a09d49e9aead9b820980013a";
        return MD5(str);
    }

    public static string MD5(string sText)
    {
        Byte[] clearBytes = Encoding.UTF8.GetBytes(sText);
        MD5 md = new MD5CryptoServiceProvider();
        byte[] ss = md.ComputeHash(UnicodeEncoding.UTF8.GetBytes(sText));
        return byteArrayToHexString(ss);
    }
    private static string[] HexCode = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };

    public static string byteToHexString(byte b)
    {
        int n = b;
        if (n < 0)
        {
            n = 256 + n;
        }
        int d1 = n / 16;
        int d2 = n % 16;
        return HexCode[d1] + HexCode[d2];
    }

    public static String byteArrayToHexString(byte[] b)
    {
        String result = "";
        for (int i = 0; i < b.Length; i++)
        {
            result = result + byteToHexString(b[i]);
        }
        return result;
    }
    private const string CKEY = "b5nHjsMrqaeNliSs3jyOzgpD";
    private const string CVEC = "wuD6keVr";
    private const string FUNNY_CKEY = "ZmF0ZWdvX2FuZHJvaWRfZnVu";
    private const string FUNNY_CVEC = "ZGVzX2l2";
    private const byte mask = 4;
    protected const int WRITE_BUFFER_SIZE = 0x4000;

    public static string Decrypt(string str, bool isPress = false)
    {
        byte[] buffer;
        byte[] buffer2 = Convert.FromBase64String(str);
        byte[] bytes = Encoding.UTF8.GetBytes("b5nHjsMrqaeNliSs3jyOzgpD");
        byte[] rgbIV = Encoding.UTF8.GetBytes("wuD6keVr");
        TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
        using (MemoryStream stream = new MemoryStream())
        {
            using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(bytes, rgbIV), CryptoStreamMode.Write))
            {
                stream2.Write(buffer2, 0, buffer2.Length);
                stream2.Close();
            }
            buffer = stream.ToArray();
            stream.Close();
        }
        if (isPress)
        {
            using (MemoryStream stream3 = new MemoryStream())
            {
                using (MemoryStream stream4 = new MemoryStream(buffer))
                {
                    using (BZip2InputStream stream5 = new BZip2InputStream(stream4))
                    {
                        int num;
                        byte[] buffer5 = new byte[0x4000];
                        while ((num = stream5.Read(buffer5, 0, buffer5.Length)) > 0)
                        {
                            stream3.Write(buffer5, 0, num);
                        }
                        stream5.Close();
                    }
                    stream4.Close();
                }
                buffer = stream3.ToArray();
                stream3.Close();
            }
        }
        return Encoding.UTF8.GetString(buffer);
    }


    public static string Rsa(string key, string content)
    {
        var provider = RSAProvider(key);
        var bytes = provider.Encrypt(Encoding.UTF8.GetBytes(content), false);
        return Convert.ToBase64String(bytes);
    }

    private static RSACryptoServiceProvider RSAProvider(string publicKeyString)
    {
        publicKeyString = publicKeyString.Replace("-----BEGIN PUBLIC KEY-----\n", string.Empty).Replace("\n-----END PUBLIC KEY-----\n", string.Empty);
        // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
        byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
        byte[] x509key;
        byte[] seq = new byte[15];
        int x509size;

        x509key = Convert.FromBase64String(publicKeyString);
        x509size = x509key.Length;

        // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
        using (MemoryStream mem = new MemoryStream(x509key))
        {
            using (BinaryReader binr = new BinaryReader(mem))  //wrap Memory Stream with BinaryReader for easy reading
            {
                byte bt = 0;
                ushort twobytes = 0;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                seq = binr.ReadBytes(15);       //read the Sequence OID
                if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8203)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x00)     //expect null byte next
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;

                if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                    lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte(); //advance 2 bytes
                    lowbyte = binr.ReadByte();
                }
                else
                    return null;
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                int modsize = BitConverter.ToInt32(modint, 0);

                int firstbyte = binr.PeekChar();
                if (firstbyte == 0x00)
                {   //if first byte (highest order) of modulus is zero, don't include it
                    binr.ReadByte();    //skip this null byte
                    modsize -= 1;   //reduce modulus buffer size by 1
                }

                byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                    return null;
                int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                byte[] exponent = binr.ReadBytes(expbytes);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAKeyInfo = new RSAParameters();
                RSAKeyInfo.Modulus = modulus;
                RSAKeyInfo.Exponent = exponent;
                RSA.ImportParameters(RSAKeyInfo);

                return RSA;
            }
        }
    }
    private static bool CompareBytearrays(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;
        int i = 0;
        foreach (byte c in a)
        {
            if (c != b[i])
                return false;
            i++;
        }
        return true;
    }

    public static string EncryptMD5(string str)
    {
        MD5 md = new MD5CryptoServiceProvider();
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        byte[] buffer2 = md.ComputeHash(bytes);
        StringBuilder builder = new StringBuilder();
        foreach (byte num2 in buffer2)
        {
            builder.AppendFormat("{0:x2}", num2);
        }
        return builder.ToString();
    }

    public static string FunnyKeyDecrypt(string str)
    {
        byte[] buffer;
        byte[] buffer2 = Convert.FromBase64String(str);
        byte[] bytes = Encoding.UTF8.GetBytes("ZmF0ZWdvX2FuZHJvaWRfZnVu");
        byte[] rgbIV = Encoding.UTF8.GetBytes("ZGVzX2l2");
        TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
        using (MemoryStream stream = new MemoryStream())
        {
            using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(bytes, rgbIV), CryptoStreamMode.Write))
            {
                stream2.Write(buffer2, 0, buffer2.Length);
                stream2.Close();
            }
            buffer = stream.ToArray();
            stream.Close();
        }
        using (MemoryStream stream3 = new MemoryStream())
        {
            using (MemoryStream stream4 = new MemoryStream(buffer))
            {
                int num;
                byte[] buffer5 = new byte[0x4000];
                while ((num = stream4.Read(buffer5, 0, buffer5.Length)) > 0)
                {
                    stream3.Write(buffer5, 0, num);
                }
                stream4.Close();
            }
            buffer = stream3.ToArray();
            stream3.Close();
        }
        return Encoding.UTF8.GetString(buffer);
    }

    public static string ResponseDecrypt(string str)
    {
        byte[] bytes = Convert.FromBase64String(str);
        return Encoding.UTF8.GetString(bytes);
    }

    public static string TextDecrypt(string str)
    {
        try
        {
            byte[] buffer = Convert.FromBase64String(str);
            byte[] bytes = new byte[buffer.Length];
            int length = buffer.Length;
            for (int i = 0; i < length; i++)
            {
                bytes[i] = (byte)(buffer[i] ^ 4);
            }
            return Encoding.UTF8.GetString(bytes);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static string TextEncrypt(string str)
    {
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] inArray = new byte[bytes.Length];
            int length = bytes.Length;
            for (int i = 0; i < length; i++)
            {
                inArray[i] = (byte)(bytes[i] ^ 4);
            }
            return Convert.ToBase64String(inArray);
        }
        catch (Exception)
        {
            return null;
        }
    }
}

