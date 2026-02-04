// Copyright (c) 2018-2019 fate/loli
//
// This software is released under the MIT License.
// https://opensource.org/licenses/MIT

using Newtonsoft.Json;

namespace nhitomi.Core
{
    public class ProxyInfo
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
