<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UrunListe.aspx.cs" Inherits="stok.UrunListe" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>STOK LİSTESİ</title>
    <style>
        body { font-family: 'Segoe UI', sans-serif; background: #f5f7fa; margin: 0; padding: 20px; }
        h2 { text-align: center; color: #34495e; margin-bottom: 20px; }
        .top-controls { display: flex; flex-wrap: wrap; justify-content: center; align-items: center; gap: 10px; margin-bottom: 20px; }
        .top-controls label { font-size: 13px; margin-right: 4px; }
        .top-controls .txtDate, #txtArama { padding: 6px 8px; border-radius: 6px; border: 1px solid #ccc; font-size: 13px; }
        .btn { padding: 8px 18px; background: #2980b9; color: #fff; border: none; border-radius: 6px; cursor: pointer; font-size: 13px; font-weight: 600; transition: 0.3s; }
        .btn:hover { background: #1c5980; }
        .summary { display: flex; justify-content: center; gap: 20px; flex-wrap: wrap; margin-bottom: 25px; }
        .summary-card { background: #fff; padding: 12px 20px; border-radius: 10px; box-shadow: 0 4px 12px rgba(0,0,0,0.1); font-weight: bold; color: #2c3e50; min-width: 120px; text-align: center; transition: transform 0.2s; }
        .summary-card:hover { transform: translateY(-2px); }
        .grid-container { margin: 0 auto; width: 100%; overflow-x: visible; }
        .gridview { border-collapse: separate; border-spacing: 0; width: 100%; background: #fff; box-shadow: 0 4px 15px rgba(0,0,0,0.08); border-radius: 8px; overflow: hidden; table-layout: fixed; }
        .gridview th, .gridview td { white-space: normal; word-wrap: break-word; font-size: 13px; padding: 8px 6px; }
        .gridview th { background-color: #2c3e50; color: #ecf0f1; text-align: left; font-weight: 600; }
        .gridview tr:nth-child(even) { background-color: #f7f9fc; }
        .gridview tr:hover { background-color: #d6eaf8; }
        .gridview tfoot td { font-weight: bold; background-color: #ecf0f1; color: #2c3e50; text-align: right; }
        .gridview .delete-btn { background-color: #e74c3c; color: #fff; padding: 5px 12px; border-radius: 6px; text-decoration: none; font-size: 13px; }
        .gridview .delete-btn:hover { background-color: #c0392b; }
        .gridview .edit-btn, .gridview .update-btn, .gridview .cancel-btn { background-color: #2980b9; color: #fff; padding: 5px 12px; border-radius: 6px; text-decoration: none; font-size: 13px; border: none; cursor: pointer; }
        .gridview .edit-btn:hover, .gridview .update-btn:hover, .gridview .cancel-btn:hover { background-color: #1c5980; }
        #lblTarihUyari { font-size: 13px; margin-left: 8px; color: red; font-weight: bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>STOK LİSTESİ</h2>
        <div class="top-controls">
            <asp:Button ID="btnEkleyeGit" runat="server" Text="Yeni Stok Ekle" CssClass="btn" OnClick="btnEkleyeGit_Click" />
            <asp:Button ID="btnHareketler" runat="server" Text="Stok Hareketleri" CssClass="btn" OnClick="btnHareketler_Click" />
            <asp:TextBox ID="txtArama" runat="server" placeholder="Ürün Adı veya Sipariş No..." AutoPostBack="true" OnTextChanged="txtArama_TextChanged"></asp:TextBox>
            <label for="txtBaslangic">Başlangıç:</label>
            <asp:TextBox ID="txtBaslangic" runat="server" CssClass="txtDate" TextMode="Date"></asp:TextBox>
            <label for="txtBitis">Bitiş:</label>
            <asp:TextBox ID="txtBitis" runat="server" CssClass="txtDate" TextMode="Date"></asp:TextBox>
            <label for="ddlTedarikci">Tedarikçi:</label>
            <asp:DropDownList ID="ddlTedarikci" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTedarikci_SelectedIndexChanged">
                <asp:ListItem Text="Tümü" Value="" Selected="True"></asp:ListItem>
            </asp:DropDownList>
            <label for="ddlDepo">Depo:</label>
            <asp:DropDownList ID="ddlDepo" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlDepo_SelectedIndexChanged">
                <asp:ListItem Text="Tümü" Value="" Selected="True"></asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="btnTarihFiltre" runat="server" Text="Filtrele" CssClass="btn" OnClick="btnTarihFiltre_Click" />
            <asp:Label ID="lblTarihUyari" runat="server"></asp:Label>
            <asp:Button ID="btnExcelAktar" runat="server" Text="Excel'e Aktar" CssClass="btn" OnClick="btnExcelAktar_Click" />
            <asp:Button ID="btnCikis" runat="server" Text="Çıkış Yap" CssClass="btn btn-danger" OnClick="btnCikis_Click" />
            <asp:Button ID="btnRapor" runat="server" Text="Rapor Sayfası" CssClass="btn" OnClick="btnRapor_Click" />
        </div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="grid-container">
                    <asp:GridView ID="gvUrunler" runat="server" CssClass="gridview" AutoGenerateColumns="False" DataKeyNames="UrunID" ShowFooter="True"
                        OnRowEditing="gvUrunler_RowEditing" OnRowCancelingEdit="gvUrunler_RowCancelingEdit" OnRowUpdating="gvUrunler_RowUpdating"
                        OnRowDeleting="gvUrunler_RowDeleting" OnRowDataBound="gvUrunler_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="UrunID" HeaderText="ID" ReadOnly="True" ItemStyle-Width="4%" />
                            <asp:TemplateField HeaderText="Ürün Adı" ItemStyle-Width="12%">
                                <ItemTemplate><%# Eval("UrunAdi") %></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtUrunAdi" runat="server" Text='<%# Bind("UrunAdi") %>' Width="95%"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Açıklama" ItemStyle-Width="18%">
                                <ItemTemplate><%# Eval("Aciklama") %></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAciklama" runat="server" Text='<%# Bind("Aciklama") %>' Width="95%"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Stok Adet" ItemStyle-Width="6%">
                                <ItemTemplate><%# Eval("StokAdet") %></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtStokAdet" runat="server" Text='<%# Bind("StokAdet") %>' Width="90%"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Fiyat (₺)" ItemStyle-Width="8%">
                                <ItemTemplate><%# Eval("Fiyat", "{0:N2}") %></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtFiyat" runat="server" Text='<%# Bind("Fiyat", "{0:N2}") %>' Width="90%"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Oluşturma Tarihi" ItemStyle-Width="10%">
                                <ItemTemplate><%# Eval("OlusturmaTarihi", "{0:yyyy-MM-dd}") %></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtOlusturmaTarihi" runat="server" Text='<%# Bind("OlusturmaTarihi", "{0:yyyy-MM-dd}") %>' Width="95%"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Stok Kodu" ItemStyle-Width="8%">
                                <ItemTemplate><%# Eval("SiparisNo") %></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtSiparisNo" runat="server" Text='<%# Bind("SiparisNo") %>' Width="90%"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="İstek Yapan" ItemStyle-Width="10%">
                                <ItemTemplate><%# Eval("IstekYapan") %></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtIstekYapan" runat="server" Text='<%# Bind("IstekYapan") %>' Width="95%"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tedarikçi" ItemStyle-Width="12%">
                                <ItemTemplate><%# Eval("Tedarikci") %></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtTedarikci" runat="server" Text='<%# Bind("Tedarikci") %>' Width="95%"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Depo" ItemStyle-Width="10%">
                                <ItemTemplate><%# Eval("Depo") %></ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtDepo" runat="server" Text='<%# Bind("Depo") %>' Width="95%"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowEditButton="True" ButtonType="Button" EditText="Düzenle" CancelText="İptal" UpdateText="Güncelle" ItemStyle-Width="6%" />
                            <asp:TemplateField HeaderText="Sil" ItemStyle-Width="6%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnSil" runat="server" CommandName="Delete" CssClass="delete-btn" Text="Sil" OnClientClick="return confirm('Bu ürünü silmek istediğinize emin misiniz?');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
