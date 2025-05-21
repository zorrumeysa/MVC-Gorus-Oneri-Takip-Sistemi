using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCGorusOneriSistemiApp.Models
{
    public class YetkiTalepler
    {
        public int id { get; set; }

        public int? birimId { get; set; }
        [Display(Name ="Talep Başlık")]

        public string talepBaslik { get; set; }
        [Display(Name = "Tarih")]

        public DateTime olusturulmaTarihi { get; set; }

        [Display(Name = "Birim Ad")]
        public string birimAd {  get; set; }
        
    }
}