using System;
using System.Collections.Generic;

namespace Ollio.Common.Models
{
    public class History
    {
        public DateTime DateCreated { get; set; }
        public DateTime? DateEdited { get; set; }
        public DateTime DateExpires { get; set; }
        public int MessageId { get; set; }
        public List<HistoryMessage> Messages { get; set; }
    }
}