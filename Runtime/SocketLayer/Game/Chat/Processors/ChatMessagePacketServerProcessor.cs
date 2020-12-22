using Assets.Scripts.Chat.ChatFilter;
using Assets.Scripts.Chat.ChatFilter.Base;
using Assets.Scripts.SocketLayer.Models;
using Assets.SocketLayer.PacketParser;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SocketLayer.Game.Chat.Processors
{
    class ChatMessagePacketServerProcessor : PacketServerProcessor<ChatMessage, Guid>
    {
        private readonly Dictionary<int, List<ChatFilterBase>> chatFilter;

        public override INetworkSerializer<ChatMessage> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public ChatMessagePacketServerProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = new BaseNetworkSerializer<ChatMessage>();

            chatFilter = new Dictionary<int, List<ChatFilterBase>>();

            InitializeChatFilter();
        }

        private void InitializeChatFilter()
        {


            foreach (ChatFilterBase word in ChatFilter.Instance.chatFilterBases)
            {
                int length = word.OriginalText.Length;

                if (!chatFilter.ContainsKey(length))
                {
                    chatFilter.Add(length, new List<ChatFilterBase>());
                }

                chatFilter[length].Add(word);
            }
        }

        public override bool Validate(ref ChatMessage data)
        {
            UseFilterTry(ref data);

            return true;
        }

        private void UseFilterTry(ref ChatMessage data)
        {
            List<Tuple<int, int, ChatFilterBase>> censorList = new List<Tuple<int, int, ChatFilterBase>>();

            // get the indexes of the words to censor

            foreach (KeyValuePair<int, List<ChatFilterBase>> kv in chatFilter)
            {
                // if word is too short , skip

                if (data.Content.Length < kv.Key)
                {
                    continue;
                }


                for (int start = 0; start <= data.Content.Length - kv.Key; start++)
                {

                    string currentWord = data.Content.Substring(start, kv.Key);

                    // if the word exists in the list of filtered words

                    ChatFilterBase wordFilterResult = kv.Value.FirstOrDefault(f => f.OriginalText.Equals(currentWord));

                    if (wordFilterResult != null)
                    {
                        censorList.Add(new Tuple<int, int, ChatFilterBase>(start, kv.Key, wordFilterResult));
                    }
                }
            }

            // replace the words with the '*' character

            StringBuilder chatArray = new StringBuilder(data.Content);

            // order by word position

            censorList = censorList.OrderBy(t => t.Item1).ToList();

            for (int i1 = 0; i1 < censorList.Count; i1++)
            {
                Tuple<int, int, ChatFilterBase> indexTuple = censorList[i1];

                for (int i = indexTuple.Item1; i < indexTuple.Item1 + indexTuple.Item2; i++)
                {
                    string originalText = indexTuple.Item3.OriginalText;
                    string replacementText = indexTuple.Item3.ReplacementText;
                    int startIndex = indexTuple.Item1;
                    int length = indexTuple.Item2;

                    // difference in length between old word and new word
                    int lengthCompensation = originalText.Length - replacementText.Length;

                    chatArray.Replace(originalText, replacementText, startIndex, length);

                    // pus hthe counters back to compensate for the character number change
                    for (int next = i1; next < censorList.Count; next++)
                    {
                        Tuple<int, int, ChatFilterBase> replacement = new Tuple<int, int, ChatFilterBase>(censorList[next].Item1 + lengthCompensation, censorList[next].Item2, censorList[next].Item3);

                        censorList[next] = replacement;
                    }

                }
            }

            data.Content = chatArray.ToString();
        }
    }
}
