﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;

/// <summary>
/// BSLog 的摘要说明
/// </summary>
public class BSLog
{
    private DateTime _CreateDate;

    public DateTime CreateDate
    {
        get { return _CreateDate; }
        set { _CreateDate = value; }
    }

    private Guid _logID;
    public Guid LogID
    {
        get
        {
            return _logID;
        }
        set
        {
            _logID = value;
        }
    }

    private BSLogType _logType;

    public BSLogType LogType
    {
        get { return _logType; }
        set { _logType = value; }
    }

    private string _message;
    public string Message
    {
        get
        {
            return _message;
        }
        set
        {
            _message = value;
        }
    }

    private string _rawUrl;
    public string RawUrl
    {
        get
        {
            return _rawUrl;
        }
        set
        {
            _rawUrl = value;
        }
    }

    private string _source;
    public string Source
    {
        get
        {
            return _source;
        }
        set
        {
            _source = value;
        }
    }

    private string _stackTrace;

    public string StackTrace
    {
        get
        {
            if (_stackTrace == null)
                _stackTrace = string.Empty;
            return _stackTrace;
        }
        set { _stackTrace = value; }
    }

    private MethodBase _targetSite;
    public MethodBase TargetSite
    {
        get
        {
            return _targetSite;
        }
        set
        {
            _targetSite = value;
        }
    }

    private string _Url;

    public string Url
    {
        get
        {
            if (_Url == null)
                _Url = string.Empty;
            return _Url;
        }
        set { _Url = value; }
    }

    private int _UserID;

    public int UserID
    {
        get { return _UserID; }
        set { _UserID = value; }
    }

    private static BSLog _lastLog;
    public static BSLog LastLog
    {
        get { return _lastLog; }
        set { _lastLog = value; }
    }

    public void Save()
    {
        LastLog = this;

        XmlDocument document = new XmlDocument();
        string strLogFile = LogFile();
        document.Load(strLogFile);
        XmlNode node = document.SelectSingleNode("Logs");

        XmlNode newNode = NewElement(document, "Event", null, null);

        NewAttribute(document, newNode, "LogID", this.LogID.ToString());
        NewAttribute(document, newNode, "LogType", this.LogType.ToString());
        NewAttribute(document, newNode, "RawUrl", this.RawUrl);
        NewAttribute(document, newNode, "Url", this.Url);
        NewAttribute(document, newNode, "UserID", this.UserID.ToString());
        NewAttribute(document, newNode, "CreateDate", String.Format("{0:s}", this.CreateDate));

        NewElement(document, "Message", this.Message, newNode);
        NewElement(document, "Source", this.Source, newNode);
        NewElement(document, "StackTrace", this.StackTrace, newNode);
        NewElement(document, "TargetSite", this.TargetSite.ToString(), newNode);

        node.PrependChild(newNode);

        document.Save(strLogFile);
    }

    private static XmlAttribute NewAttribute(XmlDocument xDoc, XmlNode xNode, string strKey, string strValue)
    {
        XmlAttribute xAttr = xDoc.CreateAttribute(strKey);
        xAttr.Value = strValue;
        xNode.Attributes.Append(xAttr);
        return xAttr;
    }

    private static XmlNode NewElement(XmlDocument xDoc, string strKey, string strValue, XmlNode parentNode)
    {
        XmlElement xEl = xDoc.CreateElement(strKey);

        if (!String.IsNullOrEmpty(strValue))
            xEl.InnerXml = String.Format("<![CDATA[{0}]]>", strValue);

        if (parentNode != null)
            parentNode.AppendChild(xEl);

        return xEl;
    }

    private static string LogFile()
    {
        string strLogPath = HttpContext.Current.Server.MapPath("~/App_Data/Log");
        if (!Directory.Exists(strLogPath))
            Directory.CreateDirectory(strLogPath);

        string strFileName = Path.Combine(strLogPath,
            String.Format("{0}-{1}-{2}.xml", DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00")));

        if (!File.Exists(strFileName))
        {
            XmlWriterSettings wSettings = new XmlWriterSettings();
            wSettings.Encoding = System.Text.Encoding.UTF8;
            using (XmlWriter writer = XmlWriter.Create(strFileName, wSettings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Logs");
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        return strFileName;
    }
}

public enum BSLogType : byte
{
    Information = 0,
    Error = 1,
    Warning = 2
}