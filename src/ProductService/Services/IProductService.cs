using Loft.Common.DTOs;
using Loft.Common.Enums;

namespace ProductService.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDTO>> GetAllProducts(int page, int pageSize); // ��������� ���� ���������
    Task<int> GetApprovedProductsCount(); // ��������� ���������� ���������� ���������
    Task<ProductDTO?> GetProductById(long productId); // ��������� �������� �� Id
    Task<ProductDTO> CreateProduct(ProductDTO product); // �������� ������ ��������
    Task<ProductDTO?> UpdateProduct(long productId, ProductDTO product); // ���������� ��������
    Task DeleteProduct(long productId); // �������� ��������
    Task<IEnumerable<ProductDTO>> SearchProducts(string? searchText, decimal? minPrice, decimal? maxPrice); // ����� ��������� �� ���������
    Task UpdateStock(long productId, int newQuantity);  // ���������� ���������� ������ �� ������
    Task AddComent(long productId, string newComent);    // ���������� ����������� � ��������
}