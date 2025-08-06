﻿namespace Product.Application.DTOs
{
    public record UpdateProductDto(
        string Name,
        string Description,
        decimal Price,
        int Stock
    );
}