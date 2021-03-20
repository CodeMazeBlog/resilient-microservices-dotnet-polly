using System;
using System.Collections.Generic;
using System.Threading;
using Monolith.Models;

namespace Monolith.Data
{
    public class Repository
    {
        private IEnumerable<Author> _authors { get; set; }
        private bool _shouldFail = true;
        private DateTime _startTime = DateTime.UtcNow;

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

        public IEnumerable<Author> GetAuthors()
        {
            if (_shouldFail)
            {
                _shouldFail = false;
                throw new Exception("Oops!");
            }

            if (_startTime.AddMinutes(1) > DateTime.UtcNow)
            {
                Thread.Sleep(5000);
                throw new Exception("Timeout!");
            }

            return _authors;
        }
    }
}
