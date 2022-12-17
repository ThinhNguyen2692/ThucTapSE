using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicketDesk.Domain;
using TicketDesk.Domain.Model;
using TicketDesk.Web.Identity.Model;
using TicketDesk.Web.Client.Models;
using TicketDesk.Localization.Controllers;
using TicketDesk.Web.Identity;
using TicketDesk.Web.Identity.Model;
using System.Collections.Generic;
using System;

namespace TicketDesk.Web.Client.Controllers
{
    [RoutePrefix("admin")]
    [Route("{action=index}")]
    [TdAuthorize(Roles = "TdAdministrators,TdHelpDeskUsers")]
    public class ProjectSettingsController : Controller
    {

          private TdDomainContext Context { get; set; }
        private TdIdentityContext _idIdentityContext { get; set; }
       

        public ProjectSettingsController(TdDomainContext context, TdIdentityContext _idIdentityContext)
        {
            Context = context;
            this._idIdentityContext = _idIdentityContext;
        }

        [Route("projects")]
        public async Task<ActionResult> Index()
        {
            var model = Context.Projects.Select(p => new ProjectListViewModelItem { Project = p, NumberOfTickets = p.Tickets.Count()});
            await model.LoadAsync();
            return View(model);
        }

        [Route("project/new")]
        public async Task<ActionResult> New()
        {
            ProjectNew viewModel = new ProjectNew();

           viewModel.UserProjectViewModles = await GetUserByRole("05a83c59-20cd-409b-b424-ea5f5f15a835");
            return View(viewModel);
        }

        [Route("project/new")]
        [HttpPost]
        public async Task<ActionResult> New(ProjectViewModel projectViewModel, List<string> UserIds)
        {
            if (ModelState.IsValid)
            {
                Project project = new Project();
                project.ProjectName = projectViewModel.ProjectName;
                project.ProjectDescription = projectViewModel.ProjectDescription;
                project.ReasonableTime = projectViewModel.ReasonableTime;
                foreach (var item in UserIds)
                {
                    project.ProjectUsers.Add(new ProjectUser { ID = 0, ProjectId = project.ProjectId, UserId = item });
                }
                Context.Projects.Add(project);
                if (await Context.SaveChangesAsync() > 0)
                {
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError("", Strings.UnableToCreateProject);
            ProjectNew viewModel = new ProjectNew();
            viewModel.UserProjectViewModles = await GetUserByRole("05a83c59-20cd-409b-b424-ea5f5f15a835");
            return View(projectViewModel);
        }

        [Route("project/delete/{projectId:int}")]
        [HttpPost]
        public async Task<ActionResult> Delete(int projectId, int reassignTo)
        {
            //does project to delete exist?
            var project = await Context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return RedirectToAction("Index");
            }

            //is there more than one project, and does target project exist?
            var numProjects = await Context.Projects.CountAsync();
            var reassignProject = await Context.Projects.FindAsync(reassignTo);
            if (numProjects < 2 || reassignProject == null)
            {
                return RedirectToAction("Edit", new {projectId});
            }

            //move tickets, then remove project
            var ticketsToReassign = Context.Tickets.Where(t => t.ProjectId == projectId);
            foreach (var ticket in ticketsToReassign)
            {
                ticket.ProjectId = reassignTo;
            }
            Context.Projects.Remove(project);

            var result = await Context.SaveChangesAsync();
            if (result > 0)
            {
                RedirectToAction("Index");
            }

            ModelState.AddModelError("", Strings.UnableToRemoveProject);
            return RedirectToAction("Edit", new {projectId});
        }

        [Route("project/{projectId:int}")]
        public async Task<ActionResult> Edit(int projectId)
        {
            var project = await Context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return RedirectToAction("Index");
            }
            await AddViewDataForEdit(projectId);
            return View(project);
        }

      
        [Route("project/{projectId:int}")]
        [HttpPost]
        public async Task<ActionResult> Edit(Project project, int projectId)
        {
            var existing = await Context.Projects.FindAsync(projectId);
            if (ModelState.IsValid && TryUpdateModel(existing))
            {
                var result = await Context.SaveChangesAsync();
                if (result > 0)
                {
                    return RedirectToAction("Index");
                }
            }
            await AddViewDataForEdit(projectId);
            return View(project);
        
        }

        private async Task<List<UserProjectViewModle>> GetUserByRole(string roleId)
        {

            var role = await _idIdentityContext.Roles.Where(r => r.Id == roleId).FirstOrDefaultAsync();
            var users = _idIdentityContext.Users.Where(u => u.LockoutEndDateUtc == null && u.Roles.Where(r => r.RoleId == roleId).ToList().Count > 0)
             .Select(u => new UserProjectViewModle { UserId = u.Id, UserDisplayName = u.DisplayName})
                .ToListAsync();
            

            return await users;
        
        }

        private async Task AddViewDataForEdit(int projectId)
        {
            ViewBag.EnableDelete = await Context.Projects.CountAsync() > 1;
            ViewBag.ProjectReassignList = Context.Projects
                .Where(p => p.ProjectId != projectId)
                .ToSelectList(p => p.ProjectId.ToString(), p => p.ProjectName);
        }

    }


}