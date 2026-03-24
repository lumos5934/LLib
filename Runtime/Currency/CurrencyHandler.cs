using System.Collections.Generic;

namespace LumosLib
{
    public class CurrencyHandler
    {
        private readonly Dictionary<int, Currency> _currencies = new();

        
        public void Register(Currency currency)
        {
            _currencies.TryAdd(currency.ID, currency);
        }


        public void Unregister(int id)
        {
            _currencies.Remove(id);
        }

        
        public Currency Get(int id)
        {
            return _currencies.GetValueOrDefault(id);
        }

        
        public bool Consume(int id, long amount)
        {
            var currency = Get(id);
            if (currency == null) 
                return false;
            
            if (currency.Value >= amount)
            {
                currency.Add(-amount);
                
                return true;
            }
            
            return false;
        }


        public void Add(int id, long amount)
        {
            var currency = Get(id);
            if (currency == null)
                return;
            
            currency.Add(amount);
        }
    }
}
