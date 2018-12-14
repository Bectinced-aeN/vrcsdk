using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ThirdParty.Json.LitJson;

namespace Amazon.Runtime.Internal.Util
{
	public class RequestMetrics : IRequestMetrics
	{
		private object metricsLock = new object();

		private Stopwatch stopWatch;

		private Dictionary<Metric, Timing> inFlightTimings;

		private List<MetricError> errors = new List<MetricError>();

		private long CurrentTime => stopWatch.GetElapsedDateTimeTicks();

		public Dictionary<Metric, List<object>> Properties
		{
			get;
			set;
		}

		public Dictionary<Metric, List<IMetricsTiming>> Timings
		{
			get;
			set;
		}

		public Dictionary<Metric, long> Counters
		{
			get;
			set;
		}

		public bool IsEnabled
		{
			get;
			internal set;
		}

		private void LogError_Locked(Metric metric, string messageFormat, params object[] args)
		{
			errors.Add(new MetricError(metric, messageFormat, args));
		}

		private static void Log(StringBuilder builder, Metric metric, object metricValue)
		{
			LogHelper(builder, metric, metricValue);
		}

		private static void Log(StringBuilder builder, Metric metric, List<object> metricValues)
		{
			if (metricValues == null || metricValues.Count == 0)
			{
				LogHelper(builder, metric);
			}
			else
			{
				LogHelper(builder, metric, metricValues.ToArray());
			}
		}

		private static void LogHelper(StringBuilder builder, Metric metric, params object[] metricValues)
		{
			builder.AppendFormat(CultureInfo.InvariantCulture, "{0} = ", metric);
			if (metricValues == null)
			{
				builder.Append(ObjectToString(metricValues));
			}
			else
			{
				for (int i = 0; i < metricValues.Length; i++)
				{
					string value = ObjectToString(metricValues[i]);
					if (i > 0)
					{
						builder.Append(", ");
					}
					builder.Append(value);
				}
			}
			builder.Append("; ");
		}

		private static string ObjectToString(object data)
		{
			if (data == null)
			{
				return "NULL";
			}
			return data.ToString();
		}

		public RequestMetrics()
		{
			stopWatch = Stopwatch.StartNew();
			Properties = new Dictionary<Metric, List<object>>();
			Timings = new Dictionary<Metric, List<IMetricsTiming>>();
			Counters = new Dictionary<Metric, long>();
			inFlightTimings = new Dictionary<Metric, Timing>();
			IsEnabled = false;
		}

		public TimingEvent StartEvent(Metric metric)
		{
			lock (metricsLock)
			{
				if (inFlightTimings.ContainsKey(metric))
				{
					LogError_Locked(metric, "Starting multiple events for the same metric");
				}
				inFlightTimings[metric] = new Timing(CurrentTime);
			}
			return new TimingEvent(this, metric);
		}

		public Timing StopEvent(Metric metric)
		{
			lock (metricsLock)
			{
				if (!inFlightTimings.TryGetValue(metric, out Timing value))
				{
					LogError_Locked(metric, "Trying to stop event that has not been started");
					return new Timing();
				}
				inFlightTimings.Remove(metric);
				value.Stop(CurrentTime);
				if (!IsEnabled)
				{
					return value;
				}
				if (!Timings.TryGetValue(metric, out List<IMetricsTiming> value2))
				{
					value2 = new List<IMetricsTiming>();
					Timings[metric] = value2;
				}
				value2.Add(value);
				return value;
			}
		}

		public void AddProperty(Metric metric, object property)
		{
			if (IsEnabled)
			{
				lock (metricsLock)
				{
					if (!Properties.TryGetValue(metric, out List<object> value))
					{
						value = new List<object>();
						Properties[metric] = value;
					}
					value.Add(property);
				}
			}
		}

		public void SetCounter(Metric metric, long value)
		{
			if (IsEnabled)
			{
				lock (metricsLock)
				{
					Counters[metric] = value;
				}
			}
		}

		public void IncrementCounter(Metric metric)
		{
			if (IsEnabled)
			{
				lock (metricsLock)
				{
					long value = Counters.TryGetValue(metric, out value) ? (value + 1) : 1;
					Counters[metric] = value;
				}
			}
		}

