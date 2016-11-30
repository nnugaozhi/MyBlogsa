﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Configuration;

/// <summary>
/// DataProcess 的摘要说明
/// </summary>
public class DataProcess : IDisposable
{
    private DbConnection _connection;
    private DbConnection Connection
    {
        get { return _connection; }
        set { _connection = value; }
    }

    private string _providerName;
    public string ProviderName
    {
        get { return _providerName; }
        set { _providerName = value; }
    }

    private string _connectionString;
    public string ConnectionString
    {
        get { return _connectionString; }
        set { _connectionString = value; }
    }

    private DbProviderFactory _factory;

    private DbProviderFactory Factory
    {
        set
        {
            _factory = value;
        }
        get
        {
            if (String.IsNullOrEmpty(ProviderName))
            {
                throw new Exception("Provider Name cannot be null!");
            }
            if (_factory == null)
                _factory = DbProviderFactories.GetFactory(ProviderName);
            return _factory;
        }
    }

    private string _connectionStringName;
    public string ConnectionStringName
    {
        get { return _connectionStringName; }
        set { _connectionStringName = value; }
    }

    public DataProcess()
    {
        Initialize("ConnectionString");
    }

    private void Initialize(string pConnectionStringName)
    {
        ConnectionStringName = pConnectionStringName;
        ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
        ProviderName = ConfigurationManager.AppSettings["Provider"];

        Connection = Factory.CreateConnection();
        if (Connection != null)
        {
            Connection.ConnectionString = ConnectionString;
            Connection.Open();
        }
    }
    private DataReturnValue _return;

    /// <summary>
    /// Last Executed Process.
    /// </summary>
    public DataReturnValue Return
    {
        get { return _return; }
        set { _return = value; }
    }

    public DataReturnValue ExecuteReader(string strSqlCommand)
    {
        DataReturnValue rv = new DataReturnValue();
        try
        {
            DataSet DS = new DataSet();
            DataTable DT = new DataTable();
            using (DbCommand Comm = Factory.CreateCommand())
            {
                if (Comm != null)
                {
                    Comm.CommandText = strSqlCommand;
                    Comm.Connection = Connection;
                    FillParameters(Comm);
                    rv.Value = Comm.ExecuteReader();
                }
            }
            rv.Status = DataProcessState.Success;
        }
        catch (Exception Ex)
        {
            rv.Status = DataProcessState.Error;
            rv.Error = Ex;
        }
        Return = rv;
        WriteLog(Return);
        return rv;
    }

    private Dictionary<string, object> _parameters;
    
    public void AddParameter(string name, object value)
    {
        if (_parameters == null)
            _parameters = new Dictionary<string, object>();
        if (_parameters.ContainsKey(name))
            _parameters[name] = value;
        else
            _parameters.Add(name, value);
    }

    private void FillParameters(DbCommand dbCommand)
    {
        if (_parameters != null && _parameters.Count > 0)
        {
            foreach (string key in _parameters.Keys)
            {
                DbParameter dbParameter = Factory.CreateParameter();

                if (dbParameter != null)
                {
                    dbParameter.ParameterName = String.Format("{0}", key);

                    object value = _parameters[key];

                    dbParameter.Value = value ?? DBNull.Value;

                    if (ProviderName.ToLowerInvariant().Equals("system.data.oledb") && dbParameter.Value is DateTime)
                        dbParameter.Value = ((DateTime)dbParameter.Value).ToOADate();

                    dbCommand.Parameters.Add(dbParameter);
                }
            }
        }
        ClearParameters();
    }

    private void ClearParameters()
    {
        if (_parameters != null)
            _parameters.Clear();
    }

    #region IDisposable Members
    public void Dispose()
    {
        if (Connection != null)
        {
            Connection.Close();
            Connection.Dispose();
        }

        GC.SuppressFinalize(this);
    }
    #endregion

    #region Log
    /// <summary>
    /// If return value status type is Error then write log.
    /// </summary>
    /// <param name="returnValue"></param>
    private void WriteLog(DataReturnValue returnValue)
    {
        if (returnValue.Status == DataProcessState.Error)
        {
            BSLog l = new BSLog();
            l.CreateDate = DateTime.Now;
            l.LogID = Guid.NewGuid();
            l.LogType = BSLogType.Error;
            l.Message = returnValue.Error.Message;
            l.RawUrl = HttpContext.Current.Request.RawUrl;
            l.Source = returnValue.Error.Source;
            l.StackTrace = returnValue.Error.StackTrace;
            l.TargetSite = returnValue.Error.TargetSite;
            l.Url = HttpContext.Current.Request.Url.ToString();
            l.UserID = Blogsa.ActiveUser != null ? Blogsa.ActiveUser.UserID : 0;
            l.Save();
        }
    }
    #endregion
}