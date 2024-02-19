/*using HalloDoc.DBModels;*/
using HalloDoc.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using HalloDocRepo.DBModels;
using HalloDocRepo.Interfaces;
using HalloDocService.Interfaces;


namespace HalloDoc.Controllers
{
    public class PatientController : Controller
    {

        private readonly HalloDocContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly IDashboardService _dashboardService;
        public PatientController(HalloDocContext context, IHttpContextAccessor httpContextAccessor, IDashboardService dashboardService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Profile(int? id)
        {
            var Id = _httpContextAccessor.HttpContext.Session.GetInt32("ID");
            var profile = _dashboardService.profile(Id);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == id);
            
            return View("Profile",profile);
        }

        #region Dashboard
        public async  Task<IActionResult> Dashboard()
        {
           
            var Id = _httpContextAccessor.HttpContext.Session.GetInt32("ID");

            var dashboard = _dashboardService.dashboard(Id);

            var user = await _context.Users.FirstOrDefaultAsync(x=> x.AspNetUserId == Id);

          
            return View(dashboard);
        }
        #endregion

        public IActionResult CreateMe(int? id)
        {
            var user = _context.Users.FirstOrDefault(x=> x.UserId == id);

            PatientRequest patientRequest = new PatientRequest();
            patientRequest.FirstName = user.FirstName;
            patientRequest.LastName = user.LastName;
            patientRequest.Email = user.Email;
            patientRequest.Street = user.Street;
            patientRequest.State = user.State;
            patientRequest.City = user.City;
            patientRequest.ZipCode = user.ZipCode;
            patientRequest.UserId = user.UserId;

            return View("CreateMePage", patientRequest);
        }

        public async Task<IActionResult> CreateMyRequest(PatientRequest patientRequest) {

            var user = await _context.Users.FirstOrDefaultAsync(x=>x.UserId == patientRequest.UserId);

            Request request = new()
            {
                RequestTypeId = 1,
                UserId = user.UserId,
                FirstName = patientRequest.FirstName,
                LastName = patientRequest.LastName,
                PhoneNumber = patientRequest.Mobile,
                Email = patientRequest.Email,
                Status = 1,
                CreatedDate = DateTime.Now,
                PatientAccountId = user.AspNetUserId.ToString(),
                CreatedUserId = user.UserId

            };
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();




            RequestClient requestClient = new()
            {
                RequestId = request.RequestId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Notes = patientRequest.Notes,
                Street = patientRequest.Street,
                City = patientRequest.City,
                State = patientRequest.State,
                ZipCode = patientRequest.ZipCode,

            };

            if (patientRequest.File != null)
            {
                string filename = request.RequestId + patientRequest.LastName.Substring(0, 2) + patientRequest.File.FileName;
                string path = Path.Combine("D:\\Training\\HTML Training Assignment\\MVC\\HalloDoc\\HalloDoc\\Documents", filename);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    patientRequest.File.CopyToAsync(stream).Wait();
                }

                RequestWiseFile requestWiseFile = new RequestWiseFile();
                requestWiseFile.RequestId = request.RequestId;
                requestWiseFile.FileName = filename;
                requestWiseFile.DocType = 1;
                requestWiseFile.CreatedDate = DateTime.Now;

                _context.RequestWiseFiles.Add(requestWiseFile);
            }
            _context.RequestClients.Add(requestClient);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", new { id = user.AspNetUserId });
        }

        #region Download
        public IActionResult download(string file)
        {
            var path = "~\\..\\Documents\\" + file;
            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", file);
        }
        #endregion

