using Interfaces.BusinessIntefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Dto;
using Library.Enums;
using Library.Entities;
using Interfaces.DALINterfaces;
using BusinessServices.Repository;
using Library.Requests;
using ClosedXML.Excel;
using System.IO;
using Interfaces.BusinessInterfaces;
using System.Net.Mail;

namespace BusinessServices
{
    public class CustomerAccountBs : GenericRepositoryBs<ICustomerAccountDAL, CustomerAccount>, ICustomerAccountBS
    {
        private ITransactionBS _transactionBs;
        private ITransactionDAL _transactionDal;
        private ICustomerAccountDAL _customerAccountDal;
        private ICurrencyDAL _currencyDal;
        private ICustomerDAL _customerDal;
        private ISettingsDAL _settingsDal;
        private ISmsRequestDAL _smsRequestDal;
        private List<int?> incomeTypes = new List<int?> { 1, 3 };
        private readonly IEmailSenderBS _emailSenderBS;
        private int colIndex = 2;
        public CustomerAccountBs(ITransactionBS transactionBs,
            ITransactionDAL transactionDal,
            ICustomerAccountDAL customerAccountDal,
            ICurrencyDAL currencyDal,
            ICustomerDAL customerDal,
            ISettingsDAL settingsDal,
            ISmsRequestDAL smsRequestDal,
            IEmailSenderBS emailSenderBS) : base(customerAccountDal)
        {
            _transactionBs = transactionBs;
            _transactionDal = transactionDal;
            _customerAccountDal = customerAccountDal;
            _currencyDal = currencyDal;
            _customerDal = customerDal;
            _settingsDal = settingsDal;
            _smsRequestDal = smsRequestDal;
            _emailSenderBS = emailSenderBS;
        }
        public bool ExchangeTransfer(CustomerExchangeTransferDto customerAccountDto)
        {
            bool result = false;
            Transaction transaction = new Transaction();
            if (customerAccountDto.TransactionType == ExchangeConversionType.Cikis)
            {
                //çıkışsa çıkış yapıp o ayki işlemlerin %'lik bölümünü havuza at
                result = MoneyOut(customerAccountDto);
                MailMessage mail = CreateMailMessageForMoneyInOut(customerAccountDto);
                _emailSenderBS.SendEmail(mail);
            }
            else if (customerAccountDto.TransactionType == ExchangeConversionType.Giris)
            {
                //girişse yeni hesap oluştur
                result = MoneyIn(customerAccountDto);
                MailMessage mail = CreateMailMessageForMoneyInOut(customerAccountDto);
                _emailSenderBS.SendEmail(mail);
            }
            else if(customerAccountDto.TransactionType == ExchangeConversionType.Transfer)
            {
                //transferse çıkış giriş yap
                result = MoneyOut(customerAccountDto) && MoneyIn(customerAccountDto);
                MailMessage mail = CreateMailMessageForMoneyTransfer(customerAccountDto);
                _emailSenderBS.SendEmail(mail);
            }
            else if (customerAccountDto.TransactionType == ExchangeConversionType.KurluGiris)
            {
                var acc = _customerAccountDal.GetByFilter(x => x.IsActive == true && x.Currency == customerAccountDto.Currency && x.CustomerId == customerAccountDto.CustomerId).FirstOrDefault();
                var toAcc = _customerAccountDal.GetByFilter(x => x.Id == customerAccountDto.ToAccountId).FirstOrDefault();
                var currencies = _currencyDal.GetAllCurrencies();
                Currency fromCurrency = currencies.FirstOrDefault(x => x.Id == acc.Currency);
                Currency toCurrency = currencies.FirstOrDefault(x => x.Id == toAcc.Currency);
                customerAccountDto.Description += $"Kurlu Giriş :{customerAccountDto.Amount} {fromCurrency.CurrencyCode} * {customerAccountDto.CurrencySellingRate}/{customerAccountDto.CurrencyBuyingRate} = {(customerAccountDto.Amount / customerAccountDto.CurrencyBuyingRate * customerAccountDto.CurrencySellingRate).ToString("0.##")} {toCurrency.CurrencyCode}";
                var tmpDto = new CustomerExchangeTransferDto
                {
                    Amount = customerAccountDto.Amount,
                    TransactionType = ExchangeConversionType.Giris,
                    CurrencyBuyingRate = customerAccountDto.CurrencyBuyingRate,
                    CurrencySellingRate = customerAccountDto.CurrencySellingRate,
                    CustomerId = customerAccountDto.CustomerId,
                    Description = customerAccountDto.Description,
                    CurrencyRate = customerAccountDto.CurrencyRate,
                    FromAccountId = customerAccountDto.FromAccountId,
                    ToAccountId = acc.Id,
                    TransactionDate = customerAccountDto.TransactionDate,
                    UserName = customerAccountDto.UserName,
                    IsEndOfMonth = customerAccountDto.IsEndOfMonth
                };
                int tmpAccountId = MoneyInWithAccountId(tmpDto);
                customerAccountDto.FromAccountId = tmpAccountId;
                result = MoneyOut(customerAccountDto) && MoneyIn(customerAccountDto);
            }
            else if (customerAccountDto.TransactionType == ExchangeConversionType.KurluCikis)
            {
                var outAcc = _customerAccountDal.GetByFilter(x => x.IsActive == true && x.Currency == customerAccountDto.Currency && x.CustomerId == customerAccountDto.CustomerId).FirstOrDefault();
                var toAcc = _customerAccountDal.GetByFilter(x => x.Id == customerAccountDto.FromAccountId).FirstOrDefault();
                var currencies = _currencyDal.GetAllCurrencies();
                Currency fromCurrency = currencies.FirstOrDefault(x => x.Id == outAcc.Currency);
                Currency toCurrency = currencies.FirstOrDefault(x => x.Id == toAcc.Currency);
                

                var amount = Math.Round(customerAccountDto.Amount / customerAccountDto.CurrencyBuyingRate * customerAccountDto.CurrencySellingRate,2);
                customerAccountDto.Description += $"Kurlu Çıkış :{amount} {toCurrency.CurrencyCode} * {customerAccountDto.CurrencyBuyingRate}/{customerAccountDto.CurrencySellingRate} = {customerAccountDto.Amount} {fromCurrency.CurrencyCode}";
                var dto = new CustomerExchangeTransferDto
                {
                    FromAccountId = toAcc.Id,
                    ToAccountId = outAcc.Id,
                    Amount = amount,
                    Currency = customerAccountDto.Currency,
                    CurrencyBuyingRate = customerAccountDto.CurrencyBuyingRate,
                    CurrencySellingRate = customerAccountDto.CurrencySellingRate,
                    CurrencyRate = 1,
                    CustomerId = customerAccountDto.CustomerId,
                    Description = customerAccountDto.Description,
                    IsEndOfMonth = false,
                    TransactionDate = customerAccountDto.TransactionDate,
                    UserName = customerAccountDto.UserName
                };
                result = MoneyOut(dto);
                var moneyInDto = new CustomerExchangeTransferDto
                {
                    FromAccountId = toAcc.Id,
                    ToAccountId = outAcc.Id,
                    Amount = amount,
                    Currency = customerAccountDto.Currency,
                    CurrencyBuyingRate = customerAccountDto.CurrencySellingRate,
                    CurrencySellingRate = customerAccountDto.CurrencyBuyingRate,
                    CurrencyRate = 1,
                    CustomerId = customerAccountDto.CustomerId,
                    Description = customerAccountDto.Description,
                    IsEndOfMonth = false,
                    TransactionDate = customerAccountDto.TransactionDate,
                    UserName = customerAccountDto.UserName
                };
                result = result && MoneyIn(moneyInDto);
                var moneyOutDto = new CustomerExchangeTransferDto
                {
                    FromAccountId = dto.ToAccountId,
                    ToAccountId = outAcc.Id,
                    Amount = customerAccountDto.Amount,
                    Currency = customerAccountDto.Currency,
                    CurrencyBuyingRate = customerAccountDto.CurrencyBuyingRate,
                    CurrencySellingRate = customerAccountDto.CurrencySellingRate,
                    CurrencyRate = 1,
                    CustomerId = customerAccountDto.CustomerId,
                    Description = customerAccountDto.Description,
                    IsEndOfMonth = false,
                    TransactionDate = customerAccountDto.TransactionDate,
                    UserName = customerAccountDto.UserName
                };
                result = result && MoneyOut(moneyOutDto);
            }
            return result;
        }
        private MailMessage CreateMailMessageForMoneyTransfer(CustomerExchangeTransferDto request)
        {
            var customer = _customerDal.Get(request.CustomerId);
            MailMessage eMail = new MailMessage();
            StringBuilder sb = new StringBuilder();

            var currencies = _currencyDal.GetAllCurrencies();

            CustomerAccount custAccountIn = _transactionDal.GetAccount(request.ToAccountId);
            CustomerAccount custAccountOut = _transactionDal.GetAccount(request.FromAccountId);
            string currencyIn = currencies.FirstOrDefault(x => x.Id == custAccountIn.Currency).CurrencyCode;
            string currencyOut = currencies.FirstOrDefault(x => x.Id == custAccountOut.Currency).CurrencyCode;
            sb.AppendLine($"{customer.CustomerName} {request.Amount} {currencyOut} X {request.CurrencySellingRate}/{request.CurrencyBuyingRate} = {request.Amount / request.CurrencyBuyingRate * request.CurrencySellingRate} {currencyIn} Döndük");
            sb.AppendLine($"Tarih = {request.TransactionDate.ToShortDateString()}");

            eMail.Subject = $"Transfer işlemi.";
            eMail.Body = sb.ToString();
            return eMail;
        }
        private MailMessage CreateMailMessageForMoneyInOut(CustomerExchangeTransferDto request)
        {
            var customer = _customerDal.Get(request.CustomerId);
            MailMessage eMail = new MailMessage();
            StringBuilder sb = new StringBuilder();
            string tranType = request.TransactionType == ExchangeConversionType.Cikis ? "Çıktı" : "Geldi";
            var currencies = _currencyDal.GetAllCurrencies();
            int accountId = request.TransactionType == ExchangeConversionType.Cikis ? request.FromAccountId : request.ToAccountId;
            CustomerAccount custAccount = _transactionDal.GetAccount(accountId);
            string currency = currencies.FirstOrDefault(x => x.Id == custAccount.Currency).CurrencyCode;
            sb.AppendLine($"{customer.CustomerName} {request.Amount} {currency} {tranType}");
            sb.AppendLine($"Tarih = {request.TransactionDate.ToShortDateString()}");

            eMail.Subject = $"{tranType} işlemi.";
            eMail.Body = sb.ToString();
            return eMail;
        }

