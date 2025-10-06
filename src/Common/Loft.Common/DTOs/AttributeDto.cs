using Loft.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loft.Common.DTOs
{
    public class AttributeDto
    {
        public int Id { get; set; }     // Уникальный идентификатор
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;   // Название атрибута
        public AttributeType Type { get; set; }     // Тип атрибута (текст, число, список)
        [MaxLength(50)]
        public string? TypeName { get; set; } // Название типа (если нужно кастомное)
        public string? ListOptions { get; set; } // JSON или строка с вариантами списка (например, "Red,Green,Blue")
        public ModerationStatus Status { get; set; } = ModerationStatus.Pending;
    }
}
