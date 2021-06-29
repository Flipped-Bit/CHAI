using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.IO;

namespace CHAI.Data
{
    /// <summary>
    /// Class for building <see cref="CHAIDbContext"/> from factory.
    /// </summary>
    public class CHAIDbContextFactory : IDesignTimeDbContextFactory<CHAIDbContext>
    {
        /// <summary>
        /// The path to <see cref="Environment.SpecialFolder.ApplicationData"/> folder.
        /// </summary>
        private static readonly string APPDATAFOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <inheritdoc/>
        public CHAIDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CHAIDbContext>();
            optionsBuilder.UseSqlite($"Data Source = {Path.Join(APPDATAFOLDER, "CHAI", "Main.db")}");

            return new CHAIDbContext(optionsBuilder.Options);
        }
    }
}
