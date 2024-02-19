using HalloDocRepo.DBModels;
using HalloDocRepo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDocRepo.DBModels;
using HalloDocRepo.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HalloDocRepo.Implementation;

    public class DashboardRepository : IDashboardRepository
    {
        private readonly HalloDocContext _context;

        public DashboardRepository(HalloDocContext context)
        {
        _context = context;
        }
        public  List<RequestWiseFile> GetAllRequestWiseFile()
        {
            return   _context.RequestWiseFiles.ToList();
            
        }

        public  List<User> GetAllUsers()
        {
            return  _context.Users.ToList();
        }

        public  List<Request> GetAllRequests()
        {
            return _context.Requests.ToList();
        }

        public void UpdateUser(user)
    {
        _context.Add(user);
        _context.SaveChanges();
    }
    }

