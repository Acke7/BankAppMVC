# BankApp - User Guide

## Welcome to BankApp

BankApp is a secure internal system for bank staff to manage customers, accounts, and transactions.  
This system is for employees only, such as Admins and Cashiers, and not for customers.

---

## How to Use the Website

### Login credentials:

- **Admin**: `richard.chalk@admin.se` / `Abc123#`  
- **Cashier**: `richard.chalk@cashier.se` / `Abc123#`

> The **start page is public** – no login is needed.

---

## What You Can Do

### As a Cashier:
- Search for customers by name or city  
- View customer details and accounts  
- View transactions per account  
- Make deposits, withdrawals, or transfers  
- Add new customers (auto-creates an account)  

### As an Admin:
- All cashier features  
- Manage system users (Admins and Cashiers)  

---

## Start Page Overview

- Total number of customers  
- Total number of accounts  
- Total balance per country  

Clicking a country shows the **top 10 customers** by total balance.

---

## Suspicious Transaction Scanner (Console App)

The console app checks for suspicious activity using these rules:

1. Any transaction **over 15,000 kr**  
2. **Total transactions in 72 hours over 23,000 kr**

It creates a **report per country**.

---

## What Files Will Be Created?

**Folder structure:**  
`SuspiciousReports/YYYY-MM-DD_HH-MM-SS/`

**Example files:**
- `Sweden.txt`  
- `Norway.txt`  
- `Danmark.txt`  
- `Finland.txt`  

Each file includes entries like:

```
CustomerID: 10125
Account: 45872957
Transaction: 99801
Reason: Transaction over 15000 kr

CustomerID: 10399
Account: 48911340
Transaction: 100122
Reason: Total over 23000 kr in 72h
```

If no suspicious activity is found, the file will say:

> **No new suspicious transactions found for [Country]**

---

## Visit Our Website

- 🌐 [alolabi.site](https://alolabi.site)  
- 🌐 [BankApp on Azure](https://bankappazue-exbzcdavfhb2bdck.swedencentral-01.azurewebsites.net)
