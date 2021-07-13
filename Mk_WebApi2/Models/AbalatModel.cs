using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mk_WebApi2.Models
{
    public class AbalatModel
    {
        [Key]
        public int abalat_id { get; set; }

        public int? maekel_id { get; set; }
        
        public int? kifil_id { get; set; }
        
        public int? sub_kifil_id { get; set; }
        
        [Column(TypeName = "nvarchar(50)")]
        public string name { get; set; }
        
        [Column(TypeName = "nchar(5)")]
        public string? sex { get; set; }
        
        public int age { get; set; }
        
        [Column(TypeName = "nchar(20)")]
        public string phone { get; set; }
        
        [Column(TypeName = "nchar(50)")]
        public string? email { get; set; }
        
        [Column(TypeName = "varbinary(MAX)")]
        public string? photo { get; set; }
        
        [Column(TypeName = "nchar(10)")]
        public string? yegebubet_amet { get; set; }
        
        [Column(TypeName = "nchar(10)")]
        public string? marital_status { get; set; }
        
        [Column(TypeName = "nvarchar(200)")]
        public string? children_status { get; set; }
        
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
