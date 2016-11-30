using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// BSMetas 的摘要说明
/// </summary>
public class BSMetas
{
    internal List<BSMeta> objectList;

	public BSMetas()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
        objectList = new List<BSMeta>();
	}

    public BSMeta this[int index]
    {
        get
        {
            return (BSMeta)objectList[index];
        }
        set
        {
            objectList[index] = value;
        }
    }

    public BSMeta this[String key]
    {
        get
        {
            foreach (BSMeta item in objectList)
            {
                if (item.Key.Equals(key))
                    return item;
            }
            return null;
        }
        set
        {
            foreach (BSMeta item in objectList)
            {
                if (item.Key.Equals(key))
                    objectList[objectList.IndexOf(item)] = value;
            }
        }
    }

}