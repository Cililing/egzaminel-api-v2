using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EgzaminelAPI.Helpers
{

    public enum ERROR_CODE
    {
        UNKNOWN = -1,
        USER_VALIDATION_ERROR = 1
    }

    public class EgzaminelException : Exception
    {
        public ERROR_CODE ErrorCode { get; private set; }

        public EgzaminelException()
        {
            this.ErrorCode = ERROR_CODE.UNKNOWN;
        }

        public EgzaminelException(ERROR_CODE errorCode)
        {
            this.ErrorCode = errorCode;
        }

    }
}
