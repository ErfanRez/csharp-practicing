using MassTransit;
using SMS.Common;
using SMS.Consumer.Data;

namespace SMS.Consumer.Services;



internal class SingleSmsConsumer : IConsumer<SendSingleSms>
{
    public async Task Consume(ConsumeContext<SendSingleSms> context)
    {
        var sequenceNumberKey = context.Message.SequenceNumber;
        var processedKey = sequenceNumberKey + context.Message.Receiver;

        if (SMSData.DeDupeSet.Contains(processedKey)) return;


        if (!SMSData.SMSDic.TryGetValue(sequenceNumberKey, out List<bool>? resultList)) resultList = SMSData.SMSDic[sequenceNumberKey] = new();

        bool status = await SendSmsAsync(
              context.Message.Sender,
              context.Message.Receiver,
              context.Message.Message);

        resultList.Add(status);

        if (resultList.Count == context.Message.TotalCount)
        {
            var apiEndpoint = await context.GetSendEndpoint(
                       new Uri("queue:web-service"));

            await apiEndpoint.Send(new BulkSmsIsProcessed
            {
                SequenceNumber = sequenceNumberKey,
                SentStatus = resultList.ToArray(),
            });
        }
    }

    private Task<bool> SendSmsAsync(string sender, string receiver, string text)
    {
        // Mock SMS Processing Logic
        return Task.FromResult(true);
    }
}
