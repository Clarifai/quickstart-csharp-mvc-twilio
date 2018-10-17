using Clarifai.API;
using Clarifai.DTOs.Inputs;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebApplication23.Controllers
{
    public class SmsController : TwilioController
    {
        [HttpPost]
        public async Task<TwiMLResult> Index(SmsRequest request)
        {
            // TODO: Define the environment variable or plug your api key in
            var CLARIFAI_API_KEY = System.Environment.GetEnvironmentVariable("CLARIFAI_API_KEY") ?? "YOUR_CLARIFAI_API_KEY_HERE";

            var response = new MessagingResponse();

            var url = Request.Form["MediaUrl0"];

            if (string.IsNullOrEmpty(url))
            {
                response.Message("Picture not found. Please send one.");
                return TwiML(response);
            }

            try
            {
                var client = new ClarifaiClient(CLARIFAI_API_KEY);

                var image = new ClarifaiURLImage(url);

                var result = await client.PublicModels.GeneralModel.Predict(image, maxConcepts: 5).ExecuteAsync();

                if (result.IsSuccessful)
                {
                    var concepts = result.Get().Data.Select(c => $"{c.Name}:{c.Value}");
                    var body = string.Join(", ", concepts);

                    response.Message(body);
                }
                else
                {
                    response.Message($"The request was not successful: {result.Status.Description}");
                }
            }
            catch (Exception e)
            {
                response.Message($"Something went wrong: {e.Message}");
            }

            return TwiML(response);
        }
    }
}