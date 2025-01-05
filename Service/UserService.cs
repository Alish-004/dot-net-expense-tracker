using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using coursework.Models;

namespace coursework.Service
{
    public class UserService
    {
        private readonly string _filePath;

        public UserService()
        {
            // Adjust the file path to point to Desktop
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _filePath = Path.Combine(desktopPath, "expenses.json");

            // Ensure the JSON file exists at the start
            if (!File.Exists(_filePath))
            {
                // Create the file with an empty Users array if it doesn't exist
                File.WriteAllText(_filePath, "{\"Users\":[]}");
            }
        }

        // Method to get all users from the JSON file
        public List<User> GetAllUsers()
        {
            string json = File.ReadAllText(_filePath);
            var data = JsonSerializer.Deserialize<Root>(json);
            return data?.Users ?? new List<User>();
        }

        // Method to get a user by ID
        public User GetUserById(int userId)
        {
            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.ID == userId);
        }

        // Method to add a new user to the JSON file
        public void AddUser(User user)
        {
            var users = GetAllUsers();
            user.ID = users.Any() ? users.Max(u => u.ID) + 1 : 1; // auto-increment ID
            users.Add(user);

            SaveUsers(users);
        }

        // Method to add an expense to a user
        public bool AddExpenseToUser(int userId, Expense expense)
        {
            var users = GetAllUsers();
            var user = users.FirstOrDefault(u => u.ID == userId);

            if (user != null)
            {
                expense.Id = user.Expenses.Any() ? user.Expenses.Max(e => e.Id) + 1 : 1; // auto-increment ID
                user.Expenses.Add(expense);
                SaveUsers(users);
                return true;
            }

            return false; // User not found
        }

        // Method to update a user's information
        public bool UpdateUser(User updatedUser)
        {
            var users = GetAllUsers();
            var existingUser = users.FirstOrDefault(u => u.ID == updatedUser.ID);

            if (existingUser != null)
            {
                existingUser.Name = updatedUser.Name;
                existingUser.Password = updatedUser.Password;
                existingUser.Address = updatedUser.Address;
                existingUser.ContactNumber = updatedUser.ContactNumber;

                SaveUsers(users);
                return true;
            }

            return false; // User not found
        }

        // Method to update an expense for a user
        public bool UpdateExpense(int userId, Expense updatedExpense)
        {
            var users = GetAllUsers();
            var user = users.FirstOrDefault(u => u.ID == userId);

            if (user != null)
            {
                var existingExpense = user.Expenses.FirstOrDefault(e => e.Id == updatedExpense.Id);

                if (existingExpense != null)
                {
                    existingExpense.Name = updatedExpense.Name;
                    existingExpense.Description = updatedExpense.Description;
                    existingExpense.Type = updatedExpense.Type;
                    existingExpense.amount = updatedExpense.amount;

                    SaveUsers(users);
                    return true;
                }
            }

            return false; // Expense or User not found
        }

        // Method to delete a user by ID
        public bool DeleteUser(int userId)
        {
            var users = GetAllUsers();
            var userToRemove = users.FirstOrDefault(u => u.ID == userId);

            if (userToRemove != null)
            {
                users.Remove(userToRemove);
                SaveUsers(users);
                return true;
            }

            return false; // User not found
        }

        // Method to delete an expense for a user
        public bool DeleteExpense(int userId, int expenseId)
        {
            var users = GetAllUsers();
            var user = users.FirstOrDefault(u => u.ID == userId);

            if (user != null)
            {
                var expenseToRemove = user.Expenses.FirstOrDefault(e => e.Id == expenseId);

                if (expenseToRemove != null)
                {
                    user.Expenses.Remove(expenseToRemove);
                    SaveUsers(users);
                    return true;
                }
            }

            return false; // Expense or User not found
        }

        // Helper method to save users and their expenses to the JSON file
        private void SaveUsers(List<User> users)
        {
            var root = new Root { Users = users };
            string json = JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        // Root class to represent the entire JSON structure
        public class Root
        {
            public List<User> Users { get; set; }
        }
    }
}
