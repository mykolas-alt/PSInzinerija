using System;

namespace Frontend.Exceptions
{
    public class WordListLoadException : Exception
    {
        public WordListLoadException(string message) : base(message) { }
    }
}