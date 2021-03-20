using System.Collections.Generic;
using Monolith.Models;

namespace Monolith.Data
{
    public class Repository
    {
        private IEnumerable<Author> _authors { get; set; }

        public Repository()
        {
            _authors = new[]
            {
                new Author
                {
                    AuthorId = 1,
                    Name = "John Doe",
                    Country = "Australia"
                },
                new Author
                {
                    AuthorId = 2,
                    Name = "Jane Smith",
                    Country = "United States"
                }
            };
        }

        public IEnumerable<Author> GetAuthors() => _authors;
    }
}
