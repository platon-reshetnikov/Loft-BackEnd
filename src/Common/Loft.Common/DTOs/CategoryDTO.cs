using Loft.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Loft.Common.DTOs;

public class CategoryDto
{
    public int Id { get; set; } // ���������� �������������
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;   // �������� ���������
    public int? ParentId { get; set; } // ��� ������������
    public string ImgUrl { get; set; } = null!;    // URL ����������� ���������
    public ModerationStatus Status { get; set; } = ModerationStatus.Pending;
    public int ViewCount { get; set; }  = 0; // ������� ����������
    public List<int> AttributeIds { get; set; } = new List<int>(); // ������ ��������������� ��������� ��������� (�� 10)
}