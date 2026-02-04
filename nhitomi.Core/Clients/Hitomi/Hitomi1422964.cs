using System;
using nhitomi.Core;

namespace nhitomi.Core.Clients.Hitomi
{
    // Ignored: External data changes frequently, breaking integration tests
    [Ignored]
    public class Hitomi1422964 : ClientTestCase
    {
        public override string DoujinId => "1422964";
        public override Type ClientType => typeof(HitomiClient);

        public override DoujinInfo KnownValue { get; } = new DoujinInfo
        {
            PrettyName = "Nama Emo - Muboubi na JC Pri Chan Idol no Oshiego no Tame ni Otona Chinpo de Torotoro " +
                         "Asedaku Wakarase Koubi Shidou!",
            OriginalName = "생에모 무방비한 J○ 프리챤 아이돌 제자에게 어른 자지로 찐득찐득 땀 범벅이 되는 교미 지도!",
            UploadTime   = new DateTime(2019, 5, 30, 8, 52, 0, DateTimeKind.Utc),
            SourceId     = "1422964",
            Artist       = "tokomaya keita",
            Group        = "circle tokomaya",
            Language     = "korean",
            Parody       = "kiratto pri chan",
            Characters = new[]
            {
                "emo moegi"
            },
            Tags = new[]
            {
                "ahegao",
                "loli",
                "prism jump 23",
                "sole female",
                "sole male",
                "twintails",
                "unusual pupils"
            },
            PageCount = 22
        };
    }
}