namespace clever_charging_key_api.Models
{
    public class ChargingkeyCreatedDTO
    {
        public string Id { get; set; }
    }

    public class ChargingkeyVerifyDTO
    {
        public bool Exists { get; set; }
        public bool Blocked { get; set; }
    }

    public class ChargingkeyBlockDTO
    {
        public string Id { get; set; }
        public bool Blocked { get; set; }
    }
}
