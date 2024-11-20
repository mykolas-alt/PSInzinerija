using System;

namespace PSInzinerija1.Exceptions
{
    public class WordListLoadException : Exception
    {
        public WordListLoadException(string message) : base(message) { }
    }
}