        private bool MoneyOut(CustomerExchangeTransferDto customerAccountDto)
        {
            var custAcc = _customerAccountDal.GetBalance(customerAccountDto.FromAccountId);
            var customer = _customerDal.Get(customerAccountDto.CustomerId);
            CustomerAccount custAccount = _transactionDal.GetAccount(customerAccountDto.FromAccountId);
            int poolCustomerId = Convert.ToInt32(_settingsDal.Get("PoolCustomer").Settingvalue);
            var poolCustomers = _customerDal.GetCustomerAccounts(poolCustomerId).ToList();
            var alltransactions = _transactionDal.GetByFilter(x => x.TransactionDate <= customerAccountDto.TransactionDate && x.CustomerAccountId == customerAccountDto.FromAccountId && x.IsProcessed == false).ToList();

            _transactionBs.TakeProfitComission(customerAccountDto, customer, custAccount, poolCustomerId, poolCustomers, alltransactions, false);
            _transactionDal.Save();
            custAcc = _customerAccountDal.GetBalance(customerAccountDto.FromAccountId);
            var lastBalance = custAcc.AccountBalance;

            decimal? transactionAmount = (customerAccountDto.Amount > lastBalance && !customer.IsJustForBalance) ? lastBalance : customerAccountDto.Amount;
            customerAccountDto.Amount = (decimal)transactionAmount;
            Transaction tran = new Transaction
            {
                CustomerId = customerAccountDto.CustomerId,
                CustomerAccountId = customerAccountDto.FromAccountId,
                TransactionType = (int)TransactionType.ManuelOutCome,
                TransactionAmount = transactionAmount,
                TransactionDate = customerAccountDto.TransactionDate,
                IsProcessed = true,
                CurrencyId = custAccount.Currency,
                CurrencyRate = customerAccountDto.CurrencyRate,
                BuyingRate = customerAccountDto.CurrencyBuyingRate,
                SellingRate = customerAccountDto.CurrencySellingRate,
                CreationDate = DateTime.Now,
                IsShopProfitTaken = false,
                BalanceBeforeTransaction = custAcc.AccountBalance,
                Description = customerAccountDto.Description
            };

            _transactionDal.Add(tran);

            return true;
        }

