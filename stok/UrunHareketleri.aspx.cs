using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace stok
{
    public partial class UrunHareketleri : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null)
            {
                Response.Redirect("Login.aspx"); 
            }

            if (!IsPostBack)
            {
                ddlHareketTipi.Items.Add(new ListItem("Tümü", ""));
                ddlHareketTipi.Items.Add(new ListItem("Giriş", "Giriş"));
                ddlHareketTipi.Items.Add(new ListItem("Çıkış", "Çıkış"));
                HareketleriGetir();
            }
        }


        private void HareketleriGetir(string filtre = "", string hareketTipi = "", DateTime? baslangic = null, DateTime? bitis = null)
        {
            string connStr = ConfigurationManager.ConnectionStrings["StokDb"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = @"
            SELECT 
                h.HareketID,
                ISNULL(u.UrunAdi, 'Silinmiş Ürün') AS UrunAdi,
                h.HareketTipi,
                h.Miktar,
                h.Tarih,
                h.Aciklama,
                u.SiparisNo,
                u.IstekYapan,
                u.Tedarikci,
                ISNULL(u.Depo, '-') AS Depo
            FROM UrunHareketleri h
            LEFT JOIN Urunler u ON h.UrunID = u.UrunID
            WHERE 1=1
        ";

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(filtre))
                {
                    query += " AND (ISNULL(u.UrunAdi, 'Silinmiş Ürün') LIKE @Filtre + '%' OR u.SiparisNo LIKE @Filtre + '%')";
                    cmd.Parameters.AddWithValue("@Filtre", filtre);
                }

                if (!string.IsNullOrEmpty(hareketTipi))
                {
                    query += " AND h.HareketTipi = @HareketTipi";
                    cmd.Parameters.AddWithValue("@HareketTipi", hareketTipi);
                }

                if (baslangic.HasValue)
                {
                    query += " AND h.Tarih >= @Baslangic";
                    cmd.Parameters.AddWithValue("@Baslangic", baslangic.Value.Date);
                }

                if (bitis.HasValue)
                {
                    query += " AND h.Tarih < @Bitis";
                    cmd.Parameters.AddWithValue("@Bitis", bitis.Value.Date.AddDays(1));
                }

                query += " ORDER BY h.Tarih DESC";
                cmd.CommandText = query;

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                gvHareketler.DataSource = dt;
                gvHareketler.DataBind();
            }
        }

        private void Filtrele()
        {
            string filtre = txtArama.Text.Trim();
            string hareketTipi = ddlHareketTipi.SelectedValue;

            DateTime? baslangic = null;
            DateTime temp;
            if (DateTime.TryParse(txtBaslangic.Text, out temp))
                baslangic = temp;

            DateTime? bitis = null;
            if (DateTime.TryParse(txtBitis.Text, out temp))
                bitis = temp;

            if (baslangic.HasValue && bitis.HasValue && baslangic > bitis)
            {
                lblTarihUyari.Text = "Başlangıç tarihi bitişten büyük olamaz!";
                return;
            }
            lblTarihUyari.Text = "";

            HareketleriGetir(filtre, hareketTipi, baslangic, bitis);
        }

        protected void txtArama_TextChanged(object sender, EventArgs e) => Filtrele();
        protected void ddlHareketTipi_SelectedIndexChanged(object sender, EventArgs e) => Filtrele();
        protected void btnTarihFiltre_Click(object sender, EventArgs e) => Filtrele();
        protected void btnGeri_Click(object sender, EventArgs e)
        {
            Response.Redirect("UrunListe.aspx");
        }

        protected void gvHareketler_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (DateTime.TryParse(e.Row.Cells[4].Text, out DateTime tarih))
                    e.Row.Cells[4].Text = tarih.ToString("dd.MM.yyyy HH:mm");

                if (int.TryParse(e.Row.Cells[3].Text, out int miktar) && miktar == 0)
                {
                    e.Row.BackColor = System.Drawing.Color.LightYellow;
                    e.Row.ToolTip = "Ürün silindi veya hareket miktarı 0";
                }

                string hareketTipi = e.Row.Cells[2].Text;
                if (hareketTipi == "Giriş")
                    e.Row.ForeColor = System.Drawing.Color.Green;
                else if (hareketTipi == "Çıkış")
                    e.Row.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
