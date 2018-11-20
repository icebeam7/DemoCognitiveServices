using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using DemoCognitiveServices.Modelos.Face;
using DemoCognitiveServices.Helpers;

namespace DemoCognitiveServices.Servicios
{
    public static class ServicioFace
    {
        private static readonly HttpClient FaceApiClient = CreateHttpClient();
        
        private static HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Constantes.FaceApiEndpoint);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.ConnectionClose = false;
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Constantes.FaceApiSubscriptionKey);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        public static async Task<bool> CreateGroup(string name)
        {
            try
            {
                var uri = Constantes.PersonGroup;

                var info = new MetaDataInfo() { Name = name, UserData = "Empleados de mi empresa" };
                var json = JsonConvert.SerializeObject(info);
                var content = new StringContent(json);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await FaceApiClient.PutAsync(uri, content);
                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async static Task<string> CreatePerson(string name)
        {
            try
            {
                var uri = $"{Constantes.PersonGroup}{Constantes.Person}";

                var info = new MetaDataInfo() { Name = name, UserData = "Miembro del grupo" };
                var json = JsonConvert.SerializeObject(info);
                var content = new StringContent(json);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await FaceApiClient.PostAsync(uri, content);
                var person = await response.Content.ReadAsStringAsync();

                var obj = JObject.Parse(person);
                return obj["personId"].ToString();
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

        public async static Task<FaceModel> DetectFaces(Stream stream)
        {
            try
            {
                var parameters = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";
                var uri = $"{Constantes.Detect}?{parameters}";

                var ms = new MemoryStream();
                stream.CopyTo(ms);

                using (var content = new ByteArrayContent(ms.ToArray()))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    var response = await FaceApiClient.PostAsync(uri, content);
                    var faces = await response.Content.ReadAsStringAsync();

                    var models = JsonConvert.DeserializeObject<List<FaceModel>>(faces);
                    return models[0];
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async static Task<string> RegisterPerson(string id, Stream stream)
        {
            try
            {
                var parameters = $"/{id}/persistedFaces";
                var uri = $"{Constantes.PersonGroup}{Constantes.Person}{parameters}";

                var ms = new MemoryStream();
                stream.CopyTo(ms);

                using (var content = new ByteArrayContent(ms.ToArray()))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    var response = await FaceApiClient.PostAsync(uri, content);
                    var person = await response.Content.ReadAsStringAsync();

                    var obj = JObject.Parse(person);
                    return obj["persistedFaceId"].ToString();
                }
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

        public async static Task<bool> TrainGroup()
        {
            try
            {
                var uri = $"{Constantes.PersonGroup}{Constantes.GroupTrain}";

                var content = new StringContent("");
                var response = await FaceApiClient.PostAsync(uri, content);

                return (response.StatusCode == System.Net.HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async static Task<string> IdentifyPerson(Stream stream)
        {
            try
            {
                var ms = new MemoryStream();
                stream.CopyTo(ms);
                var model = await DetectFaces(ms);

                var uri = Constantes.Identify;

                var identifyModel = new IdentifyModel()
                {
                    FaceIds = new List<string>() { model.FaceID },
                    PersonGroupId = Constantes.Group,
                    MaxNumOfCandidatesReturned = 1,
                    ConfidenceThreshold = 0.5
                };

                var json = JsonConvert.SerializeObject(identifyModel);
                var content = new StringContent(json);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await FaceApiClient.PostAsync(uri, content);

                var stringCandidates = await response.Content.ReadAsStringAsync();
                var array = JArray.Parse(stringCandidates);

                var obj = JObject.Parse(array[0].ToString());
                var candidates = obj["candidates"].ToString();

                var arrayCandidates = JArray.Parse(candidates);
                var best = JObject.Parse(arrayCandidates[0].ToString());
                var personId = best["personId"].ToString();
                var confidence = double.Parse(best["confidence"].ToString());

                string name = await GetPerson(personId);
                return $"{name} ({confidence * 100}%)";
            }
            catch (Exception ex)
            {
                return "Desconocido";
            }
        }

        public async static Task<string> GetPerson(string id)
        {
            try
            {
                var parameters = $"/{id}";
                var uri = $"{Constantes.PersonGroup}{Constantes.Person}{parameters}";

                var response = await FaceApiClient.GetAsync(uri);
                var person = await response.Content.ReadAsStringAsync();

                var obj = JObject.Parse(person);
                return obj["name"].ToString();
            }
            catch (Exception ex)
            {
                return "Desconocido";
            }
        }
    }
}