        private decimal MoneyOutReturnBalance(CustomerExchangeTransferDto customerAccountDto)
        {
            var custAcc = _customerAccountDal.GetBalance(customerAccountDto.FromAccountId);
            var customer = _customerDal.Get(customerAccountDto.CustomerId);
            CustomerAccount custAccount = _transactionDal.GetAccount(customerAccountDto.FromAccountId);
            int poolCustomerId = Convert.ToInt32(_settingsDal.Get("PoolCustomer").Settingvalue);
            var poolCustomers = _customerDal.GetCustomerAccounts(poolCustomerId).ToList();
            var alltransactions = _transactionDal.GetByFilter(x => x.TransactionDate <= customerAccountDto.TransactionDate && x.CustomerAccountId == customerAccountDto.FromAccountId && x.IsProcessed == false).ToList();

            _transactionBs.TakeProfitComission(customerAccountDto, customer, custAccount, poolCustomerId, poolCustomers, alltransactions, false);
            _transactionDal.Save();
            custAcc = _customerAccountDal.GetBalance(customerAccountDto.FromAccountId);
            var lastBalance = custAcc.AccountBalance;

            decimal? transactionAmount = (customerAccountDto.Amount > lastBalance && !customer.IsJustForBalance) ? lastBalance : customerAccountDto.Amount;
            customerAccountDto.Amount = (decimal)transactionAmount;
            Transaction tran = new Transaction
            {
                CustomerId = customerAccountDto.CustomerId,
                CustomerAccountId = customerAccountDto.FromAccountId,
                TransactionType = (int)TransactionType.ManuelOutCome,
                TransactionAmount = transactionAmount,
                TransactionDate = customerAccountDto.TransactionDate,
                IsProcessed = true,
                CurrencyRate = customerAccountDto.CurrencyRate,
                BuyingRate = customerAccountDto.CurrencyBuyingRate,
                SellingRate = customerAccountDto.CurrencySellingRate,
                CreationDate = DateTime.Now,
                IsShopProfitTaken = false,
                BalanceBeforeTransaction = custAcc.AccountBalance
            };

            _transactionDal.Add(tran);

            return transactionAmount ?? 0;
        }

