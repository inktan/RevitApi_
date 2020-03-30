using System.Threading;

namespace Dwelling_Assembly
{
    public enum RequestId : int
    {
        None = 0,
        Creat_1T2H_aa_ = 1,
        Creat_1T2H_aaaa_ = 2,
        Creat_1T2H_aaaaaa_ = 3,
        Creat_1T2H_ab_ = 4,
        Creat_1T2H_abba_ = 5,
        Creat_1T2H_aaab_ = 6,
        Creat_1T2H_aabbaa_ = 7,
        Creat_1T2H_abbbba_ = 8,
        Creat_1T2H_aaaaab_ = 9,
        Creat_1T2H_abbc_ = 10,
        Creat_1T2H_aabbac_ = 11,
        Creat_1T2H_abbbbc_ = 12,
        Creat_1T2H_abccba_ = 13,
        Creat_1T2H_abccbd_ = 14,
        Creat_1T2H_abaaba_ = 15,
        Creat_1T2H_abaabc_ = 16,
        Creat_2T4H_abba_ = 17,
        Creat_2T4H_abbaabba_ = 18,
        Creat_2T4H_abbc_ = 19,
        Creat_2T4H_abbccbba_ = 20,
        Creat_2T4H_abbaabbc_ = 21,
        Creat_2T4H_abbccbbd_ = 22,
    }

    public class Request
    {
        private int m_request = (int)RequestId.None;
        public RequestId Take()
        {
            return (RequestId)Interlocked.Exchange(ref m_request, (int)RequestId.None);
        }
        public void Make(RequestId request)
        {
            Interlocked.Exchange(ref m_request, (int)request);
        }
    }
}
