using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachodromo.Shared.Entities
{
    public class ItemCategory
    {
        public int Id { get; set; }

        public Item Item { get; set; } = null!;

        public int ItemId { get; set; }

        public Category Category { get; set; } = null!;

        public int CategoryId { get; set; }

    }
}
