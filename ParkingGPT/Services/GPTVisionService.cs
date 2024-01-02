using Azure;
using Azure.AI.OpenAI;
using ParkingGPT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
                              text = "You are a helpful assistant that reads the parking signs. You need to make sure that you calculate time correctly and do not get confuse with increasing times as increasing numbers. For example, 12 PM falls under 9 to 5 PM."
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
                                text = @$"Can I park now? The date and time is {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}. Just give me an answer as true if it is a yes or false if it is a no. In addition to this, also include the description as why you have chosen this decision.
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
                    var responseData = JsonSerializer.Deserialize<Parking>(await response.Content.ReadAsStringAsync());
                    return responseData;
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
