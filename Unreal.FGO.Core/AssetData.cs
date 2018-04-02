using System;
using System.Collections.Generic;
using Unreal.FGO.Core;

public class AssetData
{
    protected string attrib;
    protected uint crc;
    protected int entryCount;
    protected string name;
    protected string new_name;
    protected int newVersion;
    protected int nowVersion;
    protected int size;
    protected Type type;
    internal string Name { get { return name; } set { name = value; } }
    public bool IsExists
    {
        get
        {
            if (!isExists)
                isExists = System.IO.File.Exists(BinPath);
            return isExists;

        }
    }
    private bool isExtrated = false;
    private bool isExists = false;

    public bool IsExtrated
    {
        get
        {
            if (!isExtrated)
                isExtrated = System.IO.Directory.Exists(AssetManage.AssetPath + Name.Replace("/", "@"));
            return isExtrated;

        }
    }

    public bool IsDownloading { get; set; }
    public AssetData()
    {
        this.attrib = string.Empty;
        this.nowVersion = 0;
        this.newVersion = 0;
        this.size = 0;
        this.crc = 0;
    }

    public AssetData(Type type, string name, int version, string attrib, int size, uint crc)
    {
        this.type = type;
        string str = string.Empty;
        if (name.Contains("%"))
        {
            char[] separator = new char[] { '%' };
            char[] chArray2 = new char[] { '%' };
            str = name.Split(separator)[0] + name.Split(chArray2)[2];
        }
        else
        {
            str = name;
        }
        this.new_name = name;
        this.name = str;
        this.attrib = attrib;
        this.nowVersion = version;
        this.newVersion = version;
        this.size = size;
        this.crc = crc;
    }

    public void AddEntry()
    {
        if (this.entryCount >= 0)
        {
            this.entryCount++;
        }
    }

    protected string GetBaseName()
    {
        if (this.name != null)
        {
            int startIndex = this.name.LastIndexOf('/');
            int num2 = this.name.LastIndexOf('.');
            startIndex = (startIndex >= 0) ? (startIndex + 1) : 0;
            num2 = (num2 >= 0) ? num2 : this.name.Length;
            if (startIndex < num2)
            {
                return this.name.Substring(startIndex, num2 - startIndex);
            }
        }
        return null;
    }


    public string GetExt()
    {
        if (this.name != null)
        {
            int num = this.name.LastIndexOf('.');
            if ((num >= 0) && (this.name.Length > num))
            {
                return this.name.Substring(num + 1);
            }
        }
        return null;
    }


    public bool IsDownloadOldVersion() =>
        ((this.nowVersion > 0) && (this.nowVersion != this.newVersion));

    public bool IsNeedUpdateVersion() =>
        (this.nowVersion != this.newVersion);

    public bool IsSame(string name) =>
        this.name.Equals(name);

    public bool IsSame(Type type, string name) =>
        ((this.type == type) && this.name.Equals(name));


    public void ResetVersion()
    {
        this.nowVersion = 0;
    }

    public bool SetUpdateInfo(int version, string attrib, int size, uint crc)
    {
        this.newVersion = version;
        this.attrib = attrib;
        this.size = size;
        if (this.crc != crc)
        {
            this.crc = crc;
            this.nowVersion = 0;
        }
        return (this.nowVersion < version);
    }

    public bool UpdateVersion()
    {
        int nowVersion = this.nowVersion;
        this.nowVersion = this.newVersion;
        return (nowVersion != this.newVersion);
    }

    public string Attrib =>
        this.attrib;

    public uint Crc =>
        this.crc;

    public Type DataType =>
        this.type;

    public int EntryCount =>
        this.entryCount;

    public bool IsAssetBundle =>
        ((this.type == Type.ASSET_STORAGE) && (this.GetExt() == null));

    public string LastName
    {
        get
        {
            int num = this.name.LastIndexOf('/');
            if (num >= 0)
            {
                return this.name.Substring(num + 1);
            }
            return this.name;
        }
    }

    public int NowVersion =>
        this.nowVersion;

    public int Size =>
        this.size;

    public string Url
    {
        get
        {
            return AssetManage.getUrlString(this);
        }
    }

    public string NewName { get { return this.new_name; } set { this.new_name = value; } }

    public string ExtratPath { get { return AssetManage.AssetPath + Name.Replace("/", "@") + ".assets"; } }
    public string BinPath { get { return AssetManage.AssetPath + AssetManage.getBinName(Name); } }

    public enum Type
    {
        ASSET_STORAGE,
        ASSET_RESOURCE,
        ASSET_AUDIO
    }
}

