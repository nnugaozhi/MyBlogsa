using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// BSSetting 的摘要说明
/// </summary>
public class BSSetting
{
    #region Properties
    private int _SettingID;

    public int SettingID
    {
        get { return _SettingID; }
        set { _SettingID = value; }
    }
    private string _Name;

    public string Name
    {
        get { return _Name; }
        set { _Name = value; }
    }
    private string _Value;

    public string Value
    {
        get { return _Value; }
        set { _Value = value; }
    }
    private string _Title;

    public string Title
    {
        get { return _Title; }
        set { _Title = value; }
    }
    private string _Description;

    public string Description
    {
        get { return _Description; }
        set { _Description = value; }
    }
    private bool _Main;

    public bool Main
    {
        get { return _Main; }
        set { _Main = value; }
    }
    private int _Sort;

    public int Sort
    {
        get { return _Sort; }
        set { _Sort = value; }
    }

    private bool _Visible;
    public bool Visible
    {
        get { return _Visible; }
        set { _Visible = value; }
    }

    private int _SiteID;
    public int SiteID
    {
        get { return _SiteID; }
        set { _SiteID = value; }
    }

    #endregion

    #region  Methods
    public static BSSettings GetSettings()
    {
        BSSettings settings = new BSSettings();

        using (DataProcess dp = new DataProcess())
        {
            dp.ExecuteReader("SELECT * FROM Settings");

            if (dp.Return.Status == DataProcessState.Success)
            {
                using (IDataReader dr = dp.Return.Value as IDataReader)
                {
                    while (dr.Read())
                    {
                        BSSetting bsSetting = new BSSetting();

                        FillValue(dr, bsSetting);

                        settings.Add(bsSetting);
                    }
                }
            }
        }

        return settings;
    }

    private static void FillValue(IDataReader dr, BSSetting bsSetting)
    {
        bsSetting.SettingID = Convert.ToInt32(dr["SettingID"]);
        bsSetting.Name = dr["Name"].ToString();
        bsSetting.Value = dr["Value"].ToString();
        bsSetting.Title = dr["Title"].ToString();
        bsSetting.Description = dr["Description"].ToString();
        bsSetting.Main = Convert.ToBoolean(dr["Main"]);
        bsSetting.Sort = Convert.ToInt32(dr["Sort"]);
        bsSetting.Visible = Convert.ToBoolean(dr["Visible"]);
    }
    #endregion

}