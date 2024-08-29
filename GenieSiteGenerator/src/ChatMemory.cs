using Microsoft.SemanticKernel.ChatCompletion;

namespace GenieSiteGenerator.src
{
  
public class ChatMemoryService
    {
        private readonly Dictionary<string, ChatHistory> _history = new Dictionary<string, ChatHistory>();

        public void AddToHistory(string key, ChatHistory value)
        {
            _history[key] = value;
        }

        public ChatHistory GetHistory(string key)
        {
            if (_history.ContainsKey(key))
            {
                return _history[key];
            }
            else
            {
                // TODO: Replace with Chroma based history or other persistent storage
                var history = new ChatHistory();
                _history[key] = history;
                return history;
            }
        }
    }

}
