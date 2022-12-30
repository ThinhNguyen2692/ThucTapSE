using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TicketDesk.Localization.Models;
using TicketDesk.Localization;
using TicketDesk.Domain.Model;

namespace TicketDesk.Web.Client.Models
{
    public class ExportExcelViewModel
    {
        public ExportExcelViewModel() { }

       
       [DisplayName("Mã")]
        public int TicketId { get; set; }
        [DisplayName("Mã nhân viên")]
        public string UserId { get; set; } = "";
        [DisplayName("Tên nhân viên")]
        public string UserName { get; set; } = "";
        [DisplayName("Công ty")]
        public string Company { get; set; } = "";
        [DisplayName("Chức danh")]
        public string JobTitleName { get; set; } = "";
        [DisplayName("Chức vụ")]
        public string PositionName { get; set; } = "";
        [DisplayName("Phòng ban")]
        public string Department { get; set; } = "";
        [DisplayName("Nguời yêu cầu")]
        public string Owner { get; set; } = "";
        [DisplayName("Cấp bậc ")]
        public string LevelName1 { get; set; } = "";
        [DisplayName("Cấp bậc 2")]
        public string LevelName2 { get; set; } = "";
        [DisplayName("Cấp bậc 3")]
        public string LevelName3 { get; set; } = "";
        [DisplayName("Cấp bậc 4")]
        public string LevelName4 { get; set; } = "";
        [DisplayName("Cấp bậc 5")]
        public string LevelName5 { get; set; } = "";
        [DisplayName("Cấp bậc 6")]
        public string LevelName6 { get; set; } = "";
        [DisplayName("Cấp bậc7")]
        public string LevelName7 { get; set; } = "";
        [DisplayName("Tiêu đề")]
        public string Title { get; set; }
        [DisplayName("Loại phản hồi")]
        public string ProjectName { get; set; }
        [DisplayName("Nội dung")]
        public string Details { get; set; }
        [DisplayName("Nhãn")]
        public string TagList { get; set; }
        [DisplayName("Người xử lý")]
        public string AssignedTo { get; set; }
        [DisplayName("Trạng thái")]
        public string Status { get; set; }
        [DisplayName("Ngày tạo")]
        public DateTimeOffset CreatedDate { get; set; }
        [DisplayName("Ngày mục tiêu hết hạng")]
        public DateTimeOffset TargetDate { get; set; }
        [DisplayName("Ngày cập nhật")]
        public DateTimeOffset DueDate { get; set; }



        public static List<ExportExcelViewModel> GetListViewModel(List<Ticket> viewModel, string userId)
        {
            var exportExcelViewModels = new List<ExportExcelViewModel>();
          
            foreach (var item in viewModel)
            {
                var model = new ExportExcelViewModel();
                model.TicketId = item.TicketId;
                model.Title = item.Title;
                model.AssignedTo = item.GetAssignedToInfo().DisplayName;
                model.Owner = item.GetOwnerInfo().DisplayName;
                model.ProjectName = item.Project.ProjectName;
                model.Status = item.TicketStatus.ToString();
                model.Details = item.Details;
                model.TagList = item.TagList;
                model.CreatedDate = item.CreatedDate;
                model.TargetDate = item.TargetDate;
                model.DueDate = item.LastUpdateDate;
                exportExcelViewModels.Add(model);
            }
            return exportExcelViewModels;
        }
    }
}