using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketDesk.Domain;
using TicketDesk.Domain.Model;
using TicketDesk.PushNotifications;
using TicketDesk.PushNotifications.Model;
using TicketDesk.Web.Identity;

namespace TicketDesk.Web.Client.Models.Extensions
{
    public static class NotifyUser
    {
        private static string _rootUrl;
        public static string RootUrl
        {
            get
            {
                if (_rootUrl == null)
                {
                    using (var context = DependencyResolver.Current.GetService<TdDomainContext>())
                    {
                        _rootUrl = context.TicketDeskSettings.ClientSettings.GetDefaultSiteRootUrl();
                    }
                }
                return _rootUrl;
            }

        }


      
    }
}