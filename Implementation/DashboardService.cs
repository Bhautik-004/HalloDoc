using HalloDocRepo.DBModels;
using HalloDocService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDocService.ViewModels;
using HalloDocRepo.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace HalloDocService.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
        
        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public List<Dashboard1> dashboard(int? Id)
        {
            var requestWiseFile = _dashboardRepository.GetAllRequestWiseFile();
            var requests = _dashboardRepository.GetAllRequests();
            var users = _dashboardRepository.GetAllUsers();

            var user1 = users.FirstOrDefault(x=>x.AspNetUserId == Id);
            var count = requestWiseFile.GroupBy(x => x.RequestId).ToList().Select(s => new
            {
                count = s.Count(),
                requestID = s.Key,
            }).ToList();

            var result = (from req in requests.ToList()
                          join co in count
                          on req.RequestId equals co.requestID
                          into resultTable
                          from co in resultTable.DefaultIfEmpty()
                          where req.PatientAccountId == Id.ToString()
                          select new
                          {
                              req,
                              co
                          }).ToList();

            List<Dashboard1> dashboard = new List<Dashboard1>();

            result.ForEach(item =>
            {
                var count1 = 0;
                if (item.co != null)
                {
                    count1 = item.co.count;
                }
                dashboard.Add(new Dashboard1
                {
                    UserId = user1.UserId,
                    RequestId = item.req.RequestId,
                    CreatedDate = item.req.CreatedDate,
                    Status = item.req.Status,
                    count = count1
                });
            });

            return dashboard;
        }

        public Profile profile(int? Id)
        {
            var users = _dashboardRepository.GetAllUsers();
            var user = users.FirstOrDefault(x=>x.AspNetUserId == Id);


            Profile profile = new Profile();
            profile.Id = user.UserId;
            profile.AspNetUserId = user.AspNetUserId;
            profile.FirstName = user.FirstName;
            profile.LastName = user.LastName;
            profile.Email = user.Email;
            profile.PhoneNo = user.Mobile;
            profile.Street = user.Street;
            profile.City = user.City;
            profile.State = user.State;
            profile.ZipCode = user.ZipCode;

            return profile;
        }

        public  void  UpdateUser(Profile profile)
        {
            var users =  _dashboardRepository.GetAllUsers();
            var user =   users.FirstOrDefault(u=>u.Email ==  profile.Email);

            user.FirstName = profile.FirstName;
            user.LastName = profile.LastName;
            user.Mobile = profile.PhoneNo;
            user.Email = profile.Email;
            user.Street = profile.Street;
            user.City = profile.City;
            user.State = profile.State;
            user.ZipCode = profile.ZipCode;

           
        }
    }
}
