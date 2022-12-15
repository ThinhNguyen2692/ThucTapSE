using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketDesk.Web.Identity;
using TicketDesk.Localization.Domain;
using TicketDesk.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using TicketDesk.Web.Identity.Model;

namespace TicketDesk.Domain.Model
{
    public class demo2
    {
        [Key]
        public int ID { get; set; }

        [Column(TypeName = "int")]
        public int demoID { get; set; }

        [Column(TypeName = "nvarchar")]
        public string UserId { get; set; }

        public virtual ICollection<demo> Demos { get; set; }
    }
}
