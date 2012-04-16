﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;
using Machete.Web.Helpers;
using Machete.Service;
using Machete.Domain;

namespace Machete.Web.Controllers
{
    //    IDictionary<string, object> data = new RouteValueDictionary(result.Data);
    //Assert.AreEqual(0, data["iNewID"]);
    //Assert.AreEqual(null, data["sNewLabel"]);
    //Assert.AreEqual(null, data["sNewRef"]);
    public class MacheteController : Controller
    {
        private Logger log = LogManager.GetCurrentClassLogger();
        private LogEventInfo levent = new LogEventInfo(LogLevel.Debug, "Controller", "");
        /// <summary>
        /// Unified exception handling for controllers. NLog and json response.
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            string exceptionMsg = "";
            string modelerrors = null;
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            exceptionMsg = RootException.Get(filterContext.Exception, this.ToString());
            modelerrors = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            levent.Level = LogLevel.Error; levent.Message = this.ToString() + "EXCEPTIONS:" + exceptionMsg + "MODELERR:" + modelerrors;
            levent.Properties["RecordID"] = ModelState["ID"]; log.Log(levent);
            filterContext.Result = Json(new
            {
                status = exceptionMsg,
                rtnMessage = exceptionMsg,
                modelErrors = modelerrors,
                jobSuccess = false
            }, JsonRequestBehavior.AllowGet);
            filterContext.ExceptionHandled = true;
        }

        //TODO: rework into model 
        protected string _getSkillCodes(int eng, int? sk1, int? sk2, int? sk3)
        {
            string rtnstr = "E" + eng + " ";
            if (sk1 != null)
            {
                var lookup = LookupCache.getBySkillID((int)sk1);
                rtnstr = rtnstr + lookup.ltrCode + lookup.level + " ";
            }
            if (sk2 != null)
            {
                var lookup = LookupCache.getBySkillID((int)sk2);
                rtnstr = rtnstr + lookup.ltrCode + lookup.level + " ";
            }
            if (sk3 != null)
            {
                var lookup = LookupCache.getBySkillID((int)sk3);
                rtnstr = rtnstr + lookup.ltrCode + lookup.level;
            }
            return rtnstr;
        }
    }

}