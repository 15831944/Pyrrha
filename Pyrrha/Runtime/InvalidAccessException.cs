﻿
#region Referencing

using System.Linq;

#endregion

namespace Pyrrha.Runtime
{
    public class InvalidAccessException : PyrrhaException
    {
        public InvalidAccessException(string message) 
            : base(message)
        {
        }
    }
}
