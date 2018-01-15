using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kpfu_schedule
{
    public class Entity
    {
        public int Day { get; set; }
        public string Time { get; set; }
        public string Subject { get; set; }
        public string Auditorium { get; set; }
    }

    public class User
    {
        public long ChatId { get; set; }
        public string Group { get; set; }
    }
}
