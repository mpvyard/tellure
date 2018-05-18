using System;

namespace Domain.Entity
{
	public class Either
	{
		public Either(bool success, Exception exception = null)
		{
			this.Success = success;
			this.Exception = exception;
		}

		public bool Success { get; private set; }
		public Exception Exception { get; private set; }
	}
}
