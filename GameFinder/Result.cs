using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace GameFinder
{
    /// <summary>
    /// Result of an operation.
    /// </summary>
    /// <typeparam name="T">Type of the Value</typeparam>
    [PublicAPI]
    public class Result<T>
    {
        /// <summary>
        /// Value of the result.
        /// </summary>
        public readonly T Value;

        /// <summary>
        /// List of Errors that occurred during the operation.
        /// </summary>
        public readonly List<string> Errors = new List<string>();

        /// <summary>
        /// Determines if the Result has Errors.
        /// </summary>
        public bool HasErrors => Errors.Count != 0;
        
        /// <summary>
        /// Determines if the Result has a Value.
        /// </summary>
        public bool HasValue => Value != null;

        internal Result()
        {
            Value = default!;
        }

        internal Result(T value)
        {
            Value = value;
        }
        
        /// <summary>
        /// Try get the value of the result.
        /// </summary>
        /// <param name="value">Value if true, else null</param>
        /// <returns></returns>
        public bool TryGetValue([MaybeNullWhen(false)] out T value)
        {
            value = default;
            if (Value == null) return false;
            value = Value!;
            return true;
        }

        internal void AddError(string error)
        {
            Errors.Add(error);
        }

        internal void AppendErrors<TO>(Result<TO> otherResult)
        {
            Errors.AddRange(otherResult.Errors);
        }

        internal void AppendErrors(IEnumerable<string> errors)
        {
            Errors.AddRange(errors);
        }

        /// <summary>
        /// Aggregates all Errors and returns a String.
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string ErrorsToString(char separator = '\n')
        {
            return Errors.Any() ? Errors.Aggregate((x, y) => $"{x}{separator}{y}") : string.Empty;
        }
    }

    [PublicAPI]
    internal static class ResultUtils
    {
        internal static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value);
        }

        internal static Result<bool> Ok()
        {
            return new Result<bool>(true);
        }

        internal static Result<bool> NotOk()
        {
            return new Result<bool>(false);
        }

        internal static Result<bool> Ok(Result<bool> prev)
        {
            var res = Ok();
            res.AppendErrors(prev);
            return res;
        }
        
        internal static Result<bool> NotOk(Result<bool> prev)
        {
            var res = NotOk();
            res.AppendErrors(prev);
            return res;
        }

        internal static Result<bool> NotOk(string error)
        {
            var res = new Result<bool>(false);
            res.AddError(error);
            return res;
        }

        public static Result<bool> NotOk(IEnumerable<string> errors)
        {
            var res = new Result<bool>(false);
            res.AppendErrors(errors);
            return res;
        }
        
        internal static Result<T> Err<T>(string error)
        {
            var res = new Result<T>();
            res.Errors.Add(error);
            return res;
        }

        internal static Result<T> Errs<T>(IEnumerable<string> errors)
        {
            var res = new Result<T>();
            res.Errors.AddRange(errors);
            return res;
        }

        internal static Result<T> Err<T, TO>(Result<TO> otherRes)
        {
            var res = new Result<T>();
            res.AppendErrors(otherRes);
            return res;
        }
        
        public static bool IsOk(this Result<bool> res)
        {
            return res.Value;
        }
    }
}
