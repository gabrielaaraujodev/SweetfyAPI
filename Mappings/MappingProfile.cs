using AutoMapper;
using SweetfyAPI.DTOs.IndredientDTO;
using SweetfyAPI.DTOs.OrderDTO;
using SweetfyAPI.DTOs.ProductDTO;
using SweetfyAPI.DTOs.RecipeDTO;
using SweetfyAPI.DTOs.ServiceDTO;
using SweetfyAPI.DTOs.UserDTO;
using SweetfyAPI.Entities;

namespace SweetfyAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserDto>(); 


            CreateMap<Ingredient, IngredientDto>();
            CreateMap<CreateIngredientDto, Ingredient>();
            CreateMap<UpdateIngredientDto, Ingredient>();


            CreateMap<Service, ServiceDto>();
            CreateMap<CreateServiceDto, Service>();
            CreateMap<UpdateServiceDto, Service>();



            CreateMap<CreateRecipeDto, Recipe>();
            CreateMap<CreateRecipeIngredientDto, RecipeIngredient>();
            CreateMap<CreateRecipeServiceDto, RecipeService>();
            CreateMap<UpdateRecipeDto, Recipe>()
                .ForMember(dest => dest.RecipeIngredients, opt => opt.MapFrom(src => src.RecipeIngredients))
                .ForMember(dest => dest.RecipeServices, opt => opt.MapFrom(src => src.RecipeServices));

            CreateMap<Recipe, RecipeDto>(); 
            CreateMap<Recipe, RecipeDetailsDto>(); 
            CreateMap<RecipeIngredient, RecipeIngredientDetailsDto>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src => src.Ingredient.Name));
            CreateMap<RecipeService, RecipeServiceDetailsDto>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name));



            CreateMap<CreateProductDto, Product>();
            CreateMap<CreateProductIngredientDto, ProductIngredient>();
            CreateMap<CreateProductRecipeDto, ProductRecipe>();
            CreateMap<CreateProductServiceDto, global::ProductService>(); 
            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.ProductIngredients, opt => opt.MapFrom(src => src.ProductIngredients))
                .ForMember(dest => dest.ProductRecipes, opt => opt.MapFrom(src => src.ProductRecipes))
                .ForMember(dest => dest.ProductServices, opt => opt.MapFrom(src => src.ProductServices));

            CreateMap<Product, ProductDto>(); 
            CreateMap<Product, ProductDetailsDto>(); 
            CreateMap<ProductIngredient, ProductIngredientDetailsDto>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src => src.Ingredient.Name));
            CreateMap<ProductRecipe, ProductRecipeDetailsDto>()
                .ForMember(dest => dest.RecipeName, opt => opt.MapFrom(src => src.Recipe.Name));
            CreateMap<global::ProductService, ProductServiceDetailsDto>() 
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name));



            CreateMap<CreateOrderDto, Order>();
            CreateMap<CreateOrderProductDto, OrderProduct>();
            CreateMap<CreateOrderRecipeDto, OrderRecipe>();
            CreateMap<UpdateOrderDto, Order>(); 

            CreateMap<Order, OrderDto>(); 
            CreateMap<Order, OrderDetailsDto>(); 
            CreateMap<OrderProduct, OrderProductDetailsDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<OrderRecipe, OrderRecipeDetailsDto>()
                .ForMember(dest => dest.RecipeName, opt => opt.MapFrom(src => src.Recipe.Name));
        }
    }
}