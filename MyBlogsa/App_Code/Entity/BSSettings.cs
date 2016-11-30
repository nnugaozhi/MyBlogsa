using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// BSSettings 的摘要说明
/// </summary>
public class BSSettings
{
    internal List<BSSetting> objectList;

    public BSSettings()
    {
        objectList = new List<BSSetting>();
    }

    public BSSetting this[int index]
    {
        get
        {
            return (BSSetting)objectList[index];
        }
        set
        {
            objectList[index] = value;
        }
    }

    public BSSetting this[String settingName]
    {
        get
        {
            foreach (BSSetting setting in objectList)
            {
                if (setting.Name.Equals(settingName))
                    return setting;
            }
            return null;
        }
        set
        {
            int foundedIndex = -1;

            foreach (BSSetting setting in objectList)
            {
                if (setting.Name.Equals(settingName))
                    foundedIndex = objectList.IndexOf(setting);
            }

            if (foundedIndex != -1)
            {
                objectList[foundedIndex] = value;
            }
        }
    }

    public void Add(BSSetting item)
    {
        objectList.Add(item);
    }

}