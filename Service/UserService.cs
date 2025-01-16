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
        private readonly string _expensesFilePath;
        private List<User> _users;
        private  UserData userData
            ;

        public UserService(UserData userData)
        {
            string userHomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _expensesFilePath = Path.Combine(userHomeDirectory, "expenses.json");
            this.userData = userData;
            LoadUsersFromJson();
        }

        private void EnsureTransactionsFileExists()
        {
            if (!File.Exists(_expensesFilePath))
            {
                File.WriteAllText(_expensesFilePath, "[]");
            }
        }

        // Load users and expenses from the JSON file
        private void LoadUsersFromJson()
        {
            if (File.Exists(_expensesFilePath))
            {
                var jsonData = File.ReadAllText(_expensesFilePath);
                _users = JsonSerializer.Deserialize<List<User>>(jsonData) ?? new List<User>();
                //foreach (var user in _users)
                //{
                //    if (user.Transactions == null)
                //    {
                //        user.Transactions = new List<Transaction>();
                //    }
                //    else if (user.Debts == null)
                //    {
                //        user.Debts = new List<Debt>();
                //    }
                //    else if (user.Tags == null)
                //    {

                //        user.Tags = new List<String>();
                //    }
                //    {

                //    }
                //}

                {

                }
            }
            else
            {
                _users = new List<User>();
                SaveUsersToJson(); // Create the file if it doesn't exist
            }
        }

        // Save users and expenses back to the JSON file
        public void SaveUsersToJson()
        {
            var jsonData = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_expensesFilePath, jsonData);
        }

        // Authenticate the user and return user details if login is successful
        public User? Login(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.UserName == username && u.Password == password);
            
            if (user != null)
            {
                userData.GetUser = user;
            }

            return user; // Return the user if authentication is successful, otherwise null
        }

        // Get expenses for the currently logged-in user
        public List<Transaction> GetUserExpenses()
        {
            var user = userData.GetUser;
            return user?.Transactions ?? new List<Transaction>();
        }

        // Add a new expense for the currently logged-in user and return true/false
        public bool AddExpenseForUser(Transaction expense)
        {
            var user = userData.GetUser;

            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            switch (expense.TransactionType)
            {
                case "Debit":
                    // Check if the total balance is sufficient to deduct the expense
                    if (user.AvailableBalance < expense.Amount)
                    {
                        return false; // Insufficient balance, return false
                    }
                    user.AvailableBalance -= expense.Amount; // Deduct the amount
                    break;

                case "Credit":
                case "Debt":
                    user.AvailableBalance += expense.Amount; // Add the amount
                    break;

                default:
                    throw new ArgumentException("Invalid expense type");
            }

            // Assign a unique ID to the expense
            expense.Id = user.Transactions.Any() ? user.Transactions.Max(e => e.Id) + 1 : 1;

            user.Transactions.Add(expense); // Add the expense to the user's list
            SaveUsersToJson(); // Persist changes

            return true; // Expense added successfully, return true
        }

        // Get expenses by tag for the currently logged-in user
        public List<Transaction> GetExpensesByTag(string tag)
        {
            var user = userData.GetUser;
            return user?.Transactions.Where(e => e.ExpenseTag == tag).ToList() ?? new List<Transaction>();
        }

        public List<string> GetAllTags()
        {
            var user = userData.GetUser;
            List<string> list = userData.GetUser.Tags.ToList();
            return list;
        }

        public User? GetCurrentUser()
        {
            return userData.GetUser;
        }

        // Add a new user (e.g., during registration)
        public void AddUser(User newUser)
        {
            if (_users.Any(u => u.UserName == newUser.UserName))
            {
                throw new Exception("User already exists");
            }

            newUser.Transactions = new List<Transaction>();
            _users.Add(newUser);
            SaveUsersToJson();
        }

        // Get all users (if needed)
        public List<User> GetAllUsers()
        {
            return _users;
        }

        public bool AddTagToUser(string tag)
        {
            var user = userData.GetUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            if (user.Tags.Contains(tag))
            {
                return false; // Tag already exists
            }

            user.Tags.Add(tag);
            SaveUsersToJson(); // Persist the updated tags
            return true;
        }

        //public List<Debt> GetAllDebtsForUser()
        //{
        //    var user = _userContext.CurrentUser;
        //    return user?.Debts ?? new List<Debt>();
        //}

        //public bool AddDebtForUser(Debt debt)
        //{
        //    var user = _userContext.CurrentUser;

        //    if (user == null)
        //    {
        //        throw new Exception("No user is logged in");
        //    }

        //    try
        //    {
        //        // Check for sufficient balance before adding the debt
        //        if (user.TotalBalance < debt.Amount)
        //        {
        //            throw new Exception("Insufficient balance to add debt");
        //        }
        //        // Assign a unique ID to the debt using Guid for uniqueness across sessions
        //        //debt.Id = user.Debts.Any() ? user.Debts.Max(d => d.Id) + 1 : 1;
        //        // Add the debt to the user's list
        //        List<Debt> debts = user.Debts;
        //        Console.WriteLine(user.Debts);
        //        user.Debts.Add(debt);

        //        // Update the user's balance based on the debt amount
        //        user.TotalBalance -= debt.Amount;

        //        // Persist the changes to the data storage
        //        SaveUsersToJson(); // Ensure this persists data correctly

        //        return true; // Debt added successfully
        //    }
        //    catch (Exception e)
        //    {
        //        // Log error for debugging and troubleshooting
        //        // Ideally, use a logger here instead of Console.WriteLine
        //        Console.WriteLine($"Error adding debt: {e.Message}");
        //        return false;
        //    }
        //}

        //public bool MarkDebtAsPaid(int debtId)
        //{
        //    var user = userData.GetUser;

        //    if (user == null)
        //    {
        //        throw new Exception("No user is logged in");
        //    }

        //    var debt = user.Debts.FirstOrDefault(d => d.Id == debtId);
        //    if (debt == null)
        //    {
        //        throw new Exception("Debt not found");
        //    }

        //    if (debt.Paid)
        //    {
        //        return false; // Debt is already marked as paid
        //    }

        //    // Mark the debt as paid
        //    debt.Paid = true;

        //    // Deduct the debt amount from the user's balance after clearing the debt
        //    user.AvailableBalance -= debt.Amount;

        //    // Persist changes to the JSON file
        //    SaveUsersToJson(); // Save the updated list of debts and user balance to JSON

        //    return true; // Debt successfully marked as paid
        //}

        // Method to calculate total inflow (credit)
        public double GetTotalInflow()
        {
            var user = userData.GetUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            return user.Transactions.Where(e => e.TransactionType == "Credit").Sum(e => e.Amount);
        }

        // Method to calculate total outflow (debit)
        public double GetTotalOutflow()
        {
            var user = userData.GetUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            return user.Transactions.Where(e => e.TransactionType == "Debit").Sum(e => e.Amount);
        }

        public int GetRemaingBalance()

        {
            return userData.GetUser.AvailableBalance;
        }


        // Method to get the total number of inflows (credits)
        public int GetTotalNumberOfInflows()
        {
            var user = userData.GetUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            return user.Transactions.Count(e => e.TransactionType == "Credit");
        }

        // Method to get the total number of outflows (debits)
        public int GetTotalNumberOfOutflows()
        {
            var user = userData.GetUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            return user.Transactions.Count(e => e.TransactionType == "Debit");
        }

        // Method to calculate the total expenses (inflows + outflows)
        public double GetTotalExpenses()
        {
            var user = userData.GetUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            return user.Transactions.Sum(e => e.Amount); // Sum of both credits and debits
        }
    }

    }
