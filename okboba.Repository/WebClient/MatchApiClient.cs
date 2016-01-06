using Newtonsoft.Json;
using okboba.Entities;
using okboba.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.WebClient
{
    public class MatchApiClient
    {
        private string _matchBaseAddress;
        private Cookie _authCookie;

        /// <summary>
        /// MatchApiClient is created per request since we need the authentication cookie to make the web request.
        /// The cookie info comes from the HttpContext object in the controllers.
        /// </summary>
        public MatchApiClient(string baseAddress, Cookie cookie)
        {
            _matchBaseAddress = baseAddress;
            _authCookie = cookie;
        }
        
        /// <summary>
        /// Makes a web service call to calculate the match score between two users.
        /// </summary>
        public async Task<MatchModel> CalculateMatchAsync(int otherProfileId)
        {
            //we call ConfigureAwait(false) here and other library async methods since we don't need to resume on the
            //original context. We can't use async all the way through because Child Actions can't be asynchronous. 
            //This function is used by the ProfileHeader action which calls Task.Result, which would deadlock if
            //ConfigureAwait(false) wasn't used.
            //
            //From: https://msdn.microsoft.com/en-us/magazine/jj991977.aspx
            //"The root cause of this deadlock is due to the way await handles contexts. By default, 
            // when an incomplete Task is awaited, the current “context” is captured and used to resume the 
            // method when the Task completes. This “context” is the current SynchronizationContext unless 
            // it’s null, in which case it’s the current TaskScheduler. GUI and ASP.NET applications have a 
            // SynchronizationContext that permits only one chunk of code to run at a time. When the await 
            // completes, it attempts to execute the remainder of the async method within the captured context. 
            // But that context already has a thread in it, which is (synchronously) waiting for the async 
            // method to complete. They’re each waiting for the other, causing a deadlock."
                        
            var result = await CallMatchApiAsync<MatchModel>("/api/matches/calculatematch?otherProfileId=" + otherProfileId, false).ConfigureAwait(false);
            return result;
        }        

        /// <summary>
        /// Makes a web service call to retrieve a page of matches, and returns a List of MatchModels
        /// </summary>
        public async Task<IList<MatchModel>> GetMatchesAsync(MatchCriteriaModel criteria, int page = 1)
        {
            var matches = await CallMatchApiAsync<IList<MatchModel>>(FormatMatchQuery(page, criteria), false).ConfigureAwait(false);
            return matches;
        }

        /// <summary>
        /// Makes a web service call to add/update the answer in the memory cache
        /// </summary>
        /// <param name="answer"></param>
        public async Task AnswerAsync(Answer answer)
        {
            await CallMatchApiAsync<string>("/api/answer", true, answer).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a generic method that sends out a web request and returns an object of type T. It uses
        /// HttpClient.  It will set the cookies for authentication. It expects JSON from the web service 
        /// and deserializes this into an object of type T. ConfigureAwait(false) is also used here because
        /// we don't need to resume on original context
        ///  
        ///     T - this is the type returned. It is deserialized from JSON
        ///     query - this is the query string used to make the request
        ///     post - flag which if set to true will make a POST request, otherwise uses GET
        ///     data - this is the data to send with the POST request.
        /// </summary>
        private async Task<T> CallMatchApiAsync<T>(string query, bool post, object data = null)
        {
            var cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler))
            {
                //setup the http client
                client.BaseAddress = new Uri(_matchBaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                cookieContainer.Add(new Uri(_matchBaseAddress), _authCookie);

                HttpResponseMessage response;
                if(post)
                {
                    response = await client.PostAsJsonAsync(query, data).ConfigureAwait(false);
                }
                else
                {
                    response = await client.GetAsync(query).ConfigureAwait(false);
                }
                
                response.EnsureSuccessStatusCode();

                //var content = await response.Content.ReadAsAsync<T>().ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        private string FormatMatchQuery(int page, MatchCriteriaModel criteria)
        {
            var str = "/api/matches?page=" + page;
            str += "&Gender=" + criteria.Gender;
            str += "&LocationId1=" + criteria.LocationId1;
            return str;
        }
    }
}
