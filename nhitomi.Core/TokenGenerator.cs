// Copyright (c) 2018-2019 fate/loli
//
// This software is released under the MIT License.
// https://opensource.org/licenses/MIT

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace nhitomi.Core
{
    public static class TokenGenerator
    {
        public static string CreateToken<T>(T payload, string secret, JsonSerializer serializer = null)
        {
            serializer = serializer ?? JsonSerializer.CreateDefault();

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, payload);
                var json = writer.ToString();

                var data = Encoding.UTF8.GetBytes(json);
                var signature = ComputeSignature(data, secret);

                return Convert.ToBase64String(data) + "." + Convert.ToBase64String(signature);
            }
        }

        public static bool TryDeserializeToken<T>(string token, string secret, out T payload, JsonSerializer serializer = null)
        {
            payload = default;
            serializer = serializer ?? JsonSerializer.CreateDefault();

            try
            {
                var parts = token.Split('.');
                if (parts.Length != 2)
                    return false;

                var data = Convert.FromBase64String(parts[0]);
                var signature = Convert.FromBase64String(parts[1]);

                var expectedSignature = ComputeSignature(data, secret);

                if (!ConstantTimeEquals(signature, expectedSignature))
                    return false;

                var json = Encoding.UTF8.GetString(data);
                using (var reader = new StringReader(json))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    payload = serializer.Deserialize<T>(jsonReader);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static byte[] ComputeSignature(byte[] data, string secret)
        {
            var key = Encoding.UTF8.GetBytes(secret);
            using (var hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(data);
            }
        }

        private static bool ConstantTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            var result = 0;
            for (int i = 0; i < a.Length; i++)
                result |= a[i] ^ b[i];

            return result == 0;
        }

        public class ProxyGetPayload
        {
            public string Url { get; set; }
            public bool IsCached { get; set; }
        }

        public class ProxyDownloadPayload
        {
            public DateTime Expires { get; set; }
            public double RequestThrottle { get; set; }
        }

        public class ProxySetCachePayload
        {
            public string Url { get; set; }
        }

        public class ProxyRegistrationPayload
        {
            public string ProxyUrl { get; set; }
        }
    }
}
