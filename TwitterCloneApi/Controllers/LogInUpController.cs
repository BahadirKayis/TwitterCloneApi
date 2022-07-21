
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterCloneApi.HelperClass;
using TwitterCloneApi.Hubc;
using TwitterCloneApi.Models;

namespace TwitterCloneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogInUpController : ControllerBase
    {
         TwitterCloneContext db;
     

        public LogInUpController(TwitterCloneContext _db)
        {
            db =_db ;

            
        }
        //kayıt olmak için hesap oluştur butonuna bastıktan sonra getUserNameAndEmail() değerler gelecek direkt,kayıt ola basıncada postSignUp() çalışcak
        //giriş yapmak için,kullanıcı adını girdikten sonra ileri butonuna basınca LoginUserName() çalışcak şifre dönecek mobilde kontrol edeceğim şifreyi 
        //şifremi unuttum forgetpassword() bilgi var ise id dönecek,şifreyi girdikten sonra yeni şifre oluştur butonuna basınca gelen id ve şifre updatePassword() mothoduna gönderilecek
        [HttpPost]
        [Route("postSignUp")]
        public async Task<Boolean> postSignUp(string user_name, string user_password, string name, string email, string phone, string photo_url, DateTime date) 
        {
            try
            {
                var control = db.Users.Where(x => x.UserName.Equals(user_name) || x.Email.Equals(email));
                if (control!=null)
                {
                    return false;
                }
            User newUser = new User();
            newUser.UserName = user_name;
            newUser.UserPassword = user_password;
            newUser.Name = name;
            newUser.Email= email;
            newUser.Phone = phone;
            newUser.PhotoUrl = photo_url;
            newUser.Date = date;
            db.Users.Add(newUser);
            db.SaveChanges();

            return true;
            }
            catch (Exception)
            {

                return false;
            }
           
        }
        
        [HttpGet]
        [Route("getUserNameAndEmail")]
        public async Task<List<UsernameEmail>> getUserNameAndEmail() {
            List<UsernameEmail> return_UserNameEmail = new List<UsernameEmail>();
            UsernameEmail classUserName;

            try
            {
                var userEmail = db.Users.ToList();

                foreach (var item in userEmail)
            {
                    classUserName = new UsernameEmail();
                    classUserName.Email = item.Email;
                classUserName.UserName = item.UserName;
                return_UserNameEmail.Add(classUserName);
                //classUserName = new UsernameEmail(); Silmesse değerleri boşaltcaz içini ekledikten sonra

            }
            

                return  return_UserNameEmail;
            }
            catch (Exception e)
            {
                return return_UserNameEmail;

            }

           


        }

        [HttpPost]
        [Route("updatePassword")]
        public async Task<Boolean> updatePassword(int  user_id, string password) {
            User user = db.Users.Where(x => x.Id==user_id).FirstOrDefault();
            if (user!=null)
            {
                user.UserPassword = password;
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }
        [HttpGet]
        [Route("getForgetPassword")]
        public async Task<int> forgetpassword(string userControl) 
        {
            var userInfo = db.Users.Where(x => x.UserName == userControl || x.Email==userControl).FirstOrDefault();

            if (userInfo !=null)
            {
                return userInfo.Id;
            }
            else
            {
                return 0;
            }
        }

        [HttpGet]
        [Route("getLoginUserName")]
        public async Task<User> LoginUserName(string userControl)
        {
            var userInfo = db.Users.Where(x => x.UserName == userControl || x.Email == userControl).FirstOrDefault();

            if (userInfo != null)
            {
                return userInfo;
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        [Route("getLoginUserNameAndPassword")] 
        public Boolean loginAuto(string userName,string password)
        {
            var signIn = db.Users.Where(x => x.UserName == userName && x.UserPassword == password).FirstOrDefault();

            if (signIn!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    
    }
}
