﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website.Models;

namespace Website.Controllers
{
    public class TaskController : Controller
    {
        private readonly MyDbContext _db;
        public TaskController (MyDbContext db)
        {
            _db = db;
        }

        /*public IActionResult Index()
        {
            List<TaskModel> tasks = new List<TaskModel>();
            for (int i = 1; i < 11; i++)
            {
                tasks.Add(_db.Tasks.FirstOrDefault(p => p.TaskId == i));
            }
            return View(tasks);
        }*/

        public IActionResult Create([Bind("TaskId", "TaskName","TaskDescription", "StartDate", "DueDate", "Progress", "Flags", "Comments", "ProjectId", "UserId")]Models.TaskModel addTask)
        {
            if(ModelState.IsValid)
            {
                _db.Tasks.Add(addTask);
                _db.SaveChanges();
                return Redirect("Index");
            }
            return View("Create");

        }

        public IActionResult Details(int? id)
        {
            List<TaskModel> tasks = new List<TaskModel>();
            if (!id.HasValue)
            {
                for (int i = 1; i < 11; i++)
                {
                    tasks.Add(_db.Tasks.FirstOrDefault(p => p.TaskId == i));
                }
                return View(tasks);
            }

            tasks.Add(_db.Tasks.FirstOrDefault(p => p.TaskId == id.Value));
            return View(tasks);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _db.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _db.Tasks
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _db.Tasks.Any(e => e.TaskId == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId", "TaskName","TaskDescription", "StartDate", "DueDate", "Progress", "Flags", "Comments", "ProjectId", "UserId")] TaskModel task)
        {
            if (id != task.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(task);
                    await _db.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.TaskId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var tasks = from m in _db.Tasks
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(s => s.TaskName.Contains(searchString));
            }

            return View(await tasks.ToListAsync());
        }

    }
}