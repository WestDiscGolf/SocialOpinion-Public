﻿using SocialOpinionAPI.Clients;
using SocialOpinionAPI.Core;
using SocialOpinionAPI.DTO.RecentSearch;
using SocialOpinionAPI.Models.FilteredStream;
using SocialOpinionAPI.Models.Followers;
using SocialOpinionAPI.Models.Following;
using SocialOpinionAPI.Models.HideReplies;
using SocialOpinionAPI.Models.RecentSearch;
using SocialOpinionAPI.Models.SampledStream;
using SocialOpinionAPI.Models.Timeline;
using SocialOpinionAPI.Models.TweetMetrics;
using SocialOpinionAPI.Models.Tweets;
using SocialOpinionAPI.Models.Users;
using SocialOpinionAPI.Services;
using SocialOpinionAPI.Services.FilteredStream;
using SocialOpinionAPI.Services.HideReply;
using SocialOpinionAPI.Services.RecentSearch;
using SocialOpinionAPI.Services.SampledStream;
using SocialOpinionAPI.Services.Timeline;
using SocialOpinionAPI.Services.Tweet;
using SocialOpinionAPI.Services.TweetMetrics;
using SocialOpinionAPI.Services.Users;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace SocialOpinionConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string _ConsumerKey = ConfigurationManager.AppSettings.Get("ConsumerKey");
            string _ConsumerSecret = ConfigurationManager.AppSettings.Get("ConsumerSecret");
            string _AccessToken = ConfigurationManager.AppSettings.Get("AccessToken");
            string _AccessTokenSecret = ConfigurationManager.AppSettings.Get("AccessTokenSecret");

            OAuthInfo oAuthInfo = new OAuthInfo
            {
                AccessSecret = _AccessTokenSecret,
                AccessToken = _AccessToken,
                ConsumerSecret = _ConsumerSecret,
                ConsumerKey = _ConsumerKey
            };

            TimelineService timeLineService = new TimelineService(oAuthInfo);

            UserTweetTimelineModel timelineModel = timeLineService.GetUserTweetsTimeline("958676983", null, false, false, 100, null, null, null, null);



            UserMentionedTimelineModel userMentionedTimeline = 
                timeLineService.GetUserMentionedTimeline("958676983", null, 10, null, null, null, null);

            HideReplyService hideRepliesService = new HideReplyService(oAuthInfo);
            HideReplyModel model = hideRepliesService.HideReply("1296341968176451585");

            // Sampled Stream Service Test
            SampledStreamService streamService = new SampledStreamService(oAuthInfo);
            streamService.DataReceivedEvent += StreamService_DataReceivedEvent;
            streamService.StartStream("https://api.twitter.com/2/tweets/sample/stream?expansions=attachments.poll_ids,attachments.media_keys,author_id,entities.mentions.username,geo.place_id,in_reply_to_user_id,referenced_tweets.id,referenced_tweets.id.author_id", 100, 5);

            // Recent Search 
            RecentSearchService searchService = new RecentSearchService(oAuthInfo);

            List<RecentSearchResultsModel> resultsModels = searchService.SearchTweets("iphone", 100, 3);

            // Tweet(s)
            TweetService tweetsService = new TweetService(oAuthInfo);
            TweetModel tweetModel = tweetsService.GetTweet("1293779846691270658");

            List<string> tids = new List<string>();
            tids.Add("1293779846691270658"); // social opinion tweet
            tids.Add("1293779846691270658"); // social opinion tweet
            TweetsModel tweetModels = tweetsService.GetTweets(tids);

            //// User(s)
            UserService userService = new UserService(oAuthInfo);
            UserModel userModel = userService.GetUser("socialopinions");

            UserService userService2 = new UserService(oAuthInfo);
            UserModel userModel2 = userService2.GetUser("jamie_maguire1");

            List<string> users = new List<string>();
            users.Add("jamie_maguire1");
            users.Add("socialopinions");
            UsersModel usersResults = userService.GetUsers(users);

            UserService myUserService = new UserService(oAuthInfo);

            FollowersModel followers = myUserService.GetFollowers("958676983", "100", null);

            FollowingModel following = myUserService.GetFollowing("38906681", "100", null);

            // Metrics  
            List<string> ids = new List<string>();
            ids.Add("1258736674844094465"); // social opinion tweet
            TweetMetricsService service = new TweetMetricsService(oAuthInfo);
            List<TweetMetricModel> metricModels = service.GetTweetMetrics(ids);

            // testing Filtered Stream
            FilteredStreamService filteredStreamService = new FilteredStreamService(oAuthInfo);

            List<FilteredStreamRule> rules = filteredStreamService.CreateRule(
                new MatchingRule { tag = "testing #iPhone", Value = "#iphone" });

            filteredStreamService.DataReceivedEvent += FilteredStreamService_DataReceivedEvent;
            filteredStreamService.StartStream("https://api.twitter.com/2/tweets/search/stream?tweet.fields=attachments,author_id,context_annotations,conversation_id,created_at,entities,geo,id,in_reply_to_user_id,lang,public_metrics,possibly_sensitive,referenced_tweets,source,text,withheld&expansions=author_id&user.fields=created_at,description,entities,id,location,name,pinned_tweet_id,profile_image_url,protected,public_metrics,url,username,verified,withheld", 10, 5);
        }

        private static void StreamService_DataReceivedEvent(object sender, EventArgs e)
        {
            SampledStreamService.DataReceivedEventArgs eventArgs = e as SampledStreamService.DataReceivedEventArgs;
            SampledStreamModel model = eventArgs.StreamDataResponse;
        }

        private static void FilteredStreamService_DataReceivedEvent(object sender, EventArgs e)
        {
            FilteredStreamService.DataReceivedEventArgs eventArgs = e as FilteredStreamService.DataReceivedEventArgs;
            FilteredStreamModel model = eventArgs.FilteredStreamDataResponse;
        }
    }
}
