using System.Linq;

namespace TicketDesk.Domain.Model
{
    public static class UserTicketListSettingsCollectionExtensions
    {

        internal static bool HasRequiredDefaultListSettings(this UserTicketListSettingsCollection listSettings, bool isHelpDeskOrAdmin)
        {
            var hasLists = true;
            if (isHelpDeskOrAdmin)
            {
                hasLists = 
                    
                    listSettings.Any(s => s.ListName == "assignedToMe");
            }
            else
            {
                hasLists = listSettings.Any(s => s.ListName == "mytickets");
            }
            return
                hasLists &&  
                //hiện danh sách opentickets(UserTicketListSetting.cs(sửa))
                //listSettings.Any(s => s.ListName == "opentickets") &&
                listSettings.Any(s => s.ListName == "historytickets");
        }
    }
}
