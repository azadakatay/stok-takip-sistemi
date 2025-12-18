<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="stok.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Stok Yönetimi - Giriş</title>
    <meta charset="utf-8" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;600&display=swap" rel="stylesheet" />
    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: 'Inter', sans-serif;
            background: #e9ecef; 
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }

        .login-box {
            width: 360px;
            background: #fff;
            padding: 40px 32px;
            border-radius: 8px;
            box-shadow: 0 6px 20px rgba(0, 0, 0, 0.08);
        }

        .login-box h2 {
            text-align: center;
            font-size: 22px;
            font-weight: 600;
            margin-bottom: 28px;
            color: #2c3e50;
        }

        .form-group {
            margin-bottom: 18px;
        }

        label {
            display: block;
            margin-bottom: 6px;
            font-size: 14px;
            color: #444;
        }

        .form-control {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid #ccc;
            border-radius: 6px;
            font-size: 14px;
            transition: border-color 0.2s;
        }

        .form-control:focus {
            border-color: #4a90e2;
            outline: none;
        }

        .checkbox {
            display: flex;
            align-items: center;
            gap: 6px;
            font-size: 13px;
            color: #555;
            margin-bottom: 16px;
        }

        .btn {
            width: 100%;
            padding: 12px;
            border: none;
            border-radius: 6px;
            font-size: 15px;
            font-weight: 600;
            background: #4a90e2;
            color: #fff;
            cursor: pointer;
            transition: background 0.3s;
        }

        .btn:hover {
            background: #357abd;
        }

        .message {
            margin-top: 14px;
            text-align: center;
            font-size: 13px;
            color: #e74c3c;
            min-height: 18px;
        }
    </style>

    <script type="text/javascript">
        function togglePassword() {
            var pwd = document.getElementById("<%= txtSifre.ClientID %>");
            pwd.type = (pwd.type === "password") ? "text" : "password";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-box">
            <h2>Stok Yönetim Sistemi</h2>

            <div class="form-group">
                <label for="txtKullaniciAdi">Kullanıcı Adı</label>
                <asp:TextBox ID="txtKullaniciAdi" runat="server" CssClass="form-control" placeholder="Kullanıcı adınızı girin"></asp:TextBox>
            </div>

            <div class="form-group">
                <label for="txtSifre">Şifre</label>
                <asp:TextBox ID="txtSifre" runat="server" TextMode="Password" CssClass="form-control" placeholder="Şifrenizi girin"></asp:TextBox>
            </div>

            <div class="checkbox">
                <input type="checkbox" id="chkShow" onclick="togglePassword()" />
                <label for="chkShow">Şifreyi Göster</label>
            </div>

            <asp:Button ID="btnGiris" runat="server" Text="Giriş Yap" CssClass="btn" OnClick="btnGiris_Click" />

            <asp:Label ID="lblMesaj" runat="server" CssClass="message"></asp:Label>
        </div>
    </form>
</body>
</html>
