using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using taskDz.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace taskDz
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            
            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {   
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            var tasks = new List<TaskObject>();

            app.MapGet("/api/tasks", (HttpContext context) =>
            {
                var result = tasks.AsEnumerable();

                if (context.Request.Query["completed"] == "true")
                    result = result.Where(t => t.Completed);
                
                if (context.Request.Query["completed"] == "false")
                    result = result.Where(t => !t.Completed);

                if (context.Request.Query["expired"] == "true")
                    result = result.Where(t => t.Deadline < DateTime.Now && !t.Completed);
                
                if (context.Request.Query["current"] == "true")
                    result = result.Where(t => t.Deadline >= DateTime.Now && !t.Completed);

                return Results.Ok(new { Status = 200, Message = "OK", Data = result.ToList() });
            });

            app.MapGet("/api/task/{id}", (string id) =>
            {
                if (!Guid.TryParse(id, out Guid guidId))
                    return Results.BadRequest(new { Status = 400, Message = "Invalid ID format", Error = "ID must be a GUID" });

                var task = tasks.FirstOrDefault(t => t.Id == guidId);
                
                if (task == null)
                    return Results.NotFound(new { Status = 404, Message = "Task not found", Error = "Task does not exist" });

                return Results.Ok(new { Status = 200, Message = "OK", Data = task });
            });

            app.MapPost("/api/tasks/create", (HttpContext context, CreateTask model) =>
            {
                var token = context.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(token))
                    return Results.Json(new { Status = 403, Message = "Forbidden", Error = "Token is missing" }, statusCode: 403);

                if (string.IsNullOrWhiteSpace(model.Title))
                    return Results.BadRequest(new { Status = 400, Message = "Validation error", Error = "Title is required" });

                var newTask = new TaskObject
                {
                    Title = model.Title,
                    Description = model.Description ?? string.Empty,
                    Deadline = model.Deadline, 
                    Completed = false
                };

                tasks.Add(newTask);

                return Results.Ok(new { Status = 200, Message = "Task created", Data = newTask });
            });

            app.MapDelete("/api/task/{id}", (HttpContext context, string id) =>
            {
                var token = context.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(token))
                    return Results.Json(new { Status = 403, Message = "Forbidden", Error = "Token is missing" }, statusCode: 403);

                if (!Guid.TryParse(id, out Guid guidId))
                    return Results.BadRequest(new { Status = 400, Message = "Invalid ID format", Error = "ID must be a GUID" });

                var task = tasks.FirstOrDefault(t => t.Id == guidId);
                
                if (task == null)
                    return Results.NotFound(new { Status = 404, Message = "Task not found", Error = "Task does not exist" });

                tasks.Remove(task);
                return Results.Ok(new { Status = 200, Message = "Task deleted" });
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}