# Government Pension Management System

A Windows Forms (.NET 4.8) desktop application for managing civil service pension records.

---

## Quick Start

### 1. Database Setup
1. Open **SQL Server Management Studio (SSMS)**.
2. Connect to your `.\SQLEXPRESS` instance.
3. Open and run `Data\DatabaseSetup.sql`.
4. This creates the `PensionDB` database with all required tables.

### 2. Connection String
If your SQL Server instance name is not `SQLEXPRESS`, edit `Data\DbAccess.cs`:
```
Data Source=.\SQLEXPRESS;Initial Catalog=PensionDB;Integrated Security=True
```
Change `SQLEXPRESS` to your instance name.

### 3. Build & Run
- Open `PensionMgmt.csproj` in **Visual Studio 2019 or 2022**.
- Build → Run (F5).

---

## Default Login

| Employee ID  | Password | Role         |
|--------------|----------|--------------|
| 1000000001   | 123456   | System Admin |

---

## User Registration & Approval Workflow

This system uses a **request-then-approve** model so all access is controlled.

### Step 1 – New User Registers
1. On the Login screen, click **REGISTER**.
2. Fill in: Employee ID (10 digits), Full Name, Requested Role, Password (6 digits).
3. Click **Submit Registration**.
4. A confirmation message says the request is pending.

### Step 2 – Admin Reviews
1. A **System Admin** or **Pension Admin** logs in.
2. Navigates to **User Control → Pending Registrations tab**.
3. Sees all pending requests with name, ID, role, and date.
4. Clicks **Approve Selected** or **Reject Selected**.

> **Important:** If someone requests the **System Admin** role, only an
> existing System Admin can approve that request. Pension Admins will
> see a "Permission Denied" message if they try.

### Step 3 – User Can Now Log In
- Once approved, the user's account is activated immediately.
- They log in using their Employee ID and Password from the Login screen.
- If still pending, the login screen tells them to wait for admin approval.

---

## Roles & Permissions

| Role             | Dashboard | Employees | Payouts | Pension Calc | Retirement | User Control | My Pension Info | My Payouts |
|------------------|-----------|-----------|---------|--------------|------------|--------------|-----------------|------------|
| System Admin     | Yes       | Yes       | Yes     | Yes          | Yes        | Yes (full)   | No              | No         |
| Pension Admin    | Yes       | Yes       | Yes     | Yes          | Yes        | Yes (approve)| No              | No         |
| Pension Manager  | Yes       | Yes       | Yes     | Yes          | Yes        | No           | No              | No         |
| Pension Holder   | Yes       | No        | No      | Yes (own)    | No         | No           | Yes             | Yes        |

### About the Pension Holder Role
The **Pension Holder** role replaces the old "Viewer" role with meaningful features:

- **My Pension Info** – Shows the employee record linked to this account and automatically calculates a pension estimate based on current salary, rank, and service years.
- **Pension Calc** – The general pension calculator for running custom what-if scenarios.
- **My Payouts** – Shows all payout records processed for this account's employee ID.

A Pension Holder's account is linked to an employee record by matching their Employee ID. The System Admin can also set the link manually from the `PensionHolder` database table.

---

## Multiple System Admins

Multiple System Admin accounts are fully supported:

- Any existing System Admin can **approve** a new System Admin registration request.
- Any existing System Admin can **manually add** a new System Admin via User Control → Active Users → Add User Manually.
- The last remaining System Admin **cannot be deleted** — the system prevents this to avoid lock-out.
- Only one person logs in at a time (standard Windows Forms session), but multiple accounts can exist.

---

## Project Structure

```
PensionMgmt/
├── Data/
│   ├── DatabaseSetup.sql          <- Run this first in SSMS
│   └── DbAccess.cs                <- All DB operations
├── Forms/
│   ├── LoginForm.cs/.Designer.cs  <- Login + Register button
│   ├── RegisterForm.cs            <- Self-registration (all roles including System Admin)
│   ├── MainForm.cs/.Designer.cs   <- Shell window with sidebar nav
│   ├── DashboardPanel.cs          <- Summary stats (role-aware)
│   ├── EmployeesPanel.cs          <- Employee CRUD
│   ├── PayoutsPanel.cs            <- Payout records (admin/manager)
│   ├── PensionCalcPanel.cs        <- Pension calculator
│   ├── RetirementPanel.cs         <- Retirement tracking
│   ├── PensionHolderPanel.cs      <- (NEW) Pension Holder's own pension info
│   ├── MyPayoutsPanel.cs          <- (NEW) Pension Holder's own payout history
│   └── UserControlPanel.cs        <- Pending approvals + active users
├── Services/
│   └── PensionCalculator.cs       <- Pension calculation logic
├── Session/
│   └── CurrentUser.cs             <- Session state
├── Program.cs
└── PensionMgmt.csproj
```

---

## Database Tables

| Table                 | Purpose                                                     |
|-----------------------|-------------------------------------------------------------|
| SystemAdmin           | System Admin login credentials                              |
| PensionAdmin          | Pension Admin login credentials                             |
| Manager               | Pension Manager login credentials                           |
| PensionHolder         | Pension Holder login credentials + linked EmployeeId        |
| Users                 | Master list of all approved users                           |
| PendingRegistrations  | Self-registration requests awaiting approval                |
| Employees             | Employee records                                            |
| Payouts               | Processed pension payouts                                   |

---

## Requirements
- Windows 7 or later
- .NET Framework 4.8
- SQL Server Express (any recent version)
- Visual Studio 2019 or 2022
