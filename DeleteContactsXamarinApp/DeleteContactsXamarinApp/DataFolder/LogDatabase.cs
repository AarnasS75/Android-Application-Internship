using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using HGB.Model;

namespace HGB.DataFolder
{
    public class LogDatabase
    {
        readonly SQLiteAsyncConnection database;

        public LogDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Logs>().Wait();
        }

        public Task<List<Logs>> GetLogsAsync()
        {
            //Get all notes.
            return database.Table<Logs>().ToListAsync();
        }

        public Task<Logs> GetLogAsync(int id)
        {
            // Get a specific note.
            return database.Table<Logs>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveLogAsync(Logs logs)
        {
            if (logs.ID != 0)
            {
                // Update an existing note.
                return database.UpdateAsync(logs);
            }
            else
            {
                // Save a new note.
                return database.InsertAsync(logs);
            }
        }

        public Task<int> DeleteLogAsync(Logs logs)
        {
            // Delete a note.
            return database.DeleteAsync(logs);
        }
    }
}