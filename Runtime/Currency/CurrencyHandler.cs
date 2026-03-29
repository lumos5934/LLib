using System.Collections.Generic;
using System.Numerics;

namespace LumosLib
{
    public class CurrencyHandler
    {
        private readonly Dictionary<int, Currency> _currencies = new();

        
        public Currency Get(int id)
        {
            if (!_currencies.TryGetValue(id, out var currency))
            {
                currency =  new Currency(id);
                _currencies.Add(id, currency);
            }

            return currency;
        }


        public BigInteger GetValue(int id)
        {
            return Get(id).Value;
        }


        public void SetValue(int id, BigInteger value)
        {
            Get(id).Set(value);
        }
        
        
        public bool Consume(int id, BigInteger amount)
        {
            var currency = Get(id);
            
            if (currency.Value >= amount)
            {
                currency.Add(-amount);
                
                return true;
            }
            
            return false;
        }


        public void Add(int id, BigInteger amount)
        {
            var currency = Get(id);
            
            currency.Add(amount);
        }
    }
}
