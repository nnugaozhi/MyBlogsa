using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// BSUser 的摘要说明
/// </summary>
public class BSUser
{

    #region Variables
    private int _SiteID;
    private int _UserID;
    private string _UserName;
    private string _Password;
    private short _State;
    private string _Name;
    private string _Email;
    private string _WebPage;
    private DateTime? _LastLoginDate;
    private string _Role;
    private DateTime _CreateDate;
    #endregion

    #region Properties
    public int SiteID
    {
        get { return _SiteID; }
        set { _SiteID = value; }
    }

    public int UserID
    {
        get { return _UserID; }
        set { _UserID = value; }
    }

    public string UserName
    {
        get { return _UserName; }
        set { _UserName = value; }
    }

    public string Password
    {
        get { return _Password; }
        set { _Password = value; }
    }

    public short State
    {
        get { return _State; }
        set { _State = value; }
    }

    public string Name
    {
        get { return _Name; }
        set { _Name = value; }
    }

    public string Email
    {
        get { return _Email; }
        set { _Email = value; }
    }

    public string WebPage
    {
        get { return _WebPage; }
        set { _WebPage = value; }
    }

    public DateTime? LastLoginDate
    {
        get { return _LastLoginDate; }
        set { _LastLoginDate = value; }
    }

    public string Role
    {
        get { return _Role; }
        set { _Role = value; }
    }


    public DateTime CreateDate
    {
        get { return _CreateDate; }
        set { _CreateDate = value; }
    }
    #endregion

    public static BSUser GetUser(int iUserID)
    {
        using (DataProcess dp = new DataProcess())
        {
            dp.AddParameter("UserID", iUserID);
            dp.ExecuteReader("SELECT * FROM Users WHERE [UserID]=@UserID");
            if (dp.Return.Status == DataProcessState.Success)
            {
                using (IDataReader dr = dp.Return.Value as IDataReader)
                {
                    if (dr != null && dr.Read())
                    {
                        BSUser user = new BSUser();
                        FillUser(dr, user);
                        return user;
                    }
                }
            }
        }
        return null;
    }

    private static void FillUser(IDataReader dr, BSUser user)
    {
        user.UserID = Convert.ToInt32(dr["UserID"]);
        user.Email = dr["Email"].ToString();
        user.LastLoginDate = dr["LastLoginDate"] == DBNull.Value ? Convert.ToDateTime(dr["CreateDate"]) : Convert.ToDateTime(dr["LastLoginDate"]);
        user.WebPage = dr["WebPage"].ToString();
        user.Name = dr["Name"].ToString();
        user.UserName = dr["UserName"].ToString();
        user.Role = dr["Role"].ToString();
        user.State = Convert.ToInt16(dr["State"]);
        user.Password = dr["Password"].ToString();
        user.CreateDate = Convert.ToDateTime(dr["CreateDate"]);
    }
}