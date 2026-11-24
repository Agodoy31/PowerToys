// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CmdPal.Ext.WebSearch.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.CmdPal.Ext.WebSearch.UnitTests;

[TestClass]
public class FallbackOpenURLItemTests
{
    [TestMethod]
    [DataRow("www.example.com", "https://www.example.com")]
    [DataRow("192.168.1.1:8080", "https://192.168.1.1:8080")]
    [DataRow("example.com:8080/path", "https://example.com:8080/path")]
    [DataRow("intranet.corp:8443", "https://intranet.corp:8443")]
    [DataRow("example.com:443/path", "https://example.com:443/path")]
    [DataRow("example.com:8080/login?next=https://other.com", "https://example.com:8080/login?next=https://other.com")]
    [DataRow("example.com:8080/path?email=a@b.com", "https://example.com:8080/path?email=a@b.com")]
    [DataRow("  example.com:8080/path  ", "https://example.com:8080/path")]
    [DataRow("https://example.com:8080/path", "https://example.com:8080/path")]
    [DataRow("http://test.org", "http://test.org")]
    [DataRow("ftp://files.example.net", "ftp://files.example.net")]
    [DataRow("file:///C:/Users/Test/file.txt", "file:///C:/Users/Test/file.txt")]
    [DataRow("mailto:someone@example.com", "mailto:someone@example.com")]
    [DataRow("foo:8080?v=1.0", "foo:8080?v=1.0")]
    [DataRow("com.example.app://open/item", "com.example.app://open/item")]
    [DataRow("com.example.app:callback", "com.example.app:callback")]
    [DataRow("com.example.app:/callback", "com.example.app:/callback")]
    [DataRow("com.example.app:", "com.example.app:")]
    [DataRow("com.example.app:secret@other.com", "com.example.app:secret@other.com")]
    [DataRow("example.com:8080@other.com", "example.com:8080@other.com")]
    [DataRow("example.com:0", "example.com:0")]
    [DataRow("example.com:65536", "example.com:65536")]
    public void UpdateQuery_NormalizesHostPortUrls_WithoutRewritingOtherSchemes(string query, string expectedUrl)
    {
        var item = new FallbackOpenURLItem(new MockSettingsInterface(), new MockBrowserInfoService());

        item.UpdateQuery(query);

        Assert.AreEqual(expectedUrl, ((OpenURLCommand)item.Command!).Url);
    }
}
