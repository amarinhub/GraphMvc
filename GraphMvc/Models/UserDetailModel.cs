﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace GraphMvc.Models
{
    public class UserDetailModel
    {

        public UserModel User { get; set; }
        public UserModel Manager { get; set; }
        public List<UserModel> DirectReports { get; set; }
        public List<FileModel> Files { get; set; }

        // GetUserDetail( "me" , "eyhksskdhdsdSDSF......")
        public async static Task<UserDetailModel> GetUserDetail(string path, string token)
        {
            UserDetailModel data = new UserDetailModel();

            //get the user              GetJson("https://graph.microsoft.com/v1.0/me", "ehjedjhasajhsgbda=")

            var json = await GetJson(String.Format("https://graph.microsoft.com/v1.0/{0}", path), token);
            data.User = JsonConvert.DeserializeObject<UserModel>(json);

            //get the manager...might not exist
            json = await GetJson(String.Format("https://graph.microsoft.com/v1.0/{0}/manager", path), token);
            if (json == null)
                data.Manager = new UserModel();
            else
                data.Manager = JsonConvert.DeserializeObject<UserModel>(json);

            //get the direct reports
            json = await GetJson(String.Format("https://graph.microsoft.com/v1.0/{0}/directReports", path), token);
            data.DirectReports = JObject.Parse(json).SelectToken("value").ToObject<List<UserModel>>();

            //get the files
            json = await GetJson(String.Format("https://graph.microsoft.com/v1.0/{0}/drive/root/children", path), token);
            if (json == null)
                data.Files = new List<FileModel>();
            else
                data.Files = JObject.Parse(json).SelectToken("value").ToObject<List<FileModel>>();

            return data;
        }

        private async static Task<string> GetJson(string endpoint, string accessToken)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            using (HttpResponseMessage response = await client.GetAsync(new Uri(endpoint)))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                    return null;
            }
        }
    }
}