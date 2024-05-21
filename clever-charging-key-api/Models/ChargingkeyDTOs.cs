namespace clever_charging_key_api.Models
{
    /// <summary>
    /// These are data transfer objects for returning the necessary data in the endpoints
    /// </summary>
    public class ChargingKeyCreatedDTO
    {
        public string Id { get; set; }
    }

    public class ChargingKeyVerifyDTO
    {
        public bool Exists { get; set; }
        public bool Blocked { get; set; }
    }

    public class ChargingKeyBlockDTO
    {
        public string Id { get; set; }
        public bool Blocked { get; set; }
    }
}
