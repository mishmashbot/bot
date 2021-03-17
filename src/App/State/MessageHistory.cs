using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ollio.Common.Models;

namespace Ollio.State
{
    public class MessageHistory
    {
        public static List<History> Items { get; set; }
        static bool IsFlushing { get; set; }
        //const int MaxMessageAge = 2879; // NOTE: 45h59m
        const int MaxMessageAge = 1;

        public static void Add(int messageId, List<HistoryMessage> messages)
        {
            if (Items == null)
                Items = new List<History>();

            History item = new History
            {
                DateCreated = DateTime.Now,
                DateExpires = DateTime.Now.AddMinutes(MaxMessageAge),
                MessageId = messageId,
                Messages = messages
            };

            Items.Add(item);
        }

        public static void Flush()
        {
            if(!IsFlushing)
            {
                IsFlushing = true;

                IEnumerable<History> toRemove = Items.Where(i => DateTime.Now > i.DateExpires);
                foreach(var item in toRemove)
                    Items.Remove(item);

                IsFlushing = false;
            }
        }

        public static History Get(int messageId)
        {
            if(Items != null)
                return Items.Where(i => i.MessageId == messageId).FirstOrDefault();
            else
                return null;
        }

        public static History Update(int messageInId)
        {
            History item = Get(messageInId);

            if (item != null && !IsFlushing)
            {
                if (DateTime.Now > item.DateExpires)
                {
                    Items.Remove(item);
                    item = null;
                }
                else
                {
                    item.DateEdited = DateTime.Now;
                }
            }

            return item;
        }
    }
}