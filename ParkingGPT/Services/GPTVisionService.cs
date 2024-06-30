using Azure;
using Azure.AI.OpenAI;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.SemanticKernel;
using ParkingGPT.Model;
using Kernel = Microsoft.SemanticKernel.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.SemanticKernel.ChatCompletion;

namespace ParkingGPT.Services
{
    public class GPTVisionService
    {
        Kernel kernel;
        SettingsService settingsService;

        public GPTVisionService(SettingsService settingsService)
        {
            this.settingsService = settingsService;
            InitializeKernel();
        }

        public void InitializeKernel()
        {
            var settings = this.settingsService.GetSettingsFromStorage();

            //Create Kernel builder
            var builder = Kernel.CreateBuilder();

            if (!settings.IsUseOpenAI)
            {
                builder.AddAzureOpenAIChatCompletion(settings.DeploymentModel, settings.EndpointURL, settings.EndpointKey);
            }
            else
            {
                builder.AddOpenAIChatCompletion("gpt-4o", settings.EndpointKey);
            }

            // Build the kernel
            kernel = builder.Build();
        }

        public async Task<Parking> GetParkingResult(byte[] byteImage)
        {
            try
            {

                var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
                var chatHistory = new ChatHistory("You are a helpful assistant that reads the parking signs. " +
                    "You need to make sure that you calculate time correctly and " +
                    "do not get confuse with increasing times as increasing numbers. " +
                    "For example, 12 PM falls under 9 to 5 PM. You also need to ensure that the timings outside of the listed zone are also permitted as long as it is not a no parking area which has a different sign. \" +\r\n                              \"Always include the current time provided to you to give a proof that you did the job right.\" +\r\n                              \"If there's a no stopping sign with school days then make sure you only restrict it during the specified hours.\"");

                chatHistory.AddUserMessage(
                [
                    new TextContent(@$"Can I park now? The date and time is {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}. Just give me an answer as true if it is a yes or false if it is a no. In addition to this, also include the description as why you have chosen this decision. The below format should not contain any delimeters such as ```json```, it should also contain the specified format.
                                #########
                                FORMAT 
                                #########
                                {{
                                   ""decision"":,
                                   ""description"":""
                                }}"),
                    new ImageContent(byteImage, "image/jpg")
                ]);

                var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
                ChatResponseMessage chatResponse = response.InnerContent as ChatResponseMessage;

                if(chatResponse.Content is not null)
                {
                    var parkingData = JsonSerializer.Deserialize<Parking>(chatResponse.Content);
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
}
