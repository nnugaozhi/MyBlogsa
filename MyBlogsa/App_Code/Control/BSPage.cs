using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///BSPage 的摘要说明
/// </summary>
public class BSPage:BSPageBase
{
    protected override void OnPreInit(EventArgs e)
    {
        if (!Blogsa.IsInstalled)
            Response.Redirect("~/Setup/Default.aspx");

        Title = String.Format("{0} - {1}", Blogsa.Title, Blogsa.Description);

        Page.UICulture = Blogsa.CurrentBlogLanguage;

        base.OnInit(e);
    }
}