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

        public MatchApiClient(string baseAddress, Cookie cookie)
        {
            _matchBaseAddress = baseAddress;
            _authCookie = cookie;
        }
        
        public async Task<MatchModel> CalculateMatchAsync(int otherProfileId)
        {
            //we call ConfigureAwait(false) because caller requires context, and to prevent deadlock
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

        public void UpdateCacheAnswer(Answer answer)
        {
            CallMatchApiAsync<object>("/matches/updatecacheanswer", true, answer).Wait();
        }

        public async Task<List<MatchModel>> GetMatchesAsync(MatchCriteriaModel criteria, int page = 1)
        {
            var matches = await CallMatchApiAsync<List<MatchModel>>(FormatMatchQuery(page, criteria), false);
            return matches;
        }

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

                var matches = await response.Content.ReadAsAsync<T>();

                return matches;
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
