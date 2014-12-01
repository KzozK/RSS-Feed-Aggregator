using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using FeedReader;

namespace FeedReaderTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Debit_WithValidAmount_UpdatesBalance()
        {
            // arrange
            double beginningBalance = 11.99;
            double debitAmount = 4.55;
            double expected = 7.44;

            BankAccount account = new BankAccount("New Customer", beginningBalance);

            // act
            account.Debit(debitAmount);

            //assert
            double actual = account.Balance;
            Assert.AreEqual(expected, actual, 0.001, "Account not debited corrected");
        }
    }
}
