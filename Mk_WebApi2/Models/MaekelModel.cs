using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mk_WebApi2.Models
{
    public class MaekelModel
    {
        [Key]
        public int maekel_id { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string name { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? location { get; set; }
        [Column(TypeName = "text")]
        public string? description { get; set; }
        public int status { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int modified_by { get; set; }
        public DateTime modified_on { get; set; }
        public int trash { get; set; }
    }
}
