﻿using Azure;
using Azure.AI.OpenAI;
using CommunityToolkit.Mvvm.ComponentModel;
using ParkingGPT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ParkingGPT.Services
{
    public class GPTVisionService
    {
        SettingsService settingsService;

        public GPTVisionService(SettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        // Commented due to the lack of support for more than 64 KB images 
        // https://github.com/Azure/azure-sdk-for-net/issues/40855
        //public async Task<Parking> GetParkingResult(string base64image)
        //{
        //    try
        //    {
        //        var settings = this.settingsService.GetSettingsFromStorage();
        //        OpenAIClient client = settings.IsUseOpenAI
        //                                ? new OpenAIClient(settings.EndpointKey)
        //                                : new OpenAIClient(
        //                                    new Uri(settings.EndpointURL),
        //                                    new AzureKeyCredential(settings.EndpointKey));

        //        string rawImageUri = $"data:image/jpeg;base64,{base64image}";
        //        ChatCompletionsOptions chatCompletionsOptions = new()
        //        {
        //            DeploymentName = settings.DeploymentModel,
        //            Messages =
        //            {
        //                new ChatRequestSystemMessage("You are a helpful assistant that reads the parking signs. All you have to give is the answer as true or false."),
        //                new ChatRequestUserMessage(
        //    new ChatMessageTextContentItem($"Can I park now? The date and time is {DateTime.Now}. Just give me an answer as true if it is a yes or false if it is a no."),
        //    new ChatMessageImageContentItem(new Uri(rawImageUri))),
        //            },
        //            Temperature = 0,
        //            MaxTokens = 300,
        //            // ResponseFormat = ChatCompletionsResponseFormat.JsonObject - This does not work with Vision
        //        };

        //        Response<ChatCompletions> chatResponse = await client.GetChatCompletionsAsync(chatCompletionsOptions);
        //        ChatChoice choice = chatResponse.Value.Choices[0];
        //        if (choice.FinishDetails is StopFinishDetails stopDetails)
        //        {
        //            return JsonSerializer.Deserialize<Parking>(choice.Message.Content);
        //        }

        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return null;
        //}

        public async Task<Parking> GetParkingResult(string base64image)
        {
            try
            {

                var settings = this.settingsService.GetSettingsFromStorage();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.EndpointKey);

                var payload = new
                {

                    model = "gpt-4-vision-preview",
                    messages = new[]
                    {
                    new {
                      role = "system",
                      content = new object[]
                      {
                          new
                          {
                              type = "text",
                              text = "You are a helpful assistant that reads the parking signs. You need to make sure that you calculate time correctly and do not get confuse with increasing times as increasing numbers. For example, 12 PM falls under 9 to 5 PM. You also need to ensure that the timings outside of the listed zone are also permitted as long as it is not a no parking area which has a different sign. " +
                              "Always include the current time provided to you to give a proof that you did the job right." +
                              "If there's a no stopping sign with school days then make sure you only restrict it during the specified hours."
                          }
                      }
                    },
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new
                            {
                                type = "text",
                                text = @$"The date and time is {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}. Can I park now? In the below JSON format, provide a 'true' answer if parking is allowed, and 'false' if it is not. Additionally, explain your decision based on the sign information.
                                #########
                                FORMAT 
                                #########
                                {{
                                   decision:
                                   description:
                                }}"
                            },
                            new
                            {
                                type = "image_url",
                                image_url = new
                                {
                                    url = $"data:image/jpeg;base64,{base64image}"
                                }
                            }
                        }
                    }
                },
                    temperature = 0,
                    max_tokens = 300
                };

                var payloadJson = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = JsonSerializer.Deserialize<GPTVisionResponse>(await response.Content.ReadAsStringAsync());
                    var parkingData = JsonSerializer.Deserialize<Parking>(responseData.choices[0].message.content.Replace("```json", "").Replace("```", "").Trim());
                    return parkingData;
                }

            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }
    }

    public class GPTVisionResponse
    {
        public string id { get; set; }
        public string _object { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public Usage usage { get; set; }
        public Choice[] choices { get; set; }
    }

    public class Usage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }

    public class Choice
    {
        public Message message { get; set; }
        public string finish_reason { get; set; }
        public int index { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}
