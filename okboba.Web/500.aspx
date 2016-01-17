<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="500.aspx.cs" Inherits="okboba.Web._500" %>
<% Response.StatusCode = 500; %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>OkBoba - 500 Internal Error</title>
    <style>
        body {
            background-color: #D81B60;
            color: #FFF;
        }
    </style>
</head>
<body>
    <h1>Oops, something went wrong :( </h1>
    <h2><a href="#" onclick="window.location.reload(true);">Try Again?</a></h2>
    <img src="/content/images/spilled-drink.png" alt="Spilled Drink" width="400" />
</body>
</html>
