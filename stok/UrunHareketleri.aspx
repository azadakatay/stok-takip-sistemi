<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UrunHareketleri.aspx.cs" Inherits="stok.UrunHareketleri" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>STOK HAREKETLERİ</title>
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background: #f0f2f5;
            margin: 0;
            padding: 20px;
            color: #2c3e50;
        }

        h2 {
            text-align: center;
            font-size: 28px;
            color: #34495e;
            margin-bottom: 25px;
        }

        /* Butonlar */
        .btn {
            display: inline-block;
            padding: 10px 20px;
            margin: 5px 0;
            background: #2980b9;
            color: #fff;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 600;
            transition: 0.3s;
        }
        .btn:hover {
            background: #1c5980;
        }

        /* Filtre alanı */
        .filters {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
            align-items: center;
            background-color: #fff;
            padding: 20px;
            border-radius: 12px;
            box-shadow: 0 6px 20px rgba(0,0,0,0.08);
            margin-bottom: 25px;
        }
        .filters label {
            font-size: 14px;
            font-weight: 600;
        }
        .filters input[type="text"],
        .filters input[type="date"],
        .filters select {
            padding: 8px 10px;
            border-radius: 8px;
            border: 1px solid #ccc;
            font-size: 13px;
            min-width: 140px;
        }

        .filters input[type="text"] {
            flex: 1;
        }

        #lblTarihUyari {
            font-size: 13px;
            color: #c0392b;
            font-weight: bold;
            margin-left: 10px;
        }

        /* Grid alanı */
        .grid-container {
            width: 100%;
            overflow-x: auto;
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 6px 20px rgba(0,0,0,0.08);
        }
        .gridview {
            border-collapse: collapse;
            width: 100%;
            min-width: 900px;
        }
        .gridview th, .gridview td {
            padding: 12px 10px;
            text-align: left;
            font-size: 14px;
        }
        .gridview th {
            background: #34495e;
            color: #ecf0f1;
            font-weight: 600;
            position: sticky;
            top: 0;
            z-index: 1;
        }
        .gridview tr:nth-child(even) {
            background: #f7f9fc;
        }
        .gridview tr:hover {
            background: #d0e6f7;
        }

        /* Responsive küçük ekranlar */
        @media screen and (max-width: 768px) {
            .filters {
                flex-direction: column;
                align-items: flex-start;
            }
            .filters label {
                margin-bottom: 5px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>STOK HAREKETLERİ</h2>

        <div class="filters">
            <asp:Button ID="btnGeri" runat="server" Text="Stok Listesine Dön" CssClass="btn" OnClick="btnGeri_Click" />

            <asp:TextBox ID="txtArama" runat="server" placeholder="Ürün/Sipariş Ara..." AutoPostBack="true" OnTextChanged="txtArama_TextChanged"></asp:TextBox>

            <label for="ddlHareketTipi">Hareket Tipi:</label>
            <asp:DropDownList ID="ddlHareketTipi" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlHareketTipi_SelectedIndexChanged"></asp:DropDownList>

            <label for="txtBaslangic">Başlangıç:</label>
            <asp:TextBox ID="txtBaslangic" runat="server" CssClass="txtDate" TextMode="Date"></asp:TextBox>

            <label for="txtBitis">Bitiş:</label>
            <asp:TextBox ID="txtBitis" runat="server" CssClass="txtDate" TextMode="Date"></asp:TextBox>

            <asp:Button ID="btnTarihFiltre" runat="server" Text="Filtrele" CssClass="btn" OnClick="btnTarihFiltre_Click" />

            <asp:Label ID="lblTarihUyari" runat="server"></asp:Label>
        </div>

        <div class="grid-container">
            <asp:GridView ID="gvHareketler" runat="server" CssClass="gridview" AutoGenerateColumns="False"
                DataKeyNames="HareketID" OnRowDataBound="gvHareketler_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="HareketID" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="UrunAdi" HeaderText="Ürün Adı" />
                    <asp:BoundField DataField="HareketTipi" HeaderText="Hareket Tipi" />
                    <asp:BoundField DataField="Miktar" HeaderText="Miktar" />
                    <asp:BoundField DataField="Tarih" HeaderText="Tarih" DataFormatString="{0:dd.MM.yyyy HH:mm}" />
                    <asp:BoundField DataField="Aciklama" HeaderText="Açıklama" />
                    <asp:BoundField DataField="SiparisNo" HeaderText="Stok Kodu" />
                    <asp:BoundField DataField="IstekYapan" HeaderText="İstek Yapan" />
                    <asp:BoundField DataField="Tedarikci" HeaderText="Tedarikçi" />
                    <asp:BoundField DataField="Depo" HeaderText="Depo" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
