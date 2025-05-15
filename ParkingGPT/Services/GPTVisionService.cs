// Load the plugins
#pragma warning disable SKEXP0010
using Microsoft.SemanticKernel;
using ParkingGPT.Model;
using Kernel = Microsoft.SemanticKernel.Kernel;
using System.Text.Json;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Chat;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

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

            // Check if EndpointKey is empty
            if (string.IsNullOrEmpty(settings.EndpointKey))
            {
                return; 
            }

            //Create Kernel builder
            var builder = Kernel.CreateBuilder();

            if (!settings.IsUseOpenAI)
            {
                builder.AddAzureOpenAIChatCompletion(settings.DeploymentModel, settings.EndpointURL, settings.EndpointKey);
            }
            else
            {
                builder.AddOpenAIChatCompletion("gpt-4o-mini", settings.EndpointKey);
            }

            // Build the kernel
            kernel = builder.Build();
        }

        public async Task<Parking> GetParkingResult(byte[] byteImage)
        {
            try
            {
                var settings = this.settingsService.GetSettingsFromStorage();
                var chatCompletionService = new AzureOpenAIChatCompletionService(settings.DeploymentModel, settings.EndpointURL, settings.EndpointKey);
                var chatHistory = new ChatHistory("You are a helpful assistant that reads the parking signs. " +
                    "You need to make sure that you calculate time correctly and do not get confuse with increasing times as increasing numbers. For example, 12 PM falls under 9 to 5 PM.\n" +
                    "You also need to ensure that the timings outside of the listed zone are also permitted as long as it is not a no parking area which has a different sign.\n" +
                    "Always include the current time provided to you to give a proof that you did the job right.\n" +
                    "If there is no stopping sign with school days then make sure you only restrict it during the specified hours.\n" +
                    "If there are multiple parking signs provided to you where one may permit you while the other one may not then you need to give.\n" +
                    "You need to provide yes, no or warning in decision response.");

                ChatResponseFormat chatResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "parking_decision",
                jsonSchema: BinaryData.FromString("""
                    {
                      "type": "object",
                      "properties": {
                        "decision": {
                          "type": "string",
                          "enum": ["yes", "no", "warning"],
                          "description": "The assistant's decision on whether parking is allowed ('yes'), not allowed ('no'), or requires caution ('warning')."
                        },
                        "description": {
                          "type": "string",
                          "description": "A detailed explanation supporting the decision, including any relevant time calculations and considerations."
                        }
                      },
                      "required": ["decision", "description"],
                      "additionalProperties": false
                    }
                    """),
                jsonSchemaIsStrict: true);

                // Specify response format by setting ChatResponseFormat object in prompt execution settings.
                var executionSettings = new Microsoft.SemanticKernel.Connectors.OpenAI.OpenAIPromptExecutionSettings
                {
                    ResponseFormat = chatResponseFormat
                };

                chatHistory.AddUserMessage(
                [
                    new TextContent(@$"Can I park now? The date and time is {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}. Just give me the right answer as yes, no or warning. In addition to this, also include the description as why you have chosen this decision"),
                    new ImageContent(byteImage, "image/jpg")
                ]);

                var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings);
                
                if (response is not null)
                {
                    var parkingData = JsonSerializer.Deserialize<Parking>(response.Content);
                    return parkingData;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return null;
        }
    }
}
