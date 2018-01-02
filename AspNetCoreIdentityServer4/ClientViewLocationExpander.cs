﻿using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

public class ClientViewLocationExpander : IViewLocationExpander
{
    private const string THEME_KEY = "theme";

    public void PopulateValues(ViewLocationExpanderContext context)
    {
        var query = context.ActionContext.HttpContext.Request.Query;
        var exists = query.TryGetValue("client_id", out StringValues culture);

        if (!exists)
        {
            exists = query.TryGetValue("returnUrl", out StringValues requesturl);

            if (exists)
            {
                var request = requesturl.ToArray()[0];
                Uri uri = new Uri("http://faketopreventexception" + request);
                var query1 = QueryHelpers.ParseQuery(uri.Query);
                var client_id = query1.FirstOrDefault(t => t.Key == "client_id").Value;

                context.Values[THEME_KEY] = client_id.ToString();
            }
        }
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        string theme = null;
        if (context.Values.TryGetValue(THEME_KEY, out theme))
        {
            viewLocations = new[] {
                $"/Themes/{theme}/{{1}}/{{0}}.cshtml",
                $"/Themes/{theme}/Shared/{{0}}.cshtml",
            }
            .Concat(viewLocations);
        }
        else
        {
            viewLocations = new[] {
                $"/Themes/Default/{{1}}/{{0}}.cshtml",
                $"/Themes/Default/Shared/{{0}}.cshtml",
            }
           .Concat(viewLocations);
        }


        return viewLocations;
    }
}