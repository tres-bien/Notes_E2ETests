using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes_WebApp.Shared
{
    public class Note
    {
        [Key]
        public int Id { get; set; }
        public string Title {  get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsEditing { get; set; }
    }
}
