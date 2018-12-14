namespace VRC.Core.BestHTTP.Decompression.Zlib
{
	internal enum BlockState
	{
		NeedMore,
		BlockDone,
		FinishStarted,
		FinishDone
	}
}
