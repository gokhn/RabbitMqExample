using System;

namespace Models
{
    public class Order
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public DateTime OrderDate { get; set; }

        public string CreateUserName { get; set; }
    }
}
