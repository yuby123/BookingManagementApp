﻿using API.Models;

namespace API.Contracts;

public interface IEmployeeRepository : IGeneralRepository<Employee>
{
    string? GetLastNik();
    Employee GetEmail(string email);
}




