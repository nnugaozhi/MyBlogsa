using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// DataReturnValue 的摘要说明
/// </summary>
public class DataReturnValue
{
	public DataReturnValue()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}
    private Exception _error;
    private DataProcessState _status;
    private object _value;

    public DataReturnValue(object value, DataProcessState state)
    {
        this.Value = value;
        this.Status = state;
        this.Error = null;
    }

    public DataReturnValue(Exception error)
    {
        this.Value = null;
        this.Error = error;
        this.Status = DataProcessState.Error;
    }

    public Exception Error
    {
        get { return _error; }
        set { _error = value; }
    }

    public DataProcessState Status
    {
        get { return _status; }
        set { _status = value; }
    }

    public object Value
    {
        get { return _value; }
        set { _value = value; }
    }
}

public enum DataProcessState { Success = 1, Error = 0 }