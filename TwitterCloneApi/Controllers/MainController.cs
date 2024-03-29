﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TwitterCloneApi.Hubss;
using TwitterCloneApi.Models;

namespace TwitterCloneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        //post oluşturma arama yapma ve takip etme methotları kaldı
        TwitterCloneContext db;
        private readonly IHubContext<NewTweetHub> _hubContext;

        public MainController(TwitterCloneContext _db, IHubContext<NewTweetHub> hubContext)
        {
            db = _db;
            _hubContext = hubContext;
        }
        //idim geldi folowers tablosuna gittim kendi idim olanları bulup followers id leri liste attım sonra user tablosuna gelip o listin içinde olmayan userları gönderdim
        // eğer kimseyi takip etmemişse kendi hariç tüm userlar
        [HttpGet]
        [Route("users")]
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
        [Route("user")]
        public async Task<User> user(int UserId)
        {
            var user = db.Users.Where(x => x.Id == UserId).FirstOrDefault();
            return user;
        }
        [HttpGet]
        [Route("tweetOne")]
        public async Task<Post> tweetOne(int id)
        {
           var posts= db.Posts.Where(x => x.Id == id).FirstOrDefault();
         
            posts.User = db.Users.Where(x => x.Id == posts.UserId).FirstOrDefault();
            return posts;
        }
            [HttpGet]
        [Route("tweets")]
        public async Task<List<Post>> Posts(int user_id)
        {
            List<Post> returnPosts = new List<Post>();
            List<Post> convertPost = new List<Post>();

            var followList = db.Followers.Where(x => x.UserId == user_id).OrderBy(x => x.Date).Take(20).ToList();
            if (followList != null && followList.Count() != 0)
            {
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
                var myTweets = db.Posts.Where(x => x.UserId == user_id).OrderByDescending(x => x).ToList();

                var users = db.Users.Where(x => x.Id == user_id).FirstOrDefault<User>();


                returnPosts.AddRange(myTweets);

                convertPost.AddRange(returnPosts.OrderByDescending(x => x.Date));
                returnPosts.Clear();
                returnPosts.AddRange(convertPost);

            }
            else
            {
                var posts = db.Posts.Where(x => x.UserId == user_id).OrderByDescending(x => x).ToList();
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
        [Route("tags")]
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
        [Route("postTags")]
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
        [Route("searchUserName")]
        public async Task<List<User>> Search(string userName)
        {
            if (userName.Equals("allUsers"))
            {
                var serachName = db.Users.ToList();

                return serachName;
            }
            else {
                var serachName = db.Users.Where(x => x.UserName.Contains(userName)).ToList();

                return serachName;
            }
        

       


        }
        [HttpPost]
        [Route("userFollow")]
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
        [HttpPost]
        [Route("createTweet")]
        public async Task<Boolean> CreatePost(int userId, string content, string image_url,string date)
        {

            try
            {
                Post post = new Post();
                post.Date =date;
                post.PostImageUrl = image_url;
                post.PostContent = content;
                post.UserId = userId;
                post.PostLike = 0;
                db.Posts.Add(post);
                db.SaveChanges();

                if (await addTag(post.Id, content))
                {
                    await tweetUpdate(userId, post.Id);

                    return true;

                }
                else {
                    db.Posts.Remove(post);
                    db.SaveChanges();
                    return false;
                }

            }
            catch (Exception)
            {

                return false;
            }


        }
        [HttpGet]
        [Route("tagsAdd")]
        public async Task<Boolean> addTag(int postId, string postContent) {

            Tag tag = new Tag();
            tag.PostId = postId;
            tag.Date = DateTime.Now;

            try
            {

                if (postContent.Contains("#"))
                {
                    List<string> tagList = postContent.Split(" ").ToList();



                    foreach (var item in tagList)
                    {
                        if (item.StartsWith("#"))
                        {
                            tag.TagName = item.ToString();
                            db.Tags.Add(tag);
                            db.SaveChanges();

                        }
                    }
                    return true;

                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {

                return false;
            }
        }


        [HttpGet]
        [Route("searchNotFollow")]
        public async Task<List<User>> getSearchNotFollow(int id)
        {

            var followedId = db.Followers.Where(x => x.UserId == id).ToList();
            var user = db.Users.Where(x => x.Id != id).ToList();
            List<User> sendUsers = new List<User>();
            sendUsers.AddRange(user);
            foreach (var item in followedId)
            {
                foreach (var useritem in user)
                {
                    if (item.Followed == useritem.Id)
                    {

                        int index = sendUsers.FindIndex(x => x.Id == useritem.Id);

                        sendUsers.RemoveAt(index);
                        break;

                    }
                }


            }
            return sendUsers;
        }


        [HttpGet]
        [Route("tweetNotification")]
        public async Task<Boolean> tweetUpdate(int id, int postId)
        {
            //     Post post = new Post();
            var imagUrl = db.Users.Where(x => x.Id == id).FirstOrDefault();
            //    post = db.Posts.Where(x => x.Id == postId).FirstOrDefault();
            string tsts = id.ToString();

            try
            {
                //nt url
                //like url username name post
                //ntf url username name post tweetatanıd
                await _hubContext.Clients.All.SendAsync("newTweets", tsts, imagUrl.PhotoUrl, imagUrl.UserName, imagUrl.Name, postId.ToString());

                return true;
            }
            catch (Exception e)
            {
                await _hubContext.Clients.All.SendAsync("newTweets", id.ToString(), e.ToString(), "cac", "ac", "cac");
                return false;

            }


        }
        [HttpPost]
        [Route("tweetLiked")]
        public async Task<int> postLiked(int likeUserId, int Id, int count)//postId bu
        {

            Post post = new Post();
            post = db.Posts.Where(x => x.Id == Id).FirstOrDefault();
            post.PostLike += count;
         
            await tweetLikedHub(likeUserId, post.UserId, post);
            db.SaveChanges();
            return (int)post.PostLike;

        }

        [HttpGet]
        [Route("tweetLikedHub")]
        public async Task<Boolean> tweetLikedHub(int likedid, int userId, Post post)
        {
            var imagUrl = db.Users.Where(x => x.Id == likedid).FirstOrDefault();  
            string tsts = userId.ToString();
            try
            {
                //like url username name post
                //nt url
                //ntf url username name post tweetatanıd
                // await _hubContext.Clients.All.SendAsync(tsts, imagUrl.PhotoUrl, imagUrl.UserName,imagUrl.Name, post);
                await _hubContext.Clients.All.SendAsync(tsts, imagUrl.PhotoUrl, imagUrl.UserName, imagUrl.Name, post);

                return true;
            }
            catch (Exception e)
            {
                return false;

            }


        }
        [HttpGet]
        [Route("followedUserIdList")]
        public async Task<List<int>> getfollowList(int user_id) {
            try
            {

                var followed = db.Followers.Where(x => x.UserId == user_id).ToList();
                List<int> followedId = new List<int>();
                if (followed != null)
                {
                    foreach (var item in followed)
                    {
                        followedId.Add(item.Followed);
                    }
                }
                return followedId;
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        [HttpGet]
        [Route("followedCount")]
        public async Task<int> followedCount(int user_id) {
            var followedCount = db.Followers.Where(x => x.UserId == user_id).ToList();

            return followedCount.Count();
        }
        [HttpGet]
        [Route("followCount")]
        public async Task<int> followCount(int user_id)
        {
            var followedCount = db.Followers.Where(x => x.Followed == user_id).ToList();

            return followedCount.Count();
        }

        [HttpGet]
        [Route("tweetNew")]
            public ActionResult<Post> tweetNew(int postId){
            var post = db.Posts.Where(x => x.Id == postId).FirstOrDefault();
            return post;
}


    }
}
