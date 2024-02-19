using HalloDocService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocService.Interfaces
{
    public  interface IDashboardService
    {
        public List<Dashboard1> dashboard(int? Id);
        public Profile profile(int? Id);
        public void UpdateUser(Profile profile);
    }
}
