using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using TicketDesk.Domain.Model;
using TicketDesk.Localization.Controllers;
using TicketDesk.Web.Identity;
using TicketDesk.Web.Identity.Model;

namespace TicketDesk.Web.Client.Models
{
    public class ProjectViewModel
    {
        public ProjectViewModel() { }

        [Required]
        public int? ProjectId { get; set; } = null;

        [Required]
        [Display(Name = "Tên project")]
        public string ProjectName { get; set; }

        [Display(Name = "Mô tả")]
        public string ProjectDescription { get; set; }


        [Display(Name = "Thời gian xử lý (ngày)")]
        public int? ReasonableTime { get; set; }

    }

    public class ProjectNew
    {
        public ProjectNew() {
            projectViewModel = new ProjectViewModel();
            UserProjectViewModles = new List<UserProjectViewModle>();
            UserIds = new List<string>();
        }

        public ProjectViewModel projectViewModel { get; set; }

        [Display(Name = "Danh sách user ")]
        public List<UserProjectViewModle> UserProjectViewModles { get; set; }

        [Required]
        [Display(Name = "Người phụ trách xử lý")]
        public List<string> UserIds { get; set; } 
    }

    public class UserProjectViewModle
    {
        public UserProjectViewModle() { }
        [Display(Name = "Danh sách user ")]
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
    }

    public class NewTicker
    {
        public NewTicker() { }

        public Ticket Ticket { get; set; }
        public List<ProjectViewModelTicket> ProjectViewModels { get; set; } = new List<ProjectViewModelTicket>();
        public string json { get; set; }

        //random 1 user xử lý.
        public string GetOwner(List<UserProjectViewModle> userProjectViewModles)
        {
            Random rand = new Random();
            var shuffled = userProjectViewModles.OrderBy(_ => rand.Next()).First();
            return shuffled.UserId;
        }


    }

    public class ProjectViewModelTicket{
           public ProjectViewModelTicket() {
            ProjectViewModel = new ProjectViewModel();
            UserProjectViewModle = new UserProjectViewModle();
        }
        public ProjectViewModel ProjectViewModel { get; set; }
        public UserProjectViewModle UserProjectViewModle { get; set; }

      
    }

}