        #region UploadDocument
        [HttpPost]
        public async Task<IActionResult> UploadDocument( Document doc)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(x=> x.RequestId == doc.RequestId);
            if (doc != null)
            {
                if (doc.File != null)
                {
                    string filename = request.RequestId + request.LastName.Substring(0, 2) + doc.File.FileName  ;
                    string path = Path.Combine("D:\\Training\\HTML Training Assignment\\MVC\\HalloDoc\\HalloDoc\\Documents", filename);
                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        doc.File.CopyToAsync(stream).Wait();
                    }

                    RequestWiseFile requestWiseFile = new RequestWiseFile();
                    if (doc.RequestId != null)
                    {
                        requestWiseFile.RequestId = (int)doc.RequestId;
                    }
                    requestWiseFile.CreatedDate = DateTime.Now;
                    requestWiseFile.FileName = filename;
                    requestWiseFile.DocType = 1;
                    _context.RequestWiseFiles.Add(requestWiseFile);
                }
                await _context.SaveChangesAsync();
            }
                return RedirectToAction("ViewDocument", new { id = doc.RequestId });
        }
        #endregion

        #region ViewDocument
        public async Task<IActionResult> ViewDocument(int? id)
        {
            var result1 = (from req in _context.Requests.ToList()
                          join rwf in _context.RequestWiseFiles
                          on req.RequestId equals rwf.RequestId
                          into resultTable
                          from rwf in resultTable.DefaultIfEmpty()
                          where req.RequestId == id
                          select new {rwf, req}).ToList();

            var user = await _context.Users.FirstOrDefaultAsync(x=>x.UserId == result1.FirstOrDefault().req.UserId);
          /*  var rqid = result1?.FirstOrDefault().req.RequestId;*/

            List<ViewDocument> document = new List<ViewDocument>();

            result1?.ForEach(item =>
            {
                document.Add(new ViewDocument
                {
                    
                    FileName = item.rwf.FileName,
                    FirstName = item.req.FirstName,
                    LastName = item.req.LastName,
                    Date = item.rwf.CreatedDate

                });
            });

            Document doc = new()
            {
                document = document,
                RequestId = id,
                UserId = user.UserId,
                AspNetUserId = user.AspNetUserId

            };

            return View(doc);
        }
        #endregion
        public IActionResult Patient()
        {
            return View();
        }
        public IActionResult FamilyRequest()
        {
            return View();
        }

        #region UpdateUser
        public async Task<IActionResult> UpdateUser(Profile profile1) { 
            var user = await _context.Users.FirstOrDefaultAsync(x=> x.Email == profile1.Email);

            _dashboardService.UpdateUser(Profile profile);

            user.FirstName = profile1.FirstName;
                user.LastName = profile1.LastName;
                user.Mobile = profile1.PhoneNo;
                user.Email = profile1.Email;
                user.Street = profile1.Street;
                user.City = profile1.City;
                user.State = profile1.State;
            user.ZipCode = profile1.ZipCode;

            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", new { id = user.AspNetUserId });

        }
        #endregion


        public async Task<IActionResult> Email(String email)
        {
            var user = await _context.AspNetUsers.FirstOrDefaultAsync(u => u.Email == email);
                if(user == null)
            {
                return Json(new { emailExist = false });
            }
            else
            {
                return Json(new { emailExist = true });
            }
        }

        #region familyRagister
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> familyRagister(FamilyRequest familyRequest)
        {
            if (ModelState.IsValid)
            {
                if (familyRequest != null)
                {
                    AspNetRole aspNetRole = new()
                    {
                        Name = "Patient"
                    };

                    var guid = Guid.NewGuid().ToString();
                    AspNetUser data = new()
                    {

                        UserName = familyRequest.Email,
                        PasswordHash = guid,
                        Email = familyRequest.Email,
                        PhoneNumber = familyRequest.Mobile,
                    };
                    _context.AspNetUsers.Add(data);
                    _context.AspNetRoles.Add(aspNetRole);

                    await _context.SaveChangesAsync();



                    User user1 = new()
                    {
                        FirstName = familyRequest.FirstName,
                        AspNetUserId = data.Id,
                        Email = familyRequest.Email,
                        Mobile = familyRequest.Mobile,
                        CreatedBy = "1",
                        CreatedDate = DateTime.Now,
                        LastName = familyRequest.LastName,
                        Street = familyRequest.Street,
                        City = familyRequest.City,
                        State = familyRequest.State,
                        ZipCode = familyRequest.ZipCode,

                    };
                    _context.Users.Add(user1);
                    await _context.SaveChangesAsync();




                    Request request = new()
                    {
                        RequestTypeId = 2,
                        UserId = user1.UserId,
                        FirstName = familyRequest.FamilyFirstName,
                        LastName = familyRequest.FamilyLastName,
                        PhoneNumber = familyRequest.FamilyPhoneNumber,
                        Email = familyRequest.FamilyEmail,
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        /* PatientAccountId = user1.UserId.ToString(),
                         CreatedUserId = user1.UserId*/
                        PatientAccountId =  user1.AspNetUserId.ToString() ,
                        CreatedUserId =  user1.UserId 

                    };
                    _context.Requests.Add(request);
                    await _context.SaveChangesAsync();




                    RequestClient requestClient = new()
                    {
                        RequestId = request.RequestId,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        PhoneNumber = request.PhoneNumber,
                        Email = request.Email,
                        Notes = familyRequest.Notes,
                        Street = familyRequest.Street,
                        City = familyRequest.City,
                        State = familyRequest.State,
                        ZipCode = familyRequest.ZipCode,

                    };

                    if (familyRequest.File != null)
                    {
                        string filename = request.RequestId + familyRequest.LastName.Substring(0, 2) + familyRequest.File.FileName;
                        string path = Path.Combine("D:\\Training\\HTML Training Assignment\\MVC\\HalloDoc\\HalloDoc\\Documents", filename);
                        using (FileStream stream = new FileStream(path, FileMode.Create))
                        {
                            familyRequest.File.CopyToAsync(stream).Wait();
                        }

                        RequestWiseFile requestWiseFile = new RequestWiseFile();
                        requestWiseFile.RequestId = request.RequestId;
                        requestWiseFile.FileName = filename;
                        requestWiseFile.DocType = 1;
                        requestWiseFile.CreatedDate = DateTime.Now;

                        _context.RequestWiseFiles.Add(requestWiseFile);
                    }

                    _context.RequestClients.Add(requestClient);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("login", "Home");
            }
            else
            {
                return View("~/Views/Patient/FamilyRequest.cshtml");
            }
        }
        #endregion


        public IActionResult ConciergeRequest()
        {
            return View();
        }

        #region conciergeRagister
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> conciergeRagister(ConciergeRequest conciergeRequest)
        {
            if (ModelState.IsValid)
            {
                if (conciergeRequest != null)
                {
                    AspNetRole aspNetRole = new()
                    {
                        Name = "Patient"
                    };

                    var guid = Guid.NewGuid().ToString();
                    AspNetUser data = new()
                    {

                        UserName = conciergeRequest.Email,
                        PasswordHash = guid,
                        Email = conciergeRequest.Email,
                        PhoneNumber = conciergeRequest.Mobile,
                    };
                    _context.AspNetUsers.Add(data);
                    _context.AspNetRoles.Add(aspNetRole);

                    await _context.SaveChangesAsync();

                 

                        User user1 = new()
                        {
                            FirstName = conciergeRequest.FirstName,
                            AspNetUserId = data.Id,
                            Email = conciergeRequest.Email,
                            Mobile = conciergeRequest.Mobile,
                            CreatedBy = "1",
                            CreatedDate = DateTime.Now,
                            LastName = conciergeRequest.LastName,
                            Street = conciergeRequest.Street,
                            City = conciergeRequest.City,
                            State = conciergeRequest.State,
                            ZipCode = conciergeRequest.ZipCode,

                        };


                        _context.Users.Add(user1);
                        await _context.SaveChangesAsync();
                    
                  

                        Request request = new()
                        {
                            RequestTypeId = 3,
                            UserId = user1.UserId,
                            FirstName = conciergeRequest.ConciergeFirstName,
                            LastName = conciergeRequest.ConciergeLastName,
                            PhoneNumber = conciergeRequest.ConciergePhoneNumber,
                            Email = conciergeRequest.ConciergeEmail,
                            Status = 1,
                            CreatedDate = DateTime.Now,
                            PatientAccountId = user1.UserId.ToString(),
                            CreatedUserId = user1.UserId

                        };
                        

                        Concierge concierge = new()
                        {
                            ConciergeName = conciergeRequest.ConciergeName,
                            Address = conciergeRequest.ConciergeStreet + "," + conciergeRequest.ConciergeCity + "," + conciergeRequest.ConciergeState,
                            Street = conciergeRequest.ConciergeStreet,
                            City = conciergeRequest.ConciergeCity,
                            State = conciergeRequest.ConciergeState,
                            ZipCode = conciergeRequest.ConciergeZipCode
                        };
                        _context.Requests.Add(request);
                        _context.Concierges.Add(concierge);
                          await _context.SaveChangesAsync();

                        RequestConcierge requestConcierge = new()
                        {
                            RequestId = request.RequestId,
                            ConciergeId = concierge.ConciergeId
                        };


                       
                    
                        _context.RequestConcierges.Add(requestConcierge);

                        await _context.SaveChangesAsync();

                    



                   
                        RequestClient requestClient = new()
                        {
                            RequestId = request.RequestId,
                            FirstName = request.FirstName,
                            LastName = request.LastName,
                            PhoneNumber = request.PhoneNumber,
                            Email = request.Email,
                            Notes = conciergeRequest.Notes,
                            Street = conciergeRequest.Street,
                            City = conciergeRequest.City,
                            State = conciergeRequest.State,
                            ZipCode = conciergeRequest.ZipCode,

                        };

                        _context.RequestClients.Add(requestClient);
                        await _context.SaveChangesAsync();
                    




                    await _context.SaveChangesAsync();



                }
                    return RedirectToAction("login", "Home");
                
            }
            else
            {
                return View("~/Views/Patient/ConciergeRequest.cshtml");
            }
        }
        #endregion

        public IActionResult BussinessRequest()
        {
            return View();
        }

        #region bussinessRagister
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> bussinessRagister(BussinessRequest bussinessRequest)
        {
            if (ModelState.IsValid)
            {
                if (bussinessRequest != null)
                {
                    AspNetRole aspNetRole = new()
                    {
                        Name = "Patient"
                    };

                    var guid = Guid.NewGuid().ToString();
                    AspNetUser data = new()
                    {

                        UserName = bussinessRequest.Email,
                        PasswordHash = guid,
                        Email = bussinessRequest.Email,
                        PhoneNumber = bussinessRequest.Mobile,
                    };
                    _context.AspNetUsers.Add(data);
                    _context.AspNetRoles.Add(aspNetRole);

                    await _context.SaveChangesAsync();



                    User user1 = new()
                    {
                        FirstName = bussinessRequest.FirstName,
                        AspNetUserId = data.Id,
                        Email = bussinessRequest.Email,
                        Mobile = bussinessRequest.Mobile,
                        CreatedBy = "1",
                        CreatedDate = DateTime.Now,
                        LastName = bussinessRequest.LastName,
                        Street = bussinessRequest.Street,
                        City = bussinessRequest.City,
                        State = bussinessRequest.State,
                        ZipCode = bussinessRequest.ZipCode,

                    };
                    _context.Users.Add(user1);
                    await _context.SaveChangesAsync();
                   

                        Request request = new()
                        {
                            RequestTypeId = 4,
                            UserId = user1.UserId,
                            FirstName = bussinessRequest.BusinessFirstName,
                            LastName = bussinessRequest.BusinessLastName,
                            PhoneNumber = bussinessRequest.BusinessPhoneNumber,
                            Email = bussinessRequest.BusinessEmail,
                            Status = 1,
                            CreatedDate = DateTime.Now,
                            PatientAccountId = user1.UserId.ToString(),
                            CreatedUserId = user1.UserId

                        };
                        _context.Requests.Add(request);
                        await _context.SaveChangesAsync();


                    

                    
                        RequestClient requestClient = new()
                        {
                            RequestId = request.RequestId,
                            FirstName = request.FirstName,
                            LastName = request.LastName,
                            PhoneNumber = request.PhoneNumber,
                            Email = request.Email,
                            Notes = bussinessRequest.Notes,
                            Street = bussinessRequest.Street,
                            City = bussinessRequest.City,
                            State = bussinessRequest.State,
                            ZipCode = bussinessRequest.ZipCode,

                        };

                        _context.RequestClients.Add(requestClient);
                        await _context.SaveChangesAsync();
                    


                }
                return RedirectToAction("login", "Home");
            }
            else
            {
                return View("~/Views/Patient/BussinessRequest.cshtml");
            }
        }
        #endregion


        #region PatientRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PatientRequest( PatientRequest requestUser)
        {
            if (ModelState.IsValid) { 

                var Req = await _context.Users.FirstOrDefaultAsync(u=> u.Email == requestUser.Email);

                User user1 = new();
                AspNetUser data = new();



                if (Req == null) {
                    AspNetRole aspNetRole = new()
                    {
                        Name = "Patient"
                    };

                   


                    data.UserName = requestUser.Email;
                    data.PasswordHash = "123456";
                    data.Email = requestUser.Email;
                    data.PhoneNumber = requestUser.Mobile;
                   
                    _context.AspNetUsers.Add(data);
                    _context.AspNetRoles.Add(aspNetRole);

                    await _context.SaveChangesAsync();


                    user1.FirstName = requestUser.FirstName;
                    user1.AspNetUserId = data.Id;
                    user1.Email = requestUser.Email;
                    user1.Mobile = requestUser.Mobile;
                    user1.CreatedBy = "1";
                    user1.CreatedDate = DateTime.Now;
                    user1.LastName = requestUser.LastName;
                    user1.Street = requestUser.Street;
                    user1.City = requestUser.City;
                    user1.State = requestUser.State;
                    user1.ZipCode = requestUser.ZipCode;

                    
                    _context.Users.Add(user1);
                    await _context.SaveChangesAsync();
                }
                /*AspNetUserRoles aspNetUserRoles = new() { };*/

               

                    Request request = new()
                    {
                        RequestTypeId = 1,
                        UserId = Req == null ? user1.UserId : Req.UserId,
                        FirstName = requestUser.FirstName,
                        LastName = requestUser.LastName,
                        PhoneNumber = requestUser.Mobile,
                        Email = requestUser.Email,
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        PatientAccountId = Req == null ? user1.AspNetUserId.ToString() : Req.AspNetUserId.ToString(),
                        CreatedUserId = Req == null ? user1.UserId : Req.UserId

                    };
                    _context.Requests.Add(request);
                    await _context.SaveChangesAsync();
                   
                

               
                    RequestClient requestClient = new()
                    {
                        RequestId = request.RequestId,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        PhoneNumber = request.PhoneNumber,
                        Email = request.Email,
                        Notes = requestUser.Notes,
                        Street = requestUser.Street,   
                        City = requestUser.City,
                        State = requestUser.State,
                        ZipCode = requestUser.ZipCode,

                    };


                if (requestUser.File != null)
                {
                    string filename = request.RequestId + requestUser.LastName.Substring(0, 2) + requestUser.File.FileName;
                    string path = Path.Combine("D:\\Training\\HTML Training Assignment\\MVC\\HalloDoc\\HalloDoc\\Documents", filename);
                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        requestUser.File.CopyToAsync(stream).Wait();
                    }

                    RequestWiseFile requestWiseFile = new RequestWiseFile();
                    requestWiseFile.RequestId = request.RequestId;
                    requestWiseFile.FileName = filename;
                    requestWiseFile.DocType = 1;
                    requestWiseFile.CreatedDate = DateTime.Now;

                    _context.RequestWiseFiles.Add(requestWiseFile);
                }
                _context.RequestClients.Add(requestClient);
                    await _context.SaveChangesAsync();
               

                return RedirectToAction("login", "Home");

            }
            else
            {
                return View("~/Views/Patient/Patient.cshtml");
            }

           
        }
        #endregion
    }

}