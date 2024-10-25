using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application
{
    public class Result<T>
    {
        public T Data { get; private set; }
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }

        public Result(T data, string message)
        {
            Data = data;
            IsSuccess = true;
            Message = message;
        }

        public Result(string message)
        {
            Data = default;
            IsSuccess = false;
            Message = message;
        }

        public static Result<T> Success(T data, string message)
        {
            return new Result<T>(data, message);
        }

        public static Result<T> Failure(string message)
        {
            return new Result<T>(message);
        }
    }
}
