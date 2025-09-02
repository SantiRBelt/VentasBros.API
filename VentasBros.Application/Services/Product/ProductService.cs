using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentasBros.Application.DTOs.Common;
using VentasBros.Application.DTOs.Product;
using VentasBros.Domain.Entities;
using VentasBros.Domain.Interfaces;

namespace VentasBros.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdWithImagesAsync(id);
            return product != null ? _mapper.Map<ProductDto>(product) : null;
        }

        public async Task<PagedResult<ProductDto>> GetPagedAsync(ProductFilterDto filter)
        {
            var (items, total) = await _productRepository.GetPagedWithFiltersAsync(
                filter.Page,
                filter.PageSize,
                filter.Search,
                filter.CategoryId,
                filter.OnlyActive,
                filter.MinPrice,
                filter.MaxPrice,
                filter.Sort,
                filter.Direction);

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(items);

            return new PagedResult<ProductDto>
            {
                Items = productDtos,
                Page = filter.Page,
                PageSize = filter.PageSize,
                Total = total
            };
        }

        public async Task<PagedResult<ProductDto>> GetCatalogPagedAsync(CatalogFilterDto filter)
        {
            // Para el catálogo público, siempre filtrar solo productos activos
            var (items, total) = await _productRepository.GetPagedWithFiltersAsync(
                filter.Page,
                filter.PageSize,
                filter.Search,
                filter.CategoryId,
                onlyActive: true, // Siempre true para catálogo público
                filter.MinPrice,
                filter.MaxPrice,
                filter.Sort,
                filter.Direction);

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(items);

            return new PagedResult<ProductDto>
            {
                Items = productDtos,
                Page = filter.Page,
                PageSize = filter.PageSize,
                Total = total
            };
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var createdProduct = await _productRepository.AddAsync(product);
            return _mapper.Map<ProductDto>(createdProduct);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            product.Id = id;
            var updatedProduct = await _productRepository.UpdateAsync(product);
            return _mapper.Map<ProductDto>(updatedProduct);
        }

        public async Task DeleteAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }
    }
}
