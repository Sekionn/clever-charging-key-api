namespace clever_charging_key_api.Models
{
    public class ChargingKey
    {
        public ChargingKey(string id, KeyTag tag)
        {
            Id = id;
            Tag = tag;
        }

        public string Id { get; set; }
        public KeyTag Tag { get; set; }
        public bool Blocked { get; set; } = false;

    }

    public enum KeyTag {
        CleverOne,
        CleverBox,
        CleverGo
    }
}
