// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Text;
using Microsoft.CmdPal.Ext.WebSearch.Commands;
using Microsoft.CmdPal.Ext.WebSearch.Helpers;
using Microsoft.CmdPal.Ext.WebSearch.Helpers.Browser;
using Microsoft.CmdPal.Ext.WebSearch.Properties;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace Microsoft.CmdPal.Ext.WebSearch;

internal sealed partial class FallbackOpenURLItem : FallbackCommandItem
{
    private const string _id = "com.microsoft.cmdpal.builtin.websearch.openurl.fallback";
    private readonly IBrowserInfoService _browserInfoService;
    private readonly OpenURLCommand _executeItem;
    private static readonly CompositeFormat PluginOpenURL = System.Text.CompositeFormat.Parse(Properties.Resources.plugin_open_url);
    private static readonly CompositeFormat PluginOpenUrlInBrowser = System.Text.CompositeFormat.Parse(Properties.Resources.plugin_open_url_in_browser);

    public FallbackOpenURLItem(ISettingsInterface settings, IBrowserInfoService browserInfoService)
        : base(new OpenURLCommand(string.Empty, browserInfoService), Resources.open_url_fallback_title, _id)
    {
        ArgumentNullException.ThrowIfNull(browserInfoService);

        _browserInfoService = browserInfoService;
        _executeItem = (OpenURLCommand)Command!;
        Title = string.Empty;
        _executeItem.Name = string.Empty;
        Subtitle = string.Empty;
        Icon = Icons.WebSearch;
    }

    public override void UpdateQuery(string query)
    {
        if (!IsValidUrl(query))
        {
            _executeItem.Url = string.Empty;
            _executeItem.Name = string.Empty;
            Title = string.Empty;
            Subtitle = string.Empty;
            return;
        }

        query = query.Trim();

        var success = Uri.TryCreate(query, UriKind.Absolute, out _);

        // if url not contain schema, add https:// by default.
        if (!success || IsSchemelessHostPort(query))
        {
            query = "https://" + query;
        }

        _executeItem.Url = query;
        _executeItem.Name = string.IsNullOrEmpty(query) ? string.Empty : Resources.open_in_default_browser;

        var browserName = _browserInfoService.GetDefaultBrowser()?.Name;
        Title = string.IsNullOrWhiteSpace(browserName) ? Resources.open_in_default_browser : string.Format(CultureInfo.CurrentCulture, PluginOpenUrlInBrowser, browserName);
        Subtitle = string.Format(CultureInfo.CurrentCulture, PluginOpenURL, query);
    }

    private static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        if (!url.Contains('.', StringComparison.OrdinalIgnoreCase))
        {
            // eg: 'com', 'org'. We don't think it's a valid url.
            // This can simplify the logic of checking if the url is valid.
            return false;
        }

        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            return true;
        }

        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
        {
            if (Uri.IsWellFormedUriString("https://" + url, UriKind.Absolute))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSchemelessHostPort(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || !uri.Scheme.Contains('.', StringComparison.Ordinal))
        {
            return false;
        }

        var portStartIndex = uri.Scheme.Length + 1;
        if (portStartIndex >= url.Length || !char.IsAsciiDigit(url[portStartIndex]))
        {
            return false;
        }

        return Uri.TryCreate("https://" + url, UriKind.Absolute, out var httpsUri)
            && string.IsNullOrEmpty(httpsUri.UserInfo)
            && httpsUri.Port > 0
            && Uri.CheckHostName(httpsUri.Host) == UriHostNameType.Dns;
    }
}
