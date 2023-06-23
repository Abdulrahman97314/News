using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.Core.Entities
{
    public class Author : BaseEntity
    {
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }

        public virtual ICollection<News> News { get; set; }
    }
}
