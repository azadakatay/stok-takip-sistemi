<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UrunRapor.aspx.cs" Inherits="stok.UrunRapor" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Ürün Raporu</title>
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
            margin-bottom: 25px;
            font-size: 28px;
            color: #34495e;
        }

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

        .filters-container {
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

        .filters-container label {
            font-size: 14px;
            font-weight: 600;
            margin-right: 5px;
            display: inline-block;
            vertical-align: middle;
        }

        .filters-container select,
        .filters-container input[type="date"] {
            padding: 8px 10px;
            border-radius: 8px;
            border: 1px solid #ccc;
            font-size: 13px;
            min-width: 130px;
            display: inline-block;
            vertical-align: middle;
        }

        .summary-container {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
            gap: 15px;
            margin-bottom: 25px;
        }
        .summary-box {
            background: #fff;
            padding: 15px;
            border-radius: 12px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
            text-align: center;
            font-weight: 600;
            font-size: 14px;
            color: #34495e;
            transition: 0.3s;
        }
        .summary-box strong {
            display: block;
            font-size: 13px;
            color: #7f8c8d;
            margin-bottom: 5px;
        }
        .summary-box.giris { background: #e8f8f5; color: #27ae60; }
        .summary-box.cikis { background: #fdecea; color: #c0392b; }
        .summary-box.stok { background: #eaf2f8; color: #2980b9; }
        .summary-box.depo { background: #fff8e1; color: #f39c12; }
        .summary-box.fiyat { background: #f5e6ff; color: #8e44ad; }

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
            min-width: 800px;
        }

        .gridview th, .gridview td {
            padding: 12px 10px;
            text-align: left;
            font-size: 14px;
        }

       .gridview th {
    position: sticky;
    top: 0;
    background: #f1f1f1;
    z-index: 2;
}


        .gridview tr:nth-child(even) {
            background: #f7f9fc;
        }
        .gridview tr:hover {
            background: #d0e6f7;
        }
        .stok-dusuk {
    background: #fdecea !important; 
    color: #c0392b !important;       
    font-weight: bold;
}
.stok-dusuk td {
    color: #c0392b !important;  
}


        @media screen and (max-width: 768px) {
            .filters-container {
                flex-direction: column;
                align-items: flex-start;
            }
            .filters-container label {
                margin-bottom: 5px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>ÜRÜN RAPORU</h2>

        <div class="filters-container">
            <label for="ddlUrun">Ürün:</label>
            <asp:DropDownList ID="ddlUrun" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filtrele"></asp:DropDownList>
            <label for="ddlDepo">Depo:</label>
            <asp:DropDownList ID="ddlDepo" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filtrele"></asp:DropDownList>
            <label for="ddlHareketTipi">Hareket Tipi:</label>
            <asp:DropDownList ID="ddlHareketTipi" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filtrele"></asp:DropDownList>
            <label for="txtBaslangic">Başlangıç:</label>
            <asp:TextBox ID="txtBaslangic" runat="server" CssClass="txtDate" TextMode="Date"></asp:TextBox>
            <label for="txtBitis">Bitiş:</label>
            <asp:TextBox ID="txtBitis" runat="server" CssClass="txtDate" TextMode="Date"></asp:TextBox>
            <asp:Button ID="btnFiltrele" runat="server" Text="Filtrele" CssClass="btn" OnClick="Filtrele" />
            <asp:Button ID="btnGeri" runat="server" Text="Stok Listesine Dön" CssClass="btn" OnClick="btnGeri_Click" />
            <asp:Button ID="btnExcel" runat="server" Text="Excel'e Aktar" CssClass="btn" OnClick="btnExcel_Click" />
            <label for="ddlSirala">Sırala:</label>
<asp:DropDownList ID="ddlSirala" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSirala_SelectedIndexChanged">
    <asp:ListItem Text="Ürün (A-Z)" Value="UrunAdi_ASC" />
    <asp:ListItem Text="Ürün (Z-A)" Value="UrunAdi_DESC" />
    <asp:ListItem Text="Stok (Artan)" Value="Miktar_ASC" />
    <asp:ListItem Text="Stok (Azalan)" Value="Miktar_DESC" />
    <asp:ListItem Text="Stok Ekleme Tarihi (En Yeni)" Value="Tarih_DESC" />
    <asp:ListItem Text="Stok Ekleme Tarihi (En Eski)" Value="Tarih_ASC" />
</asp:DropDownList>
        </div>

        <div class="summary-container">
            <div class="summary-box giris">
                <strong>Toplam Giriş</strong>
                <asp:Label ID="lblToplamGiris" runat="server"></asp:Label>
            </div>
            <div class="summary-box cikis">
                <strong>Toplam Çıkış</strong>
                <asp:Label ID="lblToplamCikis" runat="server"></asp:Label>
            </div>
            <div class="summary-box stok">
                <strong>Mevcut Stok</strong>
                <asp:Label ID="lblToplamStok" runat="server"></asp:Label>
                <asp:Label ID="lblStokUyari" runat="server" CssClass="stok-uyari"></asp:Label>
            </div>
            <div class="summary-box depo">
                <strong>Depo Sayısı</strong>
                <asp:Label ID="lblDepoSayisi" runat="server"></asp:Label>
            </div>
            <div class="summary-box fiyat">
                <strong>Toplam Stok Fiyatı</strong>
                <asp:Label ID="lblToplamFiyat" runat="server"></asp:Label>
            </div>
        </div>

        <div class="grid-container">
            <asp:GridView ID="gvRapor" runat="server" CssClass="gridview" AutoGenerateColumns="False" OnRowDataBound="gvRapor_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="HareketID" HeaderText="ID" />
                    <asp:BoundField DataField="UrunAdi" HeaderText="Ürün Adı" />
                    <asp:BoundField DataField="HareketTipi" HeaderText="Hareket Tipi" />
                    <asp:BoundField DataField="Tarih" HeaderText="Tarih" DataFormatString="{0:dd.MM.yyyy HH:mm}" />
                    <asp:BoundField DataField="Aciklama" HeaderText="Açıklama" />
                    <asp:BoundField DataField="Depo" HeaderText="Depo" />

                    <asp:TemplateField HeaderText="Miktar">
    <ItemTemplate>
        <asp:Label ID="lblMiktar" runat="server" Text='<%# Eval("Miktar") %>'></asp:Label>
    </ItemTemplate>
</asp:TemplateField>

                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
