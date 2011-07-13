﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc.Resources;

namespace System.Web.Mvc.Html
{
    public static class SelectExtensions
    {
        internal static string ListItemToOption(SelectListItemEx item)
        {
            TagBuilder builder = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }
            if (item.wage != null)
            {
                builder.Attributes["wage"] = item.wage;
            }
            return builder.ToString(TagRenderMode.Normal);
        }
    }
}

namespace System.Web.Mvc
{

    public class SelectListItemEx : SelectListItem
    {
        public string wage
        {
            get;
            set;
        }
        public string minHour
        {
            get;
            set;
        }
        public string fixedJob
        {
            get;
            set;
        }
    }
}