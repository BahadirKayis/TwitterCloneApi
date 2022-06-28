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
        // eğer kimseyi takip etmemişse kendi hariç tüm userlar
        [HttpGet]
        [Route("getUsers")]
        public async Task<List<User>> Users(int id)
        {
            List<User> returnUser = new List<User>();
            User user;
            try
            {
                var followList = db.Followers.Where(x => x.UserId == id).Take(20).FirstOrDefault().Followed.ToString().ToList();
                if (followList == null)
                {
                    var allUser = db.Users.Where(x => x.Id != id).ToList();
                    return allUser;
                }

                foreach (var item in followList)
                {
                    user = new User();
                    user = db.Users.Where(x => x.Id != item).FirstOrDefault();
                    returnUser.Add(user);
                }
                return returnUser;
            }
            catch (Exception)
            {

                var allUser = db.Users.Where(x => x.Id != id).Take(20).ToList();
                return allUser;



            }

        }
        //kimseyi takip etmiyorsa list null geliyor
        [HttpGet]
        [Route("getPosts")]
        public async Task<List<Post>> Posts(int id)
        {
            List<Post> returnPosts = new List<Post>();

            var followList = db.Followers.Where(x => x.UserId == id).OrderBy(x => x.Date).Take(20).ToList();
            foreach (var item in followList)
            {
                var posts = db.Posts.Where(x => x.UserId == item.Followed).OrderByDescending(x => x).ToList();
                foreach (var post in posts)
                {
                    var user = db.Users.Where(x => x.Id == post.UserId).First();
                    post.User = user;

                }


                returnPosts.AddRange(posts);

            }
            return returnPosts;
        }
        [HttpGet]
        [Route("getTags")]
        public async Task<List<string>> PopularTags()
        {
            List<string> returnTags = new List<string>();
            var TagNames = db.Tags.Take(4).ToList();
            foreach (var item in TagNames)
            {
                returnTags.Add(item.TagName.ToString());
            }

            return returnTags;

        }
        [HttpGet]
        [Route("getPostTags")]
        public async Task<List<Post>> PostTags(string tagName)
        {
            List<Post> tagPosts = new List<Post>();

            var tagPostId = db.Tags.Where(x => x.TagName.Contains(tagName)).ToList();
            foreach (var item in tagPostId)
            {
                Post getPost = db.Posts.Where(x => x.Id == item.PostId).FirstOrDefault();
                tagPosts.Add(getPost);
            }

            return tagPosts;

        }

        [HttpGet]
        [Route("getsearchUserName")]
        public async Task<List<string>> Search(string serachUserNamee)
        {
            var serachName = db.Users.Where(x => x.UserName.Contains(serachUserNamee)).Take(6).ToList();

            List<string> nameList = new List<string>();
            foreach (var item in serachName)
            {
                nameList.Add(item.UserName);
            }


            return nameList;
        }
        [HttpPost]
        [Route("postUserFollow")]
        public async Task<Boolean> UserFollow(int userId, int followId)
        {
            try
            {
                Follower follower = new Follower();
                follower.UserId = userId;
                follower.Followed = followId;
                follower.Date = DateTime.Now;
                db.Followers.Add(follower);
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {

                return false;
            }

        }
        [HttpGet]
        [Route("postCreatPost")]
        public async Task<Boolean> CreatePost(int userId, string content, DateTime date, string image_url)
        {

            try
            {
                Post post = new Post();
                post.Date = date;
                post.PostImageUrl = image_url;
                post.PostContent = content;
                post.UserId = userId;
                db.Posts.Add(post);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }


        }

        [HttpPost]
        [Route("postLiked")]
        public async Task<int> postLiked(int Id, int count) {

            Post post = new Post();

            post = db.Posts.Where(x => x.Id == Id).FirstOrDefault();

            post.PostLike += count;

            db.SaveChanges();
            return (int)post.PostLike;

        }

        [HttpGet]
        [Route("getSearchNotFollow")]
        public async Task<List<User>> getSearchNotFollow(int id)
        {
            var followedId = db.Followers.Where(x => x.Id == id).ToList();
          
            List<User> users = new List<User>();
            foreach (var item in followedId)
            {
                var user = db.Users.Where(x => x.Id != item.Followed && x.Id!=id).ToList();
                users.AddRange(user);
            }
            return users;
        }



    }
}
