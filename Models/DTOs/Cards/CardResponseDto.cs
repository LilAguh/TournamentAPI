using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Cards
{
    public class CardResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Attack { get; set; }
        public int Defense { get; set; }
        public string IllustrationUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
    }
}
