﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyExpenses.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyExpenses.Models
{
    [assebly: Dependency (typeof(DatabaseRepository))]
    public class DatabaseRepository : DbContext, IDataStore<Expense>
    {
        string _path;
        public DatabaseRepository(string path)
        {
            _path = path;           
            Database.EnsureCreated(); // potrzebne?
            Database.Migrate();
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<BudgetCategory> Categories { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_path}");
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Expense>().Property(p => p.Amount).IsRequired();
            modelBuilder.Entity<Expense>().HasKey(p => p.Id);
            modelBuilder.Entity<Expense>().Property(p => p.PaymentType).HasConversion(new EnumToStringConverter<PaymentType>());
          
            modelBuilder.Entity<BudgetCategory>().HasKey(p => p.CategoryId);

          
        }
        
        public async Task<Expense> GetExpenseAsync(int id)
        {
            var item = await Expenses.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return item;
        }
        public async Task<BudgetCategory> GetCategoryAsync(int id)
        {
            var item = await Categories.FirstOrDefaultAsync(x => x.CategoryId == id).ConfigureAwait(false);
            return item;
        }
        public async Task<IEnumerable<Expense>> GetExpensesAsync(bool forceRefresh = false) =>       
            await Expenses.ToListAsync().ConfigureAwait(false);
         public async Task<IList<BudgetCategory>> GetCategoriesAsync(bool forceRefresh = false) =>       
            await Categories.ToListAsync().ConfigureAwait(false);
        
        public async Task<bool> AddExpenseAsync(Expense item)
        {
            await Expenses.AddAsync(item).ConfigureAwait(false);
            await SaveChangesAsync().ConfigureAwait(false);
            return true;
        }
        public async Task<bool> AddCategoryAsync(BudgetCategory item)
        {
            await Categories.AddAsync(item).ConfigureAwait(false);                 
            return true;
        }

        public async Task<bool> UpdateExpenseAsync(Expense item)
        {
            Expenses.Update(item);
            await SaveChangesAsync().ConfigureAwait(false);
            return true;
        }
        public void UpdateCategory(BudgetCategory item)
        {
            Categories.Update(item);                               
        }
        
        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var item = await Expenses.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            if (item != null)
            {
                Expenses.Remove(item);
                await SaveChangesAsync().ConfigureAwait(false);
            }
            
            return true;
        }
        public async Task DeleteCategoryAsync(int id)
        {
            var item = await Categories.FirstOrDefaultAsync(x => x.CategoryId == id).ConfigureAwait(false);
            if (item != null)
            {
                Categories.Remove(item);                    
            }
            
        }
        
    }
}