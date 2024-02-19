using HalloDocRepo.DBModels;
using System.Collections;

namespace HalloDocService.ViewModels
{
    public class Dashboard1
    {
        public int RequestId { get; set; }

        public int? RequestTypeId { get; set; }

        public int? UserId { get; set; }

        public int? Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public int? count { get; set; }



    }
}
