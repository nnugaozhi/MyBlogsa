using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Web.UI;

/// <summary>
/// BSHelper 的摘要说明
/// </summary>
public class BSHelper
{

    public static int GetCommentCount(CommentStates state)
    {
        return BSComment.GetComments(state).Count;
    }

    public static string GetGravatar(string email)
    {
        return GetGravatar(email, 96);
    }

    public static string GetPermalink(string contentType, string code, string extension)
    {
        return Blogsa.Url + contentType + "/" + code + extension;
    }

    public static string GetLink(BSPost bsPost)
    {
        string strExpression = Blogsa.Settings["permaexpression"].ToString();
        Dictionary<string, string> dicExpressions = new Dictionary<string, string>();
        dicExpressions.Add("{author}", bsPost.UserName);
        dicExpressions.Add("{name}", bsPost.Code);
        dicExpressions.Add("{year}", bsPost.Date.Year.ToString());
        dicExpressions.Add("{month}", bsPost.Date.Month.ToString("00"));
        dicExpressions.Add("{day}", bsPost.Date.Day.ToString("00"));
        dicExpressions.Add("{id}", bsPost.PostID.ToString());
        Regex rex = new Regex("({(.+?)})/");
        if (bsPost.Type == PostTypes.Page)
        {
            strExpression = strExpression.Replace("{name}", bsPost.Code);
            strExpression = strExpression.Replace("{id}", bsPost.PostID.ToString());
            strExpression = rex.Replace(strExpression, "");
        }
        else
            foreach (string key in dicExpressions.Keys)
                strExpression = strExpression.Replace(key, dicExpressions[key]);

        return Blogsa.Url + strExpression;
    }

    public static string GetGravatar(string email, int size)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        UTF8Encoding encoder = new UTF8Encoding();
        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

        byte[] hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(email));

        StringBuilder sb = new StringBuilder(hashedBytes.Length * 2);
        for (int i = 0; i < hashedBytes.Length; i++)
            sb.Append(hashedBytes[i].ToString("X2"));

        return String.Format("http://www.gravatar.com/avatar.php?gravatar_id={0}&rating={1}&size={2}&default=identicon", sb.ToString(), "G", size); ;
    }

    public static StringDictionary GetLanguageDictionary(string strFile)
    {
        StringDictionary sdLang = new StringDictionary();

        string fileName = HttpContext.Current.Server.MapPath("~/" + strFile);

        XmlDocument docLang = new XmlDocument();
        using (StreamReader sr = new StreamReader(fileName))
        {
            docLang.Load(sr);
        }

        XmlNodeList nodesWord = docLang.SelectNodes("/language/word");

        if (nodesWord != null)
            foreach (XmlNode word in nodesWord)
                if (word.Attributes != null) sdLang.Add(word.Attributes["Keyword"].Value, word.InnerText);

        return sdLang;
    }

    public static Control FindChildControl(Control sourceControl, string controlId)
    {
        if (sourceControl != null)
        {
            Control foundControl = sourceControl.FindControl(controlId);

            if (foundControl != null)
                return foundControl;

            foreach (Control c in sourceControl.Controls)
            {
                foundControl = FindChildControl(c, controlId);
                if (foundControl != null)
                    return foundControl;
            }
        }
        return null;
    }

    public static string CreateCode(String input)
    {
        string[] pattern = new string[] { "[^a-zA-Z0-9-]", "-+" };
        string[] replacements = new string[] { "-", "-" };
        input = input.Trim();
        input = input.Replace("Ç", "C");
        input = input.Replace("ç", "c");
        input = input.Replace("Ğ", "G");
        input = input.Replace("ğ", "g");
        input = input.Replace("Ü", "U");
        input = input.Replace("ü", "u");
        input = input.Replace("Ş", "S");
        input = input.Replace("ş", "s");
        input = input.Replace("İ", "I");
        input = input.Replace("ı", "i");
        input = input.Replace("Ö", "O");
        input = input.Replace("ö", "o");
        for (int i = 0; i < pattern.Length; i++)
            input = Regex.Replace(input, pattern[i], replacements[i]);
        return input;
    }
}