using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;

public class XmlUtils
{
    static byte[] _keyArray = UTF8Encoding.UTF8.GetBytes("54354143154365437656411314354326");
    public static string SerializeObject(object mObject, Type t)
    {
        MemoryStream ms = new MemoryStream();
        XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.UTF8);

        XmlSerializer xs = new XmlSerializer(t);
        xs.Serialize(xtw, mObject);

        ms = (MemoryStream)xtw.BaseStream;
        string xmlString = UTF8ByteArrayToString(ms.ToArray());
        Debug.Log("" + xmlString);
        return xmlString;
    }

    public static object DeserializeObject(string s, Type t)
    {
        XmlSerializer xs = new XmlSerializer(t);
        MemoryStream mStream = new MemoryStream(StringToUTF8ByteArray(s));
        return xs.Deserialize(mStream);
    }

    public static void CreateXML(string fileName, string s)
    {
        StreamWriter writer;
        FileInfo fileInfo = new FileInfo(fileName);
        writer = fileInfo.CreateText();
        writer.Write(s);
        writer.Close();
    }

    public static string loadXML(string fileName)
    {
        StreamReader reader = File.OpenText(fileName);
        string dataString = reader.ReadToEnd();
        reader.Close();
        return dataString;
    }

    public static bool hasFile(string fileName)
    {
        return File.Exists(fileName);
    }

    public static string encrypt(string toEncrypt)
    {
        ICryptoTransform cTransform = getRijndaelManaged().CreateEncryptor();
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    public static string decrypt(string toDecrypt)
    {
        ICryptoTransform cTransform = getRijndaelManaged().CreateEncryptor();
        byte[] toDecryptArray = Convert.FromBase64String(toDecrypt);
        byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
        return UTF8Encoding.UTF8.GetString(resultArray);  
    }

    static RijndaelManaged getRijndaelManaged()
    {
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = _keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        return rDel;
    }

    public static string UTF8ByteArrayToString(byte[] b)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        string s = encoding.GetString(b);
        return (s);
    }

    public static byte[] StringToUTF8ByteArray(string s)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] b = encoding.GetBytes(s);
        return b;
    }  
}
