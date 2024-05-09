using SQLite;
using WorkoutHelper.Shared.Models;

namespace WorkoutHelper.Data
{
    public struct DatabaseConstants()
    {
        public const string DatabaseFilename = "WorkoutHelperSQLite.db3";
        public const SQLiteOpenFlags Flags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;
        public static string DatabasePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DatabaseFilename);
    }
    public class TableDB<T> where T : new()
    {
        protected static SQLiteAsyncConnection Database;

        public static readonly AsyncLazy<TableDB<T>> Instance = new(async () =>
        {
            var instance = new TableDB<T>();

            if (Database == null)
                throw new Exception("Database connection could not be established. Instance will not be returned.");

            CreateTableResult result = await Database.CreateTableAsync<T>();
            return instance;
        });

        public TableDB() => Database = new SQLiteAsyncConnection(DatabaseConstants.DatabasePath, DatabaseConstants.Flags);

        public async Task RecreateTableAsync()
        {
            await Database.DropTableAsync<T>();
            await Database.CreateTableAsync<T>();
        }

        public Task<List<T>> GetItemsAsync() => Database.Table<T>().ToListAsync();

        public Task<int> SaveItemAsync(T item) => Database.UpdateAsync(item);

        public Task<int> AddItemAsync(T item) => Database.InsertAsync(item);

        public Task<int> AddItemsAsync(IEnumerable<T> exercises) => Database.InsertAllAsync(exercises);

        public Task<int> DeleteItemAsync(T item) => Database.DeleteAsync(item);
    }

    //ID-able database
    public class IdTableDB<T> : TableDB<T> where T : IIDProperty, new()
    {
        public new static readonly AsyncLazy<IdTableDB<T>> Instance = new(async () =>
        {
            var instance = new IdTableDB<T>();

            if (Database == null)
                throw new Exception("Database connection could not be established. Instance will not be returned.");

            CreateTableResult result = await Database.CreateTableAsync<T>();
            return instance;
        });


        public Task<List<T>> GetItemsAsync(int id) => Database.Table<T>().Where(i => i.Id == id).ToListAsync();

        public async Task<int> SaveAddItemAsync(T item) => item.Id.HasValue
            && item.Id != -1
            && (await GetItemsAsync(item.Id.Value)).Any() 
            ? await SaveItemAsync(item) 
            : await AddItemAsync(item);
    }
}
