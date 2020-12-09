using System;
using System.Text;


namespace Proggmatic.SpaServices.VueCli.Util
{
    /// <summary>
    /// Captures the completed-line notifications from a <see cref="EventedStreamReader"/>, combining the data into a single <see cref="string"/>.
    /// Original: https://github.com/dotnet/aspnetcore/blob/master/src/Middleware/SpaServices.Extensions/src/Util/EventedStreamReader.cs
    /// </summary>
    internal class EventedStreamStringReader : IDisposable
    {
        private readonly EventedStreamReader _eventedStreamReader;
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private bool _isDisposed;


        public EventedStreamStringReader(EventedStreamReader eventedStreamReader)
        {
            _eventedStreamReader = eventedStreamReader
                                   ?? throw new ArgumentNullException(nameof(eventedStreamReader));
            _eventedStreamReader.OnReceivedLine += OnReceivedLine;
        }

        public string ReadAsString() => _stringBuilder.ToString();

        private void OnReceivedLine(string line) => _stringBuilder.AppendLine(line);

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _eventedStreamReader.OnReceivedLine -= OnReceivedLine;
                _isDisposed = true;
            }
        }
    }
}