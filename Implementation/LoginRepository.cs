using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDocRepo.DBModels;
using HalloDocRepo.Interfaces;

namespace HalloDocRepo.Implementation;

    public class LoginRepository : ILoginRepository
{
        private readonly HalloDocContext _context;

    public LoginRepository(HalloDocContext context)
    {
        _context = context;
    }

    public int Login(string Email, string Password)
    {
        var user = _context.AspNetUsers.FirstOrDefault(user => user.Email == Email && user.PasswordHash == Password);
        return user?.Id ?? 0;
    }

    public List<AspNetUser> GetAllUsers() 
    { 
        return _context.AspNetUsers.ToList();
    }
    }

