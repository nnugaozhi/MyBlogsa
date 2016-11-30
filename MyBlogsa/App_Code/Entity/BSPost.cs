using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

public enum PostVisibleTypes : short
{
    All = -1,
    Hidden = 0,
    Public = 1,
    Custom = 2
}

public enum PostStates : short
{
    Draft = 0,
    Published = 1,
    Removed = 2,
    All = 9
}

public enum PostTypes : short
{
    Article = 0,
    Page = 1,
    AutoSave = 2,
    File = 3
}

/// <summary>
/// BSPost 的摘要说明
/// </summary>
public class BSPost
{
    public BSMeta this[String key]
    {
        get { return Metas[key]; }
        set { _metas[key] = value; }
    }

    #region Variables

    private bool _addComment;
    private string _categories;
    private string _code;
    private int _commentCount;
    private string _content;
    private DateTime _date = DateTime.Now;
    private string _link;
    private int _postID;
    private int _readCount;
    private PostVisibleTypes _show;
    private PostStates _state;
    private string _tags;
    private string _title;
    private PostTypes _type = PostTypes.Article;
    private DateTime _updateDate;
    private int _userID;
    private string _userName;
    private int _parentID;
    private BSMetas _metas;
    private string _languageCode = System.Globalization.CultureInfo.InvariantCulture.TwoLetterISOLanguageName;
    private static BSPost _currentPost;

    #endregion

    #region Properties

    public static BSPost CurrentPost
    {
        get { return BSPost._currentPost; }
        set { BSPost._currentPost = value; }
    }

    public int PostID
    {
        get { return _postID; }
        set { _postID = value; }
    }

    public PostStates State
    {
        get { return _state; }
        set { _state = value; }
    }

    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    public string Content
    {
        get { return _content; }
        set { _content = value; }
    }

    public string Code
    {
        get { return _code; }
        set { _code = value; }
    }

    public bool AddComment
    {
        get { return _addComment; }
        set { _addComment = value; }
    }

    public DateTime Date
    {
        get { return _date; }
        set { _date = value; }
    }

    public DateTime UpdateDate
    {
        get { return _updateDate; }
        set { _updateDate = value; }
    }

    public int ReadCount
    {
        get { return _readCount; }
        set { _readCount = value; }
    }

    public string Link
    {
        get { return _link; }
        set { _link = value; }
    }

    public string UserName
    {
        get { return _userName; }
        set { _userName = value; }
    }

    public int UserID
    {
        get { return _userID; }
        set { _userID = value; }
    }

    public string Categories
    {
        get { return _categories; }
        set { _categories = value; }
    }

    public string Tags
    {
        get { return _tags; }
        set { _tags = value; }
    }

    public int CommentCount
    {
        get { return _commentCount; }
        set { _commentCount = value; }
    }

    public PostTypes Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public PostVisibleTypes Show
    {
        get { return _show; }
        set { _show = value; }
    }

    public string LinkedTitle
    {
        get { return String.Format("<a href=\"{0}\">{1}</a>", Link, Title); }
    }

    public BSMetas Metas
    {
        get { return _metas; }
        set { _metas = value; }
    }

    public int ParentID
    {
        get { return _parentID; }
        set { _parentID = value; }
    }

    public string LanguageCode
    {
        get { return _languageCode; }
        set { _languageCode = value; }
    }

    #endregion

	public BSPost()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    public static BSPost GetPost(int iPostID)
    {
        using (DataProcess dp = new DataProcess())
        {
            dp.AddParameter("PostID", iPostID);
            dp.ExecuteReader("SELECT * FROM Posts WHERE [PostID]=@PostID");
            if (dp.Return.Status == DataProcessState.Success)
            {
                using (IDataReader dr = dp.Return.Value as IDataReader)
                {
                    if (dr != null && dr.Read())
                    {
                        BSPost bsPost = new BSPost();
                        FillPost(dr, bsPost);
                        return bsPost;
                    }
                }
            }
        }
        return null;
    }

    public List<BSTerm> GetCategories()
    {
        return BSTerm.GetTermsByObjectID(PostID, TermTypes.Category);
    }

    public string GetCategoriesHtml()
    {
        List<BSTerm> categories = GetCategories();
        string html = String.Empty;
        if (categories.Count > 0)
        {
            foreach (BSTerm category in categories)
            {
                html += "<a href=\"" + BSHelper.GetPermalink("Category", category.Code, Blogsa.UrlExtension) + "\">"
                + category.Name + "</a> ";
            }
        }
        else
        {
            html = Language.Get["NoCategory"];
        }
        return html;
    }

    public string GetTagsHtml()
    {
        return GetTagsHtml("<a href=\"{0}\">{1}</a>");
    }

    public string GetTagsHtml(string format)
    {
        return BSTerm.GetTermsByFormat(TermTypes.Tag, PostID, 0, format);
    }

    public List<BSComment> GetComments(CommentStates state)
    {
        return BSComment.GetCommentsByPostID(PostID, state);
    }

    public static void FillPost(IDataReader dr, BSPost bsPost)
    {
        bsPost.Title = dr["Title"].ToString();
        bsPost.PostID = Convert.ToInt32(dr["PostID"]);
        bsPost.Code = dr["Code"].ToString();
        bsPost.Content = dr["Content"].ToString();
        bsPost.State = (PostStates)Convert.ToInt16(dr["State"]);
        bsPost.AddComment = Convert.ToBoolean(dr["AddComment"]);
        bsPost.Categories = bsPost.GetCategoriesHtml();
        bsPost.Tags = bsPost.GetTagsHtml();
        bsPost.CommentCount = bsPost.GetComments(CommentStates.Approved).Count;
        bsPost.ReadCount = Convert.ToInt32(dr["ReadCount"]);
        bsPost.UserID = Convert.ToInt32(dr["UserID"]);
        bsPost.UserName = BSUser.GetUser(bsPost.UserID).UserName;
        bsPost.Date = Convert.ToDateTime(dr["CreateDate"]);
        bsPost.UpdateDate = dr["ModifyDate"] == DBNull.Value
                              ? Convert.ToDateTime(dr["CreateDate"])
                              : Convert.ToDateTime(dr["ModifyDate"]);
        bsPost.Link = BSHelper.GetLink(bsPost);
        bsPost.Type = (PostTypes)Convert.ToInt16(dr["Type"]);
        bsPost.Show = (PostVisibleTypes)Convert.ToInt16(dr["Show"]);
        bsPost.ParentID = Convert.ToInt32(dr["ParentID"]);
        bsPost.LanguageCode = Convert.ToString(dr["LanguageCode"]);

        bsPost.Metas = BSMeta.GetMetas(bsPost.PostID);
    }
}