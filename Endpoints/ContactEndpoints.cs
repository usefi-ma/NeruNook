using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NeruNook.Models;
using NeruNook.Services;

namespace NeruNook.Endpoints;

public static class ContactEndpoints
{
    public static IEndpointRouteBuilder MapContactEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/contact/submit", async (
            [FromForm] ContactSubmissionRequest request,
            ContactInquiryService inquiryService,
            CancellationToken cancellationToken) =>
        {
            if (!string.IsNullOrWhiteSpace(request.Website))
            {
                return Results.LocalRedirect("/?contactStatus=success#contact");
            }

            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, context, results, validateAllProperties: true))
            {
                return Results.LocalRedirect("/?contactStatus=invalid#contact");
            }

            await inquiryService.SubmitAsync(request, cancellationToken);
            return Results.LocalRedirect("/?contactStatus=success#contact");
        })
        .RequireRateLimiting("contact");

        return endpoints;
    }
}
