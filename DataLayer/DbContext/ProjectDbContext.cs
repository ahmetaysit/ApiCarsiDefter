using Library.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer
{
    public class ProjectDbContext : DbContext
    {
        public int UniqueValue { get; set; }
        public ProjectDbContext(DbContextOptions options) : base(options)
        {
            var rand = new Random();
            UniqueValue = rand.Next(0,100);
        }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<CustomerAccount> CustomerAccount { get; set; }
        public virtual DbSet<CustomerAccountBalance> CustomerAccountBalance { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<TransactionRequest> TransactionRequest { get; set; }
        public virtual DbSet<TransactionRequestType> TransactionRequestType { get; set; }
        public virtual DbSet<TransactionRequestStatus> TransactionRequestStatus { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<TransactionTypes> TransactionTypes { get; set; }
        public virtual DbSet<CustomerGroup> CustomerGroup { get; set; }
        public virtual DbSet<SmsRequest> SmsRequest { get; set; }
        public virtual DbSet<PoolIncomeFromGroup> PoolIncomeFromGroup { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<CustomerRequest> CustomerRequest { get; set; }
    }
}
