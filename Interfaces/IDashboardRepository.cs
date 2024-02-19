using HalloDocRepo.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepo.Interfaces;

    public interface IDashboardRepository
    {
        public List<RequestWiseFile> GetAllRequestWiseFile();
        public List<User> GetAllUsers();
        public List<Request> GetAllRequests();
        public void UpdateUser(user);
    }

