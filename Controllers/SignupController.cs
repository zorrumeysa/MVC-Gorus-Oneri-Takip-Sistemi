﻿using MVCGorusOneriSistemiApp.Models;
using Newtonsoft.Json;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System;

namespace MVCGorusOneriSistemiApp.Content
{
    public class SignupController : Controller
    {
        GorusOneriSistemiDBEntities1 entity = new GorusOneriSistemiDBEntities1();
        // GET: Signup
        public ActionResult Index()
        {
            ViewBag.resim = ResimOlustur();
            ViewBag.hataMesaj = null;
            return View();
        }


        [HttpPost]
        public ActionResult Index(string email, string parola, string parolaTekrar, string guvenlikKod)
        {
            string kod = Session["kod"].ToString();

            if (kod == guvenlikKod && parola == parolaTekrar)
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                bool parolaKontrolDeger = ParolaKontrol(parola);

                if (parolaKontrolDeger == true)
                {
                    var uyeKontrol = entity.Uyeler.Where(u => u.uyeEmail == email).FirstOrDefault();
                    if (uyeKontrol != null)
                    {
                        ViewBag.hataMesaj = "Bu email hesabına sahip kullanıcı sistemde bulunuyor.";
                        ViewBag.resim = ResimOlustur();
                        return View();
                    }



                    try
                    {
                        string sifrelenmisVeri = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(parola)));

                        Uyeler uye = new Uyeler();
                        uye.uyeEmail = email;
                        uye.uyeParola = sifrelenmisVeri;
                        uye.uyeAktiflik = true;
                        uye.uyelikTarihi = DateTime.Now;

                        entity.Uyeler.Add(uye);
                        entity.SaveChanges();

                        Session.Add("uyeId", uye.uyeId);

                        return RedirectToAction("Index", "Uye");
                    }
                    catch (Exception)
                    {

                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewBag.hataMesaj = "Parola en az 6 karakter en az bir harf bir rakam bir alfanumerik karakter içermelidir.";
                    ViewBag.resim = ResimOlustur();
                    return View();
                }

            }
            else
            {
                ViewBag.hataMesaj = "Güvenlik kodu ya da parola eşleşmedi.";
                ViewBag.resim = ResimOlustur();
                return View();
            }
        }
        public ActionResult Login()
        {
            ViewBag.hataMesaj = null;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string parola)
        {
            var response = Request["g-recaptcha-response"];
            const string secret = "6LeMSOsqAAAAAJP6RIS2X-C6ZH__3bEK8aLSJJYR";

            var client = new WebClient();
            var reply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));


            var captchaResponse = JsonConvert.DeserializeObject<RECaptcha>(reply);

            if (captchaResponse.Success)
            {
                //giriş işlemleri

                SHA1 sha = new SHA1CryptoServiceProvider();

                string sifrelenmisVeri = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(parola)));

                var uye = (from u in entity.Uyeler
                           where u.uyeEmail == email && u.uyeParola == sifrelenmisVeri &&
                           u.uyeAktiflik == true
                           select u).FirstOrDefault();
                if (uye != null)
                {
                    Session.Add("uyeId", uye.uyeId);
                    return RedirectToAction("Index", "Uye");
                }
                else
                {
                    ViewBag.hataMesaj = "Sistemde kullanıcı bulunamadı.";
                    return View();
                }
            }
            else
            {
                //güvenlik doğrulama
                ViewBag.hataMesaj = "Lütfen güvenliği doğrulayınız.";
                return View();
            }

        }

        [HttpPost]
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }
        public string RastgeleVeriUret()
        {
            string deger = "";
            string dizi = "ABCDEFGHIJKLMNOPRSTUVYZ0123456789";

            Random r = new Random();
            for (int i = 0; i < 5; i++)
            {
                deger = deger + dizi[r.Next(0, 33)];
            }
            return deger;
        }
        public string ResimOlustur()
        {
            string kod = "";
            kod = RastgeleVeriUret();
            Session.Add("kod", kod);
            Bitmap bmp = new Bitmap(100, 21);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawString(kod, new Font("Comic Sanns MS", 15), new SolidBrush(Color.Black), 20, 0);
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            var base64Data = Convert.ToBase64String(ms.ToArray());
            string ImageUrl = "data:image/png;base64," + base64Data;

            g.Dispose();
            bmp.Dispose();
            ms.Close();

            return ImageUrl;
        }

        public bool ParolaKontrol(string parola)
        {
            if (parola.Length <= 6) return false;
            bool alfanumerikMi = false;
            bool harfMi = false;
            bool rakamMi = false;

            string alfalar = "@$,.*?+&!/-\\";

            for (int n = 0; n < parola.Length; n++)
            {
                char a = Convert.ToChar(parola.Substring(n, 1));

                if (Char.IsLetter(a))
                {
                    harfMi = true;
                }
                else if (Char.IsDigit(a))
                {
                    rakamMi = true;
                }
                else if (alfalar.Contains(a))
                {
                    alfanumerikMi = true;
                }
            }

            if (alfanumerikMi == true && harfMi == true && rakamMi == true)
                return true;
            else return false;



        }
    }
}