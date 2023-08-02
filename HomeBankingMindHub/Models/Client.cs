﻿using System.Collections;
using System.Collections.Generic;

namespace HomeBankingMindHub.Models
{
    public class Client
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public ICollection<Account> Accounts { get; set; }

        public ICollection <ClientLoan> ClientLoans { get; set; }
    }
}
