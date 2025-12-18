using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace stok
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["KullaniciAdi"] = null;
                Session["Rol"] = null;
                lblMesaj.Text = string.Empty;
            }
        }

        protected void btnGiris_Click(object sender, EventArgs e)
        {
            lblMesaj.ForeColor = System.Drawing.Color.Red;

            string kullaniciAdi = txtKullaniciAdi.Text?.Trim();
            string sifre = txtSifre.Text ?? "";

            if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
            {
                lblMesaj.Text = "Lütfen kullanıcı adı ve şifre girin.";
                return;
            }

            try
            {
                string cs = ConfigurationManager.ConnectionStrings["StokDb"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(cs))
                {
                    string query = "SELECT Rol FROM Kullanicilar WHERE KullaniciAdi=@KullaniciAdi AND Sifre=@Sifre";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@KullaniciAdi", kullaniciAdi);
                        cmd.Parameters.AddWithValue("@Sifre", sifre);

                        conn.Open();
                        object rolObj = cmd.ExecuteScalar();

                        if (rolObj != null)
                        {
                            string rol = rolObj.ToString();

                            Session["KullaniciAdi"] = kullaniciAdi;
                            Session["Rol"] = rol;

                            Response.Redirect("UrunListe.aspx", false);
                        }
                        else
                        {
                            lblMesaj.Text = "Kullanıcı adı veya şifre hatalı.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMesaj.Text = "Giriş sırasında hata oluştu: " + ex.Message;
            }
        }
    }
}
