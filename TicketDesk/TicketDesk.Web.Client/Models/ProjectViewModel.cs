using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using TicketDesk.Domain.Model;
using TicketDesk.Localization.Controllers;
using TicketDesk.Web.Identity.Model;

namespace TicketDesk.Web.Client.Models
{
    public class ProjectViewModel
    {
        public ProjectViewModel() { }

        public int ProjectId { get; set; }

        [Required]
        [Display(Name = "Tên project")]
        public string ProjectName { get; set; }

        [Display(Name = "Mô tả")]
        public string ProjectDescription { get; set; }


        [Display(Name ="Thời gian xử lý")]
        public int? ReasonableTime { get; set; }

        
        [Display(Name = "Danh sách user ")]
        public List<UserProjectViewModle> UserProjectViewModles { get; set; }

        [Required]
        public List<string> UserIds { get; set; }
    }

    public class UserProjectViewModle
    {
        public UserProjectViewModle() { }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
    }

    public class NewTicker
    {
        public NewTicker() { }

        public Ticket Ticket { get; set; }
        public List<ProjectViewModel> ProjectViewModels { get; set; } = new List<ProjectViewModel>();
        public string json { get; set; }
    }
    
}