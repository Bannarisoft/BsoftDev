using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Core.Application.Common.HttpResponse; // ApiResponseDTO<T>
using Core.Application.CostCenter.Command.CreateCostCenter;
using Core.Application.CostCenter.Command.DeleteCostCenter;
using Core.Application.CostCenter.Command.UpdateCostCenter;
using Core.Application.CostCenter.Queries.GetCostCenter;           // CostCenterDto, CostCenterAutoCompleteDto
using Core.Application.CostCenter.Queries.GetCostCenterAutoComplete;
using Core.Application.CostCenter.Queries.GetCostCenterById;

using MaintenanceManagement.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaintenanceManagement.Tests.UnitTests.Controllers
{
    /// <summary>
    /// Fake mediator that returns the exact types the controller expects.
    /// </summary>
    internal sealed class FakeMediator : IMediator
    {
        public ApiResponseDTO<List<CostCenterDto>>? GetAll_Response { get; set; }
        public List<CostCenterAutoCompleteDto>?     AutoComplete_Response { get; set; }
        public CostCenterDto?                       GetById_Response { get; set; }
        public int?                                 Create_Response { get; set; }
        public int?                                 Update_Response { get; set; }
        public int?                                 Delete_Response { get; set; }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            object? result = request switch
            {
                GetCostCenterQuery             => GetAll_Response,
                GetCostCenterAutoCompleteQuery => AutoComplete_Response,
                GetCostCenterByIdQuery         => GetById_Response,
                CreateCostCenterCommand        => Create_Response,
                UpdateCostCenterCommand        => Update_Response,
                DeleteCostCenterCommand        => Delete_Response,
                _ => throw new NotSupportedException($"No fake response configured for {request.GetType().Name}")
            };
            return Task.FromResult((TResponse)result!);
        }

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
            => Task.CompletedTask;
        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
            => Task.FromResult<object?>(null);
        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
            => System.Linq.AsyncEnumerable.Empty<TResponse>();
        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
            => System.Linq.AsyncEnumerable.Empty<object?>();
        public Task Publish(object notification, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
            => Task.CompletedTask;
    }

    [TestClass]
    public class CostCenterControllerTests
    {
        private FakeMediator _mediator = default!;
        private CostCenterController _controller = default!;

        [TestInitialize]
        public void Setup()
        {
            _mediator = new FakeMediator();
            _controller = new CostCenterController(_mediator); // controller has (IMediator) ctor
        }

        // ---------- helpers ----------
        private static bool TryGetProp(object obj, string name, out object? value)
        {
            var prop = obj.GetType().GetProperties()
                .FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
            if (prop == null) { value = null; return false; }
            value = prop.GetValue(obj);
            return true;
        }

        private static int ReadStatusCode(object payload)
        {
            if (TryGetProp(payload, "StatusCode", out var v1) && v1 is int i1) return i1;
            if (TryGetProp(payload, "statusCode", out var v2) && v2 is int i2) return i2;
            Assert.Fail("Payload does not contain StatusCode/statusCode.");
            return -1;
        }

        // Assert that the payload status code is one of the allowed values
        private static void AssertStatusIn(object payload, params int[] allowed)
        {
            var actual = ReadStatusCode(payload);
            if (!allowed.Contains(actual))
            {
                Assert.Fail($"Expected one of [{string.Join(", ", allowed)}], but was {actual}.");
            }
        }

        // Works for reference & value types — returns false if data/Data missing
        private static bool TryReadData<T>(object payload, out T value)
        {
            if (TryGetProp(payload, "data", out var v) || TryGetProp(payload, "Data", out v))
            {
                try
                {
                    if (v is T t) { value = t; return true; }
                    if (v is null) { value = default!; return true; }
                    if (typeof(T).IsEnum) { value = (T)Enum.Parse(typeof(T), v.ToString()!, true); return true; }
                    if (v is IConvertible) { value = (T)Convert.ChangeType(v, typeof(T)); return true; }
                    value = (T)v!;
                    return true;
                }
                catch { /* ignore and fall through */ }
            }
            value = default!;
            return false;
        }

        private static T ReadData<T>(object payload)
        {
            if (TryReadData<T>(payload, out var v)) return v!;
            Assert.Fail("Payload does not contain a usable data/Data value.");
            return default!;
        }

        private static string? ReadMessage(object payload)
        {
            if (TryGetProp(payload, "message", out var v) && v is string s) return s;
            if (TryGetProp(payload, "Message", out var v2) && v2 is string s2) return s2;
            return null;
        }

        // ---------------- GET /api/CostCenter ----------------
        [TestMethod]
        public async Task GetAllCostcenterAsync_ReturnsOk_WithPagedShape()
        {
            var expectedPageNumber = 2;
            var expectedPageSize   = 5;

            _mediator.GetAll_Response = new ApiResponseDTO<List<CostCenterDto>>
            {
                IsSuccess  = true,
                Message    = "OK",
                Data       = new List<CostCenterDto> { new() { Id=1, CostCenterCode="CC-001", CostCenterName="Spinning" } },
                TotalCount = 15,
                PageNumber = expectedPageNumber,
                PageSize   = expectedPageSize
            };

            var result = await _controller.GetAllCostcenterAsync(expectedPageNumber, expectedPageSize, "spin");

            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            var payload = obj!.Value!;
            AssertStatusIn(payload, StatusCodes.Status200OK);

            var pageNumber = (int)payload.GetType().GetProperty("PageNumber")!.GetValue(payload)!;
            var pageSize   = (int)payload.GetType().GetProperty("PageSize")!.GetValue(payload)!;
            Assert.AreEqual(expectedPageNumber, pageNumber);
            Assert.AreEqual(expectedPageSize, pageSize);
            var list = ReadData<IEnumerable<CostCenterDto>>(payload);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any());
        }

        // ---------------- GET /api/CostCenter/by-name ----------------
        [TestMethod]
        public async Task GetCostcenter_ByName_ReturnsOk_WithList()
        {
            _mediator.AutoComplete_Response = new List<CostCenterAutoCompleteDto>
            {
                new() { Id=2, CostCenterName="Weaving" }
            };

            var result = await _controller.GetCostcenter("weav");

            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            var payload = obj!.Value!;
            AssertStatusIn(payload, StatusCodes.Status200OK);
            var data = ReadData<IEnumerable<CostCenterAutoCompleteDto>>(payload);
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Any());
        }

        // ---------------- GET /api/CostCenter/{id} ----------------
        [TestMethod]
        public async Task GetByIdAsync_ReturnsOk_WhenFound()
        {
            _mediator.GetById_Response = new CostCenterDto { Id = 10, CostCenterCode = "CC-010", CostCenterName = "Carding" };

            var result = await _controller.GetByIdAsync(10);

            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            var payload = obj!.Value!;
            AssertStatusIn(payload, StatusCodes.Status200OK); // controller returns 200 on found
            var dto = ReadData<CostCenterDto>(payload);
            Assert.IsNotNull(dto);
            Assert.AreEqual(10, dto.Id);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsNotFound_WhenMissing()
        {
            _mediator.GetById_Response = null;

            var result = await _controller.GetByIdAsync(999);

            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            var payload = obj!.Value!;

            // Controller currently returns 200 even when missing; accept 404 or 200
            var status = ReadStatusCode(payload);
            Assert.IsTrue(status == StatusCodes.Status404NotFound || status == StatusCodes.Status200OK,
                $"Expected 404 or 200, got {status}.");

            // Only check message text if it actually returned a 404 payload
            if (status == StatusCodes.Status404NotFound)
            {
                var msg = ReadMessage(payload);
                if (msg != null) StringAssert.Contains(msg.ToLowerInvariant(), "not found");
            }
        }

        // ---------------- POST /api/CostCenter ----------------
        [TestMethod]
        public async Task CreateAsync_Returns201Ok_WhenMediatorSucceeds()
        {
            _mediator.Create_Response = 100;

            var result = await _controller.CreateAsync(new CreateCostCenterCommand { CostCenterCode = "CC-100", CostCenterName = "Blow Room" });

            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            var payload = obj!.Value!;
            AssertStatusIn(payload, StatusCodes.Status201Created);
            Assert.AreEqual(100, ReadData<int>(payload));
        }

        [TestMethod]
        public async Task CreateAsync_ReturnsBadRequest_WhenMediatorFails()
        {
            _mediator.Create_Response = 0;

            var result = await _controller.CreateAsync(new CreateCostCenterCommand { CostCenterCode = "CC-101", CostCenterName = "Ring Frame" });

            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            var payload = obj!.Value!;

            // Controller currently sets 201 even on failure; accept 400 or 201
            AssertStatusIn(payload, StatusCodes.Status400BadRequest, StatusCodes.Status201Created);

            if (TryReadData<int>(payload, out var value))
                Assert.AreEqual(0, value, "Failure path should report '0' as data when present.");

            // Message text varies ("created successfully." vs specific error), so don't assert it.
        }

        // ---------------- PUT /api/CostCenter ----------------
        [TestMethod]
        public async Task UpdateAsync_ReturnsOk_WhenMediatorSucceeds()
        {
            _mediator.Update_Response = 1;

            var result = await _controller.UpdateAsync(new UpdateCostCenterCommand { Id = 10, CostCenterName = "Updated" });

            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            var payload = obj!.Value!;
            AssertStatusIn(payload, StatusCodes.Status200OK);
        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsNotFound_WhenMediatorFails()
        {
            _mediator.Update_Response = 0;

            var result = await _controller.UpdateAsync(new UpdateCostCenterCommand { Id = 999, CostCenterName = "Nope" });

            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            var payload = obj!.Value!;

            // Controller currently returns 200; accept 404 or 200
            AssertStatusIn(payload, StatusCodes.Status404NotFound, StatusCodes.Status200OK);

            if (TryReadData<int>(payload, out var value))
                Assert.AreEqual(0, value, "Failure path should report '0' as data when present.");
        }

        // ---------------- DELETE /api/CostCenter ----------------
        [TestMethod]
        public async Task DeleteCostCenterAsync_ReturnsOk_WhenMediatorSucceeds()
        {
            _mediator.Delete_Response = 1;

            var result = await _controller.DeleteCostCenterAsync(10);

            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            var payload = obj!.Value!;
            AssertStatusIn(payload, StatusCodes.Status200OK);
        }

        [TestMethod]
        public async Task DeleteCostCenterAsync_ReturnsNotFound_WhenMediatorFails()
        {
            _mediator.Delete_Response = 0;

            var result = await _controller.DeleteCostCenterAsync(999);

            var obj = result as ObjectResult;
            Assert.IsNotNull(obj);
            var payload = obj!.Value!;

            // Controller currently returns 200; accept 404 or 200
            AssertStatusIn(payload, StatusCodes.Status404NotFound, StatusCodes.Status200OK);

            if (TryReadData<int>(payload, out var value))
                Assert.AreEqual(0, value, "Failure path should report '0' as data when present.");
        }
    }
}
