using HWPLoginControl.Data;
using HWPLoginControl.Data.Models;
using HWPLoginControl.Models;

namespace HWPLoginControl.Services
{
    public class AccountService
    {
        private readonly IDataAccess _dbAccess;
        private readonly EncryptionService _encryptionService;

        public AccountService(IDataAccess dbAccess, EncryptionService encryptionService)
        {
            _dbAccess = dbAccess;
            _encryptionService = encryptionService;
        }

        public async Task<User?> GetUser(int id)
        {
            try
            {
                var result = (await _dbAccess.LoadData<User, dynamic>("SELECT * FROM user WHERE id = @Id", new { Id = id })).FirstOrDefault();
                return result;
            }
            catch
            {
                return new User();
            }
        }

        public async Task<User?> GetUser(string email)
        {
            try
            {
                var result = (await _dbAccess.LoadData<User, dynamic>("SELECT * FROM user WHERE email = @Email", new { Email = email })).FirstOrDefault();
                return result;
            }
            catch
            {
                return new User();
            }
        }

        public async Task<bool> EnableUser(int id, int gateway)
        {
            try
            {
                var result = await _dbAccess.SaveData("UPDATE user SET enabled = 1, gateway = @Gateway WHERE id = @Id", new { Id = id, Gateway = gateway });
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UserEnabled(int id)
        {
            try
            {
                var result = (await _dbAccess.LoadData<User, dynamic>("SELECT * FROM user WHERE id = @Id", new { Id = id })).FirstOrDefault();
                if (result == null) return false;
                if (result.enabled == 1) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UserAlreadyExists(string email)
        {
            try
            {
                var result = await _dbAccess.LoadData<User, dynamic>("SELECT * FROM user WHERE email = @Email", new { Email = email });
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
                var result = await _dbAccess.SaveData<dynamic>("INSERT INTO user (firstname, lastname, email, password) VALUES (@FirstName, @LastName, @Email, @Password)",
                    new
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Password = model.Password
                    });

                if (result < 1) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdatePassword(string email, string password)
        {
            try
            {
                var result = await _dbAccess.SaveData<dynamic>("UPDATE user SET password = @Password WHERE email = @Email",
                    new
                    {
                        Email = email,
                        Password = password
                    });

                if (result < 1) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetForgottenPasswordToken(string email)
        {
            var encyrptString = _encryptionService.Encrypt(email);

            return encyrptString;
        }

        public string GetEmailFromString(string cypherText)
        {
            var decryptString = _encryptionService.Decrypt(cypherText);

            return decryptString;
        }

        public bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
