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
using System.Web;

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
        /// Gets the intersection of p1's answers with p2's answers and returns a dictionary
        /// of p1's answers.
        /// </summary>
        public async Task<IDictionary<short,Answer>> GetIntersectionAsync(int p1, int p2)
        {
            var query = "/api/answer/getintersect?p1=" + p1 + "&p2=" + p2;
            var result = await CallMatchApiAsync<IDictionary<short, Answer>>(query, false).ConfigureAwait(false);
            return result;
        }
        
        /// <summary>
        /// Makes a web service call to calculate the match score between two users.
        /// </summary>
        public async Task<MatchModel> CalculateMatchAsync(int otherProfileId)
        {                        
            var result = await CallMatchApiAsync<MatchModel>("/api/matches/calculatematch?otherProfileId=" + otherProfileId, false).ConfigureAwait(false);
            return result;
        }        

        /// <summary>
        /// Makes a call to the Web API to calculate and save the user's matches in the cache
        /// </summary>
        public async Task CalculateAndSaveMatchesAsync(MatchCriteriaModel criteria)
        {
            var query = "/api/matches/calculateandsavematches?" + GetQueryString(criteria);
            await CallMatchApiAsync<bool>(query, false).ConfigureAwait(false);
        }

        /// <summary>
        /// Makes a web service call to add/update the answer in the memory cache
        /// </summary>
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
        ///     
        /// Note: use ConfigureAwait(false) to prevent deadlock when called from synchronous methods.
        /// Reference: https://msdn.microsoft.com/en-us/magazine/jj991977.aspx
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

        /// <summary>
        /// Serializes the given object to an HTTP query string
        /// </summary>
        private string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }

    }
}
