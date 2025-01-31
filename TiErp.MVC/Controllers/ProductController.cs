﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TiErp.Application.Customer.Commands.DeleteCustomer;
using TiErp.Application.Order.Queries.GetAllOrders;
using TiErp.Application.Order.Queries.GetOrderById;
using TiErp.Application.OrderItem.Commands.EditOrderItem;
using TiErp.Application.OrderItem.Queries.GetOrderItemById;
using TiErp.Application.Product.Commands.CreateProduct;
using TiErp.Application.Product.Commands.DeleteProduct;
using TiErp.Application.Product.Commands.EditProduct;
using TiErp.Application.Product.DTOs;
using TiErp.Application.Product.Queries.GetAllProducts;
using TiErp.Application.Product.Queries.GetProductById;
using TiErp.Application.ProductionItem.Queries.GetAllProductionItems;
using TiErp.Application.ProductionItem.Queries.GetProductionItemById;
using TiErp.Application.ProductionItem.Queries.GetProductionItemByIdEntity;
using TiErp.Application.ProductionLine.Commands.CreateProductionLine;
using TiErp.Application.ProductionLine.Queries.GetAllProductionLines;
using TiErp.Application.ProductionLine.Queries.GetProductionLineById;
using TiErp.Domain.Entities;
using TiErp.Domain.Interfaces;
using TiErp.MVC.Models;

namespace TiErp.MVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IProductionLineRepository _productionLineRepository;

        public ProductController(IMediator mediator, IMapper mapper, IProductionLineRepository productionLineRepository)
        {
            _mediator = mediator;
            _mapper = mapper;
            _productionLineRepository = productionLineRepository;
        }
        [Authorize(Roles = "Kierownik, Admin")]
        public async Task<IActionResult> Index()
        {
            var products = await _mediator.Send(new GetAllProductsQuery());
            return View(products);
        }
        [Authorize(Roles = "Kierownik, Admin")]
        [Route("Product/{id}/Details")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));
            return View(product);
        }

        [Authorize(Roles = "Kierownik, Admin")]
        public async Task<IActionResult> Create()
        {
            var model = new CreateProductView
            {
                productionLines = await _mediator.Send(new GetAllProductionLinesQuery()),
                AvailableProductionItems = await _mediator.Send(new GetAllProductionItemsQuery())
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Kierownik, Admin")]
        public async Task<IActionResult> Create(CreateProductView view)
        {
            var productionLine = await _productionLineRepository.GetById(view.ProductionLineId);
            view.CreateProductCommand.ProductionLine = productionLine;
            view.CreateProductCommand.Name = view.Name;
            view.CreateProductCommand.ProductionItems ??= new List<ProductionItem>();


            foreach (var id in view.SelectedProductionItemIds)
            {
                var productionItem = await _mediator.Send(new GetProductionItemByIdEntityQuery(id));
                if (productionItem != null)
                {
                    view.CreateProductCommand.ProductionItems.Add(productionItem);
                }
            }

            await _mediator.Send(view.CreateProductCommand);
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Kierownik, Admin")]
        [Route("Product/{id}/Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));
            if (product == null)
            {
                return NotFound();
            }
            var model = new EditProductView
            {
                Id = product.Id,
                Name = product.Name,
                SelectedProductionItemIds = product.ProductionItems.Select(p => p.Id).ToList(),
                ProductionLineId = product.ProductionLine.Id,
                productionLines = await _mediator.Send(new GetAllProductionLinesQuery()),
                AvailableProductionItems = await _mediator.Send(new GetAllProductionItemsQuery()),
            };
            return View(model);
        }
        [Authorize(Roles = "Kierownik, Admin")]
        [HttpPost]
        [Route("Product/{id}/Edit")]
        public async Task<IActionResult> Edit(EditProductView view)
        {
            var productionLine = await _productionLineRepository.GetById(view.ProductionLineId);
            view.EditProductCommand.ProductionLine = productionLine;
            view.EditProductCommand.Name = view.Name;
            view.EditProductCommand.Id = view.Id;


            foreach (var id in view.SelectedProductionItemIds)
            {
                var productionItem = await _mediator.Send(new GetProductionItemByIdEntityQuery(id));
                if (productionItem != null)
                {
                    view.EditProductCommand.ProductionItems.Add(productionItem);
                }
            }

            await _mediator.Send(view.EditProductCommand);
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Kierownik, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteProductCommand(id));
            return RedirectToAction(nameof(Index));
        }
    }

}
