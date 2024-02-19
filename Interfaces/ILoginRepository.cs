using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDocRepo.DBModels;

namespace HalloDocRepo.Interfaces;

    public interface ILoginRepository
    {
      
     int Login(string Email, string Password);
    }

