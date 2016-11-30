using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

/// <summary>
///LoginBaseForm 的摘要说明
/// </summary>
public class LoginFormBase:UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString.ToString() == "logout")
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Redirect("Login.aspx");
        }

        Panel divLogin = (Panel)FindControl("divLogin");
        HtmlControl divLostPassword = (HtmlControl)FindControl("divLostPassword");

        TextBox txtUserName = (TextBox)FindControl("txtUserName");
        TextBox txtPassword = (TextBox)FindControl("txtPassword");

        if (Request.QueryString.ToString() == "lostpassword")
        {
            divLogin.Visible = false;
            divLostPassword.Visible = true;
        }
        else if (Page.User.Identity.IsAuthenticated && Session["ActiveUser"] != null)
        {
            if (Page.User.IsInRole("Admin"))
                Response.Redirect("~/Admin/");
            else if (Page.User.IsInRole("Editor"))
                Response.Redirect("~/Admin/Editor.aspx");
        }
        else
        {
            divLogin.Visible = true;
            divLostPassword.Visible = false;

            if (string.IsNullOrEmpty(txtUserName.Text))
                txtUserName.Focus();
            else
                txtPassword.Focus();
        }
    }
}