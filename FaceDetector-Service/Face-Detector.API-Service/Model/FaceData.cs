namespace Face_Detector.API_Service.Model
{
    public class FaceData
    {
        public FaceRectangle faceRectangle { get; set; }
    }

    public class FaceRectangle
    {
        public int top { get; set; }
        public int left { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}
