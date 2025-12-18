using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace stok
{
    public partial class UrunRapor : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlHareketTipi.Items.Add(new ListItem("Tümü", ""));
                ddlHareketTipi.Items.Add(new ListItem("Giriş", "Giriş"));
                ddlHareketTipi.Items.Add(new ListItem("Çıkış", "Çıkış"));

                DoldurUrunler();
                DoldurDepolar();
                Filtrele(null, null);
            }
        }

        private void DoldurUrunler()
        {
            string connStr = ConfigurationManager.ConnectionStrings["StokDb"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT UrunID, UrunAdi FROM Urunler ORDER BY UrunAdi", conn);
                SqlDataReader dr = cmd.ExecuteReader();
                ddlUrun.DataSource = dr;
                ddlUrun.DataTextField = "UrunAdi";
                ddlUrun.DataValueField = "UrunID";
                ddlUrun.DataBind();
                ddlUrun.Items.Insert(0, new ListItem("Tümü", ""));
            }
        }

        private void DoldurDepolar()
        {
            string connStr = ConfigurationManager.ConnectionStrings["StokDb"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT Depo FROM UrunHareketleri ORDER BY Depo", conn);
                SqlDataReader dr = cmd.ExecuteReader();
                ddlDepo.DataSource = dr;
                ddlDepo.DataTextField = "Depo";
                ddlDepo.DataValueField = "Depo";
                ddlDepo.DataBind();
                ddlDepo.Items.Insert(0, new ListItem("Tümü", ""));
            }
        }

        protected void Filtrele(object sender, EventArgs e)
        {
            string urunID = ddlUrun.SelectedValue;
            string depo = ddlDepo.SelectedValue;
            string hareketTipi = ddlHareketTipi.SelectedValue;

            DateTime? baslangic = null, bitis = null;
            if (DateTime.TryParse(txtBaslangic.Text, out DateTime tmp)) baslangic = tmp;
            if (DateTime.TryParse(txtBitis.Text, out tmp)) bitis = tmp;

            string where = " WHERE 1=1 ";
            if (!string.IsNullOrEmpty(urunID)) where += " AND h.UrunID=@urunID ";
            if (!string.IsNullOrEmpty(depo)) where += " AND h.Depo=@depo ";
            if (!string.IsNullOrEmpty(hareketTipi)) where += " AND h.HareketTipi=@HareketTipi ";
            if (baslangic.HasValue) where += " AND h.Tarih >= @baslangic ";
            if (bitis.HasValue) where += " AND h.Tarih < @bitis ";

            string connStr = ConfigurationManager.ConnectionStrings["StokDb"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                string orderBy = "h.Tarih DESC";
                if (!string.IsNullOrEmpty(ddlSirala.SelectedValue))
                {
                    string[] parts = ddlSirala.SelectedValue.Split('_');
                    if (parts.Length == 2)
                        orderBy = $"{parts[0]} {parts[1]}";
                    else
                        orderBy = ddlSirala.SelectedValue;
                }

                cmd.CommandText = $@"
                SELECT h.HareketID, u.UrunAdi, h.HareketTipi, h.Miktar, h.Tarih, h.Aciklama, h.Depo
                FROM UrunHareketleri h
                INNER JOIN Urunler u ON h.UrunID = u.UrunID
                {where} 
                ORDER BY {orderBy}";

                if (!string.IsNullOrEmpty(urunID)) cmd.Parameters.AddWithValue("@urunID", urunID);
                if (!string.IsNullOrEmpty(depo)) cmd.Parameters.AddWithValue("@depo", depo);
                if (!string.IsNullOrEmpty(hareketTipi)) cmd.Parameters.AddWithValue("@HareketTipi", hareketTipi);
                if (baslangic.HasValue) cmd.Parameters.AddWithValue("@baslangic", baslangic.Value.Date);
                if (bitis.HasValue) cmd.Parameters.AddWithValue("@bitis", bitis.Value.Date.AddDays(1));

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                ViewState["GridData"] = dt;

                gvRapor.DataSource = dt;
                gvRapor.DataBind();

                cmd.Parameters.Clear();
                int giris = HesaplaToplam("Giriş", urunID, depo, baslangic, bitis, cmd);
                int cikis = HesaplaToplam("Çıkış", urunID, depo, baslangic, bitis, cmd);
                int mevcutStok = giris - cikis;

                lblToplamGiris.Text = giris.ToString();
                lblToplamCikis.Text = cikis.ToString();
                lblToplamStok.Text = mevcutStok.ToString();
                lblStokUyari.Text = mevcutStok <= 5 ? "Stok kritik seviyede!" : "";
                lblDepoSayisi.Text = HesaplaDepoSayisi(urunID, depo, baslangic, bitis, cmd).ToString();
                lblToplamFiyat.Text = HesaplaToplamFiyat(urunID, depo, baslangic, bitis, cmd).ToString("N2");
            }
        }

        private int HesaplaToplam(string hareketTipi, string urunID, string depo, DateTime? baslangic, DateTime? bitis, SqlCommand cmd)
        {
            cmd.CommandText = "SELECT ISNULL(SUM(h.Miktar),0) FROM UrunHareketleri h WHERE 1=1";
            cmd.Parameters.Clear();
            if (!string.IsNullOrEmpty(urunID)) { cmd.CommandText += " AND h.UrunID=@urunID"; cmd.Parameters.AddWithValue("@urunID", urunID); }
            if (!string.IsNullOrEmpty(depo)) { cmd.CommandText += " AND h.Depo=@depo"; cmd.Parameters.AddWithValue("@depo", depo); }
            if (baslangic.HasValue) { cmd.CommandText += " AND h.Tarih >= @baslangic"; cmd.Parameters.AddWithValue("@baslangic", baslangic.Value.Date); }
            if (bitis.HasValue) { cmd.CommandText += " AND h.Tarih < @bitis"; cmd.Parameters.AddWithValue("@bitis", bitis.Value.Date.AddDays(1)); }
            cmd.CommandText += " AND h.HareketTipi=@HareketTipi";
            cmd.Parameters.AddWithValue("@HareketTipi", hareketTipi);

            object result = cmd.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }

        private int HesaplaDepoSayisi(string urunID, string depo, DateTime? baslangic, DateTime? bitis, SqlCommand cmd)
        {
            cmd.CommandText = "SELECT COUNT(DISTINCT Depo) FROM UrunHareketleri h WHERE 1=1";
            cmd.Parameters.Clear();
            if (!string.IsNullOrEmpty(urunID)) { cmd.CommandText += " AND h.UrunID=@urunID"; cmd.Parameters.AddWithValue("@urunID", urunID); }
            if (!string.IsNullOrEmpty(depo)) { cmd.CommandText += " AND h.Depo=@depo"; cmd.Parameters.AddWithValue("@depo", depo); }
            if (baslangic.HasValue) { cmd.CommandText += " AND h.Tarih >= @baslangic"; cmd.Parameters.AddWithValue("@baslangic", baslangic.Value.Date); }
            if (bitis.HasValue) { cmd.CommandText += " AND h.Tarih < @bitis"; cmd.Parameters.AddWithValue("@bitis", bitis.Value.Date.AddDays(1)); }

            object result = cmd.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }

        private decimal HesaplaToplamFiyat(string urunID, string depo, DateTime? baslangic, DateTime? bitis, SqlCommand cmd)
        {
            cmd.CommandText = "SELECT ISNULL(SUM(u.Fiyat * (CASE WHEN h.HareketTipi='Giriş' THEN h.Miktar ELSE -h.Miktar END)),0) " +
                              "FROM UrunHareketleri h INNER JOIN Urunler u ON h.UrunID = u.UrunID WHERE 1=1";
            cmd.Parameters.Clear();
            if (!string.IsNullOrEmpty(urunID)) { cmd.CommandText += " AND h.UrunID=@urunID"; cmd.Parameters.AddWithValue("@urunID", urunID); }
            if (!string.IsNullOrEmpty(depo)) { cmd.CommandText += " AND h.Depo=@depo"; cmd.Parameters.AddWithValue("@depo", depo); }
            if (baslangic.HasValue) { cmd.CommandText += " AND h.Tarih >= @baslangic"; cmd.Parameters.AddWithValue("@baslangic", baslangic.Value.Date); }
            if (bitis.HasValue) { cmd.CommandText += " AND h.Tarih < @bitis"; cmd.Parameters.AddWithValue("@bitis", bitis.Value.Date.AddDays(1)); }

            object result = cmd.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        protected void gvRapor_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int kritikMiktar = 5; 
                int stokMiktar = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Miktar"));

                Label lblMiktar = (Label)e.Row.FindControl("lblMiktar");
                if (lblMiktar != null)
                {
                    if (stokMiktar < kritikMiktar)
                    {
                        lblMiktar.Text = $"⚠️ {stokMiktar}";

                        e.Row.CssClass = "stok-dusuk";
                    }
                    else
                    {
                        lblMiktar.Text = stokMiktar.ToString();
                    }
                }
            }
        }

        protected void gvRapor_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRapor.PageIndex = e.NewPageIndex;
            gvRapor.DataSource = ViewState["GridData"];
            gvRapor.DataBind();
        }

        protected void ddlSirala_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ViewState["GridData"] == null) return;

            DataTable dt = ViewState["GridData"] as DataTable;
            if (dt.Rows.Count == 0) return;

            string[] parts = ddlSirala.SelectedValue.Split('_');
            string sortExpression = parts[0];
            string sortDirection = parts[1];

            dt.DefaultView.Sort = $"{sortExpression} {sortDirection}";
            gvRapor.DataSource = dt.DefaultView;
            gvRapor.DataBind();
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            Filtrele(null, null);

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=UrunRapor.xls");
            Response.Charset = "utf-8";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Response.ContentType = "application/vnd.ms-excel";

            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                gvRapor.RenderControl(hw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        protected void btnGeri_Click(object sender, EventArgs e)
        {
            Response.Redirect("UrunListe.aspx");
        }
    }
}
