using Microsoft.AspNetCore.Mvc.RazorPages;
using Try.DTO;
using Try.Service;
using Try.Service.ProductService;

namespace Try.Pages;

public class IndexModel : PageModel
{
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;

    public List<ProductAssociationDto>? Asociate { get; set; }

    public IndexModel(IOrderService orderService, IProductService productService)
    {
        _orderService = orderService;
        _productService = productService;
    }

    public void OnGet()
    {
        var asocieri = _orderService.GetAssociatedProducts();
        Asociate = new List<ProductAssociationDto>();

        foreach (var a in asocieri)
        {
            var p1 = _productService.GetProductById(a.Item1).Result;
            var p2 = _productService.GetProductById(a.Item2).Result;

            Asociate.Add(new ProductAssociationDto
            {
                Product1Id = a.Item1,
                Product2Id = a.Item2,
                Count = a.Item3,
                Product1 = p1,
                Product2 = p2
            });
        }
    }
}
