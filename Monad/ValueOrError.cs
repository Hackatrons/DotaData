namespace DotaData.Monad
{
    /// <summary>
    /// A discriminated union that represents a value or exception, and never both.
    /// </summary>
    internal readonly struct ValueOrError<T>
    {
        public bool IsError => !IsSuccess;
        public bool IsSuccess => _error == null;

        readonly T? _value;
        readonly Exception? _error;

        public ValueOrError(T value) { _value = value; }
        public ValueOrError(Exception error) { _error = error; }

        public static implicit operator ValueOrError<T>(T value) { return new ValueOrError<T>(value); }
        public static implicit operator ValueOrError<T>(Exception error) { return new ValueOrError<T>(error); }

        public T GetValue()
        {
            if (IsError)
                throw new InvalidOperationException("Value is in error state", _error);

            return _value!;
        }

        public Exception GetError()
        {
            if (IsError)
                return _error!;

            throw new InvalidOperationException("Value is not in error state");
        }
    }
}
