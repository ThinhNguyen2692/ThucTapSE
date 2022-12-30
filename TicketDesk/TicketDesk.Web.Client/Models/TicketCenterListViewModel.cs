// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (https://github.com/stephenredd)
//
// This file is distributed under the terms of the Microsoft Public 
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use. 
//
// For any distribution that contains code from this file, this notice of 
// attribution must remain intact, and a copy of the license must be 
// provided to the recipient.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using PagedList;
using TicketDesk.Domain;
using TicketDesk.Domain.Model;
using TicketDesk.Localization.Controllers;
using TicketDesk.Web.Identity;

namespace TicketDesk.Web.Client.Models
{
    public class TicketCenterListViewModel
    {
        private FilterBarViewModel _filterBar;

        public static async Task<TicketCenterListViewModel> GetViewModelAsync(int currentPage, string listName, TdDomainContext context, string userId)
        {
            var userSettings = await context.UserSettingsManager.GetSettingsForUserAsync(userId);
           
            var vm = new TicketCenterListViewModel()
            {
                UserListSettings = userSettings.ListSettings.OrderBy(
                        lp => lp.ListMenuDisplayOrder),
                CurrentPage = currentPage,
                CurrentListSetting = userSettings.GetUserListSettingByName(listName)
            };


            vm.Tickets = await vm.ListTicketsAsync(currentPage, context);

            return vm;
        }

        //danh sách xuất excel 
        //danh sách xuất excel 
        public static async Task<IList<Ticket>> GetViewModelAsync(string listName, TdDomainContext context, string userId)
        {
            var userSettings = await context.UserSettingsManager.GetSettingsForUserAsync(userId);
            var vm = new TicketCenterListViewModel()
            {
                UserListSettings = userSettings.ListSettings.OrderBy(
                        lp => lp.ListMenuDisplayOrder),
                CurrentPage = 20,
                CurrentListSetting = userSettings.GetUserListSettingByName(listName)
            };
            var list = await vm.ExportListTicketsAsync(context);
            return  list;
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="TicketCenterListViewModel" /> class.
        /// </summary>
        private TicketCenterListViewModel()
        {
            DisplayProjects = false;
        }//private ctor

        public int CurrentPage { get; set; }

        public bool DisplayProjects { get; set; }

        public TicketDeskUserManager UserManager
        {
            get { return DependencyResolver.Current.GetService<TicketDeskUserManager>(); }
        }

        //lấy danh sách phân trang ticket
        public async Task<IPagedList<Ticket>> ListTicketsAsync(int pageIndex, TdDomainContext context)
        {
            var query = await GetTicketsQuery(context);
            var pageSize = CurrentListSetting.ItemsPerPage;
            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        //lấy danh sách xuất excel
        public async Task<IList<Ticket>> ExportListTicketsAsync(TdDomainContext context)
        {
            var filterColumns = CurrentListSetting.FilterColumns.ToList();
            var query = await GetTicketsQueryList(context);
            query = filterColumns.ApplyToQuery(query);
            return await query.ToListAsync();
        }

        public async Task<ObjectQuery<Ticket>> GetTicketsQuery(TdDomainContext context)
        {
            var filterColumns = CurrentListSetting.FilterColumns.ToList();
            //if filtering by project, add filter for selected project
            var projectId = await context.UserSettingsManager.GetUserSelectedProjectIdAsync(context);
            if (projectId != default(int))
            {
                filterColumns.Add(new UserTicketListFilterColumn("ProjectId", true, projectId));
            }
            DisplayProjects = context.Projects.Count() > 1;
            var sortColumns = CurrentListSetting.SortColumns.ToList();
            var query = context.GetObjectQueryFor(context.Tickets);
            query = filterColumns.ApplyToQuery(query);
            query = sortColumns.ApplyToQuery(query);
            return query;
        }

        public async Task<ObjectQuery<Ticket>> GetTicketsQueryList(TdDomainContext context)
        {
            var query = context.GetObjectQueryFor(context.Tickets);
            return query;
        }




        public async Task<IPagedList<Ticket>> ListTicketsProjectAsync(int pageIndex, int projectId,TdDomainContext context)
        {
            var filterColumns = CurrentListSetting.FilterColumns.ToList();

            //if filtering by project, add filter for selected project
            
            if (projectId != default(int))
            {
                filterColumns.Add(new UserTicketListFilterColumn("ProjectId", true, projectId));
            }

            DisplayProjects = context.Projects.Count() > 1;


            var sortColumns = CurrentListSetting.SortColumns.ToList();
            var pageSize = CurrentListSetting.ItemsPerPage;
            var query = context.GetObjectQueryFor(context.Tickets);

            query = filterColumns.ApplyToQuery(query);
            query = sortColumns.ApplyToQuery(query);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }


        /// <summary>
        /// Gets or (private) sets the filter bar model.
        /// </summary>
        /// <value>The filter bar.</value>
        public FilterBarViewModel FilterBar
        {
            get { return _filterBar ?? (_filterBar = new FilterBarViewModel(CurrentListSetting)); }
        }

        /// <summary>
        /// Gets or (private) sets the list of tickets for the view.
        /// </summary>
        /// <value>The tickets.</value>
        public IPagedList<Ticket> Tickets { get; private set; }


        public IList<Ticket> ListTickets { get; private set; }

        public UserTicketListSetting CurrentListSetting { get; private set; }

        public List<ProjectUser> Projects { get; set; }

        public IOrderedEnumerable<UserTicketListSetting> UserListSettings { get; private set; }


     
    }


   
}