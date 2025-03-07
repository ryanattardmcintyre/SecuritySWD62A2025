using SecuritySWD62A2025.Data;

namespace SecuritySWD62A2025.Repositories
{
    public class BaseRepository
    {
        //we are making use of consturctor injection to get an instance
        //of (the abstraction) of the database

        //What is ApplicationDbContext?
        //ApplicationDbContext is an abstraction of database
        //- it models the database
        //- it gives me the looks of the database, i.e. it compartmentalizes the data into lists reprsenting tables
        //- as a benefit it provides me methods which facilitates the management of data
        public ApplicationDbContext _dbContext { get; set; }
        public BaseRepository(ApplicationDbContext context) { 
        _dbContext = context;
        } 
    }
}
