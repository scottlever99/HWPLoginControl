using HWPLoginControl.Data;
using HWPLoginControl.Data.Models;
using HWPLoginControl.Models;

namespace HWPLoginControl.Service
{
    public class AccountService
    {
        private readonly IDataAccess _dbAccess;

        public AccountService(IDataAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<bool> UserAlreadyExists(CreateAccount model)
        {
            try
            {
                var result = await _dbAccess.LoadData<User, dynamic>("SELECT * FROM user WHERE email = @Email", new { Email = model.Email });
                if (result == null || result.Count() < 1)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return true;
            }
        }

        public async Task<bool> CreateAccount(CreateAccount model)
        {
            try
            {
                var result = await _dbAccess.SaveData<dynamic>("INSERT INTO user (firstname, lastname, email, password, age) VALUES (@FirstName, @LastName, @Email, @Password, @Age)", 
                    new 
                    { 
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Password = model.Password,
                        Age = 0
                    });

                if (result == null || result < 1) return false;
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
