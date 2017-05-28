using System;

namespace TodoPublisher
{
    public class TodoItem
    {
        public TodoItem()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; private set; }
        public int StudentId { get; set; }
        public string TodoType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
