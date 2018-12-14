using System;
using System.Collections.Generic;
using System.IO;

namespace librsync.net
{
	internal static class DeltaCalculator
	{
		private const byte MinVariableLiteral = 65;

		private const byte MinCopyCommand = 69;

		private const int MaxLiteralLength = 4194304;

		public static void WriteCommand(BinaryWriter s, OutputCommand command)
		{
			if (command.Kind == CommandKind.Literal)
			{
				if (command.Literal.Count <= 64)
				{
					if (command.Literal.Count == 0)
					{
						throw new ArgumentException("Literal must have at least 1 byte");
					}
					s.Write((byte)command.Literal.Count);
				}
				else
				{
					int sizeNeeded = GetSizeNeeded((ulong)command.Literal.Count);
					int num = SizeToIdx(sizeNeeded);
					byte value = (byte)(65 + num);
					s.Write(value);
					StreamHelpers.WriteBigEndian(s, (ulong)command.Literal.Count, sizeNeeded);
				}
				s.Write(command.Literal.ToArray());
			}
			else if (command.Kind == CommandKind.Copy)
			{
				int sizeNeeded2 = GetSizeNeeded(command.Position);
				int sizeNeeded3 = GetSizeNeeded(command.Length);
				byte b = SizeToIdx(sizeNeeded2);
				byte b2 = SizeToIdx(sizeNeeded3);
				byte value2 = (byte)(69 + b * 4 + b2);
				s.Write(value2);
				StreamHelpers.WriteBigEndian(s, command.Position, sizeNeeded2);
				StreamHelpers.WriteBigEndian(s, command.Length, sizeNeeded3);
			}
			else if (command.Kind == CommandKind.End)
			{
				s.Write(0);
			}
		}

		private static int GetSizeNeeded(ulong value)
		{
			if (value <= 255)
			{
				return 1;
			}
			if (value <= 65535)
			{
				return 2;
			}
			if (value <= uint.MaxValue)
			{
				return 4;
			}
			return 8;
		}

		private static byte SizeToIdx(int size)
		{
			switch (size)
			{
			case 1:
				return 0;
			case 2:
				return 1;
			case 4:
				return 2;
			case 8:
				return 3;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public static IEnumerable<OutputCommand> ComputeCommands(BinaryReader inputStream, SignatureFile signatures)
		{
			OutputCommand outputCommand = default(OutputCommand);
			outputCommand.Kind = CommandKind.Reserved;
			OutputCommand outputCommand2 = outputCommand;
			long inputLength = inputStream.BaseStream.Length;
			ReadNewBlock(inputStream, signatures.BlockLength, out Queue<byte> currentBlock, out Rollsum currentSum);
			while (currentBlock.Count > 0)
			{
				List<BlockSignature> value = null;
				bool flag = false;
				if (signatures.BlockLookup.TryGetValue(currentSum.Digest, out value))
				{
					byte[] currentStrongSum = signatures.StrongSumMethod(currentBlock.ToArray());
					BlockSignature matchingBlock = FindBlockSignatureByStrongSum(value, currentStrongSum);
					if (matchingBlock != null)
					{
						if (outputCommand2.Kind == CommandKind.Copy && outputCommand2.Position + outputCommand2.Length == matchingBlock.StartPos)
						{
							outputCommand2.Length = (ulong)((long)outputCommand2.Length + (long)currentBlock.Count);
						}
						else
						{
							if (outputCommand2.Kind != CommandKind.Reserved)
							{
								yield return outputCommand2;
							}
							outputCommand = default(OutputCommand);
							outputCommand.Kind = CommandKind.Copy;
							outputCommand.Position = matchingBlock.StartPos;
							outputCommand.Length = (ulong)currentBlock.Count;
							outputCommand2 = outputCommand;
						}
						flag = true;
						ReadNewBlock(inputStream, signatures.BlockLength, out currentBlock, out currentSum);
					}
				}
				if (!flag)
				{
					byte oldestByte = currentBlock.Dequeue();
					if (inputStream.BaseStream.Position != inputLength)
					{
						byte b = inputStream.ReadByte();
						currentBlock.Enqueue(b);
						currentSum.Rotate(oldestByte, b);
					}
					else
					{
						currentSum.Rollout(oldestByte);
					}
					if (outputCommand2.Kind == CommandKind.Literal && outputCommand2.Literal.Count < 4194304)
					{
						outputCommand2.Literal.Add(oldestByte);
					}
					else
					{
						if (outputCommand2.Kind != CommandKind.Reserved)
						{
							yield return outputCommand2;
						}
						outputCommand = default(OutputCommand);
						outputCommand.Kind = CommandKind.Literal;
						outputCommand.Literal = new List<byte>
						{
							oldestByte
						};
						outputCommand2 = outputCommand;
					}
				}
			}
			yield return outputCommand2;
			outputCommand = new OutputCommand
			{
				Kind = CommandKind.End
			};
			yield return outputCommand;
		}

		private static BlockSignature FindBlockSignatureByStrongSum(List<BlockSignature> matchCandidates, byte[] currentStrongSum)
		{
			for (int i = 0; i < matchCandidates.Count; i++)
			{
				if (SignatureHelpers.UnsafeEquals(matchCandidates[i].StrongSum, currentStrongSum))
				{
					return matchCandidates[i];
				}
			}
			return null;
		}

		private static void ReadNewBlock(BinaryReader inputStream, int blockLength, out Queue<byte> newBlock, out Rollsum newRollsum)
		{
			byte[] array = inputStream.ReadBytes(blockLength);
			int num = array.Length;
			newBlock = new Queue<byte>(num);
			for (int i = 0; i < num; i++)
			{
				newBlock.Enqueue(array[i]);
			}
			newRollsum = new Rollsum();
			newRollsum.Update(array);
		}
	}
}
