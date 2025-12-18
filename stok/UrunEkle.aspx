<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UrunEkle.aspx.cs" Inherits="stok.UrunEkle" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Stok Ekle</title>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;600;700&display=swap" rel="stylesheet" />
    <style>
        * { box-sizing: border-box; margin: 0; padding: 0; font-family: 'Inter', sans-serif; }

        body {
            background: #f4f7fa;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }

        .card {
            background: #fff;
            width: 100%;
            max-width: 1400px;
            padding: 70px;
            border-radius: 16px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
        }

        h2 {
            text-align: center;
            font-size: 32px;
            font-weight: 700;
            color: #2c3e50;
            margin-bottom: 30px;
        }

        .form-row {
            display: flex;
            gap: 20px;
            margin-bottom: 30px;
        }

        .form-group {
            display: flex;
            flex-direction: column;
            flex: 1;
        }

        .form-group label {
            font-weight: 600;
            margin-bottom: 8px;
            color: #34495e;
        }

        .form-control {
            padding: 15px 18px;
            font-size: 16px;
            border-radius: 8px;
            border: 1px solid #ccd1d9;
        }

        .form-control:focus {
            border-color: #3498db;
            box-shadow: 0 0 8px rgba(52, 152, 219, 0.2);
            outline: none;
        }

        .form-control[TextMode="MultiLine"] {
            height: 140px;
        }

        .btn-row {
            display: flex;
            justify-content: flex-end;
            gap: 15px;
            margin-top: 20px;
        }

        .btn {
            padding: 15px 35px;
            font-size: 18px;
            font-weight: 600;
            border-radius: 8px;
            border: none;
            cursor: pointer;
            color: #fff;
            transition: all 0.3s ease;
        }

        .btn-primary { background: #3498db; }
        .btn-primary:hover { background: #217dbb; }

        .btn-secondary { background: #2ecc71; }
        .btn-secondary:hover { background: #27ae60; }

        .message {
            text-align: center;
            font-weight: 600;
            font-size: 16px;
            margin-top: 15px;
            min-height: 22px;
            color: #e74c3c;
        }

        @media (max-width: 1200px) {
            .form-row {
                flex-direction: column;
            }
            .btn-row { justify-content: center; }
        }

        .form-control::placeholder { color: #95a5a6; font-style: italic; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="card">
            <h2>Stok Ekle</h2>

            <div class="form-row">
                <div class="form-group">
                    <label>Stok Adı</label>
                    <asp:TextBox ID="txtUrunAdi" runat="server" CssClass="form-control" placeholder="Ürün adı girin"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Açıklama</label>
                    <asp:TextBox ID="txtAciklama" runat="server" TextMode="MultiLine" CssClass="form-control" placeholder="Açıklama girin"></asp:TextBox>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label>Miktar</label>
                    <asp:TextBox ID="txtMiktar" runat="server" CssClass="form-control" placeholder="0"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Fiyat</label>
                    <asp:TextBox ID="txtFiyat" runat="server" CssClass="form-control" placeholder="0,00 ₺"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Tarih</label>
                    <asp:TextBox ID="txtTarih" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label>Stok Kodu</label>
                    <asp:TextBox ID="txtSiparisNo" runat="server" CssClass="form-control" placeholder="Stok kodu girin"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>İstek Yapan</label>
                    <asp:TextBox ID="txtIstekYapan" runat="server" CssClass="form-control" placeholder="İstek yapan kişi"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Tedarikçi</label>
                    <asp:TextBox ID="txtTedarikci" runat="server" CssClass="form-control" placeholder="Tedarikçi adı"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Depo</label>
                    <asp:TextBox ID="txtDepo" runat="server" CssClass="form-control" placeholder="Depo adı girin"></asp:TextBox>
                </div>
            </div>

            <div class="btn-row">
                <asp:Button ID="btnKaydet" runat="server" Text="Kaydet" CssClass="btn btn-primary" OnClick="btnKaydet_Click" />
                <asp:Button ID="btnListeyeGit" runat="server" Text="Stok Listesini Gör" CssClass="btn btn-secondary" OnClick="btnListeyeGit_Click" />
            </div>

            <asp:Label ID="lblMesaj" runat="server" CssClass="message"></asp:Label>
        </div>
    </form>
</body>
</html>
