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

using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicketDesk.Domain;
using TicketDesk.Domain.Model;
using TicketDesk.IO;
using TicketDesk.Localization.Controllers;
using TicketDesk.Web.Client.Models;
using TicketDesk.Web.Client.Models.Extensions;
using TicketDesk.Web.Identity;

namespace TicketDesk.Web.Client.Controllers
{
    /// <summary>
    /// Class TicketController.
    /// </summary>
    [RoutePrefix("ticket")]
    [Route("{action=index}")]
    [TdAuthorize(Roles = "TdInternalUsers,TdHelpDeskUsers,TdAdministrators")]
    public class TicketController : Controller
    {
        private TdDomainContext Context { get; set; }
        private TdIdentityContext _idIdentityContext { get; set; }
        private TicketDeskUserManager UserManager { get; set; }
        public TicketController(TdDomainContext context, TdIdentityContext _idIdentityContext, TicketDeskUserManager UserManager)
        {
            Context = context;
            this._idIdentityContext = _idIdentityContext;
            this.UserManager = UserManager;
        }

        public RedirectToRouteResult Index()
        {
            return RedirectToAction("Index", "TicketCenter");
        }

        [Route("{id:int}")]
        public async Task<ActionResult> Index(int id)
        {

            var model = await Context.Tickets.Include(t => t.TicketSubscribers).FirstOrDefaultAsync(t => t.TicketId == id);
            if (model == null)
            {
                return RedirectToAction("Index", "TicketCenter");
            }
            ViewBag.IsEditorDefaultHtml = Context.TicketDeskSettings.ClientSettings.GetDefaultTextEditorType() == "summernote";

            return View(model);
        }

        [Route("new")]
        public async Task<ActionResult> New()
        {

            var newTicker = new NewTicker();

            newTicker.Ticket = new Ticket
            {
               Owner = User.Identity.GetUserID(),
                IsHtml = Context.TicketDeskSettings.ClientSettings.GetDefaultTextEditorType() == "summernote"
            };
            
            await SetProjectInfoForModelAsync(newTicker.Ticket);
           
            var arrProject = Context.Projects.ToArray();

            newTicker.ProjectViewModels = new List<ProjectViewModelTicket>();
            newTicker.ProjectViewModels.Add(new ProjectViewModelTicket());
            foreach (var item in arrProject)
            {
                var project = new ProjectViewModelTicket();
                project.ProjectViewModel.ProjectId = item.ProjectId;
                project.ProjectViewModel.ProjectName = item.ProjectName;
                project.ProjectViewModel.ReasonableTime = item.ReasonableTime;
                project.UserProjectViewModle = await GetUserProjectViewModle(item.ProjectUsers.ToList());
                newTicker.ProjectViewModels.Add(project);
            }

            newTicker.json = Newtonsoft.Json.JsonConvert.SerializeObject(newTicker.ProjectViewModels);
          
            ViewBag.TempId = Guid.NewGuid();
            return View(newTicker);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateOnlyIncomingValues]
        [ValidateInput(false)]
        [Route("new")]
        public async Task<ActionResult> New(Ticket ticket, Guid tempId)
        {
            if (ticket.IsHtml)
            {
                ticket.Details = ticket.Details.StripHtmlWhenEmpty();
                if (string.IsNullOrEmpty(ticket.Details))
                {
                    ModelState.AddModelError("Details", Strings.RequiredField);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await CreateTicketAsync(ticket, tempId))
                    {
                        return RedirectToAction("Index", new { id = ticket.TicketId });
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (DbEntityValidationException)
                {

                    //TODO: catch rule exceptions? or can annotations handle this fully now?
                }

            }
            ViewBag.TempId = tempId;
            await SetProjectInfoForModelAsync(ticket);
            return View(ticket);
        }

        [Route("ticket-files")]
        public ActionResult TicketFiles(int ticketId)
        {
            //WARNING! This is also used as a child action and cannot be made async in MVC 5
            var attachments = TicketDeskFileStore.ListAttachmentInfo(ticketId.ToString(CultureInfo.InvariantCulture), false);
            ViewBag.TicketId = ticketId;
            return PartialView("_TicketFiles", attachments);
        }

        [Route("ticket-events")]
        public ActionResult TicketEvents(int ticketId)
        {
            //WARNING! This is also used as a child action and cannot be made async in MVC 5
            var ticket = Context.Tickets.Find(ticketId);
            return PartialView("_TicketEvents", ticket.TicketEvents);
        }

        
     

        [Route("ticket-details")]
        public ActionResult TicketDetails(int ticketId)
        {
            //WARNING! This is also used as a child action and cannot be made async in MVC 5
            var ticket = Context.Tickets.Find(ticketId);
            ViewBag.DisplayProjects = Context.Projects.Any();
            return PartialView("_TicketDetails", ticket);
        }

        [Route("change-ticket-subscription")]
        [HttpPost]
        public async Task<JsonResult> ChangeTicketSubscription(int ticketId)
        {
            var userId = Context.SecurityProvider.CurrentUserId;
            var ticket = await Context.Tickets.Include(t => t.TicketSubscribers).Include(t => t.TicketEvents.Select(e => e.TicketEventNotifications)).FirstOrDefaultAsync(t => t.TicketId == ticketId);
            var subscriber =
                ticket.TicketSubscribers.FirstOrDefault(s => s.SubscriberId == Context.SecurityProvider.CurrentUserId);
            var isSubscribed = false;
            if (subscriber == null)
            {
                subscriber = new TicketSubscriber
                {
                    SubscriberId = userId,
                };
                ticket.TicketSubscribers.Add(subscriber);
                isSubscribed = true;
            }
            else
            {
                ticket.TicketSubscribers.Remove(subscriber);
            }
            await Context.SaveChangesAsync();
            return new JsonCamelCaseResult { Data = new { IsSubscribed = isSubscribed } };
        }

        private async Task SetProjectInfoForModelAsync(Ticket ticket)
        {
            if (ticket.ProjectId == default(int))
            {
                var projects = await Context.Projects.Select(s=>s.ProjectId).ToListAsync();
                var isMulti = (projects.Count > 1);
                ViewBag.IsMultiProject = isMulti;
               
                //set to first project if only one project exists, otherwise use user's selected project
                ticket.ProjectId = (isMulti) ? await Context.UserSettingsManager.GetUserSelectedProjectIdAsync(Context) : projects.FirstOrDefault(); 
            }
        }


        private async Task<bool> CreateTicketAsync(Ticket ticket, Guid tempId)
        {

            Context.Tickets.Add(ticket);
            await Context.SaveChangesAsync();
            ticket.CommitPendingAttachments(tempId);

            return ticket.TicketId != default(int);
        }

        public async Task<UserProjectViewModle> GetUserProjectViewModle(List<ProjectUser> projectUsers)
        {
            if(projectUsers.Count == 0)
            {
                return null;
            }
            Random rand = new Random(); 
            var shuffled = projectUsers.OrderBy(_ => rand.Next()).FirstOrDefault();
            var user = await UserManager.FindByIdAsync(shuffled.UserId);
            if(user.LockoutEndDateUtc != null) {GetUserProjectViewModle(projectUsers);  }
            return new UserProjectViewModle { UserId = user.Id, UserDisplayName = user.DisplayName};
        }

        [Route("udatengay/{id}")]
        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            var date = new DateTime(9999, 01, 01);
            var connect = Context.Tickets.Where(t => t.TicketId == id).FirstOrDefault();
            connect.TargetDate = date;
           int x = await Context.SaveChangesAsync();

            return Redirect("/ticket/" + id);

        }




     
    }
}