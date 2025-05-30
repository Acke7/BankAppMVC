﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer.DTOs.Account
{
    public class AccountDTO
    {

        public int AccountId { get; set; }
        public int AccountNumber { get; set; }
        public string Frequency { get; set; } = null!;
        public DateOnly Created { get; set; }
        public decimal Balance { get; set; }

        public bool IsActive { get; set; }
    }
}
