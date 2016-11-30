using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

public enum TermTypes : short
{
    Category,
    Tag,
    LinkCategory,
    All
}

/// <summary>
/// BSTerm 的摘要说明
/// </summary>
public class BSTerm
{
    #region Variables
    private int _siteID;
    private int _termID;
    private TermTypes _type;
    private string _name;
    private string _description;
    private string _code;
    private int _subID;
    private List<int> _objects;
    #endregion

    #region Properties
    public int SiteID
    {
        get { return _siteID; }
        set { _siteID = value; }
    }

    public int TermID
    {
        get { return _termID; }
        set { _termID = value; }
    }

    public TermTypes Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    public string Code
    {
        get { return _code; }
        set { _code = value; }
    }

    public int SubID
    {
        get { return _subID; }
        set { _subID = value; }
    }

    public string Link
    {
        get
        {
            return BSHelper.GetPermalink(Type.ToString(), Code, Blogsa.UrlExtension);
        }
    }

    public List<int> Objects
    {
        get { return _objects; }
        set { _objects = value; }
    }
    #endregion

	public BSTerm()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    public static List<BSTerm> GetTermsByObjectID(int objectId, TermTypes termType)
    {
        List<BSTerm> terms = new List<BSTerm>();

        using (DataProcess dp = new DataProcess())
        {
            dp.AddParameter("ObjectID", objectId);

            if (termType != TermTypes.All)
            {
                dp.AddParameter("Type", termType.ToString().ToLowerInvariant());
                dp.ExecuteReader("SELECT * FROM Terms WHERE [TermID] IN (SELECT TermID FROM TermsTo WHERE [ObjectID]=@ObjectID AND [Type]=@Type) ORDER BY Name");
            }
            else
            {
                dp.ExecuteReader("SELECT * FROM Terms WHERE [TermID] IN (SELECT TermID FROM TermsTo WHERE [ObjectID]=@ObjectID) ORDER BY Name");
            }

            if (dp.Return.Status == DataProcessState.Success)
            {
                using (IDataReader dr = dp.Return.Value as IDataReader)
                {
                    while (dr != null && dr.Read())
                    {
                        BSTerm bsTerm = new BSTerm();
                        FillTerm(dr, bsTerm);
                        terms.Add(bsTerm);
                    }
                }
            }
        }

        return terms;
    }

    public static List<BSTerm> GetTerms(TermTypes termType, int iTermCount)
    {
        List<BSTerm> terms = new List<BSTerm>();

        using (DataProcess dp = new DataProcess())
        {
            string top = iTermCount == 0 ? String.Empty : "TOP " + iTermCount;

            if (termType != TermTypes.All)
            {
                dp.AddParameter("Type", termType.ToString().ToLowerInvariant());
                dp.ExecuteReader(String.Format("SELECT {0} * FROM Terms WHERE [Type]=@Type ORDER BY [Name] ASC", top));
            }
            else
            {
                dp.ExecuteReader(String.Format("SELECT {0} * FROM Terms ORDER BY [Name] ASC", top));
            }

            if (dp.Return.Status == DataProcessState.Success)
            {
                using (IDataReader dr = dp.Return.Value as IDataReader)
                {
                    while (dr != null && dr.Read())
                    {
                        BSTerm bsTerm = new BSTerm();
                        FillTerm(dr, bsTerm);
                        terms.Add(bsTerm);
                    }
                }
            }
        }

        return terms;
    }


    public static string GetTermsByFormat(TermTypes termType, int objectId, int count, string format)
    {
        List<BSTerm> terms = objectId != 0 ? GetTermsByObjectID(objectId, termType) : GetTerms(termType, count);

        string html = String.Empty;
        if (terms.Count > 0)
        {
            foreach (BSTerm term in terms)
            {
                if (format.Contains("{2}"))
                    html += String.Format(format, BSHelper.GetPermalink("Tag", term.Code, Blogsa.UrlExtension), term.Name, term.Objects.Count);
                else
                    html += String.Format(format, BSHelper.GetPermalink("Tag", term.Code, Blogsa.UrlExtension), term.Name);
            }
        }
        else
        {
            html = Language.Get["NoTag"];
        }
        return html;
    }

    private static void FillTerm(IDataReader dr, BSTerm bsTerm)
    {
        bsTerm.SubID = (int)dr["SubID"];
        bsTerm.Name = (string)dr["Name"];
        bsTerm.Code = (string)dr["Code"];
        bsTerm.Description = dr["Description"].ToString();

        string termType = (string)dr["Type"];

        bsTerm.Type = GetTermType(termType);
        bsTerm.TermID = (int)dr["TermID"];

        bsTerm.Objects = new List<int>();

        using (DataProcess dp = new DataProcess())
        {
            dp.AddParameter("TermID", bsTerm.TermID);
            dp.ExecuteReader("SELECT * FROM TermsTo WHERE [TermID]=@TermID");
            if (dp.Return.Status == DataProcessState.Success)
                using (IDataReader drObjects = (IDataReader)dp.Return.Value)
                    while (drObjects.Read())
                        bsTerm.Objects.Add((int)drObjects["ObjectID"]);
        }
    }

    public static TermTypes GetTermType(string termType)
    {
        TermTypes type = TermTypes.All;

        switch (termType)
        {
            case "tag":
                type = TermTypes.Tag;
                break;
            case "category":
                type = TermTypes.Category;
                break;
            case "linkcategory":
                type = TermTypes.LinkCategory;
                break;
        }

        return type;
    }
}