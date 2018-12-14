namespace VRC.Core.BestHTTP
{
	internal interface IProtocol
	{
		bool IsClosed
		{
			get;
		}

		void HandleEvents();
	}
}
