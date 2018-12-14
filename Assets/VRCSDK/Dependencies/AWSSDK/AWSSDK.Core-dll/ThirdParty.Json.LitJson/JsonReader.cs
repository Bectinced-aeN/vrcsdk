using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ThirdParty.Json.LitJson
{
	public class JsonReader
	{
		private Stack<JsonToken> depth = new Stack<JsonToken>();

		private int current_input;

		private int current_symbol;

		private bool end_of_json;

		private bool end_of_input;

		private Lexer lexer;

		private bool parser_in_string;

		private bool parser_return;

		private bool read_started;

		private TextReader reader;

		private bool reader_is_owned;

		private object token_value;

		private JsonToken token;

		public bool AllowComments
		{
			get
			{
				return lexer.AllowComments;
			}
			set
			{
				lexer.AllowComments = value;
			}
		}

		public bool AllowSingleQuotedStrings
		{
			get
			{
				return lexer.AllowSingleQuotedStrings;
			}
			set
			{
				lexer.AllowSingleQuotedStrings = value;
			}
		}

		public bool EndOfInput => end_of_input;

		public bool EndOfJson => end_of_json;

		public JsonToken Token => token;

		public object Value => token_value;

		public JsonReader(string json_text)
			: this(new StringReader(json_text), owned: true)
		{
		}

		public JsonReader(TextReader reader)
			: this(reader, owned: false)
		{
		}

		private JsonReader(TextReader reader, bool owned)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			parser_in_string = false;
			parser_return = false;
			read_started = false;
			lexer = new Lexer(reader);
			end_of_input = false;
			end_of_json = false;
			this.reader = reader;
			reader_is_owned = owned;
		}

		private void ProcessNumber(string number)
		{
			int result2;
			uint result3;
			long result4;
			ulong result5;
			if ((number.IndexOf('.') != -1 || number.IndexOf('e') != -1 || number.IndexOf('E') != -1) && double.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
			{
				token = JsonToken.Double;
				token_value = result;
			}
			else if (int.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out result2))
			{
				token = JsonToken.Int;
				token_value = result2;
			}
			else if (uint.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out result3))
			{
				token = JsonToken.UInt;
				token_value = result3;
			}
			else if (long.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out result4))
			{
				token = JsonToken.Long;
				token_value = result4;
			}
			else if (ulong.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out result5))
			{
				token = JsonToken.ULong;
				token_value = result5;
			}
			else
			{
				token = JsonToken.ULong;
				token_value = 0uL;
			}
		}

		private void ProcessSymbol()
		{
			if (current_symbol == 91)
			{
				token = JsonToken.ArrayStart;
				parser_return = true;
			}
			else if (current_symbol == 93)
			{
				token = JsonToken.ArrayEnd;
				parser_return = true;
			}
			else if (current_symbol == 123)
			{
				token = JsonToken.ObjectStart;
				parser_return = true;
			}
			else if (current_symbol == 125)
			{
				token = JsonToken.ObjectEnd;
				parser_return = true;
			}
			else if (current_symbol == 34)
			{
				if (parser_in_string)
				{
					parser_in_string = false;
					parser_return = true;
				}
				else
				{
					if (token == JsonToken.None)
					{
						token = JsonToken.String;
					}
					parser_in_string = true;
				}
			}
			else if (current_symbol == 65541)
			{
				token_value = lexer.StringValue;
			}
			else if (current_symbol == 65539)
			{
				token = JsonToken.Boolean;
				token_value = false;
				parser_return = true;
			}
			else if (current_symbol == 65540)
			{
				token = JsonToken.Null;
				parser_return = true;
			}
			else if (current_symbol == 65537)
			{
				ProcessNumber(lexer.StringValue);
				parser_return = true;
			}
			else if (current_symbol == 65546)
			{
				token = JsonToken.PropertyName;
			}
			else if (current_symbol == 65538)
			{
				token = JsonToken.Boolean;
				token_value = true;
				parser_return = true;
			}
		}

		private bool ReadToken()
		{
			if (end_of_input)
			{
				return false;
			}
			lexer.NextToken();
			if (lexer.EndOfInput)
			{
				Close();
				return false;
			}
			current_input = lexer.Token;
			return true;
		}

		public void Close()
		{
			if (!end_of_input)
			{
				end_of_input = true;
				end_of_json = true;
				reader = null;
			}
		}

		public bool Read()
		{
			if (end_of_input)
			{
				return false;
			}
			if (end_of_json)
			{
				end_of_json = false;
			}
			token = JsonToken.None;
			parser_in_string = false;
			parser_return = false;
			if (!read_started)
			{
				read_started = true;
				if (!ReadToken())
				{
					return false;
				}
			}
			do
			{
				current_symbol = current_input;
				ProcessSymbol();
				if (parser_return)
				{
					if (token == JsonToken.ObjectStart || token == JsonToken.ArrayStart)
					{
						depth.Push(token);
					}
					else if (token == JsonToken.ObjectEnd || token == JsonToken.ArrayEnd)
					{
						if (depth.Peek() == JsonToken.PropertyName)
						{
							depth.Pop();
						}
						JsonToken jsonToken = depth.Pop();
						if (token == JsonToken.ObjectEnd && jsonToken != JsonToken.ObjectStart)
						{
							throw new JsonException("Error: Current token is ObjectEnd which does not match the opening " + jsonToken.ToString());
						}
						if (token == JsonToken.ArrayEnd && jsonToken != JsonToken.ArrayStart)
						{
							throw new JsonException("Error: Current token is ArrayEnd which does not match the opening " + jsonToken.ToString());
						}
						if (depth.Count == 0)
						{
							end_of_json = true;
						}
					}
					else if (depth.Count > 0 && depth.Peek() != JsonToken.PropertyName && token == JsonToken.String && depth.Peek() == JsonToken.ObjectStart)
					{
						token = JsonToken.PropertyName;
						depth.Push(token);
					}
					if (token == JsonToken.ObjectEnd || token == JsonToken.ArrayEnd || token == JsonToken.String || token == JsonToken.Boolean || token == JsonToken.Double || token == JsonToken.Int || token == JsonToken.UInt || token == JsonToken.Long || token == JsonToken.ULong || token == JsonToken.Null || token == JsonToken.String)
					{
						if (depth.Count == 0)
						{
							if (token != JsonToken.ArrayEnd && token != JsonToken.ObjectEnd)
							{
								throw new JsonException("Value without enclosing object or array");
							}
						}
						else if (depth.Peek() == JsonToken.PropertyName)
						{
							depth.Pop();
						}
					}
					if (!ReadToken() && depth.Count != 0)
					{
						throw new JsonException("Incomplete JSON Document");
					}
					return true;
				}
			}
			while (ReadToken());
			if (depth.Count != 0)
			{
				throw new JsonException("Incomplete JSON Document");
			}
			end_of_input = true;
			return false;
		}
	}
}
