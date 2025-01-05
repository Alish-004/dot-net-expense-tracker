using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using coursework.Models;

namespace coursework.Service
{
    public class ExpenseService
    {
        // Path to the Desktop, hardcoded for a Windows environment
        private readonly string _filePath;

        public ExpenseService()
        {
            // Construct the path to the Desktop
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _filePath = Path.Combine(desktopPath, "expenses.json");  // Path to the JSON file on Desktop

            Console.Write(_filePath);
            // Ensure the JSON file exists at the start
            if (!File.Exists(_filePath))
            {
                // Create the file with an empty list if it doesn't exist
                File.WriteAllText(_filePath, "[]");
            }
        }

        // Method to get all expenses from the JSON file
        public List<Expense> GetAllExpenses()
        {
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Expense>>(json) ?? new List<Expense>();
        }

        // Method to add a new expense to the JSON file
        public void AddExpense(Expense expense)
        {
            var expenses = GetAllExpenses();
            expense.Id = expenses.Any() ? expenses.Max(e => e.Id) + 1 : 1; // auto-increment ID
            expenses.Add(expense);

            SaveExpenses(expenses);
        }

        // Method to update an existing expense
        public bool UpdateExpense(Expense updatedExpense)
        {
            var expenses = GetAllExpenses();
            var existingExpense = expenses.FirstOrDefault(e => e.Id == updatedExpense.Id);

            if (existingExpense != null)
            {
                existingExpense.Name = updatedExpense.Name;
                existingExpense.Description = updatedExpense.Description;
                existingExpense.Type = updatedExpense.Type;
                existingExpense.amount = updatedExpense.amount;

                SaveExpenses(expenses);
                return true;
            }

            return false; // expense with the given ID not found
        }

        // Method to delete an expense by its ID
        public bool DeleteExpense(int expenseId)
        {
            var expenses = GetAllExpenses();
            var expenseToRemove = expenses.FirstOrDefault(e => e.Id == expenseId);

            if (expenseToRemove != null)
            {
                expenses.Remove(expenseToRemove);
                SaveExpenses(expenses);
                return true;
            }

            return false; // expense with the given ID not found
        }

        // Helper method to save the expenses back to the JSON file
        private void SaveExpenses(List<Expense> expenses)
        {
            string json = JsonSerializer.Serialize(expenses, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
