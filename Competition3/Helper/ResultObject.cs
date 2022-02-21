using System.Collections.Generic;

namespace Competition3.Helper
{
    public class ResultObject
    {
        public bool Success { get; set; }
        
        public int Id { get; set; }

        public string Message { get; set; }
        
        public List<string> Errors { get; set; }
        
        public object Extera { get; set; }
    }
}