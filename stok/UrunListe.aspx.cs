using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace stok
{
    public partial class UrunListe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TedarikciDoldur();
                DepoDoldur();
                UrunleriGetir();

                string rol = Session["Rol"]?.ToString() ?? "";
                if (rol == "user")
                {
                    btnEkleyeGit.Visible = false;
                    btnHareketler.Visible = false;
                }
            }
        }

        private void TedarikciDoldur()
        {
            string connStr = ConfigurationManager.ConnectionStrings["StokDb"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT Tedarikci FROM Urunler ORDER BY Tedarikci", conn);
                SqlDataReader dr = cmd.ExecuteReader();
                ddlTedarikci.Items.Clear();
                ddlTedarikci.Items.Add(new ListItem("Tümü", ""));
                while (dr.Read())
                {
                    ddlTedarikci.Items.Add(new ListItem(dr["Tedarikci"].ToString(), dr["Tedarikci"].ToString()));
                }
            }
        }

        private void DepoDoldur()
        {
            string connStr = ConfigurationManager.ConnectionStrings["StokDb"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT ISNULL(Depo, '-') AS Depo FROM Urunler ORDER BY Depo", conn);
                SqlDataReader dr = cmd.ExecuteReader();
                ddlDepo.Items.Clear();
                ddlDepo.Items.Add(new ListItem("Tümü", ""));
                while (dr.Read())
                {
                    ddlDepo.Items.Add(new ListItem(dr["Depo"].ToString(), dr["Depo"].ToString()));
                }
            }
        }

        private void UrunleriGetir(string filtre = "", DateTime? baslangic = null, DateTime? bitis = null, string tedarikci = "", string depo = "")
        {
            string connStr = ConfigurationManager.ConnectionStrings["StokDb"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT UrunID, UrunAdi, Aciklama, StokAdet, Fiyat, OlusturmaTarihi, SiparisNo, IstekYapan, Tedarikci, Depo 
                                 FROM Urunler WHERE 1=1";

                if (!string.IsNullOrEmpty(filtre))
                    query += " AND (UrunAdi LIKE @Filtre + '%' OR SiparisNo LIKE @Filtre + '%')";
                if (baslangic.HasValue)
                    query += " AND OlusturmaTarihi >= @Baslangic";
                if (bitis.HasValue)
                    query += " AND OlusturmaTarihi < @Bitis";
                if (!string.IsNullOrEmpty(tedarikci))
                    query += " AND Tedarikci = @Tedarikci";
                if (!string.IsNullOrEmpty(depo))
                    query += " AND ISNULL(Depo,'-') = @Depo";

                query += " ORDER BY UrunID DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(filtre)) cmd.Parameters.AddWithValue("@Filtre", filtre);
                    if (baslangic.HasValue) cmd.Parameters.AddWithValue("@Baslangic", baslangic.Value.Date);
                    if (bitis.HasValue) cmd.Parameters.AddWithValue("@Bitis", bitis.Value.Date.AddDays(1));
                    if (!string.IsNullOrEmpty(tedarikci)) cmd.Parameters.AddWithValue("@Tedarikci", tedarikci);
                    if (!string.IsNullOrEmpty(depo)) cmd.Parameters.AddWithValue("@Depo", depo);

                    conn.Open();
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    gvUrunler.DataSource = dt;
                    gvUrunler.DataBind();
                }
            }
        }

        protected void gvUrunler_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvUrunler.EditIndex = e.NewEditIndex;
            UrunleriGetir(txtArama.Text.Trim(),
                DateTime.TryParse(txtBaslangic.Text, out DateTime b) ? b : (DateTime?)null,
                DateTime.TryParse(txtBitis.Text, out DateTime bit) ? bit : (DateTime?)null,
                ddlTedarikci.SelectedValue, ddlDepo.SelectedValue);
        }

        protected void gvUrunler_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvUrunler.EditIndex = -1;
            UrunleriGetir(txtArama.Text.Trim(),
                DateTime.TryParse(txtBaslangic.Text, out DateTime b) ? b : (DateTime?)null,
                DateTime.TryParse(txtBitis.Text, out DateTime bit) ? bit : (DateTime?)null,
                ddlTedarikci.SelectedValue, ddlDepo.SelectedValue);
        }

        protected void gvUrunler_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int urunId = Convert.ToInt32(gvUrunler.DataKeys[e.RowIndex].Value);
            GridViewRow row = gvUrunler.Rows[e.RowIndex];

            string urunAdi = ((TextBox)row.FindControl("txtUrunAdi")).Text.Trim();
            string aciklama = ((TextBox)row.FindControl("txtAciklama")).Text.Trim();
            int yeniStok = int.Parse(((TextBox)row.FindControl("txtStokAdet")).Text.Trim());
            decimal fiyat = decimal.Parse(((TextBox)row.FindControl("txtFiyat")).Text.Trim());
            DateTime tarih = DateTime.Parse(((TextBox)row.FindControl("txtOlusturmaTarihi")).Text.Trim());
            string siparisNo = ((TextBox)row.FindControl("txtSiparisNo")).Text.Trim();
            string istekYapan = ((TextBox)row.FindControl("txtIstekYapan")).Text.Trim();
            string tedarikci = ((TextBox)row.FindControl("txtTedarikci")).Text.Trim();
            string depo = ((TextBox)row.FindControl("txtDepo")).Text.Trim();

            string connStr = ConfigurationManager.ConnectionStrings["StokDb"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                int eskiStok = 0;
                using (SqlCommand cmdEski = new SqlCommand("SELECT StokAdet FROM Urunler WHERE UrunID=@UrunID", conn))
                {
                    cmdEski.Parameters.AddWithValue("@UrunID", urunId);
                    object result = cmdEski.ExecuteScalar();
                    if (result != null) eskiStok = Convert.ToInt32(result);
                }

                string updateQuery = @"UPDATE Urunler 
                                       SET UrunAdi=@UrunAdi, Aciklama=@Aciklama, StokAdet=@StokAdet, Fiyat=@Fiyat, 
                                           OlusturmaTarihi=@Tarih, SiparisNo=@SiparisNo, IstekYapan=@IstekYapan, 
                                           Tedarikci=@Tedarikci, Depo=@Depo
                                       WHERE UrunID=@UrunID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UrunAdi", urunAdi);
                    cmd.Parameters.AddWithValue("@Aciklama", aciklama);
                    cmd.Parameters.AddWithValue("@StokAdet", yeniStok);
                    cmd.Parameters.AddWithValue("@Fiyat", fiyat);
                    cmd.Parameters.AddWithValue("@Tarih", tarih);
                    cmd.Parameters.AddWithValue("@SiparisNo", siparisNo);
                    cmd.Parameters.AddWithValue("@IstekYapan", istekYapan);
                    cmd.Parameters.AddWithValue("@Tedarikci", tedarikci);
                    cmd.Parameters.AddWithValue("@Depo", string.IsNullOrEmpty(depo) ? "-" : depo);
                    cmd.Parameters.AddWithValue("@UrunID", urunId);
                    cmd.ExecuteNonQuery();
                }

                int fark = yeniStok - eskiStok;
                if (fark != 0)
                {
                    string hareketTipi = fark > 0 ? "Giriş" : "Çıkış";
                    int miktar = Math.Abs(fark);

                    string insertHareket = @"INSERT INTO UrunHareketleri 
                                             (UrunID, HareketTipi, Miktar, Tarih, Aciklama, Depo) 
                                             VALUES (@UrunID, @HareketTipi, @Miktar, @Tarih, @Aciklama, @Depo)";
                    using (SqlCommand cmdH = new SqlCommand(insertHareket, conn))
                    {
                        cmdH.Parameters.AddWithValue("@UrunID", urunId);
                        cmdH.Parameters.AddWithValue("@HareketTipi", hareketTipi);
                        cmdH.Parameters.AddWithValue("@Miktar", miktar);
                        cmdH.Parameters.AddWithValue("@Tarih", DateTime.Now);
                        cmdH.Parameters.AddWithValue("@Aciklama", "Stok güncelleme");
                        cmdH.Parameters.AddWithValue("@Depo", string.IsNullOrEmpty(depo) ? "-" : depo);
                        cmdH.ExecuteNonQuery();
                    }
                }
            }

            gvUrunler.EditIndex = -1;
            UrunleriGetir(txtArama.Text.Trim(),
                DateTime.TryParse(txtBaslangic.Text, out DateTime b) ? b : (DateTime?)null,
                DateTime.TryParse(txtBitis.Text, out DateTime bit) ? bit : (DateTime?)null,
                ddlTedarikci.SelectedValue, ddlDepo.SelectedValue);
        }

        protected void gvUrunler_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int urunId = Convert.ToInt32(gvUrunler.DataKeys[e.RowIndex].Value);
            string connStr = ConfigurationManager.ConnectionStrings["StokDb"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Urunler WHERE UrunID=@UrunID", conn);
                cmd.Parameters.AddWithValue("@UrunID", urunId);
                cmd.ExecuteNonQuery();
            }

            UrunleriGetir(txtArama.Text.Trim(),
                DateTime.TryParse(txtBaslangic.Text, out DateTime b) ? b : (DateTime?)null,
                DateTime.TryParse(txtBitis.Text, out DateTime bit) ? bit : (DateTime?)null,
                ddlTedarikci.SelectedValue, ddlDepo.SelectedValue);
        }

        protected void gvUrunler_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string rol = Session["Rol"]?.ToString() ?? "";
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (rol != "Admin")
                {
                    foreach (Control ctrl in e.Row.Cells[0].Controls)
                    {
                        if (ctrl is LinkButton lb && (lb.CommandName == "Edit" || lb.CommandName == "Delete"))
                            lb.Visible = false;
                    }
                }

                int stok = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "StokAdet"));
                if (stok <= 5)
                {
                    e.Row.BackColor = System.Drawing.Color.LightCoral;
                    e.Row.ForeColor = System.Drawing.Color.White;
                    e.Row.Font.Bold = true;
                }
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                DataTable dt = gvUrunler.DataSource as DataTable;
                if (dt != null)
                {
                    e.Row.Cells[1].Text = "Toplam:";
                    e.Row.Cells[3].Text = dt.AsEnumerable().Sum(r => r.Field<int>("StokAdet")).ToString();
                    decimal toplamTutar = dt.AsEnumerable().Sum(r => r.Field<int>("StokAdet") * r.Field<decimal>("Fiyat"));
                    e.Row.Cells[4].Text = toplamTutar.ToString("N2") + " ₺";
                }
            }
        }

        protected void txtArama_TextChanged(object sender, EventArgs e)
        {
            UrunleriGetir(txtArama.Text.Trim(),
                DateTime.TryParse(txtBaslangic.Text, out DateTime b) ? b : (DateTime?)null,
                DateTime.TryParse(txtBitis.Text, out DateTime bit) ? bit : (DateTime?)null,
                ddlTedarikci.SelectedValue, ddlDepo.SelectedValue);
        }

        protected void btnTarihFiltre_Click(object sender, EventArgs e)
        {
            DateTime? baslangic = null, bitis = null;
            if (DateTime.TryParse(txtBaslangic.Text, out DateTime b)) baslangic = b;
            if (DateTime.TryParse(txtBitis.Text, out DateTime bit)) bitis = bit;
            if (baslangic.HasValue && bitis.HasValue && baslangic > bitis)
            {
                lblTarihUyari.Text = "Başlangıç tarihi bitişten büyük olamaz!";
                return;
            }
            lblTarihUyari.Text = "";
            UrunleriGetir(txtArama.Text.Trim(), baslangic, bitis, ddlTedarikci.SelectedValue, ddlDepo.SelectedValue);
        }

        protected void ddlTedarikci_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime? baslangic = null, bitis = null;
            if (DateTime.TryParse(txtBaslangic.Text, out DateTime b)) baslangic = b;
            if (DateTime.TryParse(txtBitis.Text, out DateTime bit)) bitis = bit;
            UrunleriGetir(txtArama.Text.Trim(), baslangic, bitis, ddlTedarikci.SelectedValue, ddlDepo.SelectedValue);
        }

        protected void ddlDepo_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime? baslangic = null, bitis = null;
            if (DateTime.TryParse(txtBaslangic.Text, out DateTime b)) baslangic = b;
            if (DateTime.TryParse(txtBitis.Text, out DateTime bit)) bitis = bit;
            UrunleriGetir(txtArama.Text.Trim(), baslangic, bitis, ddlTedarikci.SelectedValue, ddlDepo.SelectedValue);
        }

        protected void btnEkleyeGit_Click(object sender, EventArgs e)
        {
            Response.Redirect("UrunEkle.aspx");
        }

        protected void btnHareketler_Click(object sender, EventArgs e)
        {
            Response.Redirect("UrunHareketleri.aspx");
        }

        protected void btnExcelAktar_Click(object sender, EventArgs e)
        {
         
        }

        protected void btnCikis_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        protected void btnRapor_Click(object sender, EventArgs e)
        {
            Response.Redirect("UrunRapor.aspx");
        }
    }
}
