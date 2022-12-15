using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketDesk.Localization;
using TicketDesk.Localization.Domain;

namespace TicketDesk.Domain.Model
{
    public class demo
    {
        [Key]
        public int ID { get; set; }

        [Column(TypeName = "int")]
        public int ProjectId { get; set; }

        [Column(TypeName = "nvarchar")]
        public string UserId { get; set; }

        public virtual demo2 demo2 { get; set; }
    }
}
