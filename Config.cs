using Newtonsoft.Json;

namespace SSTUScheduleBot
{
    public class Config
    {
        [JsonProperty(PropertyName = "connectionString")]
        public string ConnectionString { get; set; }
        [JsonProperty(PropertyName = "telegramKey")]
        public string TelegramApiKey   { get; set; }
    }
}