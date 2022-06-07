using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterCloneApi.Models;

namespace TwitterCloneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        //post oluşturma arama yapma ve takip etme methotları kaldı
        TwitterCloneContext db;

        public MainController(TwitterCloneContext _db)
        {
            db = _db;
        }
        //idim geldi folowers tablosuna gittim kendi idim olanları bulup followers id leri liste attım sonra user tablosuna gelip o listin içinde olmayan userları gönderdim
        [HttpGet]
        [Route("getUsers")]
        public async Task<List<User>> getUsers(int id)
        {
            List<User> returnUser = new List<User>();
            User user = new User(); 
            var followList = db.Followers.Where(x => x.UserId == id).FirstOrDefault().Followed.ToList();
            foreach (var item in followList)
            {
                user = db.Users.Where(x => x.Id != item).FirstOrDefault();
                returnUser.Add(user);
            }
            return returnUser;

        }
        [HttpGet]
        [Route("getPosts")]
        public async Task<List<Post>>getPosts(int id)
        {
            List<Post> returnPosts= new List<Post>();
            Post posts = new Post();
            var followList = db.Followers.Where(x => x.UserId == id).FirstOrDefault().Followed.ToList();
            foreach (var item in followList)
            {
                posts = db.Posts.Where(x => x.UserId == item).FirstOrDefault();
                returnPosts.Add(posts);
            }
            return returnPosts;
        }
        [HttpGet]
        [Route("getTags")]
        public async Task<List<string>> getPopularTags()
        {
            List<string> returnTags = new List<string>() ;
            var TagNames = db.Tags.Take(4).FirstOrDefault().TagName.ToList();
            foreach (var item in TagNames)
            {
                returnTags.Add(item.ToString());
            }
            
            return returnTags;        

        }
        [HttpGet]
        [Route("getPostTags")]
        public async Task<List<Post>> getPostTags(string tagName)
        {
            List<Post> tagPosts = new List<Post>();
            var tagPostId = db.Tags.Where(x => x.TagName.Equals(tagName)).ToList();
            foreach (var item in tagPostId)
            {
            tagPosts.Add(item.Post);
            }

            return tagPosts;
           
        }

    }
}
