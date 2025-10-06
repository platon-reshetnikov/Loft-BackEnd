using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loft.Common.DTOs
{
    public class ProductAttributeDto
    {
        public int Id { get; set; } // Уникальный идентификатор
        public int AttributeId { get; set; }    
        [MaxLength(500)]
        public string Value { get; set; } = null!;  // Значение (строка, число или из списка; парсить по типу атрибута)
        public int ProductId { get; set; }  // Внешний ключ на таблицу Товаров
    }
}
