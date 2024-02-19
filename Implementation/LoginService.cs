using HalloDocRepo.Interfaces;
using HalloDocService.ViewModels;
using HalloDocService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocService.Implementation;

    public class LoginService : ILoginService
    {
         private readonly ILoginRepository _loginRepository;

    public LoginService(ILoginRepository loginRepository)
    {
        _loginRepository = loginRepository;
    }

    public int Login(LoginUser1 loginUser)
    {
        int Id = _loginRepository.Login(loginUser.Email, loginUser.PasswordHash);
        
        return Id;
    }

  
}

