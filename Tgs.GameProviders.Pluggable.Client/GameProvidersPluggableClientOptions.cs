using System;
using System.Text.Json;

namespace Tgs.GameProviders.Pluggable.Client
{
    public class GameProvidersPluggableClientOptions
    {
        public Uri BaseUrl { get; set; } = default!;

        public JsonSerializerOptions? JsonSerializerOptions { get; set; }
    }
}
