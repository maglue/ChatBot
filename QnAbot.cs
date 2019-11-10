
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Linq;
namespace Microsoft.BotBuilderSamples.Bots
{
    class Pair
    {
        public string Name { get; set; }
        public string Picture { get; set; }
    }
    
    public class QnABot<T> : ActivityHandler where T : Microsoft.Bot.Builder.Dialogs.Dialog
    {
        protected readonly BotState ConversationState;
        protected readonly Microsoft.Bot.Builder.Dialogs.Dialog Dialog;
        protected readonly BotState UserState;
        List<Pair> list_note = new List<Pair>();
        public string link = "https://mycleantownimages.blob.core.windows.net/mycleantownimagesmaterial/20191109_123944.jpg";
        public QnABot(ConversationState conversationState, UserState userState, T dialog)
        {
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
            
            list_note.Add(new Pair() { Name = "user1", Picture = "https://mycleantownimages.blob.core.windows.net/mycleantownimagesmaterial/20191109_123944.jpg" });
            list_note.Add(new Pair() { Name = "user2", Picture = "https://mycleantownimages.blob.core.windows.net/mycleantownimagesmaterial/20191109_123957.jpg" });
            list_note.Add(new Pair() { Name = "user3", Picture = "https://mycleantownimages.blob.core.windows.net/mycleantownimagesmaterial/20191109_124007.jpg" });
            
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
            // Run the Dialog with the new message Activity
        {
                
            
                if (turnContext.Activity.Text.Contains("user1"))
                {
                    var obj = list_note.ElementAt(0);
                    link = obj.Picture;
                }
                
                if (turnContext.Activity.Text.Contains("guest"))
                {
                    link = Microsoft.BotBuilderSamples.Dialog.QnAMakerBaseDialog.link;
                }
                
                if (turnContext.Activity.Text.Contains("user2"))
                {
                    var obj = list_note.ElementAt(1);
                    link = obj.Picture;
                }
                
                if (turnContext.Activity.Text.Contains("user3"))
                {
                    var obj = list_note.ElementAt(2);
                    link = obj.Picture;
                }
            
                
                if (turnContext.Activity.Text.Contains("Evaluate"))
                {
                    Microsoft.BotBuilderSamples.Bots.CustomVisionService.MakePredictionRequest(link);
                    var l1 = Microsoft.BotBuilderSamples.Bots.CustomVisionService.probabilities;
                    var l2 = Microsoft.BotBuilderSamples.Bots.CustomVisionService.tags;
                    string s = l1.Aggregate((a, b) => a + ", " + b) + " - Values - "+  l2.Aggregate((a, b) => a + ", " + b);
                    
                    await turnContext.SendActivityAsync("I say that your picture is " + l2.ElementAt(0).ToString() + " and " + l2.ElementAt(1).ToString() + ". Keep going.");
                    // await stepContext.SendActivityAsync(s);
                    //await Dialog.RunAsync(Message.Text(s), ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
                    //return await stepContext.SendActivityAsync(Message.Text(s), cancellationToken).ConfigureAwait(false);
                    //return await stepContext.NextAsync(Message.Text(s), cancellationToken).ConfigureAwait(false);
                    
                }
            
            //await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and welcome!"), cancellationToken);
                }
            }
        }
    }
}
