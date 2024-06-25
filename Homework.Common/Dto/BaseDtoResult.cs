using Homework.Common.Enums;

namespace Homework.Common.Dto
{
    public class BaseResult<TData>
    {
        public TData? Data { get; set; }
        public ErrorCode[] ErrorCodes { get; set; }

        public bool IsValid
        {
            get
            {
                return ErrorCodes == null || !ErrorCodes.Any();
            }
        }

        public BaseResult()
        {
        }

        public BaseResult(ErrorCode[] errorCodes)
        {
            ErrorCodes = errorCodes;
        }
    }

    public class BaseClientResult<TData>
    {
        public TData? Data { get; set; }
        public Dictionary<ErrorCode, string> Errors { get; set; }

        public bool IsValid
        {
            get
            {
                return Errors == null || !Errors.Any();
            }
        }

        public BaseClientResult()
        {
        }

        public BaseClientResult<TData> AddError(ErrorCode errorCode, string errorMessage)
        {
            Errors ??= new Dictionary<ErrorCode, string>();

            Errors.Add(errorCode, errorMessage);

            return this;
        }
    }
}
