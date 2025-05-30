﻿using MVCGorusOneriSistemiApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCGorusOneriSistemiApp.Filters
{
    public class UyeAdKontrol:FilterAttribute, IAuthorizationFilter
    {
        GorusOneriSistemiDBEntities1 entities1 = new GorusOneriSistemiDBEntities1();
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            int uyeId = Convert.ToInt32(filterContext.HttpContext.Session["uyeId"]);

            var uye = (from u in entities1.Uyeler where u.uyeId == uyeId select u).FirstOrDefault();
            if (uye == null)
            {
                filterContext.Result = new RedirectResult("~/Signup/Login");
            }

            try
            {
                if (uye.uyeAdSoyad == null || uye.uyeAdSoyad == "")
                {
                    filterContext.Result = new RedirectResult("~/Uye/Index");
                }
            }
            catch (Exception ex)
            {
                filterContext.Result = new RedirectResult("~/Signup/Login");

            }

        }
    }
}