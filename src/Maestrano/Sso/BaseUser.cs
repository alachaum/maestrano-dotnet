﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maestrano.Sso
{

    public class BaseUser
    {
        public string SsoSession { get; set; }
        public DateTime SsoSessionRecheck { get; set; }
        public string GroupUid { get; set; }
        public string GroupRole { get; set; }
        public string Uid { get; set; }
        public string VirtualUid { get; set; }
        public string Email { get; set; }
        public string VirtualEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string CompanyName { get; set; }


        /// <summary>
        /// Constructor loading user attributes from a Saml.Response
        /// </summary>
        /// <param name="samlResponse"></param>
        public BaseUser(Saml.Response samlResponse)
        {
            NameValueCollection att = samlResponse.GetAttributes();
            SsoSession = att["mno_session"];
            SsoSessionRecheck = DateTime.Parse(att["mno_session_recheck"]);
            GroupUid = att["group_uid"];
            GroupRole = att["group_role"];
            Uid = att["uid"];
            VirtualUid = att["virtual_uid"];
            Email = att["email"];
            VirtualEmail = att["virtual_email"];
            FirstName = att["name"];
            LastName = att["surname"];
            Country = att["country"];
            CompanyName = att["company_name"];

        }

        /// <summary>
        /// Return the real Uid if Maestrano.Sso.CreationMode is set
        /// to "real" and the VirtualUid otherwise ("virtual" mode)
        /// </summary>
        public string ToUid()
        {
            if (Maestrano.Sso.CreationMode.Equals("real"))
                return Uid;
            else
                return VirtualUid;
        }

        /// <summary>
        /// Return the real Email if Maestrano.Sso.CreationMode is set
        /// to "real" and the VirtualEmail otherwise ("virtual" mode)
        /// </summary>
        public string ToEmail()
        {
            if (Maestrano.Sso.CreationMode.Equals("real"))
                return Email;
            else
                return VirtualEmail;
        }

        /// <summary>
        /// Return a serializable dictionary describing the resource
        /// </summary>
        /// <returns></returns>
        public JObject ToHash()
        {
            return new JObject(
                new JProperty("provider", "maestrano"),
                new JProperty("uid", Uid),
                new JProperty("info", new JObject(
                    new JProperty("email", Email),
                    new JProperty("first_name", FirstName),
                    new JProperty("last_name", LastName),
                    new JProperty("country", Country),
                    new JProperty("company_name", CompanyName))),
                new JProperty("extra", new JObject(
                    new JProperty("uid", Email),
                    new JProperty("virtual_uid", FirstName),
                    new JProperty("real_email", LastName),
                    new JProperty("virtual_email", Country),
                    new JProperty("group", JObject(
                        new JProperty("uid", GroupUid),
                        new JProperty("role", GroupRole))),
                    new JProperty("session", JObject(
                        new JProperty("uid", Uid),
                        new JProperty("token", SsoSession),
                        new JProperty("recheck", SsoSessionRecheck),
                        new JProperty("group_uid", GroupUid)))
                        )
                )
             );
               
        }
    }
}