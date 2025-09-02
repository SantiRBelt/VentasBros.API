using System;

namespace Application.DTOs.Common
{
    [Serializable]
    public class ApiException
    {
        public DateTime TimeStamp { get; set; }
        public string? Message { get; set; } = default!;
        public string? StackTrace { get; set; } = default!;
        public bool Success { get; set; }

        public ApiException InnerException { get; set; } = default!;

        public ApiException()
        {
            this.TimeStamp = DateTime.Now;
        }

        public ApiException(string Message) : this()
        {
            this.TimeStamp = DateTime.Now;
            this.Message = Message;
            this.Success = false;
        }

        public ApiException(Exception ex)
        {
            this.TimeStamp = DateTime.Now;
            if (ex != null)
            {
                this.Message = ex.Message;
                this.StackTrace = ex.StackTrace;
                if (ex.InnerException != null)
                {
                    this.InnerException = new ApiException(ex.InnerException);
                }
            }
        }

        public override string ToString()
        {
            return this.Message + this.StackTrace;
        }
    }
}