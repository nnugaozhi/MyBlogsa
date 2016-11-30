using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// BSComment 的摘要说明
/// </summary>
public class BSComment
{
    #region Properties
    private int _CommentID;
    public int CommentID
    {
        get { return _CommentID; }
        set { _CommentID = value; }
    }
    private string _UserName;
    public string UserName
    {
        get { return _UserName; }
        set { _UserName = value; }
    }
    private string _Content;
    public string Content
    {
        get { return _Content; }
        set { _Content = value; }
    }
    private string _Email;
    public string Email
    {
        get { return _Email; }
        set { _Email = value; }
    }
    private DateTime _Date;
    public DateTime Date
    {
        get { return _Date; }
        set { _Date = value; }
    }
    private string _IP;
    public string IP
    {
        get { return _IP; }
        set { _IP = value; }
    }
    private string _WebPage;
    public string WebPage
    {
        get { return _WebPage; }
        set { _WebPage = value; }
    }
    private int _PostID;
    public int PostID
    {
        get { return _PostID; }
        set { _PostID = value; }
    }
    private int _UserID;
    public int UserID
    {
        get { return _UserID; }
        set { _UserID = value; }
    }
    private string _GravatarLink;
    public string GravatarLink
    {
        get { return _GravatarLink; }
        set { _GravatarLink = value; }
    }
    private bool _Approve;
    public bool Approve
    {
        get { return _Approve; }
        set { _Approve = value; }
    }
    public bool NotifyMe
    {
        get
        {
            return _Approve;
        }
        set
        {
            _Approve = value;
        }
    }

    private bool _isAdmin;

    public bool IsAdmin
    {
        get { return _isAdmin; }
        set { _isAdmin = value; }
    }

    public string Link
    {
        get
        {
            BSPost post = BSPost.GetPost(PostID);
            if (post != null)
                return String.Format("{0}#{1}", post.Link, CommentID);
            else
                return String.Empty;
        }
    }

    #endregion

	public BSComment()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    public static List<BSComment> GetComments(CommentStates state)
    {
        return GetComments(state, 0);
    }

    public static List<BSComment> GetComments(CommentStates state, int iCommentCount)
    {
        List<BSComment> comments = new List<BSComment>();
        using (DataProcess dp = new DataProcess())
        {
            string top = iCommentCount == 0 ? String.Empty : "TOP " + iCommentCount;

            if (state == CommentStates.All)
            {
                dp.ExecuteReader(String.Format("SELECT {0} * FROM Comments ORDER By CreateDate DESC", top));
            }
            else
            {
                dp.AddParameter("Approve", state == CommentStates.Approved);
                dp.ExecuteReader(String.Format("SELECT {0} * FROM Comments WHERE [Approve]=@Approve ORDER By CreateDate DESC", top));
            }
            if (dp.Return.Status == DataProcessState.Success)
            {
                using (IDataReader dr = dp.Return.Value as IDataReader)
                {
                    while (dr != null && dr.Read())
                    {
                        BSComment bsComment = new BSComment();
                        FillComment(dr, bsComment);
                        comments.Add(bsComment);
                    }
                }
            }
        }
        return comments;
    }

    public static List<BSComment> GetCommentsByPostID(int iPostID, CommentStates state)
    {
        List<BSComment> comments = new List<BSComment>();
        using (DataProcess dp = new DataProcess())
        {
            if (state == CommentStates.All)
            {
                dp.AddParameter("PostID", iPostID);
                dp.ExecuteReader("SELECT * FROM Comments WHERE [PostID]=@PostID ORDER By CreateDate DESC");
            }
            else
            {
                dp.AddParameter("PostID", iPostID);
                dp.AddParameter("Approve", state == CommentStates.Approved);
                dp.ExecuteReader("SELECT * FROM Comments WHERE [PostID]=@PostID AND [Approve]=@Approve ORDER By CreateDate DESC");
            }
            if (dp.Return.Status == DataProcessState.Success)
            {
                using (IDataReader dr = dp.Return.Value as IDataReader)
                {
                    while (dr != null && dr.Read())
                    {
                        BSComment bsComment = new BSComment();
                        FillComment(dr, bsComment);
                        comments.Add(bsComment);
                    }
                }
            }
        }
        return comments;
    }

    static void FillComment(IDataReader dr, BSComment bsComment)
    {
        bsComment.CommentID = Convert.ToInt32(dr["CommentID"]);
        bsComment.Content = dr["Comment"].ToString();
        bsComment.Date = Convert.ToDateTime(dr["CreateDate"]);
        bsComment.Email = dr["EMail"].ToString();
        bsComment.GravatarLink = BSHelper.GetGravatar(bsComment.Email);
        bsComment.IP = dr["IP"].ToString();
        bsComment.PostID = Convert.ToInt32(dr["PostID"]);
        bsComment.UserID = Convert.ToInt32(dr["UserID"]);
        bsComment.UserName = dr["Name"].ToString();
        bsComment.WebPage = dr["WebPage"].ToString();
        bsComment.Approve = Convert.ToBoolean(dr["Approve"]);

        if (bsComment.UserID != 0)
        {
            BSUser user = BSUser.GetUser(bsComment.UserID);
            if (user != null)
            {
                bsComment.UserName = user.Name;
                bsComment.WebPage = user.WebPage;
                bsComment.Email = user.Email;
                bsComment.IsAdmin = user.Role.Equals("admin");
            }
        }
    }
}
public enum CommentStates : short
{
    Approved = 0,
    UnApproved = 1,
    All = 9
}