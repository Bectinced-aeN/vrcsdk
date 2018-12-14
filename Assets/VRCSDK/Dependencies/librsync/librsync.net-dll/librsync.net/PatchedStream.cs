using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace librsync.net
{
	internal class PatchedStream : Stream
	{
		private class StreamCopyHelper
		{
			private long bytesLeft;

			private Stream source;

			public bool MoreData => bytesLeft > 0;

			public StreamCopyHelper(long bytes, Stream source)
			{
				bytesLeft = bytes;
				this.source = source;
			}

			public int Read(byte[] buffer, int offset, int count)
			{
				int count2 = (int)Math.Min(count, bytesLeft);
				int num = source.Read(buffer, offset, count2);
				bytesLeft -= num;
				return num;
			}

			public void SeekForward(long bytes)
			{
				source.Seek(bytes, SeekOrigin.Current);
				bytesLeft -= bytes;
			}
		}

		public struct CommandFormat
		{
			public CommandKind Kind;

			public int ImmediateValue;

			public int Length1;

			public int Length2;

			public CommandFormat(CommandKind kind, int immediate, int length1 = 0, int length2 = 0)
			{
				Kind = kind;
				ImmediateValue = immediate;
				Length1 = length1;
				Length2 = length2;
			}
		}

		private long outputPosition;

		private List<CommandPosition> commandSummary;

		private Stream input;

		private Stream delta;

		private BinaryReader deltaReader;

		private StreamCopyHelper currentCopyHelper;

		private static CommandFormat[] CommandFormatTable = new CommandFormat[20]
		{
			new CommandFormat(CommandKind.Literal, 0, 1),
			new CommandFormat(CommandKind.Literal, 0, 2),
			new CommandFormat(CommandKind.Literal, 0, 4),
			new CommandFormat(CommandKind.Literal, 0, 8),
			new CommandFormat(CommandKind.Copy, 0, 1, 1),
			new CommandFormat(CommandKind.Copy, 0, 1, 2),
			new CommandFormat(CommandKind.Copy, 0, 1, 4),
			new CommandFormat(CommandKind.Copy, 0, 1, 8),
			new CommandFormat(CommandKind.Copy, 0, 2, 1),
			new CommandFormat(CommandKind.Copy, 0, 2, 2),
			new CommandFormat(CommandKind.Copy, 0, 2, 4),
			new CommandFormat(CommandKind.Copy, 0, 2, 8),
			new CommandFormat(CommandKind.Copy, 0, 4, 1),
			new CommandFormat(CommandKind.Copy, 0, 4, 2),
			new CommandFormat(CommandKind.Copy, 0, 4, 4),
			new CommandFormat(CommandKind.Copy, 0, 4, 8),
			new CommandFormat(CommandKind.Copy, 0, 8, 1),
			new CommandFormat(CommandKind.Copy, 0, 8, 2),
			new CommandFormat(CommandKind.Copy, 0, 8, 4),
			new CommandFormat(CommandKind.Copy, 0, 8, 8)
		};

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Length => commandSummary.Sum((CommandPosition c) => c.Command.Length);

		public override long Position
		{
			get
			{
				return outputPosition;
			}
			set
			{
				Seek(value, SeekOrigin.Begin);
			}
		}

		public PatchedStream(Stream input, Stream delta)
		{
			this.input = input;
			this.delta = delta;
			deltaReader = new BinaryReader(this.delta);
			ReadHeader(deltaReader);
			long position = this.delta.Position;
			commandSummary = ReadCommandSummary(this.delta);
			this.delta.Seek(position, SeekOrigin.Begin);
			currentCopyHelper = new StreamCopyHelper(0L, input);
		}

		private int ReadInternalSync(byte[] buffer, int offset, int count)
		{
			if (!currentCopyHelper.MoreData)
			{
				Command command = ReadCommand(deltaReader);
				currentCopyHelper = ConstructCopyHelperForCommand(command, input, delta);
				if (currentCopyHelper == null)
				{
					return 0;
				}
			}
			int num = currentCopyHelper.Read(buffer, offset, count);
			outputPosition += num;
			return num;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return ReadInternalSync(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			long num = StreamHelpers.ComputeNewPosition(offset, origin, Length, Position);
			long num2 = 0L;
			int i;
			for (i = 0; i < commandSummary.Count; i++)
			{
				long num3 = num2;
				CommandPosition commandPosition = commandSummary[i];
				long num4 = num3 + commandPosition.Command.Length;
				if (num4 > num)
				{
					break;
				}
				num2 = num4;
			}
			if (i == commandSummary.Count)
			{
				throw new ArgumentException("The specified offset is past the end of the stream");
			}
			delta.Seek(commandSummary[i].DeltaStartPosition, SeekOrigin.Begin);
			Command command = ReadCommand(deltaReader);
			currentCopyHelper = ConstructCopyHelperForCommand(command, input, delta);
			currentCopyHelper.SeekForward(num - num2);
			outputPosition = num;
			return num;
		}

		public override void Flush()
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		private static void ReadHeader(BinaryReader s)
		{
			uint num = StreamHelpers.ReadBigEndianUint32(s);
			if (num != 1920139830)
			{
				throw new InvalidDataException($"Got magic number {num:x} instead of expected {1920139830u:x}");
			}
		}

		private static Command ReadCommand(BinaryReader s)
		{
			CommandFormat commandFormat = GetCommandFormat(s.ReadByte());
			Command result = default(Command);
			result.Kind = commandFormat.Kind;
			if (commandFormat.Length1 > 0)
			{
				result.Parameter1 = StreamHelpers.ConvertFromBigEndian(s.ReadBytes(commandFormat.Length1));
			}
			else
			{
				result.Parameter1 = commandFormat.ImmediateValue;
			}
			if (commandFormat.Length2 > 0)
			{
				result.Parameter2 = StreamHelpers.ConvertFromBigEndian(s.ReadBytes(commandFormat.Length2));
			}
			return result;
		}

		private static CommandFormat GetCommandFormat(byte commandCode)
		{
			if (commandCode == 0)
			{
				return new CommandFormat(CommandKind.End, 0);
			}
			if (commandCode <= 64)
			{
				return new CommandFormat(CommandKind.Literal, commandCode);
			}
			if (commandCode < 85)
			{
				return CommandFormatTable[commandCode - 65];
			}
			return new CommandFormat(CommandKind.Reserved, commandCode);
		}

		private static StreamCopyHelper ConstructCopyHelperForCommand(Command command, Stream input, Stream delta)
		{
			switch (command.Kind)
			{
			case CommandKind.Literal:
				return new StreamCopyHelper(command.Parameter1, delta);
			case CommandKind.Copy:
				input.Seek(command.Parameter1, SeekOrigin.Begin);
				return new StreamCopyHelper(command.Parameter2, input);
			case CommandKind.End:
				return null;
			default:
				throw new InvalidDataException($"Unknown command {command.Parameter1}");
			}
		}

		private static List<CommandPosition> ReadCommandSummary(Stream inputStream)
		{
			List<CommandPosition> list = new List<CommandPosition>();
			BinaryReader s = new BinaryReader(inputStream);
			bool flag = false;
			while (!flag)
			{
				long position = inputStream.Position;
				Command command = ReadCommand(s);
				list.Add(new CommandPosition
				{
					Command = command,
					DeltaStartPosition = position
				});
				if (command.Kind == CommandKind.Literal)
				{
					inputStream.Seek(command.Length, SeekOrigin.Current);
				}
				else if (command.Kind == CommandKind.End)
				{
					flag = true;
				}
			}
			return list;
		}
	}
}
