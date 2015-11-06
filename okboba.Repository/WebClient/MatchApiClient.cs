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

        public async Task<List<MatchModel>> GetMatches(MatchCriteriaModel criteria, int page = 1)
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

                var response = await client.GetAsync(FormatMatchQuery(page, criteria));

                response.EnsureSuccessStatusCode();

                var matches = await response.Content.ReadAsAsync<List<MatchModel>>();

                return matches;
            }
        }

        public void UpdateAnswer(Answer answer)
        {
            CallMatchApiAsync<object>("/answers", true, answer).Wait();
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
                    response = await client.PostAsJsonAsync(query, data);
                }
                else
                {
                    response = await client.GetAsync(query);
                }
                
                response.EnsureSuccessStatusCode();

                var matches = await response.Content.ReadAsAsync<T>();

                return matches;
            }
        }

        private string FormatMatchQuery(int page, MatchCriteriaModel criteria)
        {
            var str = "/matches/getmatches?page=" + page;
            str += "&Gender=" + criteria.Gender;
            str += "&LocationId1=" + criteria.LocationId1;
            return str;
        }
    }
}
