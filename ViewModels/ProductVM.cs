using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ECommerce.ViewModels;

public class ProductVM
{
    [ValidateNever]
    public IEnumerable <Category>Categories { get; set; }
    [ValidateNever]
    public IEnumerable <Brand> Brands{ get; set; }
    public IEnumerable<ProductColor>? ProductColors{get;set;}
    public IEnumerable<ProductSubImage>? ProductSubImages{get;set;}
    public Product? Product {get;set;}
}