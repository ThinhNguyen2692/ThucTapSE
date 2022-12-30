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

using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicketDesk.Domain;
using TicketDesk.Domain.Model;
using TicketDesk.Web.Client.Models;
using TicketDesk.Localization;
using System.Reflection;
using System.Resources;
using System.Globalization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.IO;
using TicketDesk.Web.Identity;
using TicketDesk.Web.Client.Models.Extensions;
using System.Net.Http;
using System.Net;

namespace TicketDesk.Web.Client.Controllers
{
    [RoutePrefix("tickets")]
    [Route("{action=index}")]
    [TdAuthorize(Roles = "TdInternalUsers,TdHelpDeskUsers,TdAdministrators")]
    public class TicketCenterController : Controller
    {
        private TdDomainContext Context { get; set; }
     

        public TicketCenterController(TdDomainContext context)
        {
            Context = context;
           
        }

        [Route("reset-user-lists")]
        public async Task<ActionResult> ResetUserLists()
        {  
            var uId = Context.SecurityProvider.CurrentUserId;
            await Context.UserSettingsManager.ResetAllListSettingsForUserAsync(uId);
            var x = await Context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: TicketCenter
        [Route("")]
        [Route("{listName?}/{page:int?}")]
        public async Task<ActionResult> Index(int? page, string listName)
        {
            listName = listName ?? (Context.SecurityProvider.IsTdHelpDeskUser ? "assignedToMe" : "mytickets");
            var pageNumber = page ?? 1;

            var viewModel = await TicketCenterListViewModel.GetViewModelAsync(pageNumber, listName, Context, Context.SecurityProvider.CurrentUserId);//new TicketCenterListViewModel(listName, model, Context, User.Identity.GetUserId());
            //viewModel.Projects = await GetProjectsAsync(Context.SecurityProvider.CurrentUserId);
            return View(viewModel);
        }
        [Route("pageList/{listName=mytickets}/{page:int?}")]
        public async Task<ActionResult> PageList(int? page, string listName)
        {
            return await GetTicketListPartial(page, listName);
        }
        [Route("filterList/{listName=opentickets}/{page:int?}")]
        public async Task<PartialViewResult> FilterList(
            string listName,
            int pageSize,
            string ticketStatus,
            string owner,
            string assignedTo)
        {
           await FilterListAsync(listName, pageSize, ticketStatus, owner, assignedTo);
            return await GetTicketListPartial(null, listName);
        }
        /// <summary>
        /// Tach ta tu ham FilterList, tái sử dụng cho hanh đong excel
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="pageSize"></param>
        /// <param name="ticketStatus"></param>
        /// <param name="owner"></param>
        /// <param name="assignedTo"></param>
        /// <returns></returns>
        public async Task FilterListAsync(string listName,
            int pageSize,
            string ticketStatus,
            string owner,
            string assignedTo)
        {
            var uId = Context.SecurityProvider.CurrentUserId;
            var userSetting = await Context.UserSettingsManager.GetSettingsForUserAsync(uId);
            var currentListSetting = userSetting.GetUserListSettingByName(listName);
            currentListSetting.ModifyFilterSettings(pageSize, ticketStatus, owner, assignedTo);
            await Context.SaveChangesAsync();
        }
        [Route("sortList/{listName=opentickets}/{page:int?}")]
        public async Task<PartialViewResult> SortList(
            int? page,
            string listName,
            string columnName,
            bool isMultiSort = false)
        {
            var uId = Context.SecurityProvider.CurrentUserId;
            var userSetting = await Context.UserSettingsManager.GetSettingsForUserAsync(uId);
            var currentListSetting = userSetting.GetUserListSettingByName(listName);

            var sortCol = currentListSetting.SortColumns.SingleOrDefault(sc => sc.ColumnName == columnName);

            if (isMultiSort)
            {
                if (sortCol != null)// column already in sort, remove from sort
                {
                    if (currentListSetting.SortColumns.Count > 1)//only remove if there are more than one sort
                    {
                        currentListSetting.SortColumns.Remove(sortCol);
                    }
                }
                else// column not in sort, add to sort
                {
                    currentListSetting.SortColumns.Add(new UserTicketListSortColumn(columnName, ColumnSortDirection.Ascending));
                }
            }
            else
            {
                if (sortCol != null)// column already in sort, just flip direction
                {
                    sortCol.SortDirection = (sortCol.SortDirection == ColumnSortDirection.Ascending) ? ColumnSortDirection.Descending : ColumnSortDirection.Ascending;
                }
                else // column not in sort, replace sort with new simple sort for column
                {
                    currentListSetting.SortColumns.Clear();
                    currentListSetting.SortColumns.Add(new UserTicketListSortColumn(columnName, ColumnSortDirection.Ascending));
                }
            }

            await Context.SaveChangesAsync();

            return await GetTicketListPartial(page, listName);
        }
        private async Task<PartialViewResult> GetTicketListPartial(int? page, string listName)
        {
            var pageNumber = page ?? 1;
            var viewModel = await TicketCenterListViewModel.GetViewModelAsync(pageNumber, listName, Context, Context.SecurityProvider.CurrentUserId);
            return PartialView("_TicketList", viewModel);

        }
        public async Task<List<ProjectUser>> GetProjectsAsync(string userID)
        {
            var projects = new List<ProjectUser>();
            projects = Context.ProjectUser.Where(u => u.UserId == userID).ToList();
            return projects;
        }

        [Route("Export/{id?}")]
        public async Task<ActionResult> TaskExportExcell(HttpRequestMessage request, string id)
        {
            
            string timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToUpper().Replace(':', '_').Replace('.', '_').Replace(' ', '_').Trim();
            var templateFileInfo = new FileInfo(Path.Combine("/Template", "Demo.xlsx"));
            var status = "Closed";
            if (id != "historytickets")
            {
                status = "Open";
            }
            //Cập nhật user setting  
            await FilterListAsync(id, 20, status, null, null);
                var userId = User.Identity.GetUserID();
            var viewModel = new List<ExportExcelViewModel>();
            var listTicket =  await TicketCenterListViewModel.GetViewModelAsync(id,Context, userId) as List<Ticket>;
            viewModel = ExportExcelViewModel.GetListViewModel(listTicket, userId);
            var stream = ExportExcell.Excell(viewModel, templateFileInfo);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Danhsach-" + timestamp + ".xlsx");
        }
    }
}
