<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="404.aspx.cs" Inherits="okboba.Web._404" %>
<% Response.StatusCode = 404; %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>OkBoba - 404 Not Found</title>
    <style>
        body {
            background-color: #D81B60;
            color: #FFF;
        }
    </style>
</head>
<body>
    <h1>Oops, couldn't find what you were looking for :( </h1>
    <h2>Maybe you mistyped something?</h2>
    <img src="/content/images/spilled-drink.png" alt="Spilled Drink" width="400" />
</body>
</html>
