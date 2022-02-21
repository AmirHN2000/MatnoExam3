namespace Competition3.ViewModels
{
    public class ZarinpalRequestResult
    {
        public ZarinData Data { get; set; }
    }
    
    public class ZarinData
    {
        public  int Code { get; set; }
        public  string Message { get; set; }
        public  string authority { get; set; }
    }

    public class ZarinPalVerifyResult
    {
        public  VerifyData Data { get; set; }
    }

    public class VerifyData
    {
        public  int Code { get; set; }
        public  long ref_id { get; set; }
    }
}