using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameFonts
{
    // nothing special here, just wanted a unique class so that
    // calling program can handle GameFont-specific exceptions
    class GameFontException : Exception
    {
        public GameFontException() : base() { }
        public GameFontException(string msg) : base(msg) { }
        public GameFontException(string msg, Exception inner) :
            base(msg, inner) { }
    }
}
