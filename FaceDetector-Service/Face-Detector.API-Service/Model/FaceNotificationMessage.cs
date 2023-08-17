namespace Face_Detector.API_Service.Model
{
    public class FaceNotificationMessage
    {
        public int gateId { get; set; }
        public int FaceCount { get; set; }
        public string Message { get; set; }
    }
}
