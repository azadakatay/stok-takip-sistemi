using System;
using System.Configuration;
using System.Data.SqlClient;

namespace stok
{
    public partial class UrunEkle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null)
            {
                Response.Redirect("Login.aspx"); 
            }

            if (Session["Rol"].ToString() == "user")
            {
                Response.Redirect("UrunListe.aspx"); 
            }

            if (!IsPostBack)
            {
               
            }
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            lblMesaj.ForeColor = System.Drawing.Color.Red;

            if (string.IsNullOrWhiteSpace(txtUrunAdi.Text) ||
                string.IsNullOrWhiteSpace(txtMiktar.Text) ||
                string.IsNullOrWhiteSpace(txtFiyat.Text) ||
                string.IsNullOrWhiteSpace(txtTarih.Text) ||
                string.IsNullOrWhiteSpace(txtDepo.Text))
            {
                lblMesaj.Text = "Lütfen tüm alanları doldurun.";
                return;
            }

            if (!int.TryParse(txtMiktar.Text.Trim(), out int miktar) || miktar <= 0)
            {
                lblMesaj.Text = "Miktar 0’dan büyük bir sayı olmalıdır!";
                return;
            }

            if (!decimal.TryParse(txtFiyat.Text.Trim(), out decimal fiyat) || fiyat < 0)
            {
                lblMesaj.Text = "Fiyat geçerli bir sayı olmalıdır!";
                return;
            }

            if (!DateTime.TryParse(txtTarih.Text, out DateTime tarih))
            {
                lblMesaj.Text = "Tarih geçerli bir formatta olmalıdır!";
                return;
            }

            try
            {
                string cs = ConfigurationManager.ConnectionStrings["StokDb"].ConnectionString;
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();

                    string kontrolQuery = "SELECT UrunID, StokAdet FROM Urunler WHERE LOWER(UrunAdi)=@UrunAdi";
                    using (SqlCommand cmdKontrol = new SqlCommand(kontrolQuery, con))
                    {
                        cmdKontrol.Parameters.AddWithValue("@UrunAdi", txtUrunAdi.Text.Trim().ToLower());
                        var reader = cmdKontrol.ExecuteReader();

                        if (reader.Read())
                        {
                            int urunID = reader.GetInt32(0);
                            int eskiStok = reader.GetInt32(1);
                            reader.Close();

                            string updateQuery = @"UPDATE Urunler 
                                                   SET StokAdet=@StokAdet, SiparisNo=@SiparisNo, IstekYapan=@IstekYapan, 
                                                       Tedarikci=@Tedarikci, Depo=@Depo
                                                   WHERE UrunID=@UrunID";

                            using (SqlCommand cmdUpdate = new SqlCommand(updateQuery, con))
                            {
                                cmdUpdate.Parameters.AddWithValue("@StokAdet", eskiStok + miktar);
                                cmdUpdate.Parameters.AddWithValue("@UrunID", urunID);
                                cmdUpdate.Parameters.AddWithValue("@SiparisNo", txtSiparisNo.Text.Trim());
                                cmdUpdate.Parameters.AddWithValue("@IstekYapan", txtIstekYapan.Text.Trim());
                                cmdUpdate.Parameters.AddWithValue("@Tedarikci", txtTedarikci.Text.Trim());
                                cmdUpdate.Parameters.AddWithValue("@Depo", txtDepo.Text.Trim());
                                cmdUpdate.ExecuteNonQuery();
                            }

                            string insertHareket = @"INSERT INTO UrunHareketleri 
                                                     (UrunID, HareketTipi, Miktar, Tarih, Aciklama, Depo)
                                                     VALUES (@UrunID, 'Giriş', @Miktar, @Tarih, @Aciklama, @Depo)";
                            using (SqlCommand cmdHareket = new SqlCommand(insertHareket, con))
                            {
                                cmdHareket.Parameters.AddWithValue("@UrunID", urunID);
                                cmdHareket.Parameters.AddWithValue("@Miktar", miktar);
                                cmdHareket.Parameters.AddWithValue("@Tarih", DateTime.Now);
                                cmdHareket.Parameters.AddWithValue("@Aciklama", $"Stok güncellemesi: {txtUrunAdi.Text}");
                                cmdHareket.Parameters.AddWithValue("@Depo", txtDepo.Text.Trim());
                                cmdHareket.ExecuteNonQuery();
                            }

                            lblMesaj.ForeColor = System.Drawing.Color.Green;
                            lblMesaj.Text = $"Stok güncellendi: {txtUrunAdi.Text} (+{miktar}).";
                        }
                        else
                        {
                            reader.Close();
                            string insertQuery = @"INSERT INTO Urunler 
                                                   (UrunAdi, Aciklama, StokAdet, Fiyat, OlusturmaTarihi, SiparisNo, IstekYapan, Tedarikci, Depo)
                                                   VALUES (@UrunAdi, @Aciklama, @StokAdet, @Fiyat, @OlusturmaTarihi, @SiparisNo, @IstekYapan, @Tedarikci, @Depo);
                                                   SELECT SCOPE_IDENTITY();";

                            int yeniUrunID;
                            using (SqlCommand cmdInsert = new SqlCommand(insertQuery, con))
                            {
                                cmdInsert.Parameters.AddWithValue("@UrunAdi", txtUrunAdi.Text.Trim());
                                cmdInsert.Parameters.AddWithValue("@Aciklama", txtAciklama.Text.Trim());
                                cmdInsert.Parameters.AddWithValue("@StokAdet", miktar);
                                cmdInsert.Parameters.AddWithValue("@Fiyat", fiyat);
                                cmdInsert.Parameters.AddWithValue("@OlusturmaTarihi", tarih);
                                cmdInsert.Parameters.AddWithValue("@SiparisNo", txtSiparisNo.Text.Trim());
                                cmdInsert.Parameters.AddWithValue("@IstekYapan", txtIstekYapan.Text.Trim());
                                cmdInsert.Parameters.AddWithValue("@Tedarikci", txtTedarikci.Text.Trim());
                                cmdInsert.Parameters.AddWithValue("@Depo", txtDepo.Text.Trim());

                                yeniUrunID = Convert.ToInt32(cmdInsert.ExecuteScalar());
                            }

                            string insertHareket = @"INSERT INTO UrunHareketleri 
                                                     (UrunID, HareketTipi, Miktar, Tarih, Aciklama, Depo)
                                                     VALUES (@UrunID, 'Giriş', @Miktar, @Tarih, @Aciklama, @Depo)";
                            using (SqlCommand cmdHareket = new SqlCommand(insertHareket, con))
                            {
                                cmdHareket.Parameters.AddWithValue("@UrunID", yeniUrunID);
                                cmdHareket.Parameters.AddWithValue("@Miktar", miktar);
                                cmdHareket.Parameters.AddWithValue("@Tarih", DateTime.Now);
                                cmdHareket.Parameters.AddWithValue("@Aciklama", "Yeni ürün eklendi");
                                cmdHareket.Parameters.AddWithValue("@Depo", txtDepo.Text.Trim());
                                cmdHareket.ExecuteNonQuery();
                            }

                            lblMesaj.ForeColor = System.Drawing.Color.Green;
                            lblMesaj.Text = "Yeni ürün başarıyla eklendi.";
                        }
                    }
                }

                txtUrunAdi.Text = "";
                txtAciklama.Text = "";
                txtMiktar.Text = "";
                txtFiyat.Text = "";
                txtTarih.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtSiparisNo.Text = "";
                txtIstekYapan.Text = "";
                txtTedarikci.Text = "";
                txtDepo.Text = "";
            }
            catch (Exception ex)
            {
                lblMesaj.Text = "Hata: " + ex.Message;
            }
        }

        protected void btnListeyeGit_Click(object sender, EventArgs e)
        {
            Response.Redirect("UrunListe.aspx");
        }
    }
}
