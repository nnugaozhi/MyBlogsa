using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
///Templates 的摘要说明
/// </summary>
public class Templates
{
	public Templates()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}

    public static string Login
    {
        get
        {
            const string template = "~/Contents/Templates/Login.ascx";
            string themedTemplate = String.Format("~/Themes/{0}/Templates/Login.ascx", Blogsa.ActiveTheme);
            if (File.Exists(HttpContext.Current.Server.MapPath(themedTemplate)))
                return themedTemplate;
            else
                return template;
        }
    }
}