        public bool MakeMonthEndProcess(DateTime endOfMonth)
        {
            List<Customer> customers = _customerDal.GetCustomers().ToList();
            int poolCustomerId = Convert.ToInt32(_settingsDal.Get("PoolCustomer").Settingvalue);
            var poolCustomers = _customerDal.GetCustomerAccounts(poolCustomerId).ToList();
            var alltransactions = _transactionDal.GetByFilter(x => x.TransactionDate <= endOfMonth.Date && x.IsProcessed == false).ToList();
            var allAccounts = _customerAccountDal.GetByFilter(x => x.IsActive == true).ToList();
            var allAccountsDto = _customerDal.GetAllCustomerAccounts();
            var currencies = _currencyDal.GetAllCurrencies();
            Dictionary<Transaction, List<Transaction>> dic = new Dictionary<Transaction, List<Transaction>>();
            foreach (Customer cust in customers)
            {
                try
                {
                    var accounts = allAccounts.Where(x => x.CustomerId == cust.Id).ToList();

                    foreach (CustomerAccount account in accounts)
                    {
                        CustomerExchangeTransferDto customerAccountDto = new CustomerExchangeTransferDto();
                        CustomerAccount custAcc = account;
                        customerAccountDto.CustomerId = cust.Id;
                        customerAccountDto.FromAccountId = account.Id;
                        customerAccountDto.TransactionDate = endOfMonth;
                        customerAccountDto.IsEndOfMonth = true;

                        _transactionBs.TakeProfitComission(customerAccountDto, cust, custAcc, poolCustomerId, poolCustomers, alltransactions, true, dic);
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            _smsRequestDal.Save();

            var newAllAccountsDto1 = _customerDal.GetAllCustomerAccounts();

            foreach (Customer cust in customers)
            {
                MergeAccounts(cust, endOfMonth, currencies, newAllAccountsDto1);
            }

            _smsRequestDal.Save();

            foreach (var basetran in dic)
            {
                foreach (Transaction tran in basetran.Value)
                {
                    tran.IsProcessed = true;
                    tran.ProcessTransactionId = basetran.Key.Id;
                    _transactionDal.Update(tran, true);
                }

            }
            _smsRequestDal.Save();

            DateTime startDate = new DateTime(endOfMonth.Year, endOfMonth.Month, 1);
            var smsTransactions = _transactionDal.GetByFilter(x => x.TransactionDate >= startDate && x.TransactionDate <= endOfMonth).ToList();
            var newAllAccountsDto = _customerDal.GetAllCustomerAccounts();
            foreach (Customer cust in customers)
            {
                var accounts = newAllAccountsDto.Where(x => x.CustomerId == cust.Id).ToList<CustomerAccountDto>();
                CreateSmsRequest(endOfMonth, cust, accounts, smsTransactions);
            }
            _smsRequestDal.Save();
            return true;
        }

        private void CreateSmsRequest(DateTime endOfMonth, Customer cust, List<CustomerAccountDto> accounts, List<Transaction> smsTransactions)
        {
            SmsRequest smsRequest = new SmsRequest
            {
                CustomerId = cust.Id,
                PhoneNumber = cust.PhoneNumber.HasValue ? (long)cust.PhoneNumber : 0,
                RequestId = 0,
                ResponseCode = "",
                CreationDate = endOfMonth,
                SmsText = string.Format("Selamın Aleykum {0} . Ay Sonu Kârınız: ", cust.CustomerName)
            };

            foreach (CustomerAccountDto item in accounts)
            {
                if (item.AccountBalance > 0)
                {
                    DateTime startDate = new DateTime(endOfMonth.Year, endOfMonth.Month, 1);
                    var custIdList = accounts.Where(x => x.Currency == item.Currency).Select(x => (int?)x.Id).ToList();

                    List<Transaction> allTransactions = smsTransactions.Where(x => x.CustomerId == cust.Id && x.CurrencyId == item.Currency && x.TransactionDate >= startDate && x.TransactionDate <= endOfMonth).ToList();
                    decimal profit = allTransactions.Where(x => x.TransactionType == 1 || x.TransactionType == 3).Sum(x => x.TransactionAmount).Value -
                       allTransactions.Where(x => x.TransactionType == 11).Sum(x => x.TransactionAmount).Value;
                    smsRequest.SmsText += "\n " + item.AccountNo + " Kâr :" + String.Format("{0:0.00}", profit) + "Son durum bakiyeniz :" + item.AccountBalance;
                }

            }
            _smsRequestDal.Insert(smsRequest, true);
        }

        private bool MergeAccounts(Customer cust, DateTime endOfMonth, List<Currency> currencies, List<CustomerAccountDtoEx> allAccounts)
        {
            List<CustomerAccountDtoEx> accounts = allAccounts.Where(x => x.CustomerId == cust.Id).ToList();

            foreach (Currency item in currencies)
            {
                var currencyAccounts = accounts.Where(x => x.Currency == item.Id).ToList();
                if (currencyAccounts.Count > 1)
                {
                    CustomerAccountDtoEx mainAccount = currencyAccounts.First();
                    List<CustomerAccountDtoEx> sameCurrencyWithMainAccount = currencyAccounts.Where(x => x.Id != mainAccount.Id).ToList();
                    foreach (CustomerAccountDtoEx custAcc in sameCurrencyWithMainAccount)
                    {
                        decimal amount = (decimal)custAcc.AccountBalance;

                        Transaction tran = new Transaction
                        {
                            CustomerId = cust.Id,
                            CustomerAccountId = custAcc.Id,
                            TransactionType = (int)TransactionType.ManuelOutCome,
                            TransactionAmount = amount,
                            TransactionDate = endOfMonth,
                            IsProcessed = true,
                            CurrencyRate = 1,
                            CurrencyId = item.Id,
                            BuyingRate = 1,
                            SellingRate = 1,
                            CreationDate = DateTime.Now,
                            IsShopProfitTaken = true,
                            BalanceBeforeTransaction = custAcc.AccountBalance,
                            Description = "Hesap Birlestirme"
                        };

                        _transactionDal.Add(tran, true);

                        _customerAccountDal.DeleteAccount(custAcc.Id, true);

                        Transaction tranIn = new Transaction
                        {
                            CustomerId = cust.Id,
                            CustomerAccountId = mainAccount.Id,
                            TransactionType = (int)TransactionType.ManuelIncome,
                            TransactionAmount = amount,
                            TransactionDate = endOfMonth,
                            CurrencyId = item.Id,
                            IsProcessed = true,
                            CurrencyRate = 1,
                            BuyingRate = 1,
                            SellingRate = 1,
                            CreationDate = DateTime.Now,
                            IsShopProfitTaken = true,
                            BalanceBeforeTransaction = custAcc.AccountBalance,
                            Description = "Hesap Birlestirme"
                        };

                        _transactionDal.Add(tranIn, false);

                    }

                }
            }

            return true;
        }

        private bool MoneyIn(CustomerExchangeTransferDto customerAccountDto)
        {
            CustomerAccount account = _transactionDal.GetAccount(customerAccountDto.ToAccountId);
            Currency currency = _currencyDal.GetAllCurrencies().Where(x => x.Id == account.Currency).FirstOrDefault();
            CustomerAccount customer = new CustomerAccount { AccountNo = currency.CurrencyCode + " Hesap Yeni", Currency = account.Currency, CustomerId = customerAccountDto.CustomerId, IsActive = true };

            decimal amount = customerAccountDto.Amount / customerAccountDto.CurrencyBuyingRate * customerAccountDto.CurrencySellingRate;

            var count = _transactionDal.GetAllUnProccessedByDate(customerAccountDto.ToAccountId, customerAccountDto.TransactionDate).Count;

            int toAccountId = customerAccountDto.ToAccountId;


            if (count > 0)
            {
                //CustomerAccountDto customerAccount = _customerDal.GetCustomerAccounts(customerAccountDto.CustomerId).FirstOrDefault(x => x.Id == customerAccountDto.ToAccountId);
                //var outDto = new CustomerExchangeTransferDto
                //{
                //    FromAccountId = customerAccountDto.ToAccountId,
                //    Amount = customerAccount.AccountBalance ?? 0,
                //    TransactionDate = customerAccountDto.TransactionDate,
                //    CurrencyRate = 1,
                //    CurrencyBuyingRate = 1,
                //    CurrencySellingRate = 1,
                //    CustomerId = customerAccountDto.CustomerId
                //};
                //var amountOut = MoneyOutReturnBalance(outDto);
                //amount += amountOut;
                var newAcc = _customerAccountDal.CreateNewWithBalance(customer, 0);
                toAccountId = newAcc.Id;
            }

            Transaction tran = new Transaction
            {
                CustomerId = customerAccountDto.CustomerId,
                CustomerAccountId = toAccountId,
                CurrencyId = currency.Id,
                TransactionType = (int)TransactionType.ManuelIncome,
                TransactionAmount = amount,
                TransactionDate = customerAccountDto.TransactionDate,
                IsProcessed = true,
                CurrencyRate = customerAccountDto.CurrencyRate,
                BuyingRate = customerAccountDto.CurrencyBuyingRate,
                SellingRate = customerAccountDto.CurrencySellingRate,
                CreationDate = DateTime.Now,
                IsShopProfitTaken = false,
                Description = customerAccountDto.Description
            };

            _transactionDal.Add(tran);

            return true;
        }

        private int MoneyInWithAccountId(CustomerExchangeTransferDto customerAccountDto)
        {
            CustomerAccount account = _transactionDal.GetAccount(customerAccountDto.ToAccountId);
            Currency currency = _currencyDal.GetAllCurrencies().Where(x => x.Id == account.Currency).FirstOrDefault();
            CustomerAccount customer = new CustomerAccount { AccountNo = currency.CurrencyCode + " Hesap Yeni", Currency = account.Currency, CustomerId = customerAccountDto.CustomerId, IsActive = true };

            decimal amount = customerAccountDto.Amount;// / customerAccountDto.CurrencyBuyingRate * customerAccountDto.CurrencySellingRate;

            var count = _transactionDal.GetAllUnProccessedByDate(customerAccountDto.ToAccountId, customerAccountDto.TransactionDate).Count;

            int toAccountId = customerAccountDto.ToAccountId;


            if (count > 0)
            {
                //CustomerAccountDto customerAccount = _customerDal.GetCustomerAccounts(customerAccountDto.CustomerId).FirstOrDefault(x => x.Id == customerAccountDto.ToAccountId);
                //var outDto = new CustomerExchangeTransferDto
                //{
                //    FromAccountId = customerAccountDto.ToAccountId,
                //    Amount = customerAccount.AccountBalance ?? 0,
                //    TransactionDate = customerAccountDto.TransactionDate,
                //    CurrencyRate = 1,
                //    CurrencyBuyingRate = 1,
                //    CurrencySellingRate = 1,
                //    CustomerId = customerAccountDto.CustomerId
                //};
                //var amountOut = MoneyOutReturnBalance(outDto);
                //amount += amountOut;
                var newAcc = _customerAccountDal.CreateNewWithBalance(customer, 0);
                toAccountId = newAcc.Id;
            }

            Transaction tran = new Transaction
            {
                CustomerId = customerAccountDto.CustomerId,
                CustomerAccountId = toAccountId,
                CurrencyId = currency.Id,
                TransactionType = (int)TransactionType.ManuelIncome,
                TransactionAmount = amount,
                TransactionDate = customerAccountDto.TransactionDate,
                IsProcessed = true,
                CurrencyRate = customerAccountDto.CurrencyRate,
                BuyingRate = customerAccountDto.CurrencyBuyingRate,
                SellingRate = customerAccountDto.CurrencySellingRate,
                CreationDate = DateTime.Now,
                IsShopProfitTaken = false,
                Description = customerAccountDto.Description
            };

            _transactionDal.Add(tran);

            return toAccountId;
        }

        public void ResetAllAccountBalances()
        {
            _customerAccountDal.ResetAllAccountBalances();
        }

        public CustomerAccount CreateNewWithBalance(CustomerAccount customerAccount, decimal amount)
        {
            return _customerAccountDal.CreateNewWithBalance(customerAccount, amount);
        }

        public void DeleteAllAccounts()
        {
            _customerAccountDal.DeleteAllAccounts();
        }

        public object GenerateEndOfMonthExcel(GenerateEndOfMonthExcelRequest request)
        {
            System.IO.Stream stream = new System.IO.MemoryStream();
            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Tüm Kisiler");

            var customers = _customerDal.GetCustomers();
            var groupIds = customers.Select(x => x.CustomerGroupId).Distinct().ToList();

            var groups = _customerDal.GetCustomerGroups().Where(x => groupIds.Contains(x.Id));

            var allTransactions = _transactionBs.GetByFilter(x => x.TransactionDate >= request.StartDate
            && x.TransactionDate <= request.EndDate);
            var allAccounts = _customerDal.GetAllCustomerAccounts();
            int index = 2;

            foreach (var item in customers)
            {
                GenerateTitles(ws, index);
                colIndex = 2;
                GenerateCustomerCurrencyBasedBalanceXLWorkBook(ws, allTransactions, index, item, 1, "TL", GetColNameFromIndex(colIndex), allAccounts, 0);

                GenerateCustomerCurrencyBasedBalanceXLWorkBook(ws, allTransactions, index, item, 2, "USD", GetColNameFromIndex(colIndex), allAccounts, 0);

                GenerateCustomerCurrencyBasedBalanceXLWorkBook(ws, allTransactions, index, item, 3, "EUR", GetColNameFromIndex(colIndex), allAccounts, 0);

                GenerateCustomerCurrencyBasedBalanceXLWorkBook(ws, allTransactions, index, item, 4, "GAU", GetColNameFromIndex(colIndex), allAccounts, 2);

                GenerateCustomerCurrencyBasedBalanceXLWorkBook(ws, allTransactions, index, item, 5, "GBP", GetColNameFromIndex(colIndex), allAccounts, 0);

                GenerateCustomerCurrencyBasedBalanceXLWorkBook(ws, allTransactions, index, item, 6, "CHF", GetColNameFromIndex(colIndex), allAccounts, 0);

                index += 8;
            }

            GenerateForGroup(workbook, customers, groups, allTransactions, allAccounts, index);

            workbook.SaveAs(stream);

            MemoryStream ms = (MemoryStream)stream;
            byte[] imageBytes = ms.ToArray();
            stream.CopyTo(ms);
            var result = ms.ToArray();

            return result;
        }

        private void GenerateForGroup(XLWorkbook workbook, List<Customer> customers, IEnumerable<CustomerGroup> groups, IEnumerable<Transaction> allTransactions, List<CustomerAccountDtoEx> allAccounts, int index)
        {
            foreach (var group in groups)
            {
                var ws1 = workbook.Worksheets.Add(group.GroupName);
                index = 2;

                foreach (var item in customers.Where(x => x.CustomerGroupId == group.Id))
                {
                    GenerateTitles(ws1, index);
                    colIndex = 2;
                    GenerateCustomerCurrencyBasedBalanceXLWorkBook(ws1, allTransactions, index, item, 1, "TL", GetColNameFromIndex(colIndex), allAccounts, 0);

                    GenerateCustomerCurrencyBasedBalanceXLWorkBook(ws1, allTransactions, index, item, 2, "USD", GetColNameFromIndex(colIndex), allAccounts, 0);

                    GenerateCustomerCurrencyBasedBalanceXLWorkBook(ws1, allTransactions, index, item, 3, "EUR", GetColNameFromIndex(colIndex), allAccounts, 0);

                    GenerateCustomerCurrencyBasedBalanceXLWorkBook(ws1, allTransactions, index, item, 4, "GAU", GetColNameFromIndex(colIndex), allAccounts, 2);

                    GenerateCustomerCurrencyBasedBalanceXLWorkBook(ws1, allTransactions, index, item, 5, "GBP", GetColNameFromIndex(colIndex), allAccounts, 0);

                    GenerateCustomerCurrencyBasedBalanceXLWorkBook(ws1, allTransactions, index, item, 6, "CHF", GetColNameFromIndex(colIndex), allAccounts, 0);

                    index += 8;
                }
            }
        }

        private string GetColNameFromIndex(int index)
        {
            string colName = "";

            switch (index)
            {
                case 2:
                    colName = "B";
                    break;
                case 3:
                    colName = "C";
                    break;
                case 4:
                    colName = "D";
                    break;
                case 5:
                    colName = "E";
                    break;
                case 6:
                    colName = "F";
                    break;
                case 7:
                    colName = "G";
                    break;
                case 8:
                    colName = "H";
                    break;
                default:
                    break;
            }

            return colName;
        }
        private void GenerateTitles(IXLWorksheet ws, int index)
        {
            ws.Cell("A" + index).Value = "İsim";
            ws.Cell("A" + (index + 1).ToString()).Value = "AY İÇERİSİNDEKİ GÜNCEL BAKİYE";
            ws.Cell("A" + (index + 2).ToString()).Value = "AY BAŞI BAKİYE";
            ws.Cell("A" + (index + 3).ToString()).Value = "Toplam Kar";
            ws.Cell("A" + (index + 4).ToString()).Value = "Kar Oranı";
            ws.Cell("A" + (index + 5).ToString()).Value = "Son Bakiye";
            ws.Cell("A" + (index + 6).ToString()).Value = "Hareketler";
            StyleCells(ws, index, "", "A");
        }

        private void GenerateCustomerCurrencyBasedBalanceXLWorkBook(IXLWorksheet ws, 
            IEnumerable<Transaction> allTransactions, 
            int index, Customer item,int currencyId,
            string currency,string cellName,List<CustomerAccountDtoEx> allAccounts,int digits)
        {
            var customerTransactions = allTransactions.Where(x => x.CustomerId == item.Id && x.CurrencyId == currencyId).OrderBy(x => x.Id).ToList();
            var customerAccounts = allAccounts.Where(x => x.CustomerId == item.Id && x.Currency == currencyId).ToList();
            var firstBalanceTransaction = customerTransactions.FirstOrDefault(x => x.TransactionType == 1 && x.CurrencyId == currencyId);
            var lastBalanceTransaction = customerTransactions.LastOrDefault(x => x.CurrencyId == currencyId);
            decimal totalProfitTl = customerTransactions.Where(x => incomeTypes.Contains(x.TransactionType) && x.CurrencyId == currencyId).Sum(x => x.TransactionAmount).Value
                - customerTransactions.Where(x => x.TransactionType == 11 && x.CurrencyId == currencyId).Sum(x => x.TransactionAmount).Value;

            if (firstBalanceTransaction == null) return;
            colIndex++;
            decimal firstBalance = firstBalanceTransaction.BalanceBeforeTransaction.Value;
            decimal balance = firstBalanceTransaction.BalanceBeforeTransaction.Value
            + customerTransactions.Where(x => x.Id > firstBalanceTransaction.Id && x.TransactionType == (int)TransactionType.ManuelIncome).Sum(x => x.TransactionAmount).Value
            - customerTransactions.Where(x => x.Id > firstBalanceTransaction.Id && x.TransactionType == (int)TransactionType.ManuelOutCome).Sum(x => x.TransactionAmount).Value;
            decimal profitPercent = (totalProfitTl / balance) * 100;

            decimal lastBalance = customerAccounts.Sum(x => x.AccountBalance).Value;
            ws.Cell(cellName + index).Value = item.CustomerName + " - " + currency;
            ws.Cell(cellName + (index + 1).ToString()).Value = Math.Round(balance, digits);
            ws.Cell(cellName + (index + 2).ToString()).Value = Math.Round(firstBalance, digits);
            ws.Cell(cellName + (index + 3).ToString()).Value = Math.Round(totalProfitTl, digits);
            ws.Cell(cellName + (index + 4).ToString()).Value = "%" + Math.Round(profitPercent, digits);
            ws.Cell(cellName + (index + 5).ToString()).Value = Math.Round(lastBalance, digits);

            StringBuilder sb = new StringBuilder();

            var inOutTransactions = customerTransactions.Where(x => (x.TransactionType == (int)TransactionType.ManuelIncome
            || x.TransactionType == (int)TransactionType.ManuelOutCome) && x.Id > firstBalanceTransaction.Id && x.Description != "Hesap Birlestirme").ToList();

            foreach (var tran in inOutTransactions)
            {
                string tranType = tran.TransactionType == (int)TransactionType.ManuelIncome ? "Giriş" : "Çıkış";

                sb.AppendLine($"{tran.TransactionDate.Date.ToShortDateString()} tarihinde {tran.TransactionAmount} {tranType}.");
            }
            ws.Cell(cellName + (index + 6).ToString()).Value = sb.ToString();
            
            StyleCells(ws, index, currency, cellName);


        }

        private void StyleCells(IXLWorksheet ws, int index, string currency, string cellName)
        {
            for (int i = 0; i < 7; i++)
            {
                ws.Cell(cellName + (index + i).ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell(cellName + (index + i).ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                ws.Cell(cellName + (index + i).ToString()).Style.Fill.BackgroundColor = GetCellColor(currency);
                ws.Cell(cellName + (index + i).ToString()).Style.Border.TopBorder = XLBorderStyleValues.Medium;
                ws.Cell(cellName + (index + i).ToString()).Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                ws.Cell(cellName + (index + i).ToString()).Style.Border.LeftBorder = XLBorderStyleValues.Medium;
                ws.Cell(cellName + (index + i).ToString()).Style.Border.RightBorder = XLBorderStyleValues.Medium;
                ws.Column(i + 1).AdjustToContents();                
            }
            ws.Row((index + 6)).AdjustToContents();
        }

        private XLColor GetCellColor(string currencyCode)
        {
            XLColor color = XLColor.ChromeYellow;

            switch (currencyCode)
            {
                case "TL":
                    color = XLColor.Transparent;
                    break;
                case "USD":
                    color = XLColor.AppleGreen;
                    break;
                case "EUR":
                    color = XLColor.LightPink;
                    break;
                case "GAU":
                    color = XLColor.Yellow;
                    break;
                case "GBP":
                    color = XLColor.LightSteelBlue;
                    break;
                case "CHF":
                    color = XLColor.CadetGrey;
                    break;
                default:
                    color = XLColor. Transparent;
                    break;
            }

            return color;
        }
    }
}
