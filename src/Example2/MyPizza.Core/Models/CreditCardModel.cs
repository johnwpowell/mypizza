namespace MyPizza.Core.Models
{
    public class CreditCardModel
    {
        public string Number { get; set; }

        public int ExpMonth { get; set; }

        public int ExpYear { get; set; }

        public string Cvc { get; set; }
    }
}