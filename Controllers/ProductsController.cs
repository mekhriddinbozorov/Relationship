using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using relationship_task.Dto;

[ApiController]
[Route("/api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext appDbContext;

    public ProductsController(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductDto createProduct)
    {
        if (await appDbContext.Products.AnyAsync(p => p.Name == createProduct.Name))
            return Conflict("Products with this name exists");

        var created = appDbContext.Products.Add(new Product
        {
            Id = Guid.NewGuid(),
            Name = createProduct.Name,
            Price = createProduct.Price,
            CreatedAt = createProduct.CreatedAt,
            ModifiedAt = createProduct.ModifiedAt,
            Status = createProduct.Status
        });

        await appDbContext.SaveChangesAsync();

        return Ok(created.Entity);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct([FromRoute] Guid id)
    {
        var productEntity = await appDbContext.Products
            .Where(u => u.Id == id)
            .Include(u => u.ProductDetail)
            .FirstOrDefaultAsync();

        if (productEntity is null)
            return NotFound();

        return Ok(new GetProductDto
        {
            Id = productEntity.Id,
            Name = productEntity.Name,
            Price = productEntity.Price,
            CreatedAt = productEntity.CreatedAt,
            Status = productEntity.Status
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var productEntity = appDbContext.Products.AsQueryable();

        if (productEntity is null)
            return NotFound();

        var product = await productEntity
            .Select(u => new GetProductDto
            {
                Id = u.Id,
                Name = u.Name,
                Price = u.Price,
                CreatedAt = u.CreatedAt,
                Status = u.Status
            })
            .ToListAsync();

        return Ok(product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, UpdateProductDto updateProduct)
    {
        var productEntity = await appDbContext.Products
            .FirstOrDefaultAsync(u => u.Id == id);

        if (productEntity is null)
            return NotFound();

        productEntity.Name = updateProduct.Name;
        productEntity.Price = updateProduct.Price;
        productEntity.CreatedAt = updateProduct.CreatedAt;
        productEntity.ModifiedAt = updateProduct.ModifiedAt;
        productEntity.Status = updateProduct.Status;

        await appDbContext.SaveChangesAsync();

        return Ok(productEntity.Id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
    {
        var user = await appDbContext.Products.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return NotFound();

        appDbContext.Products.Remove(user);
        await appDbContext.SaveChangesAsync();

        return Ok();
    }


    [HttpPost("{id}/details")]
    public async Task<IActionResult> CreateProductDetails([FromRoute] Guid id, CreateProductDetailDto createProductDetail)
    {
        var product = await appDbContext.Products
            .Where(u => u.Id == id)
            .Include(u => u.ProductDetail)
            .FirstOrDefaultAsync();

        Console.WriteLine(product.Name);

        if (product is null)
            return BadRequest($"product with id {id} does not exist");

        if (product.ProductDetail is not null)
            return BadRequest($"product already has product details");

        product.ProductDetail = new ProductDetail
        {
            Description = createProductDetail.Description,
            Color = createProductDetail.Color,
            Material = createProductDetail.Material,
            Weight = createProductDetail.Weight,
            QuantityInStock = createProductDetail.QuantityInStock,
            ManufactureDate = createProductDetail.ManufactureDate,
            ExpiryDate = createProductDetail.ExpiryDate,
            Size = createProductDetail.Size,
            Manufacturer = createProductDetail.Manufacturer,
            CountryOfOrigin = createProductDetail.CountryOfOrigin
        };

        //appDbContext.Add(productEntityDetails);
        await appDbContext.SaveChangesAsync();

        return Ok(product.ProductDetail.Id);
    }

    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetDetails([FromRoute] Guid id)
    {
        var product = await appDbContext.Products
            .Where(u => u.Id == id)
            .Include(u => u.ProductDetail)
            .FirstOrDefaultAsync();

        if (product is null || product.ProductDetail is null)
            return NotFound();


        return Ok(new GetProductDetailDto
        {
            Id = product.ProductDetail.Id,
            Description = product.ProductDetail.Description,
            Color = product.ProductDetail.Color,
            Material = product.ProductDetail.Material,
            Weight = product.ProductDetail.Weight,
            QuantityInStock = product.ProductDetail.QuantityInStock,
            ManufactureDate = product.ProductDetail.ManufactureDate,
            ExpiryDate = product.ProductDetail.ExpiryDate,
            Size = product.ProductDetail.Size,
            Manufacturer = product.ProductDetail.Manufacturer,
            CountryOfOrigin = product.ProductDetail.CountryOfOrigin
        });

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDetails([FromRoute] Guid id, UpdateProductDetailDto detailDto)
    {
        var product = await appDbContext.Products
           .Where(u => u.Id == id)
           .Include(u => u.ProductDetail)
           .FirstOrDefaultAsync();

        if (product is null || product.ProductDetail is null)
            return NotFound();

        product.ProductDetail.Description = detailDto.Description;
        product.ProductDetail.Color = detailDto.Color;
        product.ProductDetail.Material = detailDto.Material;
        product.ProductDetail.Weight = detailDto.Weight;
        product.ProductDetail.QuantityInStock = detailDto.QuantityInStock;
        product.ProductDetail.ManufactureDate = detailDto.ManufactureDate;
        product.ProductDetail.ExpiryDate = detailDto.ExpiryDate;
        product.ProductDetail.Size = detailDto.Size;
        product.ProductDetail.Manufacturer = detailDto.Manufacturer;
        product.ProductDetail.CountryOfOrigin = detailDto.CountryOfOrigin;

        await appDbContext.SaveChangesAsync();

        return Ok(product.ProductDetail.Id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDetails([FromRoute] Guid id)
    {
        var detail = await appDbContext.ProductDetails.FirstOrDefaultAsync(u => u.Id == id);
        if (detail is null)
            return NotFound();

        appDbContext.ProductDetails.Remove(detail);
        await appDbContext.SaveChangesAsync();

        return Ok();
    }


    private EProductStatus ConvertEntity(EProductStatusDto status)
   => status switch
   {
       EProductStatusDto.Inactive => EProductStatus.Inactive,
       EProductStatusDto.Soldout => EProductStatus.Soldout,
       _ => EProductStatus.Active,
   };
}