		public string GetErrors()
		{
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				lock (metricsLock)
				{
					if (inFlightTimings.Count > 0)
					{
						string arg = string.Join(", ", (from k in inFlightTimings.Keys
						select k.ToString()).ToArray());
						stringWriter.Write("Timings are still in flight: {0}.", arg);
					}
					if (errors.Count > 0)
					{
						stringWriter.Write("Logged {0} metrics errors: ", errors.Count);
						foreach (MetricError error in errors)
						{
							if (error.Exception != null || !string.IsNullOrEmpty(error.Message))
							{
								stringWriter.Write("{0} - {1} - ", error.Time.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", CultureInfo.InvariantCulture), error.Metric);
								if (!string.IsNullOrEmpty(error.Message))
								{
									stringWriter.Write(error.Message);
									stringWriter.Write(";");
								}
								if (error.Exception != null)
								{
									stringWriter.Write(error.Exception);
									stringWriter.Write("; ");
								}
							}
						}
					}
				}
				return stringWriter.ToString();
			}
		}

		public override string ToString()
		{
			if (!IsEnabled)
			{
				return "Metrics logging disabled";
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (AWSConfigs.LoggingConfig.LogMetricsCustomFormatter != null)
			{
				try
				{
					lock (metricsLock)
					{
						stringBuilder.Append(AWSConfigs.LoggingConfig.LogMetricsCustomFormatter.FormatMetrics(this));
					}
					return stringBuilder.ToString();
				}
				catch (Exception ex)
				{
					stringBuilder.Append("[Custom metrics formatter failed: ").Append(ex.Message).Append("] ");
				}
			}
			if (AWSConfigs.LoggingConfig.LogMetricsFormat == LogMetricsFormatOption.JSON)
			{
				lock (metricsLock)
				{
					stringBuilder.Append(ToJSON());
				}
				return stringBuilder.ToString();
			}
			lock (metricsLock)
			{
				foreach (KeyValuePair<Metric, List<object>> property in Properties)
				{
					Metric key = property.Key;
					List<object> value = property.Value;
					Log(stringBuilder, key, value);
				}
				foreach (KeyValuePair<Metric, List<IMetricsTiming>> timing in Timings)
				{
					Metric key2 = timing.Key;
					foreach (IMetricsTiming item in timing.Value)
					{
						if (item.IsFinished)
						{
							Log(stringBuilder, key2, item.ElapsedTime);
						}
					}
				}
				foreach (KeyValuePair<Metric, long> counter in Counters)
				{
					Metric key3 = counter.Key;
					long value2 = counter.Value;
					Log(stringBuilder, key3, value2);
				}
			}
			stringBuilder.Replace("\r", "\\r").Replace("\n", "\\n");
			return stringBuilder.ToString();
		}

		public string ToJSON()
		{
			if (!IsEnabled)
			{
				return "{ }";
			}
			StringBuilder stringBuilder = new StringBuilder();
			JsonWriter jsonWriter = new JsonWriter(stringBuilder);
			jsonWriter.WriteObjectStart();
			jsonWriter.WritePropertyName("properties");
			jsonWriter.WriteObjectStart();
			foreach (KeyValuePair<Metric, List<object>> property in Properties)
			{
				jsonWriter.WritePropertyName(property.Key.ToString());
				List<object> value = property.Value;
				if (value.Count > 1)
				{
					jsonWriter.WriteArrayStart();
				}
				foreach (object item in value)
				{
					if (item == null)
					{
						jsonWriter.Write(null);
					}
					else
					{
						jsonWriter.Write(item.ToString());
					}
				}
				if (value.Count > 1)
				{
					jsonWriter.WriteArrayEnd();
				}
			}
			jsonWriter.WriteObjectEnd();
			jsonWriter.WritePropertyName("timings");
			jsonWriter.WriteObjectStart();
			foreach (KeyValuePair<Metric, List<IMetricsTiming>> timing in Timings)
			{
				jsonWriter.WritePropertyName(timing.Key.ToString());
				List<IMetricsTiming> value2 = timing.Value;
				if (value2.Count > 1)
				{
					jsonWriter.WriteArrayStart();
				}
				foreach (IMetricsTiming item2 in timing.Value)
				{
					if (item2.IsFinished)
					{
						jsonWriter.Write(item2.ElapsedTime.TotalMilliseconds);
					}
				}
				if (value2.Count > 1)
				{
					jsonWriter.WriteArrayEnd();
				}
			}
			jsonWriter.WriteObjectEnd();
			jsonWriter.WritePropertyName("counters");
			jsonWriter.WriteObjectStart();
			foreach (KeyValuePair<Metric, long> counter in Counters)
			{
				jsonWriter.WritePropertyName(counter.Key.ToString());
				jsonWriter.Write(counter.Value);
			}
			jsonWriter.WriteObjectEnd();
			jsonWriter.WriteObjectEnd();
			return stringBuilder.ToString();
		}
	}
}
